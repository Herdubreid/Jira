using BlazorState;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Celin
{
    public partial class AppState
    {
        public class OMWRoleHandler : ActionHandler<OMWRoleAction>
        {
            AppState State => Store.GetState<AppState>();
            public override Task<Unit> Handle(OMWRoleAction aAction, CancellationToken aCancellationToken)
            {
                State.OMWRole = aAction.OMWRole;
                if (State.OMWRole != OMWRoles.All)
                {
                    State.ShowStatusAccordion = false;
                }

                var handler = State.Changed;
                handler?.Invoke(State, null);

                return Unit.Task;
            }
            public OMWRoleHandler(IStore store) : base(store) { }
        }
        public class GetTransitionsHandler : ActionHandler<GetTransitionsAction>
        {
            Jira.Server Jira { get; }
            AppState State => Store.GetState<AppState>();
            public override async Task<Unit> Handle(GetTransitionsAction aAction, CancellationToken aCancellationToken)
            {
                try
                {
                    var ts = await Jira.ListTransitions(aAction.IssueId);
                    State.Transitions.Add(aAction.IssueId, ts.transitions);
                }
                catch { }

                return Unit.Value;
            }
            public GetTransitionsHandler(IStore store, Jira.Server jira) : base(store)
            {
                Jira = jira;
            }
        }
        public class ClearTransitionsHandler : ActionHandler<ClearTransitionsAction>
        {
            AppState State => Store.GetState<AppState>();
            public override Task<Unit> Handle(ClearTransitionsAction aAction, CancellationToken aCancellationToken)
            {
                State.Transitions.Remove(aAction.IssueId);

                return Unit.Task;
            }
            public ClearTransitionsHandler(IStore store) : base(store) { }
        }
        public class ToggleJiraStatusAccordianHandler : ActionHandler<ToggleJiraStatusAccordianAction>
        {
            AppState State => Store.GetState<AppState>();
            public override Task<Unit> Handle(ToggleJiraStatusAccordianAction aAction, CancellationToken aCancellationToken)
            {
                State.ShowStatusAccordion = aAction.Force ?? !State.ShowStatusAccordion;

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
                    State.Transitions.Remove(aAction.IssueId);
                }
                else
                {
                    State.JiraIssues.Add(aAction.IssueId);
                }

                var handler = State.Changed;
                handler?.Invoke(State, null);

                return Unit.Task;
            }
            public ToggleJiraIssueHandler(IStore store) : base(store) { }
        }
    }
}
