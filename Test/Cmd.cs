using BlazorState;
using McMaster.Extensions.CommandLineUtils;
using MediatR;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Request = Celin.Jira.Request;
using Response = Celin.Jira.Response;

namespace Test
{
    [Subcommand(typeof(ConfigCmd))]
    [Subcommand(typeof(SearchCmd))]
    [Subcommand(typeof(OMWCmd))]
    [Subcommand(typeof(DemoCmd))]
    [Subcommand(typeof(LabelCmd))]
    [Subcommand(typeof(LoadCmd))]
    [Subcommand(typeof(CommentCmd))]
    [Subcommand(typeof(IssueCmd))]
    [Subcommand(typeof(ProjectCmd))]
    class Cmd
    {
        [Command("config", Description = "Config Commands")]
        class ConfigCmd : BaseCmd
        {
            [Argument(0, Description = "Parameter")]
            [Required]
            string Parameter { get; set; }
            [Option("-a", CommandOptionType.NoValue, Description = "Array")]
            bool Array { get; set; }
            void OnExecute()
            {
                if (Array)
                {
                    var value = State.Config.GetSection(Parameter)
                        .GetChildren()
                        .Select(e => e.Value)
                        .ToArray();
                    Console.WriteLine(string.Join(", ", value));
                }
                else
                {
                    Console.WriteLine(State.Config[Parameter]);
                }
            }
            public ConfigCmd(IStore store, IMediator mediator, Celin.Jira.Server jira) : base(store, mediator, jira) { }
        }
        [Command("omw", Description = "OMW Commands")]
        [Subcommand(typeof(AddCmd))]
        class OMWCmd
        {
            [Command("add", Description = "Add")]
            class AddCmd : BaseCmd
            {
                [Argument(0, Description = "Project Id")]
                [Required]
                string Id { get; set; }
                [Argument(1, Description = "Description")]
                [Required]
                string Description { get; set; }
                [Argument(2, Description = "Type")]
                [Required]
                string Type { get; set; }
                [Argument(3, Description = "Severity")]
                [Required]
                string Severity { get; set; }
                [Argument(4, Description = "Product Code")]
                [Required]
                string Code { get; set; }
                [Argument(5, Description = "Release")]
                [Required]
                string Release { get; set; }
                async Task OnExecuteAsync()
                {
                    await Mediator.Send(new Celin.OMWPlannerState.OMWAddAction
                    {
                        Id = Id,
                        Description = Description,
                        Type = Type,
                        Severity = Severity,
                        Code = Code,
                        Release = Release
                    });
                    Dump();
                }
                public AddCmd(IStore store, IMediator mediator, Celin.Jira.Server jira) : base(store, mediator, jira) { }
            }
        }
        [Command("demo", Description = "Demo Form")]
        class DemoCmd : BaseCmd
        {
            [Argument(0, Description = "Form Name (WxxxxxA-Z)")]
            [Required]
            string FormName { get; set; }
            async Task OnExecuteAsync()
            {
                await Mediator.Send(new Celin.OMWPlannerState.DemoFormAction
                {
                    FormName = FormName
                });

                Dump();
            }
            public DemoCmd(IStore store, IMediator mediator, Celin.Jira.Server jira) : base(store, mediator, jira) { }
        }
        [Command("label", Description = "List Labels")]
        class LabelCmd : BaseCmd
        {
            async Task OnExecuteAsync()
            {
                var rs = await Jira.GetAsync<Response.Label>(new Request.Label
                {
                    startAt = 0,
                    maxResults = 1000
                });
            }
            public LabelCmd(IStore store, IMediator mediator, Celin.Jira.Server jira) : base(store, mediator, jira) { }
        }
        [Command("load", Description = "Load Data")]
        class LoadCmd : BaseCmd
        {
            async Task OnExecute()
            {
                await Mediator.Send(new Celin.OMWPlannerState.RefreshAction
                {
                    JiraProject = "OMW"
                });
                Dump();

                var open = State.JiraIssues.Where(jira => !State.OMWProjects.Select(omw => omw.F98220_OMWPRJID).Contains(jira.key));
                foreach (var p in open)
                {
                    Console.WriteLine("{0}, {1}", p.key, p.fields.summary);
                    if (Prompt.GetYesNo("Create OMW Project?", false))
                    {
                        var add = Mediator.Send(new Celin.OMWPlannerState.OMWAddAction
                        {
                            Id = p.key,
                            Description = p.fields.summary,
                            Code = "55",
                            Type = "02",
                            Severity = "10",
                            Release = "E920"
                        });
                        add.Wait();
                    }
                }
            }
            public LoadCmd(IStore store, IMediator mediator, Celin.Jira.Server jira) : base(store, mediator, jira) { }
        }
        [Command("comment", Description = "Comment Commands")]
        [Subcommand(typeof(GetCmd))]
        [Subcommand(typeof(AddCmd))]
        [Subcommand(typeof(ListCmd))]
        class CommentCmd
        {
            [Command("get", Description = "Get Comment")]
            class GetCmd : IssueBaseCmd
            {
                [Argument(1, Description = "Id")]
                [Required]
                int Id { get; set; }
                async Task OnExecuteAsync()
                {
                    var rs = await Jira.GetAsync<Response.Comment>(new Request.Issue.Comment.Get
                    {
                        IdOrKey = IssueIdOrKey,
                        id = Id
                    });
                    var s = JsonSerializer.Serialize(rs, new JsonSerializerOptions { WriteIndented = true });
                    Console.WriteLine(s);
                }
                public GetCmd(IStore store, IMediator mediator, Celin.Jira.Server jira) : base(store, mediator, jira) { }
            }
            [Command("add", Description = "Add Comment")]
            class AddCmd : IssueBaseCmd
            {
                [Argument(1, Description = "Comment")]
                [Required]
                string[] Comment { get; set; }
                async Task OnExecuteAsync()
                {
                    var o = await Jira.AddComment(IssueIdOrKey, Comment);
                    var s = JsonSerializer.Serialize(o, new JsonSerializerOptions { WriteIndented = true });
                    Console.WriteLine(s);
                }
                public AddCmd(IStore store, IMediator mediator, Celin.Jira.Server jira) : base(store, mediator, jira) { }
            }
            [Command("list", Description = "List Comments")]
            class ListCmd : IssueBaseCmd
            {
                async Task OnExecuteAsync()
                {
                    var o = await Jira.ListComments(IssueIdOrKey);
                    var s = JsonSerializer.Serialize(o, new JsonSerializerOptions { WriteIndented = true });
                    Console.WriteLine(s);
                }
                public ListCmd(IStore store, IMediator mediator, Celin.Jira.Server jira) : base(store, mediator, jira) { }
            }
        }
        [Command("search", Description = "Issue Search")]
        class SearchCmd : BaseCmd
        {
            [Argument(0, Description = "JQL Search String")]
            [Required]
            string JQL { get; set; }
            async Task OnExecuteAsync()
            {
                await Mediator.Send(new Celin.OMWPlannerState.JiraIssueSearchAction
                {
                    JQL = JQL
                });

                Dump();
            }
            public SearchCmd(IStore store, IMediator mediator, Celin.Jira.Server jira) : base(store, mediator, jira) { }
        }
        [Command("issue", Description = "Issue Commands")]
        [Subcommand(typeof(TransitionCmd))]
        [Subcommand(typeof(EditCmd))]
        [Subcommand(typeof(GetCmd))]
        [Subcommand(typeof(AddCmd))]
        class IssueCmd
        {
            [Command("transition", Description = "Transition Issue")]
            class TransitionCmd : IssueBaseCmd
            {
                [Option("-t|--id", CommandOptionType.SingleValue, Description = "Transition Id")]
                int? Transition { get; set; }
                async Task OnExecuteAsync()
                {
                    if (Transition.HasValue)
                    {
                        await Jira.TransitionIssue(IssueIdOrKey, Transition.ToString());
                    }
                    else
                    {
                        var rs = await Jira.ListTransitions(IssueIdOrKey);
                        var s = JsonSerializer.Serialize(rs, new JsonSerializerOptions { WriteIndented = true });
                        Console.WriteLine(s);
                    }
                }
                public TransitionCmd(IStore store, IMediator mediator, Celin.Jira.Server jira) : base(store, mediator, jira) { }
            }
            [Command("get", Description = "Get Issue")]
            class GetCmd : IssueBaseCmd
            {
                async Task OnExecuteAsync()
                {
                    var rs = await Jira.GetIssue(IssueIdOrKey);
                    var s = JsonSerializer.Serialize(rs, new JsonSerializerOptions
                    {
                        WriteIndented = true
                    });
                    Console.WriteLine(s);
                }
                public GetCmd(IStore store, IMediator mediator, Celin.Jira.Server jira) : base(store, mediator, jira) { }
            }
            [Command("edit", Description = "Edit Issue")]
            class EditCmd : IssueBaseCmd
            {
                [Option("-t|--type", CommandOptionType.SingleValue, Description = "Type")]
                int? Type { get; set; }
                async Task OnExecuteAsync()
                {
                    await Jira.EditIssu(IssueIdOrKey, new Request.Fields
                    {
                        issuetype = Type.HasValue
                                ? new Request.Id
                                {
                                    id = Type.Value.ToString()
                                }
                                : null
                    });
                }
                public EditCmd(IStore store, IMediator mediator, Celin.Jira.Server jira) : base(store, mediator, jira) { }
            }
            [Command("add", Description = "Add Issue")]
            class AddCmd : ProjectBaseCmd
            {
                [Argument(1, Description = "Issue Type")]
                [Required]
                int IssueTypeId { get; set; }
                [Argument(2, Description = "Summary")]
                [Required]
                string Summary { get; set; }
                [Argument(3, Description = "Label")]
                [Required]
                string Label { get; set; }
                [Argument(4, Description = "Description")]
                [Required]
                string[] Description { get; set; }
                async Task OnExecuteAsync()
                {
                    var rs = await Jira.AddIssue(ProjectIdOrKey, IssueTypeId.ToString(), Summary, Description, new string[] { Label });
                    var s = JsonSerializer.Serialize(rs, new JsonSerializerOptions
                    {
                        WriteIndented = true
                    });
                    Console.WriteLine(s);
                }
                public AddCmd(IStore store, IMediator mediator, Celin.Jira.Server jira) : base(store, mediator, jira) { }
            }
        }
        [Command("project", Description = "Project Commands")]
        class ProjectCmd : BaseCmd
        {
            async Task OnExecuteAsync()
            {
                await Mediator.Send(new Celin.OMWPlannerState.JiraProjectAction());

                Dump();
            }
            public ProjectCmd(IStore store, IMediator mediator, Celin.Jira.Server jira) : base(store, mediator, jira) { }
        }
    }
}
