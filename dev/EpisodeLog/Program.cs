using System.Xml.Linq;

namespace EpisodeLog;

public static class Program
{
    public static void Main()
    {
        Dictionary<DateOnly, string> episodes = GetEpisodesByDate();
        DateOnly[] dates = episodes.Keys.OrderBy(x => x).ToArray();

        for (int i = 0; i < dates.Length; i++)
        {
            Console.WriteLine($"{i + 1} [{dates[i]}] {episodes[dates[i]]}");
        }
    }

    public static Dictionary<DateOnly, string> GetEpisodesByDate()
    {
        Dictionary<DateOnly, string> episodesByDate = new();

        string feedFilePath = Path.GetFullPath("../../../../../dev/sample-data/feed-podcast.xml");
        string xmlText = File.ReadAllText(feedFilePath);
        XDocument doc = XDocument.Parse(xmlText);
        foreach (XElement itemELement in doc.Descendants("item"))
        {
            DateTime datetime = DateTime.Parse(itemELement.Element("pubDate")!.Value);
            DateOnly date = DateOnly.FromDateTime(datetime);
            string title = itemELement.Element("title")!.Value;
            episodesByDate[date] = title;
        }

        return episodesByDate;
    }

    public static void DownloadFeed(string url = "https://www.codingblocks.net/feed/podcast", string saveAs = "feed-podcast.xml")
    {
        saveAs = Path.GetFullPath(saveAs);
        Console.WriteLine($"Downloading: {url}");
        DownloadFileAsync(url, saveAs).GetAwaiter().GetResult();
        Console.WriteLine($"Saved: {saveAs}");
    }

    public static async Task DownloadFileAsync(string url, string savePath)
    {
        using HttpClient client = new();
        using HttpResponseMessage response = await client.GetAsync(url);
        using HttpContent content = response.Content;
        byte[] bytes = await content.ReadAsByteArrayAsync();
        File.WriteAllBytes(savePath, bytes);
    }
}