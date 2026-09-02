using System.Collections.ObjectModel;
using System.ComponentModel;
using CabeleleiraLeila.Models;
using CabeleleiraLeila.Services.Api;
using CabeleleiraLeila.Services.Navigation;
using CabeleleiraLeila.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CabeleleiraLeila.ViewModels;

public partial class AgendarViewModel : BaseViewModel
{
    private readonly INavigationService _navigationService;
    private readonly IApiService _apiService;

    [ObservableProperty] private string nome;
    [ObservableProperty] private string telefone;
    [ObservableProperty] private string email;
    [ObservableProperty] private string observacao;
    [ObservableProperty] private DiaDisponivel diaSelecionado;
    [ObservableProperty] private string horarioSelecionado;
    [ObservableProperty] private decimal valorTotal;
    [ObservableProperty] private bool temServicoSelecionado;

    public ObservableCollection<CategoriaGrupo> CategoriasDisponiveis { get; } = new();
    public ObservableCollection<DiaDisponivel> DiasDisponiveis { get; } = new();
    public ObservableCollection<string> HorariosDisponiveis { get; } = new();

    public AgendarViewModel(INavigationService navigationService, IApiService apiService)
    {
        _navigationService = navigationService;
        _apiService = apiService;
        Title = "Agendar Horário";
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

    [RelayCommand]
    private async Task CarregarDadosAsync()
    {
        IsBusy = true;

        Nome = await SecureStorage.Default.GetAsync("user_nome") ?? string.Empty;
        Email = await SecureStorage.Default.GetAsync("user_email") ?? string.Empty;
        Telefone = await SecureStorage.Default.GetAsync("user_telefone") ?? string.Empty;

        var servicosApi = await _apiService.GetServicosAsync();
        CategoriasDisponiveis.Clear();
        var grupos = servicosApi.GroupBy(s => s.CategoriaNome ?? "Outros");

        foreach (var grupo in grupos)
        {
            var novaCategoria = new CategoriaGrupo { NomeCategoria = grupo.Key };
            foreach (var servico in grupo)
            {
                var servicoSelecionavel = new ServicoSelecionavel { Servico = servico, IsSelected = false };
                servicoSelecionavel.PropertyChanged += OnServicoPropertyChanged;
                novaCategoria.Servicos.Add(servicoSelecionavel);
            }
            CategoriasDisponiveis.Add(novaCategoria);
        }

        var disponibilidade = await _apiService.GetDisponibilidadeAsync(7);
        DiasDisponiveis.Clear();
        foreach (var dia in disponibilidade) DiasDisponiveis.Add(dia);

        IsBusy = false;
    }

    private void OnServicoPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ServicoSelecionavel.IsSelected))
        {
            var selecionados = CategoriasDisponiveis.SelectMany(c => c.Servicos).Where(s => s.IsSelected).ToList();
            ValorTotal = selecionados.Sum(s => s.Servico.Preco);
            TemServicoSelecionado = selecionados.Any();

            if (!TemServicoSelecionado)
            {
                DiaSelecionado = null;
                HorarioSelecionado = null;
            }
        }
    }

    [RelayCommand]
    private async Task ConfirmarAgendamento()
    {
        var servicosSelecionados = CategoriasDisponiveis.SelectMany(c => c.Servicos).Where(s => s.IsSelected).ToList();

        if (string.IsNullOrWhiteSpace(Nome) || string.IsNullOrWhiteSpace(Telefone) || string.IsNullOrWhiteSpace(Email) || !servicosSelecionados.Any())
        {
            await Shell.Current.DisplayAlertAsync("Aviso", "Preencha seus dados e selecione ao menos um serviço.", "OK");
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
            Nome = this.Nome,
            Telefone = this.Telefone,
            Email = this.Email,
            Observacao = this.Observacao,
            DataAgendamento = this.DiaSelecionado.Data
        };

        TimeSpan horaAtualSequencia = TimeSpan.Parse(this.HorarioSelecionado);
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

        var response = await _apiService.CriarAgendamentoExpressoAsync(request);
        IsBusy = false;

        if (response != null && string.IsNullOrEmpty(response.Error))
        {
            if (!string.IsNullOrEmpty(response.Sugestao))
            {
                await Shell.Current.DisplayAlertAsync("Dica da Leila!", $"{response.Message}\n\n{response.Sugestao}", "OK");
            }
            else
            {
                await Shell.Current.DisplayAlertAsync("Sucesso", "Agendamento realizado! A Leila agradece.", "OK");
            }

            MainThread.BeginInvokeOnMainThread(() => _navigationService.GoBackAsync());
        }
        else
        {
            await Shell.Current.DisplayAlertAsync("Erro", response?.Error ?? "O horário pode ter sido preenchido.", "OK");
        }
    }

    [RelayCommand]
    private void Voltar() => _navigationService.GoBackAsync();
}