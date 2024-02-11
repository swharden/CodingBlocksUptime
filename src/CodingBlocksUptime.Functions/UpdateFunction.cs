using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace CodingBlocksUptime.Functions;

public class UpdateFunction(ILoggerFactory loggerFactory)
{
    private readonly ILogger Logger = loggerFactory.CreateLogger<UpdateFunction>();
    private const string DB_FILENAME = "codingblocks.net.csv";
    private const string LOG_FILENAME = "codingblocks.net-log.txt";

    [Function("UpdateFunction")]
    public void Run(
#if DEBUG
    [TimerTrigger("0 0 0 1 1 0", RunOnStartup = true)] TimerInfo myTimer
#else
    [TimerTrigger("0 0 * * * *")] TimerInfo myTimer
#endif
        )
    {
        Logger.LogInformation("Function execution starting...");
        HttpResponseInfo response = HttpTester.GetResponseInfo("https://codingblocks.net");
        Console.WriteLine(response);

        BlobContainerClient containerClient = GetBlobContainer();
        Database db = LoadDatabaseFromFile(containerClient);
        db.Records.Add(response.DatabaseRecord);
        SaveDatabaseToFile(db, containerClient);
        CreateLogFile(containerClient);
        Logger.LogInformation("Function execution complete.");
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

    private Database LoadDatabaseFromFile(BlobContainerClient containerClient)
    {
        Logger.LogInformation("Downloading existing records from CSV file...");
        BlobClient blobClient = containerClient.GetBlobClient(DB_FILENAME);
        using MemoryStream memoryStream = new();
        blobClient.DownloadTo(memoryStream);
        string contentString = System.Text.Encoding.UTF8.GetString(memoryStream.ToArray());
        Logger.LogInformation("Read {LENGTH} bytes.", contentString.Length);

        Database db = Database.FromCsv(contentString);
        Logger.LogInformation("Created database containing {COUNT} records.", db.Records.Count);
        return db;
    }

    private void SaveDatabaseToFile(Database db, BlobContainerClient containerClient)
    {
        string txt = db.ToCsv();
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(txt);
        Logger.LogInformation("Writing {LENGTH} bytes...", bytes.Length);

        using MemoryStream ms = new(bytes);
        BlobClient blobClient = containerClient.GetBlobClient(DB_FILENAME);
        blobClient.Upload(ms, overwrite: true);
        blobClient.SetHttpHeaders(new BlobHttpHeaders() { ContentType = "text/plain" });
    }

    private void CreateLogFile(BlobContainerClient containerClient)
    {
        string text = DateTime.UtcNow.ToUniversalTime().ToString("o");
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(text);
        Logger.LogInformation("Writing {LENGTH} byte log file...", bytes.Length);

        using MemoryStream ms2 = new(bytes);
        BlobClient blobClient = containerClient.GetBlobClient(LOG_FILENAME);
        blobClient.Upload(ms2, overwrite: true);
        blobClient.SetHttpHeaders(new BlobHttpHeaders() { ContentType = "text/plain" });
    }
}
