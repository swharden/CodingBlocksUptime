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
        public static byte[] GeneratePng(int width, int height, SortedDictionary<DateTime, int> counts)
        {
            double[] xs = counts.Keys.Select(x => x.ToOADate()).ToArray();
            double[] ys = counts.Values.Select(x => (double)x).ToArray();

            var plt = new ScottPlot.Plot(width, height);
            plt.Title("CodingBlocks Slack");
            plt.YLabel("Members");
            plt.AddScatterStep(xs, ys);
            plt.XAxis.DateTimeFormat(true);

            byte[] bytes = plt.GetImageBytes();
            return bytes;
        }
    }
}
