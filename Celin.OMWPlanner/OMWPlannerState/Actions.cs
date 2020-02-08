using BlazorState;
using System.Collections.Generic;

namespace Celin
{
    public partial class OMWPlannerState
    {
        public class JiraEditIssueAction : IAction
        {
            public string IssueIdOrKey { get; set; }
            public Jira.Request.Fields Fields { get; set; }
            public Jira.Request.Update Update { get; set; }
        }
        public class JiraIssueTransitionAction : IAction
        {
            public string IssueIdOrKey { get; set; }
            public string TransitionId { get; set; }
            public string Comment { get; set; }
        }
        public class JiraIssueSearchAction : IAction
        {
            public string JQL { get; set; }
        }
        public class JiraProjectAction : IAction
        {
            public string ProjectIdOrKey { get; set; }
        }
        public class OMWAddAction : IAction
        {
            public string Id { get; set; }
            public string Description { get; set; }
            public string Type { get; set; }
            public string Severity { get; set; }
            public string Code { get; set; }
            public string Release { get; set; }
        }
        public class RefreshAction : IAction
        {
            public bool Force { get; set; }
        }
        public class DemoFormAction : IAction
        {
            public string FormName { get; set; }
        }
    }
}
