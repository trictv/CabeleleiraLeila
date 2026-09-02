using CabeleleiraLeila.ViewModels;

namespace CabeleleiraLeila.Views;

public partial class RedefinirSenhaPage : ContentPage
{
    public RedefinirSenhaPage(RedefinirSenhaViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}