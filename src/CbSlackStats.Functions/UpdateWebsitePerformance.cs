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
    public class UpdateWebsitePerformance
    {
        [FunctionName("UpdateWebsitePerformance")]
        public async Task Run([TimerTrigger("0 0 * * * *")] TimerInfo myTimer, ILogger log)
        {
            const string url = "https://www.codingblocks.net/";
            SitePerfRecord perfRecordNow = await SitePerf.Measure(url);
            log.LogInformation($"{url} performance: {perfRecordNow}");

            CloudStorageAccount storageAccount = ConnectToStorageAccount();
            CloudTable table = await ConnectToTable(storageAccount, log);
            await AddRecordToTable(table, perfRecordNow, log);
            SitePerfRecord[] perfRecords = await GetHistoricalRecords(table, log);
            await WriteJsonToWebStorage(storageAccount, perfRecords, log);
        }

        private static CloudStorageAccount ConnectToStorageAccount()
        {
            string storageConnectionString = Secrets.GetSecret("CB_STORAGE_CONNECTION_STRING");
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            return storageAccount;
        }

        private static async Task<CloudTable> ConnectToTable(CloudStorageAccount account, ILogger log)
        {
            const string tableName = "SitePerformance";
            CloudTableClient tableClient = account.CreateCloudTableClient();
            CloudTable statsTable = tableClient.GetTableReference(tableName);
            await statsTable.CreateIfNotExistsAsync();
            log.LogInformation($"Connected to table: {tableName}");
            return statsTable;
        }

        private static async Task AddRecordToTable(CloudTable table, SitePerfRecord perf, ILogger log)
        {
            WebsitePerformance entity = new(perf);
            TableOperation insertOrMergeOperation = TableOperation.InsertOrMerge(entity);
            await table.ExecuteAsync(insertOrMergeOperation);
            log.LogInformation($"Successfully added a record to the table.");
        }

        private static async Task<SitePerfRecord[]> GetHistoricalRecords(CloudTable table, ILogger log)
        {
            TableQuery<WebsitePerformance> tableQuery = new() { TakeCount = 100_000 };
            TableContinuationToken continuationToken = default;
            List<WebsitePerformance> results = new();

            do
            {
                var queryResult = await table.ExecuteQuerySegmentedAsync(tableQuery, continuationToken);
                results.AddRange(queryResult.Results);
                continuationToken = queryResult.ContinuationToken;
            } while (continuationToken is not null);

            SitePerfRecord[] sortedPerfRecords = results
                .Select(x => CreatePerfRecord(x))
                .OrderBy(x => x.DateTime)
                .ToArray();

            log.LogInformation($"Read and sorted {sortedPerfRecords.Length} records from the table.");
            return sortedPerfRecords;
        }

        private static SitePerfRecord CreatePerfRecord(WebsitePerformance x)
        {
            return new SitePerfRecord()
            {
                DateTime = x.DateTime,
                ResponseCode = x.ResponseCode,
                LoadTime = x.LoadTime,
                SizeBytes = x.SizeBytes,
            };
        }

        private static async Task WriteJsonToWebStorage(CloudStorageAccount account, SitePerfRecord[] records, ILogger log)
        {
            const string FILENAME = "performance.json";
            string json = Json.WebsitePerformanceToJson(records);
            CloudBlobClient blobClient = account.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("$web");
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(FILENAME);
            blockBlob.Properties.ContentType = "application/json";
            await blockBlob.UploadTextAsync(json);
            log.LogInformation($"Wrote web: {FILENAME}");
        }
    }
}
