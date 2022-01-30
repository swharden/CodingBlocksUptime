using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CbSlackStats
{
    public static class SlackAPI
    {
        public static async Task<int> GetGeneralMemberCountAsync(string token, string channelID)
        {
            var uri = new Uri($"https://slack.com/api/conversations.info?channel={channelID}&include_num_members=true");
            var auth = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            HttpClient client = new();
            client.DefaultRequestHeaders.Add("User-Agent", "CB Slack Stats App");
            client.DefaultRequestHeaders.Authorization = auth;

            Task<string> getStringTask = client.GetStringAsync(uri);
            string txt = await getStringTask;

            return txt.Length;
        }
    }
}
