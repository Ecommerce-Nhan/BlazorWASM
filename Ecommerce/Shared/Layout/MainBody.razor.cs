using Microsoft.AspNetCore.Components;

namespace Ecommerce.Shared.Layout;

public partial class MainBody
{
    [Parameter]
    public RenderFragment ChildContent { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await LoadDataAsync();
        }
    }

    private async Task LoadDataAsync()
    {
        var state = await _stateProvider.GetAuthenticationStateAsync();
        var user = state.User;
        if (user == null) return;
        if (user.Identity?.IsAuthenticated != true)
        {
            await _authenticationManager.Logout();
        }
    }
}