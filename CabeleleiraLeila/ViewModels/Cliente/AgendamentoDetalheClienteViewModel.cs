using System.Collections.ObjectModel;
using System.ComponentModel;
using CabeleleiraLeila.Models;
using CabeleleiraLeila.Services.Api;
using CabeleleiraLeila.Services.Navigation;
using CabeleleiraLeila.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CabeleleiraLeila.ViewModels.Cliente;

public partial class AgendamentoDetalheClienteViewModel : BaseClienteViewModel, IQueryAttributable
{
    private readonly IApiService _apiService;

    [ObservableProperty] private AgendamentoHistorico agendamentoAtual;
    [ObservableProperty] private bool isModoEdicao;
    [ObservableProperty] private DiaDisponivel diaSelecionado;
    [ObservableProperty] private string horarioSelecionado;
    [ObservableProperty] private decimal valorTotalEdicao;
    [ObservableProperty] private bool temServicoSelecionado;

    public bool MostrarBotoesAcao => AgendamentoAtual?.PodeAlterar == true && !IsModoEdicao;

    public ObservableCollection<CategoriaGrupo> CategoriasDisponiveis { get; } = new();
    public ObservableCollection<DiaDisponivel> DiasDisponiveis { get; } = new();
    public ObservableCollection<string> HorariosDisponiveis { get; } = new();

    public AgendamentoDetalheClienteViewModel(INavigationService navigationService, IApiService apiService)
        : base(navigationService)
    {
        _apiService = apiService;
        Title = "Detalhes do Horário";
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("Parameter", out var parameter) && parameter is AgendamentoHistorico ag)
        {
            AgendamentoAtual = ag;
            OnPropertyChanged(nameof(MostrarBotoesAcao));
        }
    }

    partial void OnDiaSelecionadoChanged(DiaDisponivel value)
    {
        HorariosDisponiveis.Clear();
        HorarioSelecionado = string.Empty;
        if (value != null && value.SlotsLivres != null)
        {
            foreach (var slot in value.SlotsLivres) HorariosDisponiveis.Add(slot);
        }
    }

    partial void OnIsModoEdicaoChanged(bool value)
    {
        OnPropertyChanged(nameof(MostrarBotoesAcao));
        if (value)
        {
            _ = CarregarDadosEdicaoAsync();
        }
    }

    private async Task CarregarDadosEdicaoAsync()
    {
        IsBusy = true;

        var servicosApi = await _apiService.GetServicosAsync();
        CategoriasDisponiveis.Clear();
        var grupos = servicosApi.GroupBy(s => s.CategoriaNome ?? "Outros");

        foreach (var grupo in grupos)
        {
            var novaCategoria = new CategoriaGrupo { NomeCategoria = grupo.Key };
            foreach (var servico in grupo)
            {
                bool jaSelecionado = AgendamentoAtual.Itens.Any(i => i.ServicoId == servico.Id);

                var servicoSelecionavel = new ServicoSelecionavel { Servico = servico, IsSelected = jaSelecionado };
                servicoSelecionavel.PropertyChanged += OnServicoPropertyChanged;
                novaCategoria.Servicos.Add(servicoSelecionavel);
            }
            CategoriasDisponiveis.Add(novaCategoria);
        }

        AtualizarTotalEdicao();

        var disponibilidade = await _apiService.GetDisponibilidadeAsync(14);
        DiasDisponiveis.Clear();
        foreach (var dia in disponibilidade) DiasDisponiveis.Add(dia);

        IsBusy = false;
    }

    private void OnServicoPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ServicoSelecionavel.IsSelected))
        {
            AtualizarTotalEdicao();
        }
    }

    private void AtualizarTotalEdicao()
    {
        var selecionados = CategoriasDisponiveis.SelectMany(c => c.Servicos).Where(s => s.IsSelected).ToList();
        ValorTotalEdicao = selecionados.Sum(s => s.Servico.Preco);
        TemServicoSelecionado = selecionados.Any();

        if (!TemServicoSelecionado)
        {
            DiaSelecionado = null;
            HorarioSelecionado = null;
        }
    }

    [RelayCommand]
    private void AlternarModoEdicao()
    {
        IsModoEdicao = !IsModoEdicao;
    }

    [RelayCommand]
    private async Task SalvarAlteracaoAsync()
    {
        var servicosSelecionados = CategoriasDisponiveis.SelectMany(c => c.Servicos).Where(s => s.IsSelected).ToList();

        if (!servicosSelecionados.Any())
        {
            await Shell.Current.DisplayAlertAsync("Aviso", "Selecione ao menos um serviço.", "OK");
            return;
        }
        if (DiaSelecionado == null || string.IsNullOrWhiteSpace(HorarioSelecionado))
        {
            await Shell.Current.DisplayAlertAsync("Aviso", "Por favor, selecione um dia e horário disponíveis.", "OK");
            return;
        }

        IsBusy = true;
        var request = new AgendamentoRequest
        {
            DataAgendamento = DiaSelecionado.Data,
            Nome = "",
            Telefone = "",
            Email = "",
            Observacao = "" 
        };

        TimeSpan horaAtualSequencia = TimeSpan.Parse(HorarioSelecionado);
        foreach (var item in servicosSelecionados)
        {
            TimeSpan horaFim = horaAtualSequencia.Add(TimeSpan.FromMinutes(item.Servico.DuracaoMinutos));
            request.Servicos.Add(new ServicoRequestItem
            {
                ServicoId = item.Servico.Id,
                HoraInicio = horaAtualSequencia.ToString(@"hh\:mm"),
                HoraFim = horaFim.ToString(@"hh\:mm")
            });
            horaAtualSequencia = horaFim;
        }

        bool sucesso = await _apiService.AlterarAgendamentoClienteAsync(AgendamentoAtual.Id, request);
        IsBusy = false;

        if (sucesso)
        {
            await Shell.Current.DisplayAlertAsync("Sucesso", "Agendamento atualizado com sucesso!", "OK");
            await NavigationService.GoBackAsync(); 
        }
        else
        {
            await Shell.Current.DisplayAlertAsync("Erro", "O horário selecionado não está mais disponível.", "OK");
        }
    }

    [RelayCommand]
    private async Task SolicitarCancelamentoAsync()
    {
        bool confirmar = await Shell.Current.DisplayAlertAsync("Cancelar", "Deseja cancelar definitivamente este agendamento?", "Sim", "Não");
        if (confirmar)
        {
            IsBusy = true;
            bool sucesso = await _apiService.CancelarAgendamentoClienteAsync(AgendamentoAtual.Id);
            IsBusy = false;

            if (sucesso)
            {
                await Shell.Current.DisplayAlertAsync("Sucesso", "Cancelado com sucesso.", "OK");
                MainThread.BeginInvokeOnMainThread(() => NavigationService.GoBackAsync());
            }
        }
    }
}