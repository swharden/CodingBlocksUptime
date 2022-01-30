using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace CbSlackStats.Tests
{
    public class SlackApiTests
    {
        const string TOKEN_ENV_VARNAME = "CB_SLACK_TOKEN";

        [SetUp]
        public void Setup_SetEnvironmentVariableFromUserSecret()
        {
            try
            {
                string token = new ConfigurationBuilder().AddUserSecrets<SlackApiTests>().Build()[TOKEN_ENV_VARNAME];
                if (token is not null)
                    Environment.SetEnvironmentVariable(TOKEN_ENV_VARNAME, token);
            }
            catch (System.IO.FileNotFoundException)
            {
                // no user secrets found, probably because we are in GitHub Actions or Azure Pipelines
                return;
            }
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

        [Test]
        public void Test_Api_GetGeneralMemberCount()
        {
            string token = GetTokenFromEnvironmentVariable();
            Task<int> task = SlackAPI.GetGeneralMemberCountAsync(token);
            int memberCount = task.Result;
            Console.WriteLine($"The General channel has {memberCount:N0} members");
            Assert.Greater(memberCount, 100);
        }
    }
}