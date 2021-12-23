using Azure.Storage.Blobs;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace XcpcArchive.CcsApi
{
    public class CcsApiImportService : BackgroundService
    {
        private readonly SemaphoreSlim _semaphore;
        private readonly Database _database;
        private readonly Container _uploadedJobs;
        private readonly BlobServiceClient _blobServiceClient;
        private readonly ConcurrentQueue<KeyValuePair<JobEntry, byte[]>> _queue;
        private readonly ILogger _logger;
        private Task? _containerCreateTask;

        public CcsApiImportService(
            CosmosClient cosmosClient,
            BlobServiceClient blobServiceClient,
            ILoggerFactory loggerFactory)
        {
            _semaphore = new SemaphoreSlim(0);
            _queue = new ConcurrentQueue<KeyValuePair<JobEntry, byte[]>>();
            _database = cosmosClient.GetDatabase("ccsapi");
            _uploadedJobs = _database.GetContainer("uploaded-jobs");
            _blobServiceClient = blobServiceClient;
            _logger = loggerFactory.CreateLogger("XcpcArchive.CcsApi.ImportService");
        }

        public async Task<JobEntry?> EnqueueAsync(string id, byte[] package)
        {
            if (_containerCreateTask == null)
            {
                return null;
            }

            await _containerCreateTask.ConfigureAwait(false);

            JobEntry entry;
            id = id.ToLower().Trim();
            try
            {
                entry = await _uploadedJobs.CreateItemAsync(
                    new JobEntry
                    {
                        HotId = id,
                        ColdId = Guid.NewGuid().ToString(),
                        PartitionKey = id,
                        CreationTime = DateTimeOffset.Now,
                        Status = JobEntry.JobStatus.Pending,
                        Type = JobEntry.JobType.FullZipUpload,
                    });
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.Conflict)
            {
                return null;
            }

            try
            {
                await _blobServiceClient
                    .GetBlobContainerClient("ccsapi")
                    .UploadBlobAsync($"{entry.PartitionKey}_{entry.ColdId}.zip", BinaryData.FromBytes(package));
            }
            catch (Azure.RequestFailedException ex)
            {
                entry.Status = JobEntry.JobStatus.Failed;
                entry.Comment = "Failed to upload backup file to Azure Blob Storage. " + ex.ToString();
                entry.FinishedTime = DateTimeOffset.Now;
                entry.HotId = entry.ColdId;
                await _uploadedJobs.ReplaceItemAsync(entry, id);
            }

            _queue.Enqueue(new KeyValuePair<JobEntry, byte[]>(entry, package));
            _semaphore.Release();
            return entry;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _containerCreateTask = InitializeAsync();
            await _containerCreateTask.ConfigureAwait(false);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await _semaphore.WaitAsync(stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    if (stoppingToken.IsCancellationRequested) return; else throw;
                }

                if (!_queue.TryDequeue(out KeyValuePair<JobEntry, byte[]> entry))
                {
                    _logger.LogError("Unknown semaphore state, no entry was dequeued.");
                    continue;
                }

                try
                {
                    await ImportAsync(entry.Key, entry.Value);
                    entry.Key.Status = JobEntry.JobStatus.Complete;
                    entry.Key.FinishedTime = DateTimeOffset.Now;
                }
                catch (RecoverableException ex)
                {
                    _logger.LogError(ex, "Recoverable exception occurred during import.");

                    entry.Key.Status = JobEntry.JobStatus.Failed;
                    entry.Key.Comment = "Recoverable exception occurred during import. " + ex.ToString();
                    entry.Key.FinishedTime = DateTimeOffset.Now;
                    entry.Key.HotId = entry.Key.ColdId;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unrecoverable exception occurred during import.");

                    entry.Key.Status = JobEntry.JobStatus.Failed;
                    entry.Key.Comment = "Unrecoverable exception occurred during import. " + ex.ToString();
                    entry.Key.FinishedTime = DateTimeOffset.Now;
                }

                try
                {
                    await _uploadedJobs.ReplaceItemAsync(entry.Key, entry.Key.PartitionKey);
                }
                catch (CosmosException ex)
                {
                    _logger.LogError(ex, "Unknown exception during finalizing job {JobColdId}.", entry.Key.ColdId);
                }
            }
        }

        private async Task InitializeAsync()
        {
            await new CcsApiInitializer(_database.Client, _blobServiceClient)
                .DoWorkAsync()
                .ConfigureAwait(false);

            ContainerProperties properties = new()
            {
                Id = "uploaded-jobs",
                PartitionKeyPath = "/_cid",
                UniqueKeyPolicy =
                {
                    UniqueKeys =
                    {
                        new UniqueKey() { Paths = { "/externalid" } },
                    }
                },
            };

            await _database
                .CreateContainerIfNotExistsAsync(properties)
                .ConfigureAwait(false);
        }

        private async Task ImportAsync(JobEntry entry, byte[] package)
        {
            string id = entry.PartitionKey;
            using MemoryStream memoryStream = new(package, writable: false);
            using ZipArchive zipArchive = new(memoryStream);
            ZipArchiveEntry? contestJson = zipArchive.GetEntry("contest.json");
            if (contestJson == null)
            {
                throw new RecoverableException("File contest.json not found.");
            }

            JObject contest = await contestJson.ReadAsJsonAsync();
            JToken? originalId = contest["id"];
            if (originalId == null)
            {
                throw new RecoverableException("Invalid contest.json");
            }

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
                    throw new RecoverableException("File " + part + ".json not found.");
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
                            throw new RecoverableException("Submission entry id not defined.");
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
                            throw new RecoverableException("File submissions/" + sid + ".zip not found.");
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
                        throw new RecoverableException($"Conflict run data of {runId}.");
                    }
                }
            }

            contest["_statistics"] = new JObject()
            {
                ["problems"] = insertRecords["problems"].Count,
                ["teams"] = insertRecords["teams"].Count,
                ["submissions"] = insertRecords["submissions"].Count,
            };

            await _database.GetContainer("contests").CreateItemAsync(contest, new PartitionKey(id));
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
                        throw new UnrecoverableException(
                            "Unknown transactional exception: " + resp.ErrorMessage);
                    }
                }
            }
        }
    }
}
