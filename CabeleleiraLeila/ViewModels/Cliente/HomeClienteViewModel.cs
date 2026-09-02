using System.Collections.ObjectModel;
using CabeleleiraLeila.Models;
using CabeleleiraLeila.Services.Api;
using CabeleleiraLeila.Services.Navigation;
using CabeleleiraLeila.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CabeleleiraLeila.ViewModels.Cliente;

public partial class HomeClienteViewModel : BaseClienteViewModel
{
    private readonly IApiService _apiService;

    [ObservableProperty] private string nomeCliente;
    [ObservableProperty] private string saudacao;
    [ObservableProperty] private bool temProximosAgendamentos;

    public bool SemProximosAgendamentos => !TemProximosAgendamentos;
    public ObservableCollection<AgendamentoHistorico> ProximosAgendamentos { get; } = new();

    public HomeClienteViewModel(INavigationService navigationService, IApiService apiService)
        : base(navigationService)
    {
        _apiService = apiService;
        Title = "Home";
    }

    private void ConfigurarSaudacao()
    {
        var hora = DateTime.Now.Hour;
        if (hora < 12) Saudacao = "Bom dia";
        else if (hora < 18) Saudacao = "Boa tarde";
        else Saudacao = "Boa noite";
    }

    [RelayCommand]
    private async Task CarregarDadosAsync()
    {
        IsBusy = true;
        var nomeSalvo = await SecureStorage.Default.GetAsync("user_nome");
        NomeCliente = !string.IsNullOrEmpty(nomeSalvo) ? nomeSalvo.Split(' ')[0] : "Cliente";
        ConfigurarSaudacao();

        var idString = await SecureStorage.Default.GetAsync("user_id");
        if (int.TryParse(idString, out int clienteId) && clienteId > 0)
        {
            var historicoCompleto = await _apiService.GetHistoricoClienteAsync(clienteId);

            var futuros = historicoCompleto
                .Where(a => a.Status != "CANCELADO" && DateTime.TryParse(a.DataAgendamento, out var d) && d.Date >= DateTime.Today.Date)
                .OrderBy(a => a.DataAgendamento)
                .ToList();

            ProximosAgendamentos.Clear();
            foreach (var item in futuros) ProximosAgendamentos.Add(item);

            TemProximosAgendamentos = ProximosAgendamentos.Any();
            OnPropertyChanged(nameof(SemProximosAgendamentos));
        }

        IsBusy = false;
    }

    [RelayCommand]
    private async Task NovoAgendamentoAsync()
    {
        await NavigationService.NavigateToAsync<ViewModels.AgendarViewModel>();
    }

    [RelayCommand]
    private async Task AbrirAgendamentoAsync(AgendamentoHistorico agendamento)
    {
        await NavigationService.NavigateToAsync<AgendamentoDetalheClienteViewModel, AgendamentoHistorico>(agendamento);
    }

    [RelayCommand]
    private async Task VerHistoricoAsync()
    {
        await NavigationService.NavigateToAsync<HistoricoClienteViewModel>();
    }
}