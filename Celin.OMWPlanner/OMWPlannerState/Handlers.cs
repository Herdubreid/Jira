using BlazorState;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Celin
{
    public partial class OMWPlannerState
    {
        public class ConfigHandler : ActionHandler<ConfigAction>
        {
            OMWPlannerState State => Store.GetState<OMWPlannerState>();
            public override Task<Unit> Handle(ConfigAction aAction, CancellationToken aCancellationToken)
            {
                State.JiraProjectKey = aAction.JiraProjectKey;
                State.JiraStatusKeys = aAction.JiraStatusKeys;

                return Unit.Task;
            }
            public ConfigHandler(IStore store) : base(store) { }
        }
        public class JiraEditIssueHandler : ActionHandler<JiraEditIssueAction>
        {
            readonly static SemaphoreSlim lockit = new SemaphoreSlim(1, 1);
            Jira.Server Jira { get; }
            OMWPlannerState State => Store.GetState<OMWPlannerState>();
            public override async Task<Unit> Handle(JiraEditIssueAction aAction, CancellationToken aCancellationToken)
            {
                try
                {
                    await lockit.WaitAsync();
                    await Jira.EditIssu(aAction.IssueIdOrKey, aAction.Fields, aAction.Update);
                    var ndx = State.JiraIssues.FindIndex(i => i.id.Equals(aAction.IssueIdOrKey));
                    State.JiraIssues[ndx] = await Jira.GetIssue(aAction.IssueIdOrKey);
                    
                }
                finally
                {
                    lockit.Release();
                }

                var handler = State.Changed;
                handler?.Invoke(State, null);

                return Unit.Value;
            }
            public JiraEditIssueHandler(IStore store, Jira.Server jira) : base(store)
            {
                Jira = jira;
            }
        }
        public class JiraIssueTransitionHandler : ActionHandler<JiraIssueTransitionAction>
        {
            readonly static SemaphoreSlim lockit = new SemaphoreSlim(1, 1);
            AIS.Server E1 { get; }
            Jira.Server Jira { get; }
            OMWPlannerState State => Store.GetState<OMWPlannerState>();
            public override async Task<Unit> Handle(JiraIssueTransitionAction aAction, CancellationToken aCancellationToken)
            {
                try
                {
                    await lockit.WaitAsync();
                    var ndx = State.JiraIssues.FindIndex(i => i.id.Equals(aAction.IssueIdOrKey));
                    var key = State.JiraIssues[ndx].key;
                    await Jira.TransitionIssue(aAction.IssueIdOrKey, aAction.TransitionId);
                    State.JiraIssues[ndx] = await Jira.GetIssue(aAction.IssueIdOrKey);
                    if (State.JiraIssues[ndx].fields.status.id.Equals(State.JiraTriggerOMW) &&
                        !State.OMWProjects.Contains(new F98220.Row { F98220_OMWPRJID = key }))
                    {
                        var rq = new W98220WC.Request();
                        rq.Set(W98220WC.Fields.OMWPRJID, key);
                        rq.Set(W98220WC.Fields.OMWDESC, State.JiraIssues[ndx].fields.summary);
                        rq.Set(W98220WC.Fields.OMWTYP, "01");
                        rq.Set(W98220WC.Fields.OMWSV, "01");
                        rq.Set(W98220WC.Fields.SYR, "55");
                        rq.Set(W98220WC.Fields.SRCRLS, "E920");
                        rq.Ok();
                        await E1.RequestAsync<W98220WC.Response>(rq);
                        var omw = await E1.RequestAsync<F98220.Response>(new F98220.Request());
                        State.OMWProjects = new List<F98220.Row>(omw.fs_DATABROWSE_F98220.data.gridData.rowset);
                    }
                }
                finally
                {
                    lockit.Release();
                }

                var handler = State.Changed;
                handler?.Invoke(State, null);

                return Unit.Value;
            }
            public JiraIssueTransitionHandler(IStore store, Jira.Server jira, AIS.Server e1) : base(store)
            {
                Jira = jira;
                E1 = e1;
            }
        }
        public class JiraIssueSearchHandler : ActionHandler<JiraIssueSearchAction>
        {
            public readonly static string[] FIELDS = new string[] { "summary", "status", "issuetype", "priority", "timetracking" };
            Jira.Server Jira { get; }
            OMWPlannerState State => Store.GetState<OMWPlannerState>();
            public override async Task<Unit> Handle(JiraIssueSearchAction aAction, CancellationToken aCancellationToken)
            {
                var issues = await Jira.Search(aAction.JQL, FIELDS);
                State.JiraIssues = new List<Jira.Response.Issue>(issues.issues);

                return Unit.Value;
            }
            public JiraIssueSearchHandler(IStore store, Jira.Server jira) : base(store)
            {
                Jira = jira;
            }
        }
        public class JiraProjectHandler : ActionHandler<JiraProjectAction>
        {
            Jira.Server Jira { get; }
            OMWPlannerState State => Store.GetState<OMWPlannerState>();
            public override async Task<Unit> Handle(JiraProjectAction aAction, CancellationToken aCancellationToken)
            {
                State.JiraProject = await Jira.GetProject(aAction.ProjectIdOrKey);

                return Unit.Value;
            }
            public JiraProjectHandler(IStore store, Jira.Server jira) : base(store)
            {
                Jira = jira;
            }
        }
        public class OMWAddHandler : ActionHandler<OMWAddAction>
        {
            readonly static SemaphoreSlim lockit = new SemaphoreSlim(1, 1);
            AIS.Server E1 { get; }
            OMWPlannerState State => Store.GetState<OMWPlannerState>();
            public override async Task<Unit> Handle(OMWAddAction aAction, CancellationToken aCancellationToken)
            {
                try
                {
                    await lockit.WaitAsync();
                    var rq = new W98220WC.Request();
                    rq.Set(W98220WC.Fields.OMWPRJID, aAction.Id);
                    rq.Set(W98220WC.Fields.OMWDESC, aAction.Description);
                    rq.Set(W98220WC.Fields.OMWTYP, aAction.Type);
                    rq.Set(W98220WC.Fields.OMWSV, aAction.Severity);
                    rq.Set(W98220WC.Fields.SYR, aAction.Code);
                    rq.Set(W98220WC.Fields.SRCRLS, aAction.Release);
                    rq.Ok();
                    State.OMWProjectForm = await E1.RequestAsync<W98220WC.Response>(rq);
                }
                catch (AIS.HttpWebException e)
                {
                    State.ErrorMessage = e.ErrorResponse.message ?? e.Message;
                }
                catch (Exception e)
                {
                    State.ErrorMessage = e.Message;
                }
                finally
                {
                    lockit.Release();
                }
                var handler = State.Changed;
                handler?.Invoke(State, null);

                return Unit.Value;
            }
            public OMWAddHandler(IStore store, AIS.Server e1) : base(store)
            {
                E1 = e1;
            }
        }
        public class RefreshHandler : ActionHandler<RefreshAction>
        {
            readonly static SemaphoreSlim lockit = new SemaphoreSlim(1, 1);
            Jira.Server Jira { get; }
            AIS.Server E1 { get; }
            OMWPlannerState State => Store.GetState<OMWPlannerState>();
            public override async Task<Unit> Handle(RefreshAction aAction, CancellationToken aCancellationToken)
            {
                try
                {
                    await lockit.WaitAsync();
                    if (aAction.Force || DateTime.Now - State.LastUpdate > TimeSpan.FromHours(1d))
                    {
                        State.LastUpdate = DateTime.Now;
                        var tasks = new Task[]
                        {
                            E1.RequestAsync<F98220.Response>(new F98220.Request()),
                            Jira.GetProject(State.JiraProjectKey),
                            Jira.GetTaskTypes(State.JiraProjectKey),
                            Jira.Search($"project={State.JiraProjectKey} AND status in ({string.Join(',', State.JiraStatusKeys)})", JiraIssueSearchHandler.FIELDS)
                        };
                        await Task.WhenAll(tasks);

                        var omw = tasks[0] as Task<F98220.Response>;
                        State.OMWProjects = new List<F98220.Row>(omw.Result.fs_DATABROWSE_F98220.data.gridData.rowset);

                        State.JiraProject = (tasks[1] as Task<Jira.Response.Project>).Result;

                        State.JiraStatuses = (tasks[2] as Task<IEnumerable<Jira.Response.TaskType>>)
                            .Result
                            .Aggregate(new List<Jira.Response.Status>(), (a, e)
                                =>
                                {
                                    a.AddRange(e.statuses);
                                    return a;
                                })
                            .Distinct(new Jira.Response.ComparableBase<Jira.Response.Status>())
                            .ToList();

                        var issues = tasks[3] as Task<Jira.Response.Issues>;
                        State.JiraIssues = new List<Jira.Response.Issue>(issues.Result.issues);
                    }
                }
                catch (Exception e)
                {
                    State.ErrorMessage = e.Message;
                }
                finally
                {
                    lockit.Release();
                }

                var handler = State.Changed;
                handler?.Invoke(State, null);

                return Unit.Value;
            }
            public RefreshHandler(IStore store, AIS.Server e1, Jira.Server jira) : base(store)
            {
                E1 = e1;
                Jira = jira;
            }
        }
        public class DemoFormHandler : ActionHandler<DemoFormAction>
        {
            static readonly Regex formPattern = new Regex("^W(.+)[A-Z]$");
            AIS.Server E1 { get; }
            OMWPlannerState State => Store.GetState<OMWPlannerState>();
            public override async Task<Unit> Handle(DemoFormAction aAction, CancellationToken aCancellationToken)
            {
                var match = formPattern.Match(aAction.FormName);
                if (match.Success)
                {
                    try
                    {
                        State.DemoRequest = await E1.RequestAsync<object>(new AIS.FormRequest
                        {
                            formName = $"P{match.Groups[1]}_{aAction.FormName}",
                            formServiceDemo = "TRUE"
                        });
                    }
                    catch (AIS.HttpWebException e)
                    {
                        State.ErrorMessage = e.ErrorResponse.message ?? e.Message;
                    }
                    catch (Exception e)
                    {
                        State.ErrorMessage = e.Message;
                    }
                }

                var handler = State.Changed;
                handler?.Invoke(State, null);

                return Unit.Value;
            }
            public DemoFormHandler(IStore store, AIS.Server e1) : base(store)
            {
                E1 = e1;
            }
        }
    }
}
