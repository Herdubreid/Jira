using BlazorState;
using System;
using System.Collections.Generic;

namespace Celin
{
    public partial class AppState : State<AppState>
    {
        public event EventHandler Changed;
        public bool MaxStatusAccordion { get; set; }
        public List<string> JiraIssues { get; set; }
        public override void Initialize()
        {
            JiraIssues = new List<string>();
        }
    }
}
