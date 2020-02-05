using BlazorState;
using McMaster.Extensions.CommandLineUtils;
using MediatR;
using System;
using System.IO;
using System.Text.Json;

namespace Test
{
    abstract class BaseCmd
    {
        protected IStore Store { get; }
        protected IMediator Mediator { get; }
        protected Celin.OMWPlannerState State => Store.GetState<Celin.OMWPlannerState>();
        protected Celin.Jira.Server Jira { get; }
        [Option("-j|--json", CommandOptionType.SingleValue, Description = "Output result to Json File")]
        protected (bool HasValue, string Value) JsonFile { get; set; }
        protected void Dump()
        {
            var state = new
            {
                State.ErrorMessage,
                State.JiraProject,
                State.JiraStatuses,
                State.JiraIssues,
                State.OMWProjects,
                State.OMWProjectForm,
                State.DemoRequest
            };
            var json = JsonSerializer.Serialize(state, new JsonSerializerOptions { IgnoreNullValues = true, WriteIndented = true });
            if (JsonFile.HasValue)
            {
                var fs = File.CreateText($"{JsonFile.Value}.json");
                fs.Write(json);
                fs.Close();
            }
            else
            {
                Console.WriteLine(json);
            }
        }
        public BaseCmd(IStore store, IMediator mediator, Celin.Jira.Server jira)
        {
            Store = store;
            Mediator = mediator;
            Jira = jira;
        }
    }
}
