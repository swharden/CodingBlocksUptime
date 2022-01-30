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
        private readonly static string DataFolder = GetDataFolder();

        private static string GetDataFolder(int maxLevelsUp = 10)
        {
            string dirname = Path.GetFullPath("./");
            for (int i = 0; i < maxLevelsUp; i++)
            {
                string dataFolderPath = Path.Combine(dirname ?? "", "dev/sample-data");
                if (Directory.Exists(dataFolderPath))
                    return dataFolderPath;
                else
                    dirname = Path.GetDirectoryName(dirname) ?? "";
            }

            throw new InvalidOperationException("could not locate data folder");
        }

        [Test]
        public void Test_JsonParsing_ChannelMemberCount()
        {
            string jsonFilePath = Path.Combine(DataFolder, "conversations.info.json");
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
    }
}
