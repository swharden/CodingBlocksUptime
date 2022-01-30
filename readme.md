# ![](docs/favicon.png)  CodingBlocks Slack Stats

[![Build and Test](https://github.com/swharden/cb-slack-stats/actions/workflows/ci.yaml/badge.svg)](https://github.com/swharden/cb-slack-stats/actions/workflows/ci.yaml)

**This project regularly records statistics about the [CodingBlock Slack](https://www.codingblocks.net/slack/) so they can be viewed in historical context.** An Azure Function periodically engages the [Slack API](https://api.slack.com/methods) and logs metrics of interest using blob table storage. Historical records are analyzed and made available as JSON files using blob storage configured as a static website. This repository serves as [GitHub Pages](https://pages.github.com/) index that uses Vanilla JS to retrieve the JSON and display it interactively using [Google Charts](https://developers.google.com/chart/interactive/docs/gallery). A static image is also generated using [ScottPlot](https://scottplot.net/) suitable for inclusion in JavaScript-free static pages.

**Interactive Graph: https://swharden.github.io/cb-slack-stats/**

<div align="center">
<a href='https://cbslackstats.z20.web.core.windows.net/general-member-count.png'><img src='https://cbslackstats.z20.web.core.windows.net/general-member-count.png'></a>
</div>

## Developer Notes

### GitHub Actions

Pushing a commit to the main branch of this repository triggers GitHub actions to build, test, and deploy the Azure Functions application and website into production. See run logs in the [actions tab](https://github.com/swharden/cb-slack-stats/actions) or inspect [ci.yaml](.github/workflows/ci.yaml) for implementation details.

### Trigger Updates Manually

By default an update is triggered hourly. An API endpoint is available to trigger an update manually, but it requires the master key for the Azure Functions app. For more see [Manually run a non HTTP-triggered function
](https://docs.microsoft.com/en-us/azure/azure-functions/functions-manually-run-non-http).

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
  * The API token is stored with the other [Azure Functions Application Settings](https://docs.microsoft.com/en-us/azure/app-service/configure-common) and accessed when running in the cloud via environment variables.