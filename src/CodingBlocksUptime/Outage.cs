namespace CodingBlocksUptime;

public class Outage(DateTime start)
{
    public DateTime Start = start;
    public DateTime End = start;
    public int TotalHours => (int)Math.Round((End - Start).TotalHours + 1);

    public override string ToString()
    {
        return TotalHours == 1
            ? $"1 hour outage at {Start}"
            : $"{TotalHours} hour outage from {Start} to {End}";
    }
}
