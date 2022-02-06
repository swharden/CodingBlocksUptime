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
        public void Test_Website_Response()
        {
            string url = "http://example.com";
            (int code, int length, double msec) = Website.GetResponseTime(url);
            Console.WriteLine($"Request to {url} returned {code} with {length:N0} bytes in {msec:N3} ms");
        }
    }
}
