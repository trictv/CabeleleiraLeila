using CabeleleiraLeila.ViewModels.Cliente;

namespace CabeleleiraLeila.Views.Cliente;

public partial class HomeClientePage : ContentPage
{
    private readonly HomeClienteViewModel _viewModel;

    public HomeClientePage(HomeClienteViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        _viewModel = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.CarregarDadosCommand.Execute(null);
    }

    protected override bool OnBackButtonPressed()
    {
        return true;
    }
}