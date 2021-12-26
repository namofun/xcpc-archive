using Azure.Storage.Blobs;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using XcpcArchive.CcsApi.Entities;
using XcpcArchive.CcsApi.Models;

namespace XcpcArchive.CcsApi
{
    public sealed class CcsApiClient
    {
        private readonly Database _database;
        private readonly BlobContainerClient _blobs;
        private readonly ILogger<CcsApiClient> _logger;
        private readonly IReadOnlyDictionary<Type, string> _containerMapping;

        public CcsApiClient(
            CosmosClient cosmosClient,
            BlobServiceClient blobServiceClient,
            ILogger<CcsApiClient> logger)
        {
            _database = cosmosClient.GetDatabase("ccsapi");
            _blobs = blobServiceClient.GetBlobContainerClient("ccsapi");
            _logger = logger;

            _containerMapping = new Dictionary<Type, string>()
            {
                { typeof(Contest), "contests" },
                { typeof(Award), "awards" },
                { typeof(Clarification), "clarifications" },
                { typeof(Group), "groups" },
                { typeof(Judgement), "judgements" },
                { typeof(JudgementType), "judgement-types" },
                { typeof(Language), "languages" },
                { typeof(Organization), "organizations" },
                { typeof(Problem), "problems" },
                { typeof(Run), "runs" },
                { typeof(Submission), "submissions" },
                { typeof(Team), "teams" },
                { typeof(JobEntry), "uploaded-jobs" },
                { typeof(CacheEntry), "cache" },
            };
        }

        public Database GetDatabase()
        {
            return _database;
        }

        public BlobContainerClient GetBlobContainer()
        {
            return _blobs;
        }

        public async Task InitializeAsync()
        {
            await _database.Client.CreateDatabaseIfNotExistsAsync(
                _database.Id,
                ThroughputProperties.CreateAutoscaleThroughput(4000));

            foreach (string collectionName in _containerMapping.Values)
            {
                if (collectionName == "uploaded-jobs" || collectionName == "cache")
                {
                    continue;
                }

                ContainerProperties properties = new()
                {
                    Id = collectionName,
                    PartitionKeyPath = "/_cid",
                };

                if (collectionName != "contests")
                {
                    properties.IndexingPolicy.CompositeIndexes.Add(new Collection<CompositePath>
                    {
                        new CompositePath { Path = "/_idlen" },
                        new CompositePath { Path = "/id" },
                    });
                }

                await _database.CreateContainerIfNotExistsAsync(properties).ConfigureAwait(false);
            }

            await _database.CreateContainerIfNotExistsAsync(
                new ContainerProperties()
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
                });

            await _database.CreateContainerIfNotExistsAsync(
                new ContainerProperties()
                {
                    Id = "cache",
                    PartitionKeyPath = "/_cid",
                });

            await _blobs.CreateIfNotExistsAsync().ConfigureAwait(false);
        }

        public string NormalizeContestId(string id)
        {
            return id.ToLower().Trim();
        }

        private async Task<List<TEntity>> GetListInternalAsync<TEntity>(Container coll, QueryDefinition sql)
        {
            Stopwatch timer = Stopwatch.StartNew();
            List<TEntity> result;
            try
            {
                using FeedIterator<TEntity> iterator = coll.GetItemQueryIterator<TEntity>(sql);
                result = await iterator.ToListAsync();
            }
            catch (CosmosException ex)
            {
                timer.Stop();
                _logger.LogQueryFailure(ex, coll, timer, sql);
                throw;
            }

            timer.Stop();
            _logger.LogQuery(coll, timer, result.Count, sql);
            return result;
        }

        public Task<List<TEntity>> GetListAsync<TEntity>(QueryDefinition sql) where TEntity : EntityBase
        {
            return GetListInternalAsync<TEntity>(_database.GetContainer(_containerMapping[typeof(TEntity)]), sql);
        }

        public Task<List<TEntity>> GetListAsync<TEntity>(string sql) where TEntity : EntityBase
        {
            return GetListAsync<TEntity>(new QueryDefinition(sql));
        }

        public Task<List<TResult>> GetCacheAsync<TResult>(string sql, object param)
        {
            QueryDefinition queryDefinition = new(sql);
            foreach ((string paramName, JToken? paramValue) in JObject.FromObject(param))
            {
                queryDefinition.WithParameter("@" + paramName, paramValue);
            }

            return GetListInternalAsync<TResult>(_database.GetContainer("cache"), queryDefinition);
        }

        public Task<List<TEntity>> GetListAsync<TEntity>(string sql, object param) where TEntity : EntityBase
        {
            QueryDefinition queryDefinition = new(sql);
            foreach ((string paramName, JToken? paramValue) in JObject.FromObject(param))
            {
                queryDefinition.WithParameter("@" + paramName, paramValue);
            }

            return GetListAsync<TEntity>(queryDefinition);
        }

        [Obsolete("We only use this to generate SQL at debug time")]
        public Task<List<TEntity>> GetListAsync<TEntity>(Func<IOrderedQueryable<TEntity>, IQueryable<TEntity>> linq) where TEntity : EntityBase
        {
            Container coll = _database.GetContainer(_containerMapping[typeof(TEntity)]);
            return GetListAsync<TEntity>(linq(coll.GetItemLinqQueryable<TEntity>()).ToQueryDefinition());
        }

        public async Task<TEntity?> GetEntityAsync<TEntity>(string id, string partitionKey) where TEntity : EntityBase
        {
            Container coll = _database.GetContainer(_containerMapping[typeof(TEntity)]);
            try
            {
                return await coll.ReadItemAsync<TEntity>(id, new PartitionKey(partitionKey));
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }
        }
    }
}
