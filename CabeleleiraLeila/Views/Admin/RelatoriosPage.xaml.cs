using CabeleleiraLeila.ViewModels.Admin;

namespace CabeleleiraLeila.Views.Admin;

public partial class RelatoriosPage : ContentPage
{
    private readonly RelatoriosViewModel _viewModel;

    public RelatoriosPage(RelatoriosViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.VerificarAcessoCommand.ExecuteAsync(null);
        await _viewModel.CarregarDadosCommand.ExecuteAsync(null);
    }
}