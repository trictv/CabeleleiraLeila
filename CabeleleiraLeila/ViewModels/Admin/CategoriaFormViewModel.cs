using CabeleleiraLeila.Models;
using CabeleleiraLeila.Services.Api;
using CabeleleiraLeila.Services.Navigation;
using CabeleleiraLeila.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CabeleleiraLeila.ViewModels.Admin;

public partial class CategoriaFormViewModel : BaseAdminViewModel, IQueryAttributable
{
    private readonly IApiService _apiService;

    [ObservableProperty]
    private CategoriaServico categoriaAtual;

    [ObservableProperty]
    private bool isEditando;

    public CategoriaFormViewModel(INavigationService navigationService, IApiService apiService)
        : base(navigationService)
    {
        _apiService = apiService;
        Title = "Nova Categoria";
        CategoriaAtual = new CategoriaServico { IsAtivo = true }; 
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("Parameter", out var parameter) && parameter is CategoriaServico cat)
        {
            CategoriaAtual = new CategoriaServico
            {
                Id = cat.Id,
                Nome = cat.Nome,
                Ativo = cat.Ativo
            };

            IsEditando = true;
            Title = "Editar Categoria";
        }
    }

    [RelayCommand]
    private async Task SalvarAsync()
    {
        if (string.IsNullOrWhiteSpace(CategoriaAtual.Nome))
        {
            await Shell.Current.DisplayAlertAsync("Aviso", "O nome da categoria é obrigatório.", "OK");
            return;
        }

        IsBusy = true;
        bool sucesso = await _apiService.SalvarCategoriaAsync(CategoriaAtual);
        IsBusy = false;

        if (sucesso)
        {
            await Shell.Current.DisplayAlertAsync("Sucesso", "Categoria salva com sucesso!", "OK");
            MainThread.BeginInvokeOnMainThread(() =>
            {
                NavigationService.GoBackAsync();
            });
        }
        else
        {
            await Shell.Current.DisplayAlertAsync("Erro", "Erro ao salvar a categoria. Tente novamente.", "OK");
        }
    }

    [RelayCommand]
    private void Cancelar() => NavigationService.GoBackAsync();
}