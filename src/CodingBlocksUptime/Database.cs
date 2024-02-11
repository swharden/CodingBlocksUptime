using System.Text;

namespace CodingBlocksUptime;

public class Database
{
    public List<DatabaseRecord> Records = [];

    private Database()
    {

    }

    public override string ToString()
    {
        return $"Database with {Records.Count} records";
    }

    public DatabaseRecord[] GetRecords() => Records.ToArray();

    public static Database FromCsv(string text)
    {
        Database db = new();

        foreach (string line in text.Split('\n'))
        {
            DatabaseRecord? record = DatabaseRecord.FromCsvLine(line);
            if (record is not null)
                db.Records.Add(record);
        }

        return db;
    }

    public string ToCsv()
    {
        StringBuilder sb = new();
        sb.AppendLine("# Date, Code, Size, Time");
        foreach (DatabaseRecord record in Records)
        {
            sb.AppendLine(record.ToCsvLine());
        }
        return sb.ToString();
    }
}
