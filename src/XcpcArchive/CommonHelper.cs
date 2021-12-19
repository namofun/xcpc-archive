using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace XcpcArchive
{
    public static class CommonHelper
    {
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
    }
}
