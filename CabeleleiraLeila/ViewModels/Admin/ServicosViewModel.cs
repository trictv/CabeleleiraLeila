using System.Collections.ObjectModel;
using CabeleleiraLeila.Models;
using CabeleleiraLeila.Services.Api;
using CabeleleiraLeila.Services.Navigation;
using CabeleleiraLeila.ViewModels.Base;
using CommunityToolkit.Mvvm.Input;

namespace CabeleleiraLeila.ViewModels.Admin;

public partial class ServicosViewModel : BaseAdminViewModel
{
    private readonly IApiService _apiService;

    public ObservableCollection<Servico> Servicos { get; } = new();

    public ServicosViewModel(INavigationService navigationService, IApiService apiService)
        : base(navigationService)
    {
        _apiService = apiService;
        Title = "Serviços";
    }

    [RelayCommand]
    private async Task CarregarDadosAsync()
    {
        IsBusy = true;

        var servicosApi = await _apiService.GetAdminServicosAsync();
        var categoriasApi = await _apiService.GetCategoriasAsync();

        Servicos.Clear();
        foreach (var servico in servicosApi)
        {
            var cat = categoriasApi.FirstOrDefault(c => c.Id == servico.CategoriaId);
            servico.CategoriaNome = cat?.Nome ?? "Sem Categoria";

            Servicos.Add(servico);
        }
        IsBusy = false;
    }

    [RelayCommand]
    private async Task NovoServicoAsync()
    {
        await NavigationService.NavigateToAsync<ServicoFormViewModel>();
    }

    [RelayCommand]
    private async Task EditarServicoAsync(Servico servico)
    {
        await NavigationService.NavigateToAsync<ServicoFormViewModel, Servico>(servico);
    }

    [RelayCommand]
    private async Task ExcluirServicoAsync(Servico servico)
    {
        bool confirmar = await Shell.Current.DisplayAlertAsync("Excluir", $"Tem certeza que deseja excluir '{servico.Nome}'?", "Sim", "Não");

        if (confirmar)
        {
            IsBusy = true;
            bool sucesso = await _apiService.ExcluirServicoAsync(servico.Id);

            if (sucesso)
            {
                Servicos.Remove(servico);
                await Shell.Current.DisplayAlertAsync("Sucesso", "Serviço excluído.", "OK");
            }
            else
            {
                await Shell.Current.DisplayAlertAsync("Erro", "Não foi possível excluir o serviço.", "OK");
            }
            IsBusy = false;
        }
    }
}