using CabeleleiraLeila.Services.Navigation;
using CommunityToolkit.Mvvm.Input;

namespace CabeleleiraLeila.ViewModels.Admin;

public partial class HomeAdminViewModel : Base.BaseAdminViewModel
{
    public HomeAdminViewModel(INavigationService navigationService) : base(navigationService)
    {
        Title = "Painel Gerencial";
    }

    [RelayCommand]
    private async Task IrParaAgendamentos()
    {
        await NavigationService.NavigateToAsync<AgendaViewModel>();
    }

    [RelayCommand]
    private async Task IrParaRelatorios()
    {
        await NavigationService.NavigateToAsync<RelatoriosViewModel>();
    }


    [RelayCommand]
    private async Task IrParaCategorias()
    {
        await NavigationService.NavigateToAsync<CategoriasViewModel>();
    }

    [RelayCommand]
    private async Task IrParaServicos()
    {
        await NavigationService.NavigateToAsync<ServicosViewModel>();
    }

    [RelayCommand]
    private async Task IrParaExpediente()
    {
        await NavigationService.NavigateToAsync<ExpedienteViewModel>();
    }

    [RelayCommand]
    private async Task IrParaClientes()
    {
        await NavigationService.NavigateToAsync<ClientesViewModel>();
    }
}