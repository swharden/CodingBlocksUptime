using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CbSlackStats.Tests
{
    internal class PlotTests
    {
        [Test]
        public void Test_Plot_Count()
        {
            string jsonFilePath = Path.Combine(SampleData.DataFolder, "general-member-count.json");
            string json = File.ReadAllText(jsonFilePath);
            SortedDictionary<DateTime, int> counts = Json.DatedCountsFromJson(json);
            Dictionary<DateTime, string> episodes = GetEpisodesByDate();
            byte[] bytes = Plot.GeneratePng(800, 600, counts, episodes);

            Assert.IsNotNull(bytes);
            Assert.IsNotEmpty(bytes);

            string filePath = Path.GetFullPath("test.png");
            File.WriteAllBytes(filePath, bytes);
            Console.WriteLine(filePath);
        }

        [Test]
        public void Test_SampleEpisodeLogFile_CanBeRead()
        {
            Dictionary<DateTime, string> episodes = GetEpisodesByDate();
            Assert.IsNotEmpty(episodes);
            foreach (var episode in episodes)
                Console.WriteLine(episode);
        }

        public static Dictionary<DateTime, string> GetEpisodesByDate()
        {
            Dictionary<DateTime, string> episodesByDate = new();

            string feedFilePath = Path.GetFullPath("../../../../../dev/sample-data/feed-podcast.xml");
            string xmlText = File.ReadAllText(feedFilePath);
            XDocument doc = XDocument.Parse(xmlText);
            foreach (XElement itemELement in doc.Descendants("item"))
            {
                DateTime datetime = DateTime.Parse(itemELement.Element("pubDate")!.Value);
                string title = itemELement.Element("title")!.Value;
                episodesByDate[datetime] = title;
            }

            return episodesByDate;
        }
    }
}
