using System.Collections.ObjectModel;
using CabeleleiraLeila.Models;
using CabeleleiraLeila.Services.Api;
using CabeleleiraLeila.Services.Navigation;
using CabeleleiraLeila.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CabeleleiraLeila.ViewModels.Admin;

public partial class AgendaViewModel : BaseAdminViewModel
{
    private readonly IApiService _apiService;
    private List<AgendamentoAdmin> _todosAgendamentos = new();

    public ObservableCollection<AgendamentoAdmin> AgendamentosFiltrados { get; } = new();
    public bool SemAgendamentos => AgendamentosFiltrados.Count == 0;

    [ObservableProperty]
    private DateTime dataFiltro = DateTime.Today;

    public AgendaViewModel(INavigationService navigationService, IApiService apiService)
        : base(navigationService)
    {
        _apiService = apiService;
        Title = "Agenda de Hoje";
    }

    partial void OnDataFiltroChanged(DateTime value)
    {
        Title = value.Date == DateTime.Today.Date ? "Agenda de Hoje" : $"Agenda: {value:dd/MM}";
        FiltrarLista();
    }

    [RelayCommand]
    private async Task CarregarDadosAsync()
    {
        IsBusy = true;
        _todosAgendamentos = await _apiService.GetAdminAgendamentosAsync();
        FiltrarLista();
        IsBusy = false;
    }

    private void FiltrarLista()
    {
        var filtrados = _todosAgendamentos
            .Where(a => DateTime.TryParse(a.DataAgendamento, out var d) && d.Date == DataFiltro.Date)
            .OrderBy(a => a.HoraInicioGeral)
            .ToList();

        AgendamentosFiltrados.Clear();
        foreach (var item in filtrados)
        {
            AgendamentosFiltrados.Add(item);
        }

        OnPropertyChanged(nameof(SemAgendamentos));
    }

    [RelayCommand]
    private async Task AbrirDetalhesAsync(AgendamentoAdmin agendamento)
    {
        await NavigationService.NavigateToAsync<AgendaDetalheViewModel, AgendamentoAdmin>(agendamento);
    }

    [RelayCommand]
    private async Task NovoAgendamentoAsync()
    {
        await NavigationService.NavigateToAsync<AgendarViewModel>();
    }
}