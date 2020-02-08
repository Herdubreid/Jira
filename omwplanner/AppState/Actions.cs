using BlazorState;
using System.Collections.Generic;

namespace Celin
{
    public partial class AppState
    {
        public class OMWRoleAction : IAction
        {
            public OMWRoles OMWRole { get; set; }
        }
        public class GetTransitionsAction : IAction
        {
            public string IssueId { get; set; }
        }
        public class ClearTransitionsAction : IAction
        {
            public string IssueId { get; set; }
        }
        public class ToggleJiraStatusAccordianAction : IAction
        {
            public bool? Force { get; set; }
        }
        public class ToggleJiraIssueAction : IAction
        {
            public string IssueId { get; set; }
        }
    }
}
