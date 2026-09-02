using System.Collections.ObjectModel;
using CabeleleiraLeila.Models;
using CabeleleiraLeila.Services.Api;
using CabeleleiraLeila.Services.Navigation;
using CabeleleiraLeila.ViewModels.Base;
using CommunityToolkit.Mvvm.Input;

namespace CabeleleiraLeila.ViewModels.Admin;

public partial class ClientesViewModel : BaseAdminViewModel
{
    private readonly IApiService _apiService;

    public ObservableCollection<ClienteAdmin> Clientes { get; } = new();

    public ClientesViewModel(INavigationService navigationService, IApiService apiService)
        : base(navigationService)
    {
        _apiService = apiService;
        Title = "Clientes";
    }

    [RelayCommand]
    private async Task CarregarDadosAsync()
    {
        IsBusy = true;
        var clientesApi = await _apiService.GetAdminClientesAsync();

        Clientes.Clear();
        foreach (var c in clientesApi)
        {
            Clientes.Add(c);
        }
        IsBusy = false;
    }

    [RelayCommand]
    private async Task NovoClienteAsync() => await NavigationService.NavigateToAsync<ClienteFormViewModel>();

    [RelayCommand]
    private async Task EditarClienteAsync(ClienteAdmin cliente) => await NavigationService.NavigateToAsync<ClienteFormViewModel, ClienteAdmin>(cliente);

    [RelayCommand]
    private async Task ExcluirClienteAsync(ClienteAdmin cliente)
    {
        bool confirmar = await Shell.Current.DisplayAlertAsync("Excluir", $"Deseja inativar/excluir {cliente.Nome}?", "Sim", "Não");
        if (confirmar)
        {
            IsBusy = true;
            if (await _apiService.ExcluirAdminClienteAsync(cliente.Id))
            {
                Clientes.Remove(cliente);
                await Shell.Current.DisplayAlertAsync("Sucesso", "Cliente excluído.", "OK");
            }
            IsBusy = false;
        }
    }
}