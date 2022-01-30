using System;
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
            log.LogInformation($"UpdateGeneralMemberCount running at {DateTime.UtcNow}");

            string token = Secrets.GetSecret("CB_SLACK_TOKEN");
            int memberCount = await SlackAPI.GetGeneralMemberCountAsync(token);
            log.LogInformation($"The General channel has {memberCount:N0} members");

            string storageConnectionString = Secrets.GetSecret("CB_STORAGE_CONNECTION_STRING");
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable statsTable = tableClient.GetTableReference("GeneralMemberCount");
            await statsTable.CreateIfNotExistsAsync();

            TableEntities.GeneralMemberCount entity = new(memberCount);
            TableOperation insertOrMergeOperation = TableOperation.InsertOrMerge(entity);
            await statsTable.ExecuteAsync(insertOrMergeOperation);
            log.LogInformation($"Successfully added a record to the table.");

            var countsByDate = new System.Collections.Generic.SortedDictionary<DateTime, int>();
            TableQuery<GeneralMemberCount> query = new();
            foreach (GeneralMemberCount memberRecord in await statsTable.ExecuteQuerySegmentedAsync(query, null))
            {
                countsByDate.Add(memberRecord.DateTime, memberRecord.Count);
            }
            log.LogInformation($"Read and sorted {countsByDate.Count} records from the table.");

            string json = Json.DatedCountsToJson(countsByDate);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("$web");
            CloudBlockBlob blockBlob = container.GetBlockBlobReference("general-member-count.json");
            blockBlob.Properties.ContentType = "application/json";
            await blockBlob.UploadTextAsync(json);
            log.LogInformation($"Wrote web: general-member-count.json");
        }
    }
}
