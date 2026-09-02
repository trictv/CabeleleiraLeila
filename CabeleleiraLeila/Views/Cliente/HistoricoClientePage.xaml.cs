using CabeleleiraLeila.ViewModels.Cliente;

namespace CabeleleiraLeila.Views.Cliente;

public partial class HistoricoClientePage : ContentPage
{
    private readonly HistoricoClienteViewModel _viewModel;

    public HistoricoClientePage(HistoricoClienteViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        _viewModel = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.CarregarHistoricoCommand.Execute(null);
    }
}