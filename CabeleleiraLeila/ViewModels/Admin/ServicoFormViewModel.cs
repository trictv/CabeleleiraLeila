using System.Collections.ObjectModel;
using CabeleleiraLeila.Models;
using CabeleleiraLeila.Services.Api;
using CabeleleiraLeila.Services.Navigation;
using CabeleleiraLeila.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CabeleleiraLeila.ViewModels.Admin;

public partial class ServicoFormViewModel : BaseAdminViewModel, IQueryAttributable
{
    private readonly IApiService _apiService;

    [ObservableProperty]
    private Servico servicoAtual;

    [ObservableProperty]
    private CategoriaServico? categoriaSelecionada;

    public ObservableCollection<CategoriaServico> CategoriasDisponiveis { get; } = new();

    public ServicoFormViewModel(INavigationService navigationService, IApiService apiService)
        : base(navigationService)
    {
        _apiService = apiService;
        Title = "Novo Serviço";
        ServicoAtual = new Servico { IsAtivo = true, DuracaoMinutos = 30, Preco = 0 };
    }

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        await CarregarCategoriasAsync();

        if (query.TryGetValue("Parameter", out var parameter) && parameter is Servico srv)
        {
            ServicoAtual = new Servico
            {
                Id = srv.Id,
                CategoriaId = srv.CategoriaId,
                Nome = srv.Nome,
                DuracaoMinutos = srv.DuracaoMinutos,
                Preco = srv.Preco,
                Ativo = srv.Ativo
            };

            CategoriaSelecionada = CategoriasDisponiveis.FirstOrDefault(c => c.Id == srv.CategoriaId);
            Title = "Editar Serviço";
        }
    }

    private async Task CarregarCategoriasAsync()
    {
        IsBusy = true;
        var categorias = await _apiService.GetCategoriasAsync();
        CategoriasDisponiveis.Clear();

        foreach (var cat in categorias)
        {
            CategoriasDisponiveis.Add(cat);
        }
        IsBusy = false;
    }

    [RelayCommand]
    private async Task SalvarAsync()
    {
        if (string.IsNullOrWhiteSpace(ServicoAtual.Nome) || CategoriaSelecionada == null || ServicoAtual.DuracaoMinutos <= 0 || ServicoAtual.Preco < 0)
        {
            await Shell.Current.DisplayAlertAsync("Aviso", "Preencha todos os campos corretamente e selecione uma categoria.", "OK");
            return;
        }

        ServicoAtual.CategoriaId = CategoriaSelecionada.Id;

        IsBusy = true;
        bool sucesso = await _apiService.SalvarServicoAsync(ServicoAtual);
        IsBusy = false;

        if (sucesso)
        {
            await Shell.Current.DisplayAlertAsync("Sucesso", "Serviço salvo com sucesso!", "OK");
            MainThread.BeginInvokeOnMainThread(() =>
            {
                NavigationService.GoBackAsync();
            });
        }
        else
        {
            await Shell.Current.DisplayAlertAsync("Erro", "Erro ao salvar. Verifique os dados.", "OK");
        }
    }

    [RelayCommand]
    private void Cancelar() => NavigationService.GoBackAsync();
}