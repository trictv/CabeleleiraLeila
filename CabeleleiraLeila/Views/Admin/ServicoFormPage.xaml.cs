using CabeleleiraLeila.ViewModels.Admin;

namespace CabeleleiraLeila.Views.Admin;

public partial class ServicoFormPage : ContentPage
{
    public ServicoFormPage(ServicoFormViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}