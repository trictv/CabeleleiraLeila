
using CabeleleiraLeila.ViewModels.Admin;

namespace CabeleleiraLeila.Views.Admin;

public partial class HomeAdminPage : ContentPage
{
    private readonly HomeAdminViewModel _viewModel;

    public HomeAdminPage(HomeAdminViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        
        await _viewModel.VerificarAcessoCommand.ExecuteAsync(null);
    }

    protected override bool OnBackButtonPressed()
    {
        return true;
    }
}