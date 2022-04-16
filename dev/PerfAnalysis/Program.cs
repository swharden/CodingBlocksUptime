using System.Text.Json;

string json = File.ReadAllText("../sample-data/performance.json");
using JsonDocument document = JsonDocument.Parse(json);

DateTime dtLast = DateTime.MinValue;
foreach (JsonProperty el in document.RootElement.GetProperty("records").EnumerateObject())
{
    DateTime dt = DateTime.Parse(el.Name);
    TimeSpan diff = dt - dtLast;
    dtLast = dt;
    int code = int.Parse(el.Value[0].ToString());
    int bytes = int.Parse(el.Value[1].ToString());
    double loadTime = double.Parse(el.Value[2].ToString());

    if (diff.TotalHours > 999)
        continue;

    if (diff.TotalHours > 1.5)
        Console.WriteLine($"Missing record: {el.Name}, diff: {diff}");

    if (code != 200)
        Console.WriteLine($"Response code {code}: {el.Name}");
}