using System.Collections.ObjectModel;
using CabeleleiraLeila.Models;
using CabeleleiraLeila.Services.Api;
using CabeleleiraLeila.Services.Navigation;
using CabeleleiraLeila.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CabeleleiraLeila.ViewModels.Cliente;

public partial class HistoricoClienteViewModel : BaseClienteViewModel
{
    private readonly IApiService _apiService;

    [ObservableProperty]
    private DateTime dataInicial;

    [ObservableProperty]
    private DateTime dataFinal;

    [ObservableProperty]
    private bool temHistorico;

    public bool SemHistorico => !TemHistorico;
    public ObservableCollection<AgendamentoHistorico> ListaHistorico { get; } = new();

    public HistoricoClienteViewModel(INavigationService navigationService, IApiService apiService)
        : base(navigationService)
    {
        _apiService = apiService;
        Title = "Histórico de Atendimentos";
        DataInicial = DateTime.Today.AddDays(-30);
        DataFinal = DateTime.Today.AddDays(30);
    }

    partial void OnDataInicialChanged(DateTime value) { CarregarHistoricoCommand.Execute(null); }
    partial void OnDataFinalChanged(DateTime value) { CarregarHistoricoCommand.Execute(null); }

    [RelayCommand]
    private async Task CarregarHistoricoAsync()
    {
        if (DataFinal < DataInicial) return;
        IsBusy = true;

        var idString = await SecureStorage.Default.GetAsync("user_id");
        if (int.TryParse(idString, out int clienteId))
        {
            var historicoApi = await _apiService.GetHistoricoClientePeriodoAsync(clienteId, DataInicial, DataFinal);
            ListaHistorico.Clear();
            foreach (var item in historicoApi)
            {
                ListaHistorico.Add(item);
            }
            TemHistorico = ListaHistorico.Any();
            OnPropertyChanged(nameof(SemHistorico));
        }
        IsBusy = false;
    }

    [RelayCommand]
    private async Task AbrirAgendamentoAsync(AgendamentoHistorico agendamento)
    {
        await NavigationService.NavigateToAsync<AgendamentoDetalheClienteViewModel, AgendamentoHistorico>(agendamento);
    }
}