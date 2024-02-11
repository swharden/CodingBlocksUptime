namespace CodingBlocksUptime.Test;

internal class AnalysisTests
{
    [Test]
    public void Test_Analysis_Downtime()
    {
        Database db = SampleData.LoadSampleDatabase();
        List<Outage> outages = OutageAnalysis.GetOutages(db);
        outages.Should().NotBeEmpty();

        Console.WriteLine(OutageAnalysis.GetOutageReport(db));
    }
}
