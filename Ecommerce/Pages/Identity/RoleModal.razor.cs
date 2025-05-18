using Blazored.FluentValidation;
using ECommerce.Infrastructure.Managers.Admin.Roles;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using SharedLibrary.Requests.Identity;

namespace Ecommerce.Pages.Identity;

public partial class RoleModal
{
    [Inject] private IRoleManager RoleManager { get; set; } = default!;

    [Parameter] public RoleRequest RoleModel { get; set; } = new();

    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; } = default!;
    //[CascadingParameter] private HubConnection HubConnection { get; set; }

    private FluentValidationValidator _fluentValidationValidator = default!;

    private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });

    public void Cancel()
    {
        MudDialog.Cancel();
    }

    protected override async Task OnInitializedAsync()
    {
        await Task.CompletedTask;
        //HubConnection = HubConnection.TryInitialize(_navigationManager, _localStorage);
        //if (HubConnection.State == HubConnectionState.Disconnected)
        //{
        //    await HubConnection.StartAsync();
        //}
    }

    private async Task SaveAsync()
    {
        var response = string.IsNullOrEmpty(RoleModel.Id) ?
            await RoleManager.CreateAsync(RoleModel) :
            await RoleManager.UpdateAsync(RoleModel.Id, RoleModel);
        if (response.Succeeded)
        {
            _snackBar.Add(response.Message, Severity.Success);
            //await HubConnection.SendAsync(ApplicationConstants.SignalR.SendUpdateDashboard);
            MudDialog.Close();
        }
        else
        {
            foreach (var message in response.Errors!)
            {
                _snackBar.Add(message, Severity.Error);
            }
        }
    }
}
