using BlazorState;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace Celin
{
    public partial class OMWPlannerState : State<OMWPlannerState>
    {
        public IConfiguration Config { get; }
        public event EventHandler Changed;
        public DateTime LastUpdate { get; set; }

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
            Config = config;
        }
    }
}
