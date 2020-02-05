using BlazorState;
using McMaster.Extensions.CommandLineUtils;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Test
{
    class ProjectBaseCmd : BaseCmd
    {
        [Argument(0, Description = "Project Key or Id")]
        [Required]
        protected string ProjectIdOrKey { get; set; }
        public ProjectBaseCmd(IStore store, IMediator mediator, Celin.Jira.Server jira) : base(store, mediator, jira) { }
    }
}
