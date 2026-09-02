using CabeleleiraLeila.ViewModels.Admin;

namespace CabeleleiraLeila.Views.Admin;

public partial class ServicosPage : ContentPage
{
    private readonly ServicosViewModel _viewModel;

    public ServicosPage(ServicosViewModel viewModel)
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