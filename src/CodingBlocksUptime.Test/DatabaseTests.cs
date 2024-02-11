namespace CodingBlocksUptime.Test;

public class DatabaseTests
{
    const string DB_FILE = "../../../../../dev/SampleData/codingblocks.net.csv";

    [Test]
    public void Test_Database_FromCsv()
    {
        string text = File.ReadAllText(DB_FILE);
        Database db = Database.FromCsv(text);
        db.Records.Count.Should().BeGreaterThan(17_000);
    }

    [Test]
    public void Test_Database_ToCsv()
    {
        string text1 = File.ReadAllText(DB_FILE);
        Database db1 = Database.FromCsv(text1);

        string text2 = db1.ToCsv();
        Database db2 = Database.FromCsv(text2);

        db2.Records.Count.Should().Be(db1.Records.Count);
    }
}
