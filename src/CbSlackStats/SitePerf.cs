using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace CbSlackStats
{
    public static class SitePerf
    {
        public static async Task<SitePerfRecord> Measure(string url)
        {
            HttpClient client = new();
            Stopwatch sw = Stopwatch.StartNew();
            using HttpResponseMessage response = await client.GetAsync(url);
            sw.Stop();
            string contentText = await response.Content.ReadAsStringAsync();

            return new SitePerfRecord()
            {
                DateTime = DateTime.UtcNow,
                LoadTime = sw.Elapsed.TotalMilliseconds,
                SizeBytes = contentText.Length,
                ResponseCode = (int)response.StatusCode,
            };
        }
    }
}
