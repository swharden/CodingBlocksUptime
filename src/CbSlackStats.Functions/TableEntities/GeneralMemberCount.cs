using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace CbSlackStats.Functions.TableEntities
{
    internal class GeneralMemberCount : TableEntity
    {
        public int Count { get; set; }

        public GeneralMemberCount()
        {
            PartitionKey = "partition1";
            RowKey = Guid.NewGuid().ToString();
        }
    }
}
