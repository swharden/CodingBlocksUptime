using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace CbSlackStats.Functions
{
    public class UpdateStats
    {
        [FunctionName("UpdateStats")]
        public async Task Run([TimerTrigger("0 0 * * * *")] TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"UpdateStats running at {DateTime.UtcNow}");

            string token = GetSecret("CB_SLACK_TOKEN");
            int memberCount = await SlackAPI.GetGeneralMemberCountAsync(token);
            log.LogInformation($"The General channel has {memberCount:N0} members");

            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(GetSecret("CB_STORAGE_CONNECTION_STRING"));
            CloudTableClient tableClient = cloudStorageAccount.CreateCloudTableClient();
            CloudTable statsTable = tableClient.GetTableReference("GeneralMemberCount");
            await statsTable.CreateIfNotExistsAsync();

            TableEntities.GeneralMemberCount entity = new() { Count = memberCount };
            TableOperation insertOrMergeOperation = TableOperation.InsertOrMerge(entity);
            await statsTable.ExecuteAsync(insertOrMergeOperation);

            log.LogInformation($"Successfully added a record to the table.");
        }

        /// <summary>
        /// Get a local user secret and if not found return the environment variable with the same name
        /// </summary>
        private static string GetSecret(string secretName)
        {
            try
            {
                var config = new ConfigurationBuilder().AddUserSecrets<UpdateStats>().Build();
                string userSecretValue = config[secretName];
                if (userSecretValue is not null)
                    return userSecretValue;
            }
            catch (System.IO.FileNotFoundException)
            {
                string envSecretValue = Environment.GetEnvironmentVariable(secretName, EnvironmentVariableTarget.Process);
                if (envSecretValue is not null)
                    return envSecretValue;
            }

            throw new InvalidOperationException($"Could not load secret: {secretName}");
        }
    }
}
