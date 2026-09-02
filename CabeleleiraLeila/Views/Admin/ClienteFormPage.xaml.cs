using CabeleleiraLeila.ViewModels.Admin;

namespace CabeleleiraLeila.Views.Admin;

public partial class ClienteFormPage : ContentPage
{
	public ClienteFormPage(ClienteFormViewModel clienteFormViewModel)
	{
		InitializeComponent();
		BindingContext = clienteFormViewModel;
    }
}