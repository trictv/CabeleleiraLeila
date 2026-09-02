using CabeleleiraLeila.ViewModels.Admin;

namespace CabeleleiraLeila.Views.Admin;

public partial class AgendaPage : ContentPage
{

	private readonly AgendaViewModel _viewModel;
    public AgendaPage(AgendaViewModel agendaViewModel)
	{
		InitializeComponent();
		_viewModel = agendaViewModel;
        BindingContext = agendaViewModel;
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.VerificarAcessoCommand.ExecuteAsync(null);
        await _viewModel.CarregarDadosCommand.ExecuteAsync(null);
    }
}