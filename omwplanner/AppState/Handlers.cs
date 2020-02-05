using BlazorState;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Celin
{
    public partial class AppState
    {
        public class ToggleJiraStatusAccordianHandler : ActionHandler<ToggleJiraStatusAccordianAction>
        {
            AppState State => Store.GetState<AppState>();
            public override Task<Unit> Handle(ToggleJiraStatusAccordianAction aAction, CancellationToken aCancellationToken)
            {
                State.MaxStatusAccordion = aAction.Force.HasValue ? aAction.Force.Value : !State.MaxStatusAccordion;

                var handler = State.Changed;
                handler?.Invoke(State, null);

                return Unit.Task;
            }
            public ToggleJiraStatusAccordianHandler(IStore store) : base(store) { }
        }
        public class ToggleJiraIssueHandler : ActionHandler<ToggleJiraIssueAction>
        {
            AppState State => Store.GetState<AppState>();
            public override Task<Unit> Handle(ToggleJiraIssueAction aAction, CancellationToken aCancellationToken)
            {
                if (State.JiraIssues.Contains(aAction.IssueId))
                {
                    State.JiraIssues.Remove(aAction.IssueId);
                }
                else
                {
                    State.JiraIssues.Insert(0, aAction.IssueId);
                }

                var handler = State.Changed;
                handler?.Invoke(State, null);

                return Unit.Task;
            }
            public ToggleJiraIssueHandler(IStore store) : base(store) { }
        }
    }
}
