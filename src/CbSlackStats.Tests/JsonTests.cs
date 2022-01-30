using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CbSlackStats.Tests
{
    internal class JsonTests
    {
        [Test]
        public void Test_JsonParsing_ChannelMemberCount()
        {
            string jsonFilePath = Path.Combine(SampleData.DataFolder, "conversations.info.json");
            Assert.That(File.Exists(jsonFilePath));
            string txt = File.ReadAllText(jsonFilePath);
            using JsonDocument doc = JsonDocument.Parse(txt);

            Assert.IsTrue(SlackAPI.RequestIsOK(doc));
            Assert.DoesNotThrow(() => SlackAPI.ThrowIfRequestIsNotOK(doc));
            Assert.AreEqual(6401, SlackAPI.MemberCountFromChannelInfo(doc));
        }

        private static SortedDictionary<DateTime, int> GetRandomCountRecords()
        {
            Random rand = new(0);
            SortedDictionary<DateTime, int> counts = new();
            int count = 0;
            DateTime dt = new(2000, 1, 1);
            for (int i = 0; i < 10; i++)
            {
                dt = dt.AddSeconds(rand.Next(10_000_000));
                count += rand.Next(100);
                counts.Add(dt, count);
            }
            return counts;
        }

        [Test]
        public void Test_DatedCounts_ToJson()
        {
            SortedDictionary<DateTime, int> counts = GetRandomCountRecords();
            string json = Json.DatedCountsToJson(counts);
            Console.WriteLine(json);

            Assert.IsNotNull(json);
            Assert.Greater(json.Length, 100);
        }

        [Test]
        public void Test_DatedCounts_FromJson()
        {
            string jsonFilePath = Path.Combine(SampleData.DataFolder, "general-member-count.json");
            string json = File.ReadAllText(jsonFilePath);
            SortedDictionary<DateTime, int> counts = Json.DatedCountsFromJson(json);

            Assert.IsNotNull(counts);
            Assert.AreEqual(118, counts.Count);
        }
    }
}
