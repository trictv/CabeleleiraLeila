using CabeleleiraLeila.Services.Api;
using CabeleleiraLeila.Services.Navigation;
using CabeleleiraLeila.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CabeleleiraLeila.ViewModels;

public partial class RedefinirSenhaViewModel : BaseViewModel
{
    private readonly INavigationService _navigationService;
    private readonly IApiService _apiService;

    [ObservableProperty] private string token;
    [ObservableProperty] private string novaSenha;
    [ObservableProperty] private bool ocultarSenha = true;
    
    public RedefinirSenhaViewModel(INavigationService navigationService, IApiService apiService)
    {
        _navigationService = navigationService;
        _apiService = apiService;
        Title = "Nova Senha";
    }

    [RelayCommand]
    private void AlternarSenha() => OcultarSenha = !OcultarSenha;

    [RelayCommand]
    private async Task SalvarNovaSenhaAsync()
    {
        if (string.IsNullOrWhiteSpace(Token) || string.IsNullOrWhiteSpace(NovaSenha))
        {
            await Shell.Current.DisplayAlertAsync("Aviso", "Preencha o token recebido no e-mail e a nova senha.", "OK");
            return;
        }

        IsBusy = true;
        bool sucesso = await _apiService.ResetarSenhaAsync(Token.Trim(), NovaSenha);
        IsBusy = false;

        if (sucesso)
        {
            await Shell.Current.DisplayAlertAsync("Sucesso", "Sua senha foi redefinida com sucesso! Você já pode fazer login.", "OK");
            MainThread.BeginInvokeOnMainThread(async () => await _navigationService.GoBackAsync());
        }
        else
        {
            await Shell.Current.DisplayAlertAsync("Erro", "Token inválido ou expirado. Volte e solicite um novo na tela de login.", "OK");
        }
    }

    [RelayCommand]
    private void Voltar() => _navigationService.GoBackAsync();
}