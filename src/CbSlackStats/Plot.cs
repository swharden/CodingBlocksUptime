using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace CbSlackStats
{
    public static class Plot
    {
        public static byte[] GeneratePng(int width, int height, SortedDictionary<DateTime, int> counts, Dictionary<DateTime, string> episodes)
        {
            DateTime viewMinX = DateTime.Now - TimeSpan.FromDays(180);
            DateTime viewMaxX = DateTime.Now + TimeSpan.FromDays(1);

            var recentCounts = counts.Where(x => x.Key > viewMinX);
            double viewMinY = recentCounts.First().Value - 10;
            double viewMaxY = recentCounts.Last().Value + 10;

            // create the plot
            var plt = new ScottPlot.Plot(width, height);
            plt.Title("CodingBlocks Slack");
            plt.YLabel("Members");

            // add member count scatter plot
            double[] xs = recentCounts.Select(x => x.Key.ToOADate()).ToArray();
            double[] ys = recentCounts.Select(x => (double)x.Value).ToArray();
            plt.AddScatter(xs, ys);
            plt.XAxis.DateTimeFormat(true);

            // mark episodes
            foreach (var episode in episodes.Where(x => x.Key > viewMinX))
            {
                DateTime date = episode.Key;
                string title = episode.Value.Replace("–", "-").Replace("—", "-").Split("-")[0];
                plt.AddVerticalLine(date.ToOADate(), System.Drawing.Color.Red);
                var an = plt.AddText(title, date.ToOADate(), viewMinY, 12, System.Drawing.Color.Red);
                an.Font.Rotation = -90;
            }

            plt.SetAxisLimits(xMin: viewMinX.ToOADate(), xMax: viewMaxX.ToOADate(), viewMinY, viewMaxY);

            return plt.GetImageBytes();
        }
    }
}
