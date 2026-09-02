using System.Collections.ObjectModel;
using System.ComponentModel;
using CabeleleiraLeila.Models;
using CabeleleiraLeila.Services.Api;
using CabeleleiraLeila.Services.Navigation;
using CabeleleiraLeila.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CabeleleiraLeila.ViewModels.Admin;

public partial class AgendaDetalheViewModel : BaseAdminViewModel, IQueryAttributable
{
    private readonly IApiService _apiService;

    [ObservableProperty] private AgendamentoAdmin agendamentoAtual;
    [ObservableProperty] private string avisoUnificacao;
    [ObservableProperty] private bool temAvisoUnificacao;
    [ObservableProperty] private bool isModoEdicao;

    // Edição
    [ObservableProperty] private DiaDisponivel diaSelecionado;
    [ObservableProperty] private SlotAdmin horarioSelecionado;
    [ObservableProperty] private decimal valorTotalEdicao;
    [ObservableProperty] private bool temServicoSelecionado;

    public bool MostrarBotoesAcao => !IsModoEdicao;

    public ObservableCollection<CategoriaGrupo> CategoriasDisponiveis { get; } = new();
    public ObservableCollection<DiaDisponivel> DiasDisponiveis { get; } = new();
    public ObservableCollection<SlotAdmin> HorariosAdmin { get; } = new();

    private List<AgendamentoHistorico> _outrosAgendamentosDaSemana = new();
    private List<int> _agendamentosParaExcluirAoSalvar = new();

    public AgendaDetalheViewModel(INavigationService navigationService, IApiService apiService)
        : base(navigationService)
    {
        _apiService = apiService;
        Title = "Detalhes do Agendamento";
    }

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("Parameter", out var parameter) && parameter is AgendamentoAdmin ag)
        {
            AgendamentoAtual = ag;
            await ChecarPossibilidadeUnificacao(ag);
        }
    }

    private async Task ChecarPossibilidadeUnificacao(AgendamentoAdmin atual)
    {
        var historico = await _apiService.GetHistoricoClienteAsync(atual.ClienteId);

        _outrosAgendamentosDaSemana = historico.Where(h =>
            h.Id != atual.Id &&
            (h.Status == "PENDENTE" || h.Status == "CONFIRMADO") &&
            DateTime.TryParse(h.DataAgendamento, out var d) &&
            Math.Abs((d - DateTime.Parse(atual.DataAgendamento)).TotalDays) <= 6
        ).ToList();

        if (_outrosAgendamentosDaSemana.Any())
        {
            var datas = string.Join(", ", _outrosAgendamentosDaSemana.Select(o => o.DataFormatada));
            AvisoUnificacao = $"Este cliente possui agendamentos extras na semana ({datas}).";
            TemAvisoUnificacao = true;
        }
    }

    [RelayCommand]
    private async Task UnificarAgendamentosAsync()
    {
        await EntrarModoEdicaoAsync();

        var allServiceIds = new HashSet<int>(AgendamentoAtual.Itens.Select(i => i.Id));

        foreach (var outro in _outrosAgendamentosDaSemana)
        {
            foreach (var item in outro.Itens) allServiceIds.Add(item.ServicoId);
        }

        foreach (var cat in CategoriasDisponiveis)
        {
            foreach (var serv in cat.Servicos)
            {
                if (allServiceIds.Contains(serv.Servico.Id)) serv.IsSelected = true;
            }
        }

        _agendamentosParaExcluirAoSalvar = _outrosAgendamentosDaSemana.Select(a => a.Id).ToList();
        TemAvisoUnificacao = false;
        await Shell.Current.DisplayAlertAsync("Unificação Iniciada", "Os serviços foram combinados. Escolha o horário final para o encaixe e clique em Salvar.", "OK");
    }

    [RelayCommand]
    private async Task EntrarModoEdicaoAsync()
    {
        IsModoEdicao = true;
        OnPropertyChanged(nameof(MostrarBotoesAcao));
        IsBusy = true;

        var servicosApi = await _apiService.GetServicosAsync();
        CategoriasDisponiveis.Clear();
        var grupos = servicosApi.GroupBy(s => s.CategoriaNome ?? "Outros");

        foreach (var grupo in grupos)
        {
            var novaCategoria = new CategoriaGrupo { NomeCategoria = grupo.Key };
            foreach (var servico in grupo)
            {
                bool jaSelecionado = AgendamentoAtual.Itens.Any(i => i.ServicoNome == servico.Nome);
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

    [RelayCommand]
    private void CancelarEdicao()
    {
        IsModoEdicao = false;
        _agendamentosParaExcluirAoSalvar.Clear();
        OnPropertyChanged(nameof(MostrarBotoesAcao));
    }

    partial void OnDiaSelecionadoChanged(DiaDisponivel value)
    {
        HorariosAdmin.Clear();
        HorarioSelecionado = null;

        if (value != null && value.Funcionamento != null)
        {
            var periodos = new[] { value.Funcionamento.Manha, value.Funcionamento.Tarde };
            foreach (var p in periodos)
            {
                if (p == null || string.IsNullOrEmpty(p.Inicio) || string.IsNullOrEmpty(p.Fim)) continue;

                TimeSpan inicio = TimeSpan.Parse(p.Inicio);
                TimeSpan fim = TimeSpan.Parse(p.Fim);

                while (inicio < fim)
                {
                    string horaStr = inicio.ToString(@"hh\:mm");
                    string horaBancoStr = inicio.ToString(@"hh\:mm\:ss");

                    bool ocupado = value.HorariosOcupados != null && value.HorariosOcupados.Any(o =>
                        string.Compare(horaBancoStr, o.HoraInicio) >= 0 &&
                        string.Compare(horaBancoStr, o.HoraFim) < 0);

                    HorariosAdmin.Add(new SlotAdmin { Hora = horaStr, IsOcupado = ocupado });
                    inicio = inicio.Add(TimeSpan.FromMinutes(30));
                }
            }
        }
    }

    [RelayCommand]
    private void SelecionarSlot(SlotAdmin slot)
    {
        foreach (var s in HorariosAdmin) s.IsSelected = false;
        slot.IsSelected = true;
        HorarioSelecionado = slot;
    }

    private void OnServicoPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ServicoSelecionavel.IsSelected)) AtualizarTotalEdicao();
    }

    private void AtualizarTotalEdicao()
    {
        var selecionados = CategoriasDisponiveis.SelectMany(c => c.Servicos).Where(s => s.IsSelected).ToList();
        ValorTotalEdicao = selecionados.Sum(s => s.Servico.Preco);
        TemServicoSelecionado = selecionados.Any();
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
        if (DiaSelecionado == null || HorarioSelecionado == null)
        {
            await Shell.Current.DisplayAlertAsync("Aviso", "Por favor, selecione um dia e horário.", "OK");
            return;
        }

        if (HorarioSelecionado.IsOcupado)
        {
            bool forcar = await Shell.Current.DisplayAlertAsync("Encaixe", "Este horário já está ocupado. Como Administrador, deseja forçar o encaixe?", "Sim", "Não");
            if (!forcar) return;
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

        TimeSpan horaAtualSequencia = TimeSpan.Parse(HorarioSelecionado.Hora);
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

        if (sucesso)
        {
            foreach (var idParaDeletar in _agendamentosParaExcluirAoSalvar)
            {
                await _apiService.DeletarAgendamentoAsync(idParaDeletar);
            }

            await Shell.Current.DisplayAlertAsync("Sucesso", "Agendamento atualizado com sucesso!", "OK");
            MainThread.BeginInvokeOnMainThread(() => NavigationService.GoBackAsync());
        }
        else
        {
            await Shell.Current.DisplayAlertAsync("Erro", "Erro ao atualizar. Tente novamente.", "OK");
        }
        IsBusy = false;
    }

    [RelayCommand]
    private async Task AlterarStatusAsync(string novoStatus)
    {
        IsBusy = true;
        bool sucesso = await _apiService.AtualizarStatusAgendamentoAsync(AgendamentoAtual.Id, novoStatus);
        IsBusy = false;

        if (sucesso)
        {
            AgendamentoAtual.Status = novoStatus;
            OnPropertyChanged(nameof(AgendamentoAtual));
            await Shell.Current.DisplayAlertAsync("Sucesso", $"Status alterado para {novoStatus}.", "OK");
        }
    }

    [RelayCommand]
    private async Task ExcluirAgendamentoAsync()
    {
        bool confirmar = await Shell.Current.DisplayAlertAsync("Excluir", "Deseja cancelar e deletar este agendamento?", "Sim", "Não");
        if (confirmar)
        {
            IsBusy = true;
            await _apiService.DeletarAgendamentoAsync(AgendamentoAtual.Id);
            IsBusy = false;
            MainThread.BeginInvokeOnMainThread(() => NavigationService.GoBackAsync());
        }
    }
}