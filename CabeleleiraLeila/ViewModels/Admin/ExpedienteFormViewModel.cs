using CabeleleiraLeila.Models;
using CabeleleiraLeila.Services.Api;
using CabeleleiraLeila.Services.Navigation;
using CabeleleiraLeila.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CabeleleiraLeila.ViewModels.Admin;

public partial class ExpedienteFormViewModel : BaseAdminViewModel, IQueryAttributable
{
    private readonly IApiService _apiService;

    [ObservableProperty]
    private HorarioFuncionamento horarioAtual;

    public ExpedienteFormViewModel(INavigationService navigationService, IApiService apiService)
        : base(navigationService)
    {
        _apiService = apiService;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("Parameter", out var parameter) && parameter is HorarioFuncionamento h)
        {
            Title = $"Editar {h.NomeDiaSemana}";

            HorarioAtual = new HorarioFuncionamento
            {
                Id = h.Id,
                DiaSemana = h.DiaSemana,
                Fechado = h.Fechado,
                HoraAberturaManha = h.HoraAberturaManha,
                HoraFechamentoManha = h.HoraFechamentoManha,
                HoraAberturaTarde = h.HoraAberturaTarde,
                HoraFechamentoTarde = h.HoraFechamentoTarde
            };
        }
    }

    [RelayCommand]
    private async Task SalvarAsync()
    {
        IsBusy = true;
        bool sucesso = await _apiService.AtualizarExpedienteAsync(HorarioAtual);
        IsBusy = false;

        if (sucesso)
        {
            await Shell.Current.DisplayAlertAsync("Sucesso", "Horários atualizados!", "OK");

            MainThread.BeginInvokeOnMainThread(() => NavigationService.GoBackAsync());
        }
        else
        {
            await Shell.Current.DisplayAlertAsync("Erro", "Erro ao salvar expediente.", "OK");
        }
    }

    [RelayCommand]
    private void Cancelar() => NavigationService.GoBackAsync();
}