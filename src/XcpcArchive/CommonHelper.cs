using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace XcpcArchive
{
    public static class CommonHelper
    {
        private static readonly Action<ILogger, string, long, string, Exception?> LoggingFailedQueryDefinition =
            LoggerMessage.Define<string, long, string>(
                LogLevel.Error,
                new EventId(10060, "CosmosDbQuery"),
                "Failed to query from [{ContainerName}] within {ElapsedTime}ms.\r\n{QueryText}");

        private static readonly Action<ILogger, string, long, int, string, Exception?> LoggingSucceededQueryDefinition =
            LoggerMessage.Define<string, long, int, string>(
                LogLevel.Information,
                new EventId(10060, "CosmosDbQuery"),
                "Queried from [{ContainerName}] within {ElapsedTime}ms, {Count} results.\r\n{QueryText}");

        public static async Task<JObject> ReadAsJsonAsync(this ZipArchiveEntry entry)
        {
            using Stream entryStream = entry.Open();
            using StreamReader streamReader = new(entryStream);
            using JsonTextReader reader = new(streamReader) { DateParseHandling = DateParseHandling.None };
            return await JObject.LoadAsync(reader).ConfigureAwait(false);
        }

        public static async Task<JArray> ReadAsJsonArrayAsync(this ZipArchiveEntry entry)
        {
            using Stream entryStream = entry.Open();
            using StreamReader streamReader = new(entryStream);
            using JsonTextReader reader = new(streamReader) { DateParseHandling = DateParseHandling.None };
            return await JArray.LoadAsync(reader).ConfigureAwait(false);
        }

        public static async Task<byte[]> ReadAllAsync(this Stream stream, long? specifiedLength = null)
        {
            long length = specifiedLength ?? stream.Length;
            byte[] memory = new byte[length];
            for (int i = 0; i < memory.Length; )
            {
                i += await stream.ReadAsync(memory, i, memory.Length - i).ConfigureAwait(false);
            }

            return memory;
        }

        public static async Task<byte[]> ReadAsByteArrayAsync(this ZipArchiveEntry entry)
        {
            using Stream entryStream = entry.Open();
            return await ReadAllAsync(entryStream, entry.Length).ConfigureAwait(false);
        }

        public static void LogQueryFailure(this ILogger logger, CosmosException exception, Container container, Stopwatch stopwatch, QueryDefinition query)
            => CommonHelper.LoggingFailedQueryDefinition(logger, container.Id, stopwatch.ElapsedMilliseconds, query.QueryText, exception);

        public static void LogQuery(this ILogger logger, Container container, Stopwatch stopwatch, int count, QueryDefinition query)
            => CommonHelper.LoggingSucceededQueryDefinition(logger, container.Id, stopwatch.ElapsedMilliseconds, count, query.QueryText, null);
    }

    public class RecoverableException : ApplicationException
    {
        public RecoverableException(string message) : base(message)
        {
        }
    }

    public class UnrecoverableException : ApplicationException
    {
        public UnrecoverableException(string message) : base(message)
        {
        }
    }
}
