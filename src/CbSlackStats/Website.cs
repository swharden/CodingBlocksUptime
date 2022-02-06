using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace CbSlackStats
{
    public static class Website
    {
        public static (int code, int length, double msec) GetResponseTime(string url)
        {
            return GetResponseTimeAsync(url).Result;
        }

        public static async Task<(int, int, double)> GetResponseTimeAsync(string url)
        {
            HttpClient client = new();
            Stopwatch sw = Stopwatch.StartNew();
            using HttpResponseMessage response = await client.GetAsync(url);
            sw.Stop();
            double msec = sw.Elapsed.TotalMilliseconds;
            using HttpContent content = response.Content;
            string contentText = await content.ReadAsStringAsync();
            int length = contentText.Length;
            int code = (int)response.StatusCode;
            return (code, length, msec);
        }
    }
}
