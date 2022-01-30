using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace CbSlackStats.Functions.TableEntities
{
    internal class GeneralMemberCount : TableEntity
    {
        public int Count { get; set; }

        public GeneralMemberCount(int count, DateTime? timestamp = null)
        {
            PartitionKey = "partition1";
            RowKey = (timestamp ?? DateTime.UtcNow)
                .ToString(System.Globalization.CultureInfo.InvariantCulture.DateTimeFormat.UniversalSortableDateTimePattern)
                .Replace(" ", "T");
            Count = count;
        }
    }
}
