using BlazorState;

namespace Celin
{
    public partial class AppState
    {
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
