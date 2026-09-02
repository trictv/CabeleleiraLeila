using System.Collections.ObjectModel;
using CabeleleiraLeila.Models;
using CabeleleiraLeila.Services.Api;
using CabeleleiraLeila.Services.Navigation;
using CabeleleiraLeila.ViewModels.Base;
using CommunityToolkit.Mvvm.Input;

namespace CabeleleiraLeila.ViewModels.Admin;

public partial class ExpedienteViewModel : BaseAdminViewModel
{
    private readonly IApiService _apiService;

    public ObservableCollection<HorarioFuncionamento> DiasDaSemana { get; } = new();

    public ExpedienteViewModel(INavigationService navigationService, IApiService apiService)
        : base(navigationService)
    {
        _apiService = apiService;
        Title = "Expediente da Semana";
    }

    [RelayCommand]
    private async Task CarregarDadosAsync()
    {
        IsBusy = true;
        var horarios = await _apiService.GetExpedienteAsync();

        DiasDaSemana.Clear();
        foreach (var dia in horarios)
        {
            DiasDaSemana.Add(dia);
        }
        IsBusy = false;
    }

    [RelayCommand]
    private async Task EditarDiaAsync(HorarioFuncionamento horario)
    {
        await NavigationService.NavigateToAsync<ExpedienteFormViewModel, HorarioFuncionamento>(horario);
    }
}