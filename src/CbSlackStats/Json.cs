using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CbSlackStats
{
    public static class Json
    {
        public static string DatedCountsToJson(SortedDictionary<DateTime, int> countsByDate)
        {
            using var stream = new MemoryStream();
            var options = new JsonWriterOptions() { Indented = true };
            using var writer = new Utf8JsonWriter(stream, options);

            writer.WriteStartObject();
            foreach (var record in countsByDate)
                writer.WriteNumber(record.Key.ToString("o"), record.Value);
            writer.WriteEndObject();

            writer.Flush();
            string json = Encoding.UTF8.GetString(stream.ToArray());

            return json;
        }
    }
}
