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
        public static string DatedCountsToJson(SortedDictionary<DateTime, int> countsByDate, bool indented = true)
        {
            using var stream = new MemoryStream();
            var options = new JsonWriterOptions() { Indented = indented };
            using var writer = new Utf8JsonWriter(stream, options);

            writer.WriteStartObject();
            writer.WriteString("updated", DateTime.UtcNow.ToString("o"));

            writer.WriteStartObject("records");
            foreach (var record in countsByDate)
            {
                string timestamp = record.Key.ToString("o").Split(".")[0] + "Z";
                writer.WriteNumber(timestamp, record.Value);
            }
            writer.WriteEndObject();

            writer.WriteEndObject();

            writer.Flush();
            string json = Encoding.UTF8.GetString(stream.ToArray());

            return json;
        }

        public static SortedDictionary<DateTime, int> DatedCountsFromJson(string json)
        {
            SortedDictionary<DateTime, int> counts = new();
            using JsonDocument document = JsonDocument.Parse(json);
            foreach (JsonProperty property in document.RootElement.GetProperty("records").EnumerateObject())
            {
                DateTime timestamp = DateTime.Parse(property.Name.ToString());
                int count = int.Parse(property.Value.ToString());
                counts.Add(timestamp, count);
            }
            return counts;
        }

        public static string WebsitePerformanceToJson(SitePerfRecord[] perfRecords, bool indented = true)
        {
            using var stream = new MemoryStream();
            var options = new JsonWriterOptions() { Indented = indented };
            using var writer = new Utf8JsonWriter(stream, options);

            writer.WriteStartObject();
            writer.WriteString("updated", DateTime.UtcNow.ToString("o"));

            writer.WriteStartArray("record value names");
            writer.WriteStringValue("HTTP Response Code");
            writer.WriteStringValue("Size (bytes)");
            writer.WriteStringValue("Load Time (ms)");
            writer.WriteEndArray();

            writer.WriteStartObject("records");
            foreach (SitePerfRecord perf in perfRecords)
            {
                string timestamp = perf.DateTime.ToString("o").Split(".")[0] + "Z";
                writer.WriteStartArray(timestamp);
                writer.WriteNumberValue(perf.ResponseCode);
                writer.WriteNumberValue(perf.SizeBytes);
                writer.WriteNumberValue(perf.LoadTime);
                writer.WriteEndArray();
            }
            writer.WriteEndObject();

            writer.WriteEndObject();

            writer.Flush();
            string json = Encoding.UTF8.GetString(stream.ToArray());

            return json;
        }
    }
}
