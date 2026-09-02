using CabeleleiraLeila.ViewModels.Admin;

namespace CabeleleiraLeila.Views.Admin;

public partial class ExpedientePage : ContentPage
{
	private readonly ExpedienteViewModel _viewModel;
    public ExpedientePage(ExpedienteViewModel expedienteViewModel)
	{
		InitializeComponent();
		_viewModel = expedienteViewModel;
        BindingContext = expedienteViewModel;
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.CarregarDadosCommand.Execute(null);
    }
}