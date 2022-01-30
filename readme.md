# CodingBlocks Slack Stats

This project aims to automate the logging and display of stats related to the CodingBlock Slack: https://www.codingblocks.net/slack/

## Developer Notes

### Accessing Secrets
Let's _not_ commit the slack API key to source control. However, this secret must be accessed in a few different places:

**Testing locally**
* The API token is stored locally using the [.NET Secret Manager](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-6.0&tabs=windows#secret-manager) ([notes](https://swharden.com/blog/2021-10-09-console-secrets/))
* When tests are run locally this is loaded from the secret manager and put into an environment variable
* _Warning: The Secret Manager tool doesn't encrypt the stored secrets and shouldn't be treated as a trusted store. It's for development purposes only. The keys and values are stored in a JSON configuration file in the user profile directory._

```
dotnet user-secrets init
dotnet user-secrets set slacktoken xoxb-1234567890
```

**Testing with GitHub Actions**
  * The API token is stored as a [GitHub Encrypted Secret](https://docs.github.com/en/actions/security-guides/encrypted-secrets) that can be accessed in a GitHub actions workflow.

  ```cs
  steps:
  - name: Step that needs the secret
    env:
      cbSlackStatsToken: ${{ secrets.SLACK_TOKEN }}
  ```

**Executing Azure Functions**
  * _not yet implemented_