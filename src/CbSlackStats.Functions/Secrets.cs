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
            try
            {
                var config = new ConfigurationBuilder().AddUserSecrets<UpdateGeneralMemberCount>().Build();
                string userSecretValue = config[secretName];
                if (userSecretValue is not null)
                    return userSecretValue;
            }
            catch (System.IO.FileNotFoundException)
            {
                string envSecretValue = Environment.GetEnvironmentVariable(secretName, EnvironmentVariableTarget.Process);
                if (envSecretValue is not null)
                    return envSecretValue;
            }

            throw new InvalidOperationException($"Could not load secret: {secretName}");
        }
    }
}
