using CabeleleiraLeila.ViewModels.Admin;

namespace CabeleleiraLeila.Views.Admin;

public partial class AgendaDetalhePage : ContentPage
{
	public AgendaDetalhePage(AgendaDetalheViewModel agendaDetalheViewModel)
	{
		InitializeComponent();
		BindingContext = agendaDetalheViewModel;
	}
}