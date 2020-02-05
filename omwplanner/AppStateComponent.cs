using BlazorState;
using System;

namespace Celin
{
    public class AppStateComponent : BlazorStateComponent
    {
        protected AppState AppState => Store.GetState<AppState>();
        protected OMWPlannerState OMWPlannerState => Store.GetState<OMWPlannerState>();
        protected void Update(object sender, EventArgs args) => InvokeAsync(StateHasChanged);
        protected override void OnInitialized()
        {
            AppState.Changed += Update;
            OMWPlannerState.Changed += Update;
        }
        public new void Dispose()
        {
            AppState.Changed += Update;
            OMWPlannerState.Changed -= Update;
            base.Dispose();
        }
    }
}
