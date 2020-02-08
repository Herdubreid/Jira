using BlazorState;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Celin
{
    public enum OMWRoles
    {
        All,
        Manager,
        Developer,
        Tester,
        CNC
    }
    public partial class OMWPlannerState : State<OMWPlannerState>
    {
        public event EventHandler Changed;
        public DateTime LastUpdate { get; set; }
        [JsonIgnore]
        public string JiraProjectKey { get; }
        [JsonIgnore]
        public Dictionary<OMWRoles, IEnumerable<string>> JiraRoleStatusKeys { get; }
        [JsonIgnore]
        public string JiraTriggerEstimate { get; set; }
        [JsonIgnore]
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
            JiraRoleStatusKeys = new Dictionary<OMWRoles, IEnumerable<string>>
            {
                { OMWRoles.All, config["jiraStatusKeys"]?.Split(',') },
                { OMWRoles.Manager, config["jiraManagerStatusKeys"]?.Split(',') },
                { OMWRoles.Developer, config["jiraDeveloperStatusKeys"]?.Split(',') },
                { OMWRoles.Tester, config["jiraTesterStatusKeys"]?.Split(',') },
                { OMWRoles.CNC, config["jiraCNCStatusKeys"]?.Split(',') }
            };
            JiraTriggerEstimate = config["jiraTriggerEstimate"];
            JiraTriggerOMW = config["jiraTriggerOMW"];
        }
    }
}
