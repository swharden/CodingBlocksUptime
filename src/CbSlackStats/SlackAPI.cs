using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CbSlackStats
{
    public static class SlackAPI
    {
        private static readonly string CB_GENERAL_CHANNEL_ID = "C0GL77VA6"; // found from conversations.list

        public static async Task<int> GetGeneralMemberCountAsync(string token)
        {
            var uri = new Uri($"https://slack.com/api/conversations.info?channel={CB_GENERAL_CHANNEL_ID}&include_num_members=true");
            var auth = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            HttpClient client = new();
            client.DefaultRequestHeaders.Add("User-Agent", "CB Slack Stats App");
            client.DefaultRequestHeaders.Authorization = auth;

            Task<string> getStringTask = client.GetStringAsync(uri);
            string txt = await getStringTask;

            // TODO: JSON PARSING
            return txt.Length;
        }
    }
}
