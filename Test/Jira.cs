using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Test
{
    class Jira : Celin.Jira.Server
    {
        public Jira(IConfiguration config, ILogger logger)
            : base(config["jiraUrl"], config["jiraToken"], logger) { }
    }
}
