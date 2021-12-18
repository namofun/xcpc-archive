using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XcpcArchive.Controllers
{
    public abstract class CosmosControllerBase : ControllerBase
    {
        protected readonly CosmosClient _cosmosClient;
        protected readonly Database _database;
        protected readonly Container _container;

        protected CosmosControllerBase(CosmosClient cosmosClient, string database, string container)
        {
            _cosmosClient = cosmosClient;
            _database = _cosmosClient.GetDatabase(database);
            _container = _database.GetContainer(container);
        }

        protected async Task<T> GetOne<T>(string id, string partitionKey)
        {
            return await _container.ReadItemAsync<T>(id, new PartitionKey(partitionKey));
        }

        protected async Task<List<T>> GetFeed<T>(FeedIterator<T> iterator)
        {
            List<T> result = new();
            while (iterator.HasMoreResults)
            {
                foreach (T item in await iterator.ReadNextAsync())
                {
                    result.Add(item);
                }
            }

            return result;
        }

        protected async Task<List<T>> GetSql<T>(string sql)
        {
            using FeedIterator<T> iterator = _container.GetItemQueryIterator<T>(sql);
            return await GetFeed(iterator);
        }

        protected async Task<List<T>> GetLinq<T>(Func<IOrderedQueryable<T>, IQueryable<T>> linq)
        {
            using FeedIterator<T> iterator = linq(_container.GetItemLinqQueryable<T>()).ToFeedIterator();
            return await GetFeed(iterator);
        }
    }
}
