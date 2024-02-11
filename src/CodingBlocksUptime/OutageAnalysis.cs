using System.Text;

namespace CodingBlocksUptime;

public static class OutageAnalysis
{
    public static string GetOutageReport(Database db)
    {
        StringBuilder sb = new();

        List<Outage> outages = GetOutages(db);

        outages.Reverse();
        foreach (var outage in outages)
        {
            sb.AppendLine(outage.ToString());
        }

        return sb.ToString();
    }

    public static List<Outage> GetOutages(Database db)
    {
        List<Outage> outages = [];

        Outage? currentOutage = null;

        for (int i = 1; i < db.Records.Count; i++)
        {
            DateTime dt = db.Records[i].DateTime;
            bool isOutage = db.Records[i].Code != 200;
            bool outageStarting = isOutage && currentOutage is null;
            bool outageContinuing = isOutage && currentOutage is not null;
            bool outageEnded = !isOutage && currentOutage is not null;

            if (outageStarting)
            {
                currentOutage = new Outage(dt);
                outages.Add(currentOutage);
            }
            else if (outageContinuing)
            {
                currentOutage!.End = dt;
            }
            else if (outageEnded)
            {
                currentOutage = null;
            }
        }

        return outages;
    }
}
