using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http;

namespace Celin
{
    public class JiraService : Jira.Server
    {
        public JiraService(IConfiguration config, ILogger<JiraService> logger, IHttpClientFactory httpClient)
            : base(config["jiraUrl"], config["jiraToken"], logger, httpClient.CreateClient()) { }
    }
}
