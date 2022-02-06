using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace CbSlackStats.Functions.TableEntities
{
    internal class WebsitePerformance : TableEntity
    {
        public int ResponseCode { get; set; }
        public double LoadTime { get; set; }
        public int SizeBytes { get; set; }
        public DateTime DateTime => DateTime.Parse(RowKey);

        public WebsitePerformance() { }

        public WebsitePerformance(SitePerfRecord perf)
        {
            PartitionKey = "partition1";
            RowKey = perf.DateTime
                .ToString(System.Globalization.CultureInfo.InvariantCulture.DateTimeFormat.UniversalSortableDateTimePattern)
                .Replace(" ", "T");
            ResponseCode = perf.ResponseCode;
            LoadTime = perf.LoadTime;
            SizeBytes = perf.SizeBytes;
        }
    }
}
