using Microsoft.AspNetCore.Components;

namespace GrowIoT.BlazorComponents
{
    public class RedirectToLogin : ComponentBase
    {
        [Inject] protected NavigationManager NavigationManager { get; set; }

        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);
            
            var returnUrl = NavigationManager.ToBaseRelativePath(NavigationManager.Uri);
            NavigationManager.NavigateTo($"/identity/account/login?returnUrl=/{returnUrl}",true);
        }

    }
}
