using System.Collections.ObjectModel;
using CabeleleiraLeila.Models;
using CabeleleiraLeila.Services.Api;
using CabeleleiraLeila.Services.Navigation;
using CabeleleiraLeila.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CabeleleiraLeila.ViewModels.Admin;

public partial class ClienteFormViewModel : BaseAdminViewModel, IQueryAttributable
{
    private readonly IApiService _apiService;

    [ObservableProperty]
    private ClienteAdmin clienteAtual;

    [ObservableProperty]
    private bool isEditando;

    public ObservableCollection<AgendamentoHistorico> Historico { get; } = new();

    public ClienteFormViewModel(INavigationService navigationService, IApiService apiService)
        : base(navigationService)
    {
        _apiService = apiService;
        Title = "Novo Cliente";
        ClienteAtual = new ClienteAdmin { IsAtivo = true };
    }

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("Parameter", out var parameter) && parameter is ClienteAdmin c)
        {
            ClienteAtual = new ClienteAdmin
            {
                Id = c.Id,
                UsuarioId = c.UsuarioId,
                Nome = c.Nome,
                Email = c.Email,
                Telefone = c.Telefone,
                Cpf = c.Cpf,
                DataNascimento = c.DataNascimento,
                Observacoes = c.Observacoes,
                Ativo = c.Ativo
            };

            IsEditando = true;
            Title = "Detalhes do Cliente";

            await CarregarHistoricoAsync();
        }
    }

    public bool SemHistorico => Historico.Count == 0;

    private async Task CarregarHistoricoAsync()
    {
        var hist = await _apiService.GetHistoricoClienteAsync(ClienteAtual.Id);
        Historico.Clear();
        foreach (var item in hist)
        {
            Historico.Add(item);
        }

        OnPropertyChanged(nameof(SemHistorico));
    }

    [RelayCommand]
    private async Task SalvarAsync()
    {
        if (string.IsNullOrWhiteSpace(ClienteAtual.Nome) || string.IsNullOrWhiteSpace(ClienteAtual.Telefone))
        {
            await Shell.Current.DisplayAlertAsync("Aviso", "Nome e Telefone são obrigatórios.", "OK");
            return;
        }

        IsBusy = true;
        bool sucesso = await _apiService.SalvarAdminClienteAsync(ClienteAtual);
        IsBusy = false;

        if (sucesso)
        {
            await Shell.Current.DisplayAlertAsync("Sucesso", "Cliente salvo com sucesso!", "OK");
            MainThread.BeginInvokeOnMainThread(() =>
            {
                NavigationService.GoBackAsync();
            });
        }
        else
        {
            await Shell.Current.DisplayAlertAsync("Erro", "Erro ao salvar. Verifique se o e-mail ou CPF já existem.", "OK");
        }
    }

    [RelayCommand]
    private void Cancelar() => NavigationService.GoBackAsync();
}