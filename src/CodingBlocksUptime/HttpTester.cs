using System.Diagnostics;

namespace CodingBlocksUptime;

public static class HttpTester
{
    public static HttpResponseInfo GetResponseInfo(string url, int timeoutMsec = 20_000) => GetResponseInfoAsync(url, timeoutMsec).Result;

    public static async Task<HttpResponseInfo> GetResponseInfoAsync(string url, int timeoutMsec)
    {
        Stopwatch sw = Stopwatch.StartNew();
        try
        {
            HttpClient client = new()
            {
                Timeout = TimeSpan.FromMilliseconds(timeoutMsec)
            };
            using HttpResponseMessage response = await client.GetAsync(url);
            string contentText = await response.Content.ReadAsStringAsync();
            return new HttpResponseInfo(sw.Elapsed.TotalMilliseconds, (int)response.StatusCode, contentText);
        }
        catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
        {
            return new HttpResponseInfo(sw.Elapsed.TotalMilliseconds, 524, "timed out");
        }
        catch (Exception ex)
        {
            return new HttpResponseInfo(sw.Elapsed.TotalMilliseconds, 0, ex.Message);
        }
    }
}
