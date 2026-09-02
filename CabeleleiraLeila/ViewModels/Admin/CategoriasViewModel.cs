using System.Collections.ObjectModel;
using CabeleleiraLeila.Models;
using CabeleleiraLeila.Services.Api;
using CabeleleiraLeila.Services.Navigation;
using CabeleleiraLeila.ViewModels.Base;
using CommunityToolkit.Mvvm.Input;

namespace CabeleleiraLeila.ViewModels.Admin;

public partial class CategoriasViewModel : BaseAdminViewModel
{
    private readonly IApiService _apiService;

    public ObservableCollection<CategoriaServico> Categorias { get; } = new();

    public CategoriasViewModel(INavigationService navigationService, IApiService apiService)
        : base(navigationService)
    {
        _apiService = apiService;
        Title = "Categorias";
    }

    [RelayCommand]
    private async Task CarregarDadosAsync()
    {
        IsBusy = true;
        var categoriasApi = await _apiService.GetCategoriasAsync();

        Categorias.Clear();
        foreach (var cat in categoriasApi)
        {
            Categorias.Add(cat);
        }
        IsBusy = false;
    }

    [RelayCommand]
    private async Task NovaCategoriaAsync()
    {
        await NavigationService.NavigateToAsync<CategoriaFormViewModel>();
    }

    [RelayCommand]
    private async Task EditarCategoriaAsync(CategoriaServico categoria)
    {
        await NavigationService.NavigateToAsync<CategoriaFormViewModel, CategoriaServico>(categoria);
    }

    [RelayCommand]
    private async Task ExcluirCategoriaAsync(CategoriaServico categoria)
    {
        bool confirmar = await Shell.Current.DisplayAlertAsync("Excluir", $"Tem certeza que deseja excluir a categoria '{categoria.Nome}'? Esta ação não pode ser desfeita se houver serviços vinculados a ela.", "Sim", "Não");

        if (confirmar)
        {
            IsBusy = true;
            bool sucesso = await _apiService.ExcluirCategoriaAsync(categoria.Id);

            if (sucesso)
            {
                Categorias.Remove(categoria);
                await Shell.Current.DisplayAlertAsync("Sucesso", "Categoria excluída.", "OK");
            }
            else
            {
                await Shell.Current.DisplayAlertAsync("Erro", "Não foi possível excluir. Podem existir serviços vinculados a esta categoria.", "OK");
            }
            IsBusy = false;
        }
    }
}