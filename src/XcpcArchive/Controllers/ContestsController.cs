using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using XcpcArchive.CcsApi;

namespace XcpcArchive.Controllers
{
    [Route("/api/contests")]
    public class ContestsController : CosmosControllerBase
    {
        public ContestsController(
            CosmosClient cosmosClient,
            ILogger<ContestsController> logger)
            : base(cosmosClient, "ccsapi", "contests", logger)
        {
        }

        [HttpGet]
        public async Task<ActionResult<List<Contest>>> GetAll()
        {
            return await GetSql<Contest>("SELECT VALUE c FROM c ORDER BY c.start_time DESC");
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Contest?>> GetOne([FromRoute] string id)
        {
            id = id.ToLower().Trim();
            return await GetOne<Contest>(id, id);
        }

        [HttpPost("{id}")]
        [Consumes("application/zip")]
        [Authorize(Roles = "XcpcArchive.Uploader")]
        public async Task<ActionResult<Contest>> PostNew(
            [FromRoute] string id,
            [FromServices] BlobServiceClient blobServiceClient)
        {
            if (Request.ContentType == "application/zip")
            {
                if (!Request.ContentLength.HasValue)
                {
                    return StatusCode((int)System.Net.HttpStatusCode.LengthRequired);
                }

                id = id.ToLower().Trim();
                QueryDefinition query = new QueryDefinition("SELECT COUNT(1) FROM c WHERE c.id = @id").WithParameter("@id", id);
                JObject counts = (await GetSql<JObject>(query)).Single();
                if (((int)counts["$1"]!) != 0)
                {
                    return Conflict(new { error = "Contest with ID already exists.", query = counts });
                }

                byte[] memory = await Request.Body.ReadAllAsync(Request.ContentLength);
                using MemoryStream memoryStream = new(memory, writable: false);
                using ZipArchive zipArchive = new(memoryStream);
                ZipArchiveEntry? contestJson = zipArchive.GetEntry("contest.json");
                if (contestJson == null)
                {
                    return BadRequest(new { error = "File contest.json not found." });
                }

                JObject contest = await contestJson.ReadAsJsonAsync();
                JToken originalId = contest["id"] ?? throw new ApplicationException("Invalid contest.json");
                contest["id"] = id;
                contest["_cid"] = id;
                contest["_original_id"] = originalId;
                Dictionary<string, List<JObject>> insertRecords = new();

                string[] neededPart = new[]
                {
                    "clarifications",
                    "groups",
                    "judgements",
                    "judgement-types",
                    "languages",
                    "organizations",
                    "problems",
                    "submissions",
                    "teams",
                };

                foreach (string part in neededPart)
                {
                    Container container = _database.GetContainer(part);
                    ZipArchiveEntry? partJson = zipArchive.GetEntry(part + ".json");
                    if (partJson == null)
                    {
                        return BadRequest(new { error = "File " + part + ".json not found." });
                    }

                    JArray partObjects = await partJson.ReadAsJsonArrayAsync();
                    List<JObject> objects = partObjects.Cast<JObject>().ToList();
                    insertRecords.Add(part, objects);

                    if (part == "submissions")
                    {
                        foreach (JObject submission in objects)
                        {
                            string? sid = (string?)submission["id"];
                            JToken? submissionFiles = submission["files"];
                            if (sid == null || submissionFiles == null)
                            {
                                return BadRequest(new { error = "Submission entry id not defined." });
                            }

                            submission["_rawfiles"] = submissionFiles.DeepClone();
                            foreach (JObject fileRef in (JArray)submissionFiles)
                            {
                                if (fileRef.TryGetValue("href", out JToken? href))
                                {
                                    fileRef["href"] = ((string)href!).Replace($"contests/{(string)originalId!}/", $"contests/{id}/");
                                }
                            }

                            ZipArchiveEntry? submissionZip = zipArchive.GetEntry("submissions/" + sid + ".zip");
                            if (submissionZip == null)
                            {
                                return BadRequest(new { error = "File submissions/" + sid + ".zip not found." });
                            }

                            using Stream stream = submissionZip.Open();
                            byte[] content = await stream.ReadAllAsync(submissionZip.Length);
                            submission["_filezip"] = content;
                        }
                    }
                }

                List<JObject> runs = new();
                Dictionary<string, JObject> runIds = new();
                for (int i = 1; i < 1000; i++)
                {
                    Container container = _database.GetContainer("runs");
                    ZipArchiveEntry? partJson = zipArchive.GetEntry($"runs.{i}.json");
                    if (partJson == null) break;

                    runs.AddRange((await partJson.ReadAsJsonArrayAsync()).Cast<JObject>());
                }

                insertRecords.Add("runs", runs);
                for (int i = 0; i < runs.Count; i++)
                {
                    string runId = (string)runs[i]["id"]!;
                    if (!runIds.TryAdd(runId, runs[i]))
                    {
                        if (runs[i].ToString() == runIds[runId].ToString())
                        {
                            runs.RemoveAt(i);
                            i--;
                        }
                        else
                        {
                            throw new Exception("Conflict data.");
                        }
                    }
                }

                await _container.CreateItemAsync(contest, new PartitionKey(id));
                foreach ((string type, List<JObject> values) in insertRecords)
                {
                    foreach (JObject value in values)
                    {
                        value["_cid"] = id;

                        if (value.TryGetValue("id", out JToken? jid)
                            && jid.Type == JTokenType.String)
                        {
                            if (jid.Type == JTokenType.String)
                            {
                                value["_idlen"] = ((string)jid!).Length;
                            }
                            else if (jid.Type == JTokenType.Integer)
                            {
                                value["_idlen"] = -1;
                            }
                        }
                    }

                    Container container = _database.GetContainer(type);
                    foreach (JObject[] chunk in values.Chunk(100))
                    {
                        TransactionalBatchResponse resp = await chunk
                            .Aggregate(
                                container.CreateTransactionalBatch(new PartitionKey(id)),
                                (batch, entry) => batch.CreateItem(entry))
                            .ExecuteAsync();

                        if (!resp.IsSuccessStatusCode)
                        {
                            throw new Exception("Unknown transactional exception: " + resp.ErrorMessage);
                        }
                    }
                }

                await blobServiceClient
                    .GetBlobContainerClient("ccsapi")
                    .UploadBlobAsync($"{id}.zip", BinaryData.FromBytes(memory));

                return CreatedAtAction(nameof(GetOne), new { id });
            }
            else
            {
                return new UnsupportedMediaTypeResult();
            }
        }
    }
}
