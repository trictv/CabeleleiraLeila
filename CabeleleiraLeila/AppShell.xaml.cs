namespace CabeleleiraLeila;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(nameof(CabeleleiraLeila.Views.AgendarPage), typeof(CabeleleiraLeila.Views.AgendarPage));
        Routing.RegisterRoute(nameof(CabeleleiraLeila.Views.LoginPage), typeof(CabeleleiraLeila.Views.LoginPage));

        Routing.RegisterRoute(nameof(CabeleleiraLeila.Views.Admin.HomeAdminPage), typeof(CabeleleiraLeila.Views.Admin.HomeAdminPage));

        Routing.RegisterRoute(nameof(CabeleleiraLeila.Views.Admin.CategoriasPage), typeof(CabeleleiraLeila.Views.Admin.CategoriasPage));
        Routing.RegisterRoute(nameof(CabeleleiraLeila.Views.Admin.CategoriaFormPage), typeof(CabeleleiraLeila.Views.Admin.CategoriaFormPage));

        Routing.RegisterRoute(nameof(CabeleleiraLeila.Views.Admin.ServicosPage), typeof(CabeleleiraLeila.Views.Admin.ServicosPage));
        Routing.RegisterRoute(nameof(CabeleleiraLeila.Views.Admin.ServicoFormPage), typeof(CabeleleiraLeila.Views.Admin.ServicoFormPage));

        Routing.RegisterRoute(nameof(CabeleleiraLeila.Views.Admin.ClientesPage), typeof(CabeleleiraLeila.Views.Admin.ClientesPage));
        Routing.RegisterRoute(nameof(CabeleleiraLeila.Views.Admin.ClienteFormPage), typeof(CabeleleiraLeila.Views.Admin.ClienteFormPage));

        Routing.RegisterRoute(nameof(CabeleleiraLeila.Views.Admin.ExpedientePage), typeof(CabeleleiraLeila.Views.Admin.ExpedientePage));
        Routing.RegisterRoute(nameof(CabeleleiraLeila.Views.Admin.ExpedienteFormPage), typeof(CabeleleiraLeila.Views.Admin.ExpedienteFormPage));

        Routing.RegisterRoute(nameof(CabeleleiraLeila.Views.Admin.AgendaPage), typeof(CabeleleiraLeila.Views.Admin.AgendaPage));
        Routing.RegisterRoute(nameof(CabeleleiraLeila.Views.Admin.AgendaDetalhePage), typeof(CabeleleiraLeila.Views.Admin.AgendaDetalhePage));

        Routing.RegisterRoute(nameof(CabeleleiraLeila.Views.Admin.RelatoriosPage), typeof(CabeleleiraLeila.Views.Admin.RelatoriosPage));


        Routing.RegisterRoute(nameof(CabeleleiraLeila.Views.Cliente.HomeClientePage), typeof(CabeleleiraLeila.Views.Cliente.HomeClientePage));
        Routing.RegisterRoute(nameof(CabeleleiraLeila.Views.Cliente.AgendamentoDetalheClientePage), typeof(CabeleleiraLeila.Views.Cliente.AgendamentoDetalheClientePage));
        Routing.RegisterRoute(nameof(CabeleleiraLeila.Views.Cliente.HistoricoClientePage), typeof(CabeleleiraLeila.Views.Cliente.HistoricoClientePage));

        Routing.RegisterRoute(nameof(CabeleleiraLeila.Views.RedefinirSenhaPage), typeof(CabeleleiraLeila.Views.RedefinirSenhaPage));

    }
}