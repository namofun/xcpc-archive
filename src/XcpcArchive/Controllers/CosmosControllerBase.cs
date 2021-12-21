using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace XcpcArchive.Controllers
{
    public abstract class CosmosControllerBase : ControllerBase, IActionFilter
    {
        protected readonly CosmosClient _cosmosClient;
        protected readonly Database _database;
        protected readonly Container _container;
        protected readonly ILogger _logger;

        protected CosmosControllerBase(CosmosClient cosmosClient, string database, string container, ILogger logger)
        {
            _cosmosClient = cosmosClient;
            _database = _cosmosClient.GetDatabase(database);
            _container = _database.GetContainer(container);
            _logger = logger;
        }

        protected async Task<T?> GetOne<T>(string id, string partitionKey) where T : class
        {
            try
            {
                return await _container.ReadItemAsync<T>(id, new PartitionKey(partitionKey));
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        protected async Task<List<T>> GetFeed<T>(FeedIterator<T> iterator)
        {
            List<T> result = new();
            while (iterator.HasMoreResults)
            {
                foreach (T item in await iterator.ReadNextAsync().ConfigureAwait(false))
                {
                    result.Add(item);
                }
            }

            return result;
        }

        protected Task<List<T>> GetSql<T>(string sql)
        {
            return GetSql<T>(new QueryDefinition(sql));
        }

        protected Task<List<T>> GetSql<T>(string sql, object param)
        {
            QueryDefinition queryDefinition = new(sql);
            foreach ((string paramName, JToken? paramValue) in JObject.FromObject(param))
            {
                queryDefinition.WithParameter("@" + paramName, paramValue);
            }

            return GetSql<T>(queryDefinition);
        }

        protected async Task<List<T>> GetSql<T>(QueryDefinition sql)
        {
            Stopwatch timer = Stopwatch.StartNew();
            List<T> result;
            try
            {
                using FeedIterator<T> iterator = _container.GetItemQueryIterator<T>(sql);
                result = await GetFeed(iterator);
            }
            catch (CosmosException ex)
            {
                timer.Stop();
                _logger.LogQueryFailure(ex, _container, timer, sql);
                throw;
            }

            timer.Stop();
            _logger.LogQuery(_container, timer, result.Count, sql);
            return result;
        }

        [Obsolete("We only use this to generate SQL at debug time")]
        protected Task<List<T>> GetLinq<T>(Func<IOrderedQueryable<T>, IQueryable<T>> linq)
        {
            return GetSql<T>(linq(_container.GetItemLinqQueryable<T>()).ToQueryDefinition());
        }

        [NonAction]
        public void OnActionExecuting(ActionExecutingContext context)
        {
        }

        [NonAction]
        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Result is ObjectResult objectResult
                && objectResult.Value == null
                && objectResult.StatusCode == 200)
            {
                context.Result = NotFound();
            }
        }
    }
}
