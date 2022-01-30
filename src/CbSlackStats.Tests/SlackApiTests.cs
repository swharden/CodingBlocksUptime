using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace CbSlackStats.Tests
{
    public class SlackApiTests
    {
        const string TOKEN_ENV_VARNAME = "cbSlackStatsToken";
        const string CB_GENERAL_CHANNEL_ID = "C0GL77VA6"; // found from conversations.list

        [SetUp]
        public void Setup_SetEnvironmentVariableFromUserSecret()
        {
            string token = new ConfigurationBuilder().AddUserSecrets<SlackApiTests>().Build()["slacktoken"];
            if (token is not null)
                Environment.SetEnvironmentVariable(TOKEN_ENV_VARNAME, token);
        }

        public static string GetTokenFromEnvironmentVariable()
        {
            return Environment.GetEnvironmentVariable(TOKEN_ENV_VARNAME) ??
                throw new InvalidOperationException($"environment variables do not contain {TOKEN_ENV_VARNAME}");
        }

        [Test]
        public void Test_Token_CanBeRetrieved()
        {
            string token = GetTokenFromEnvironmentVariable();
            Assert.IsNotNull(token);
        }

        [Test]
        public void Test_Token_IsProperFormat()
        {
            string token = GetTokenFromEnvironmentVariable();
            Assert.That(token.StartsWith("xoxb-"));
        }
}