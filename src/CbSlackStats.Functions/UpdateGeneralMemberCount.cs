using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CbSlackStats.Functions.TableEntities;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;

namespace CbSlackStats.Functions
{
    public class UpdateGeneralMemberCount
    {
        [FunctionName("UpdateGeneralMemberCount")]
        public async Task Run([TimerTrigger("0 0 * * * *")] TimerInfo myTimer, ILogger log)
        {
            int memberCountNow = await GetCurrentCountUsingSlackAPI(log);

            CloudStorageAccount storageAccount = ConnectToStorageAccount();
            CloudTable table = await ConnectToTable(storageAccount, log);
            SortedDictionary<DateTime, int> countByDate = await GetHistoricalCountsFromTable(table, log);
            await AddNewCountToTable(memberCountNow, countByDate, table, log);

            await WriteJsonToWebStorage(storageAccount, countByDate, log);
        }

        private async static Task<int> GetCurrentCountUsingSlackAPI(ILogger log)
        {
            string token = Secrets.GetSecret("CB_SLACK_TOKEN");
            int memberCount = await SlackAPI.GetGeneralMemberCountAsync(token);
            log.LogInformation($"The General channel has {memberCount:N0} members");
            return memberCount;
        }

        private static CloudStorageAccount ConnectToStorageAccount()
        {
            string storageConnectionString = Secrets.GetSecret("CB_STORAGE_CONNECTION_STRING");
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            return storageAccount;
        }

        private static async Task<CloudTable> ConnectToTable(CloudStorageAccount account, ILogger log)
        {
            const string tableName = "GeneralMemberCount";
            CloudTableClient tableClient = account.CreateCloudTableClient();
            CloudTable statsTable = tableClient.GetTableReference(tableName);
            await statsTable.CreateIfNotExistsAsync();
            log.LogInformation($"Connected to table: {tableName}");
            return statsTable;
        }

        private static async Task AddNewCountToTable(int countNow, SortedDictionary<DateTime, int> counts, CloudTable table, ILogger log)
        {
            int countPreviously = counts.Values.Last();
            log.LogInformation($"Current member count is {countNow:N0} (previously {countPreviously:N0})");
            if (countNow != countPreviously)
            {
                await AddRecordToTable(table, countNow, log);
                counts.Add(DateTime.UtcNow, countNow);
            }
            else
            {
                log.LogInformation($"No change, so not adding a record to the table.");
            }
        }

        private static async Task<SortedDictionary<DateTime, int>> GetHistoricalCountsFromTable(CloudTable table, ILogger log)
        {
            var countsByDate = new SortedDictionary<DateTime, int>();
            TableQuery<GeneralMemberCount> query = new();
            foreach (GeneralMemberCount memberRecord in await table.ExecuteQuerySegmentedAsync(query, null))
            {
                countsByDate.Add(memberRecord.DateTime, memberRecord.Count);
            }
            log.LogInformation($"Read and sorted {countsByDate.Count} records from the table.");
            return countsByDate;
        }

        private static async Task AddRecordToTable(CloudTable table, int count, ILogger log)
        {
            GeneralMemberCount entity = new(count);
            TableOperation insertOrMergeOperation = TableOperation.InsertOrMerge(entity);
            await table.ExecuteAsync(insertOrMergeOperation);
            log.LogInformation($"Successfully added a record to the table.");
        }

        private static async Task WriteJsonToWebStorage(CloudStorageAccount account, SortedDictionary<DateTime, int> counts, ILogger log)
        {
            const string FILENAME = "general-member-count.json";
            string json = Json.DatedCountsToJson(counts);

            CloudBlobClient blobClient = account.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("$web");
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(FILENAME);
            blockBlob.Properties.ContentType = "application/json";
            await blockBlob.UploadTextAsync(json);
            log.LogInformation($"Wrote web: {FILENAME}");
        }
    }
}
