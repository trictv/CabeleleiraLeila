using CabeleleiraLeila.Services.Navigation;
using CabeleleiraLeila.ViewModels.Base;
using CommunityToolkit.Mvvm.Input;

namespace CabeleleiraLeila.ViewModels;

public partial class InicioViewModel : BaseViewModel
{
    private readonly INavigationService _navigationService;

    public InicioViewModel(INavigationService navigationService)
    {
        _navigationService = navigationService;
    }

    [RelayCommand]
    private async Task VerificarSessaoAtiva()
    {
        var token = await SecureStorage.Default.GetAsync("auth_token");
        var tipo = await SecureStorage.Default.GetAsync("user_tipo");

        if (!string.IsNullOrEmpty(token))
        {
            if (tipo == "ADMIN")
            {
                await _navigationService.NavigateToAsync<Admin.HomeAdminViewModel>();
            }
            else if (tipo == "CLIENTE")
            {
                 await _navigationService.NavigateToAsync<Cliente.HomeClienteViewModel>();
            }
        }
    }


    [RelayCommand]
    private async Task IrParaLogin()
    {
        await _navigationService.NavigateToAsync<LoginViewModel>();
    }

    [RelayCommand]
    private async Task IrParaAgendamento()
    {
        await _navigationService.NavigateToAsync<AgendarViewModel>();
    }
}