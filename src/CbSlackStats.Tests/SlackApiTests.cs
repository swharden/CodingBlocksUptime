using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace CbSlackStats.Tests
{
    public class SlackApiTests
    {
        private static string SlackToken => GetSecret("CB_SLACK_TOKEN");

        /// <summary>
        /// Get a local user secret and if not found return the environment variable with the same name
        /// </summary>
        private static string GetSecret(string secretName)
        {
            try
            {
                var config = new ConfigurationBuilder().AddUserSecrets<SlackApiTests>().Build();
                string userSecretValue = config[secretName];
                if (userSecretValue is not null)
                    return userSecretValue;
            }
            catch (System.IO.FileNotFoundException)
            {
                string? envSecretValue = Environment.GetEnvironmentVariable(secretName, EnvironmentVariableTarget.Process);
                if (envSecretValue is not null)
                    return envSecretValue;
            }

            throw new InvalidOperationException($"Could not load secret: {secretName}");
        }

        [Test]
        public void Test_Token_CanBeRetrieved()
        {
            Assert.IsNotNull(SlackToken);
        }

        [Test]
        public void Test_Token_IsProperFormat()
        {
            Assert.That(SlackToken.StartsWith("xoxb-"));
        }

        [Test]
        public void Test_Api_GetGeneralMemberCount()
        {
            Task<int> task = SlackAPI.GetGeneralMemberCountAsync(SlackToken);
            int memberCount = task.Result;
            Console.WriteLine($"The General channel has {memberCount:N0} members");
            Assert.Greater(memberCount, 100);
        }
    }
}