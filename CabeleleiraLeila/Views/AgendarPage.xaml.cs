using CabeleleiraLeila.ViewModels;

namespace CabeleleiraLeila.Views;

public partial class AgendarPage : ContentPage
{
    private readonly AgendarViewModel _viewModel;

    public AgendarPage(AgendarViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        _viewModel = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();


        if (!_viewModel.CategoriasDisponiveis.Any())
        {
            _viewModel.CarregarDadosCommand.Execute(null);
        }
    }
}