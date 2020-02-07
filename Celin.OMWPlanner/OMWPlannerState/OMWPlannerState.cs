using BlazorState;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Celin
{
    public partial class OMWPlannerState : State<OMWPlannerState>
    {
        public event EventHandler Changed;
        public DateTime LastUpdate { get; set; }
        public string JiraProjectKey { get; set; }
        public IEnumerable<string> JiraStatusKeys { get; set; }
        public string JiraTriggerEstimate { get; set; }
        public string JiraTriggerOMW { get; set; }
        public string ErrorMessage { get; set; }
        public Jira.Response.Project JiraProject { get; set; }
        public List<Jira.Response.Status> JiraStatuses { get; set; }
        public List<Jira.Response.Issue> JiraIssues { get; set; }
        public List<F98220.Row> OMWProjects { get; set; }
        public W98220WC.Response OMWProjectForm { get; set; }
        public object DemoRequest { get; set; }
        public override void Initialize() { }
        public OMWPlannerState(IConfiguration config)
        {
            JiraProjectKey = config["jiraProjectKey"];
            JiraStatusKeys = config["jiraStatusKeys"]?.Split(',');
            JiraTriggerEstimate = config["jiraTriggerEstimate"];
            JiraTriggerOMW = config["jiraTriggerOMW"];
        }
    }
}
