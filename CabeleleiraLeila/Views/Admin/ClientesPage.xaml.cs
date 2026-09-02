using CabeleleiraLeila.ViewModels.Admin;

namespace CabeleleiraLeila.Views.Admin;

public partial class ClientesPage : ContentPage
{

	private readonly ClientesViewModel _viewModel;
	public ClientesPage(ClientesViewModel clientesViewModel)
	{
		InitializeComponent();
        _viewModel = clientesViewModel;
		BindingContext = clientesViewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (!_viewModel.Clientes.Any())
        {
            _viewModel.CarregarDadosCommand.Execute(null);
        }
    }
}