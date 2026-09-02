using CabeleleiraLeila.Services.Api;
using CabeleleiraLeila.Services.Navigation;
using CabeleleiraLeila.ViewModels.Admin;
using CabeleleiraLeila.ViewModels.Base;
using CabeleleiraLeila.ViewModels.Cliente;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CabeleleiraLeila.ViewModels;

public partial class LoginViewModel : BaseViewModel
{
    private readonly INavigationService _navigationService;
    private readonly IApiService _apiService;

    [ObservableProperty] private string email;
    [ObservableProperty] private string senha;
    [ObservableProperty] private bool isClienteMode = true;
    [ObservableProperty] private bool ocultarSenha = true;

    public bool IsAdminMode => !IsClienteMode;

    public LoginViewModel(INavigationService navigationService, IApiService apiService)
    {
        _navigationService = navigationService;
        _apiService = apiService;
        Title = "Entrar";
    }

    [RelayCommand]
    private void SelecionarCliente()
    {
        IsClienteMode = true;
        OnPropertyChanged(nameof(IsAdminMode));
    }

    [RelayCommand]
    private void SelecionarAdmin()
    {
        IsClienteMode = false;
        OnPropertyChanged(nameof(IsAdminMode));
    }

    [RelayCommand]
    private async Task EntrarAsync()
    {
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Senha))
        {
            await Shell.Current.DisplayAlertAsync("Aviso", "Preencha e-mail e senha.", "OK");
            return;
        }

        IsBusy = true;
        var response = await _apiService.LoginAsync(Email, Senha);
        IsBusy = false;

        if (string.IsNullOrEmpty(response?.Error) && !string.IsNullOrEmpty(response?.Token))
        {
            if ((IsAdminMode && response.User.Tipo != "ADMIN") ||
                (IsClienteMode && response.User.Tipo != "CLIENTE"))
            {
                await Shell.Current.DisplayAlertAsync("Acesso Negado", "Seu tipo de usuário não corresponde à aba selecionada.", "OK");
                return;
            }


            await SecureStorage.Default.SetAsync("auth_token", response.Token);
            await SecureStorage.Default.SetAsync("user_tipo", response.User.Tipo);
            await SecureStorage.Default.SetAsync("user_nome", response.User.Nome ?? "");
            await SecureStorage.Default.SetAsync("user_email", Email);
            await SecureStorage.Default.SetAsync("user_telefone", response.User.Telefone ?? "");
            await SecureStorage.Default.SetAsync("user_id", response.User.Id.ToString());

            if (IsAdminMode)
            {
                await _navigationService.NavigateToAsync<HomeAdminViewModel>();
            }
            else
            {
                await _navigationService.NavigateToAsync<HomeClienteViewModel>();
            }
        }
        else
        {
            await Shell.Current.DisplayAlertAsync("Ops!", response?.Error ?? "Ocorreu um erro inesperado.", "OK");
        }
    }

    [RelayCommand]
    private void Voltar() => _navigationService.GoBackAsync();

    [RelayCommand]
    private void AlternarSenha()
    {
        OcultarSenha = !OcultarSenha;
    }

    [RelayCommand]
    private async Task EsqueceuSenhaAsync()
    {

        string emailRecuperacao = await Shell.Current.DisplayPromptAsync(
            "Recuperar Senha",
            "Digite o seu e-mail cadastrado:",
            "Enviar",
            "Cancelar",
            keyboard: Keyboard.Email);

        if (string.IsNullOrWhiteSpace(emailRecuperacao)) return;

        IsBusy = true;
        bool sucesso = await _apiService.EsqueciSenhaAsync(emailRecuperacao);
        IsBusy = false;

        if (sucesso)
        {
            bool inserirToken = await Shell.Current.DisplayAlertAsync("E-mail Enviado!", "Enviamos um token de segurança para o seu e-mail. Deseja inserir o token agora para redefinir a senha?", "Inserir Token", "Agora não");

            if (inserirToken)
            {
                await _navigationService.NavigateToAsync<RedefinirSenhaViewModel>();
            }
        }
        else
        {
            await Shell.Current.DisplayAlertAsync("Ops", "Não foi possível solicitar a recuperação. Verifique se o e-mail está correto e se há conexão.", "OK");
        }
    }
}