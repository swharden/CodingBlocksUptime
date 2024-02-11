namespace CodingBlocksUptime.Test;

internal static class SampleData
{
    public const string DB_FILE = "../../../../../dev/SampleData/codingblocks.net.csv";

    public static Database LoadSampleDatabase()
    {
        string text = File.ReadAllText(DB_FILE);
        return Database.FromCsv(text);
    }
}
