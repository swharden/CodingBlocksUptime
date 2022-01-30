using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            byte[] bytes = Plot.GeneratePng(600, 400, counts);
            Assert.IsNotNull(bytes);
            Assert.IsNotEmpty(bytes);

            string filePath = Path.GetFullPath("test.png");
            File.WriteAllBytes(filePath, bytes);
            Console.WriteLine(filePath);
        }
    }
}
