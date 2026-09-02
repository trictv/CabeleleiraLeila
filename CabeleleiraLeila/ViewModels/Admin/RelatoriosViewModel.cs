using System.Collections.ObjectModel;
using CabeleleiraLeila.Models;
using CabeleleiraLeila.Services.Api;
using CabeleleiraLeila.Services.Navigation;
using CabeleleiraLeila.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CabeleleiraLeila.ViewModels.Admin;

public partial class RelatoriosViewModel : BaseAdminViewModel
{
    private readonly IApiService _apiService;

    [ObservableProperty]
    private decimal faturamentoTotal;

    [ObservableProperty]
    private int qtdAtendimentos;

    [ObservableProperty]
    private bool temServicos;

    public bool SemServicos => !TemServicos;

    [ObservableProperty]
    private DateTime dataInicial;

    [ObservableProperty]
    private DateTime dataFinal;

    public ObservableCollection<ServicoBuscado> ServicosMaisBuscados { get; } = new();

    public RelatoriosViewModel(INavigationService navigationService, IApiService apiService)
        : base(navigationService)
    {
        _apiService = apiService;
        Title = "Desempenho";

        DateTime hoje = DateTime.Today;
        DataInicial = new DateTime(hoje.Year, hoje.Month, 1);
        DataFinal = hoje;
    }

    partial void OnDataInicialChanged(DateTime value) { CarregarDadosCommand.Execute(null); }
    partial void OnDataFinalChanged(DateTime value) { CarregarDadosCommand.Execute(null); }

    [RelayCommand]
    private async Task CarregarDadosAsync()
    {
        if (DataFinal < DataInicial) return;

        IsBusy = true;

        var relatorio = await _apiService.GetRelatorioDesempenhoAsync(DataInicial, DataFinal);

        FaturamentoTotal = relatorio.Faturamento;
        QtdAtendimentos = relatorio.QtdAtendimentos;

        ServicosMaisBuscados.Clear();
        foreach (var servico in relatorio.ServicosMaisBuscados)
        {
            ServicosMaisBuscados.Add(servico);
        }

        TemServicos = ServicosMaisBuscados.Any();
        OnPropertyChanged(nameof(SemServicos));

        IsBusy = false;
    }
}