namespace CodingBlocksUptime;

public class Outage(DateTime start)
{
    public DateTime Start = start.ToUniversalTime();
    public DateTime End = start.ToUniversalTime();
    public int TotalHours => (int)Math.Round((End - Start).TotalHours + 1);

    public override string ToString()
    {
        return TotalHours == 1
            ? $"1 hour outage at {Start}"
            : $"{TotalHours} hour outage from {Start} to {End}";
    }

    public string ToCsv()
    {
        return $"{TotalHours},{Start:o},{End:o}";
    }
}
