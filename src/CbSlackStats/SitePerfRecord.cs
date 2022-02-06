namespace CbSlackStats;

public record SitePerfRecord
{
    public DateTime DateTime { get; init; }
    public int ResponseCode { get; init; }
    public double LoadTime { get; init; }
    public int SizeBytes { get; init; }
}
