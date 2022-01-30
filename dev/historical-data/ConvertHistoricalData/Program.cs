using System.Globalization;
string txtPathIn = Path.GetFullPath("../../../../david.txt");
string csvPathOut = txtPathIn + ".csv";

System.Text.StringBuilder sb = new();
sb.AppendLine("PartitionKey,RowKey,Timestamp,Count,Count@type");
string partitionKey = "partition1";
string countType = "Edm.Int32";

Console.WriteLine($"Reading: {txtPathIn}");
foreach (string rawLine in File.ReadLines(txtPathIn))
{
    string line = rawLine.Trim();
    if (line.StartsWith("#"))
        continue;
    if (!line.Contains("\t"))
        continue;

    string[] parts = line.Split("\t");
    string timestamp = DateTime.Parse(parts[0])
        .ToString(CultureInfo.InvariantCulture.DateTimeFormat.UniversalSortableDateTimePattern)
        .Replace(" ", "T");
    string count = int.Parse(parts[1]).ToString();
    string rowKey = Guid.NewGuid().ToString();
    line = string.Join(",", new string[] { partitionKey, rowKey, timestamp, count, countType });

    Console.WriteLine(line);
    sb.AppendLine(line);
}

File.WriteAllText(csvPathOut, sb.ToString());
Console.WriteLine($"Wrote: {csvPathOut}");