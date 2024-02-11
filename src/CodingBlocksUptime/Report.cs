using System.Text;
using System.Text.Json;

namespace CodingBlocksUptime;

public static class Report
{
    public static string GetJson(Database db)
    {
        List<Outage> outages = GetOutages(db);
        outages.Reverse();

        int recordsDown = db.Records.Where(x => x.Code != 200).Count();
        int recordsTotal = db.Records.Count;
        int recordsUp = recordsTotal - recordsDown;
        double uptimePercent = 100.0 * recordsUp / recordsTotal;

        using MemoryStream stream = new();
        JsonWriterOptions options = new() { Indented = true };
        using Utf8JsonWriter writer = new(stream, options);

        writer.WriteStartObject();
        writer.WriteString("lastUpdate", db.Records.Last().DateTime.ToString("o"));
        writer.WriteNumber("lastCode", db.Records.Last().Code);
        writer.WriteNumber("lastSize", db.Records.Last().Size);
        writer.WriteNumber("lastTime", db.Records.Last().Time);
        writer.WriteBoolean("isCurrentOutage", db.Records.Last().Code != 200);
        writer.WriteNumber("recordsUp", recordsUp);
        writer.WriteNumber("recordsDown", recordsDown);
        writer.WriteNumber("recordsTotal", recordsTotal);
        writer.WriteNumber("uptimePercent", uptimePercent);

        writer.WriteStartArray("Outages");
        foreach (var outage in outages)
        {
            writer.WriteStartObject();
            writer.WriteNumber("hours", outage.TotalHours);
            writer.WriteString("start", outage.Start.ToString("o"));
            writer.WriteString("end", outage.End.ToString("o"));
            writer.WriteEndObject();
        }
        writer.WriteEndArray();
        writer.WriteEndObject();

        writer.Flush();
        string json = Encoding.UTF8.GetString(stream.ToArray());

        return json;
    }

    public static List<Outage> GetOutages(Database db)
    {
        List<Outage> outages = [];

        Outage? currentOutage = null;

        for (int i = 1; i < db.Records.Count; i++)
        {
            DateTime dt = db.Records[i].DateTime;
            bool isOutage = db.Records[i].Code != 200;
            bool outageStarting = isOutage && currentOutage is null;
            bool outageContinuing = isOutage && currentOutage is not null;
            bool outageEnded = !isOutage && currentOutage is not null;

            if (outageStarting)
            {
                currentOutage = new Outage(dt);
                outages.Add(currentOutage);
            }
            else if (outageContinuing)
            {
                currentOutage!.End = dt;
            }
            else if (outageEnded)
            {
                currentOutage = null;
            }
        }

        return outages;
    }
}
