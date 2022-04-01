using Microsoft.Extensions.Configuration;
using System;

namespace CbSlackStats.Functions
{
    internal static class Secrets
    {
        /// <summary>
        /// Get a local user secret and if not found return the environment variable with the same name
        /// </summary>
        internal static string GetSecret(string secretName)
        {
            string azureFunctionsId = Environment.GetEnvironmentVariable("WEBSITE_INSTANCE_ID");
            bool isRunningLocally = string.IsNullOrEmpty(azureFunctionsId);

            if (isRunningLocally)
            {
                var config = new ConfigurationBuilder().AddUserSecrets<UpdateGeneralMemberCount>().Build();
                string secretValue = config[secretName];

                if (string.IsNullOrEmpty(secretValue))
                    throw new InvalidOperationException($"Local user secret '{secretName}' was not found");

                return secretValue;
            }
            else
            {
                string secretValue = Environment.GetEnvironmentVariable(secretName, EnvironmentVariableTarget.Process);

                if (string.IsNullOrEmpty(secretValue))
                    throw new InvalidOperationException($"Environment secret '{secretName}' was not found");

                return secretValue;
            }

        }
    }
}
