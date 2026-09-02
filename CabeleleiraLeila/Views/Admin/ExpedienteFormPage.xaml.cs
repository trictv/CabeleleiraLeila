using CabeleleiraLeila.ViewModels.Admin;

namespace CabeleleiraLeila.Views.Admin;

public partial class ExpedienteFormPage : ContentPage
{
	public ExpedienteFormPage(ExpedienteFormViewModel expedienteFormViewModel)
	{
		InitializeComponent();
		BindingContext = expedienteFormViewModel;
	}
}