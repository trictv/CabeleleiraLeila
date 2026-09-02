
using CabeleleiraLeila.ViewModels;

namespace CabeleleiraLeila.Views;

public partial class InicioPage : ContentPage
{
    private readonly InicioViewModel _viewModel;

    public InicioPage(InicioViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        _viewModel = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();


        _viewModel.VerificarSessaoAtivaCommand.Execute(null);
    }
}