using Azure.Storage.Blobs;
using Microsoft.Azure.Cosmos;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace XcpcArchive.CcsApi
{
    public class CcsApiInitializer
    {
        private readonly CosmosClient _cosmosClient;
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string[] _availableContainers;

        public CcsApiInitializer(
            CosmosClient cosmosClient,
            BlobServiceClient blobServiceClient)
        {
            _cosmosClient = cosmosClient;
            _blobServiceClient = blobServiceClient;

            _availableContainers = new[]
            {
                "contests",
                "awards",
                "events",
                "clarifications",
                "groups",
                "judgements",
                "judgement-types",
                "languages",
                "organizations",
                "problems",
                "runs",
                "submissions",
                "teams",
            };
        }

        public async Task DoWorkAsync()
        {
            Database database =
                await _cosmosClient.CreateDatabaseIfNotExistsAsync(
                    "ccsapi",
                    ThroughputProperties.CreateAutoscaleThroughput(4000));

            foreach (string collectionName in _availableContainers)
            {
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

                await database.CreateContainerIfNotExistsAsync(properties);
            }

            await _blobServiceClient
                .GetBlobContainerClient("ccsapi")
                .CreateIfNotExistsAsync();
        }
    }
}
