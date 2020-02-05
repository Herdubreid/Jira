using BlazorState;
using McMaster.Extensions.CommandLineUtils;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Test
{
    class IssueBaseCmd : BaseCmd
    {
        [Argument(0, Description = "Issue Key or Id")]
        [Required]
        protected string IssueIdOrKey { get; set; }
        public IssueBaseCmd(IStore store, IMediator mediator, Celin.Jira.Server jira) : base(store, mediator, jira) { }
    }
}
