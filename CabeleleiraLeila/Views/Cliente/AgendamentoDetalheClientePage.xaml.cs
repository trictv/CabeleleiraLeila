using CabeleleiraLeila.ViewModels.Admin;
using CabeleleiraLeila.ViewModels.Cliente;

namespace CabeleleiraLeila.Views.Cliente;

public partial class AgendamentoDetalheClientePage : ContentPage
{
	public AgendamentoDetalheClientePage(AgendamentoDetalheClienteViewModel agendaDetalheViewModel)
	{
		InitializeComponent();
		BindingContext = agendaDetalheViewModel;
	}
}