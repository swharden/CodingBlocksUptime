using System.Text.Json;

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
            using JsonDocument doc = JsonDocument.Parse(txt);
            ThrowIfRequestIsNotOK(doc);

            int memberCount = MemberCountFromChannelInfo(doc);
            return memberCount;
        }

        public static bool RequestIsOK(JsonDocument doc)
        {
            return doc.RootElement.GetProperty("ok").GetBoolean();
        }

        public static void ThrowIfRequestIsNotOK(JsonDocument doc)
        {
            if (!RequestIsOK(doc))
                throw new InvalidOperationException("request did not return OK");
        }

        public static int MemberCountFromChannelInfo(JsonDocument doc)
        {
            return doc.RootElement.GetProperty("channel").GetProperty("num_members").GetInt32();
        }
    }
}
