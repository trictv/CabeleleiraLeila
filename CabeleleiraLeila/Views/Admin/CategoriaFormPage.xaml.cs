using CabeleleiraLeila.ViewModels.Admin;

namespace CabeleleiraLeila.Views.Admin;

public partial class CategoriaFormPage : ContentPage
{
    public CategoriaFormPage(CategoriaFormViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}