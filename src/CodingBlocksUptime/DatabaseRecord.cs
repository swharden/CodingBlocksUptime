namespace CodingBlocksUptime;
public record DbRecord(DateTime Date, int Code, int Size, double Time)
{
    public static DbRecord? FromCsvLine(string line)
    {
        if (line.StartsWith('#'))
            return null;

        string[] parts = line.Split(",");
        if (parts.Length != 4)
            return null;

        return new DbRecord(
            Date: DateTime.Parse(parts[0]).ToUniversalTime(),
            Code: int.Parse(parts[1]),
            Size: int.Parse(parts[2]),
            Time: double.Parse(parts[3]));
    }

    public string ToCsvLine()
    {
        return $"{Date},{Code},{Size},{Time}";
    }
}