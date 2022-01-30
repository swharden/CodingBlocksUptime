using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CbSlackStats.Tests
{
    internal static class SampleData
    {
        internal readonly static string DataFolder = GetDataFolder();

        internal static string GetDataFolder(int maxLevelsUp = 10)
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
    }
}
