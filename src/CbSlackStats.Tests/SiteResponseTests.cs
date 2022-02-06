using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CbSlackStats.Tests
{
    internal class SiteResponseTests
    {
        [Test]
        public async void Test_Website_Response()
        {
            string url = "http://example.com";
            SitePerfRecord perf = await SitePerf.Measure(url);
            Console.WriteLine($"{url} performance: {perf}");
        }
    }
}
