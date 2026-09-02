using CabeleleiraLeila.Services.Navigation;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CabeleleiraLeila.ViewModels.Base;

public abstract partial class BaseAdminViewModel : BaseViewModel
{
    protected readonly INavigationService NavigationService;

    [ObservableProperty]
    private string nomeAdmin;

    public BaseAdminViewModel(INavigationService navigationService)
    {
        NavigationService = navigationService;
    }

    [RelayCommand]
    public virtual async Task VerificarAcessoAsync()
    {
        var token = await SecureStorage.Default.GetAsync("auth_token");
        var tipo = await SecureStorage.Default.GetAsync("user_tipo");

        NomeAdmin = await SecureStorage.Default.GetAsync("user_nome") ?? "Administrador";

        if (string.IsNullOrEmpty(token) || tipo != "ADMIN")
        {
            SecureStorage.Default.RemoveAll();
            await Shell.Current.DisplayAlertAsync("Sessão Expirada", "Por favor, faça login novamente para acessar esta área.", "OK");
            await NavigationService.GoToRootAsync();
        }
    }

    [RelayCommand]
    private async Task SairAsync()
    {
        bool confirmar = await Shell.Current.DisplayAlertAsync("Sair", "Deseja realmente encerrar a sessão?", "Sim", "Não");
        if (confirmar)
        {
            SecureStorage.Default.RemoveAll();
            MainThread.BeginInvokeOnMainThread(() =>
            {

                NavigationService.GoToRootAsync();
            });
        }
    }
}