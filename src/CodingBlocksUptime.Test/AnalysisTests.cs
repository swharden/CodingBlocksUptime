namespace CodingBlocksUptime.Test;

internal class AnalysisTests
{
    [Test]
    public void Test_Analysis_Report()
    {
        Database db = SampleData.LoadSampleDatabase();
        List<Outage> outages = Report.GetOutages(db);
        outages.Should().NotBeEmpty();

        string report = Report.GetJson(db);
        Console.WriteLine(report);
    }
}
