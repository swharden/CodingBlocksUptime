using Azure.Storage.Blobs;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace CodingBlocksUptime.Functions;

public class UpdateFunction(ILoggerFactory loggerFactory)
{
    private readonly ILogger Logger = loggerFactory.CreateLogger<UpdateFunction>();

    [Function("UpdateFunction")]
    public void Run(
#if DEBUG
    [TimerTrigger("0 0 0 1 1 0", RunOnStartup = true)] TimerInfo myTimer
#else
    [TimerTrigger("0 0 * * * *")] TimerInfo myTimer
#endif
        )
    {
        HttpResponseInfo response = HttpTester.GetResponseInfo("https://codingblocks.net");
        Console.WriteLine(response);

        BlobContainerClient containerClient = GetBlobContainer();
    }

    private BlobContainerClient GetBlobContainer()
    {
        Logger.LogInformation("connecting to blob storage...");
        string connectionString = Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING")
            ?? throw new InvalidOperationException("connection string environment variable is missing");
        BlobServiceClient blobServiceClient = new(connectionString);
        BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient("$web");
        Logger.LogInformation("connected.");
        return containerClient;
    }
}
