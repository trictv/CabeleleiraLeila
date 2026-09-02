using CabeleleiraLeila.Services.Api;
using CabeleleiraLeila.Services.Navigation;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Handlers;
using UraniumUI;

namespace CabeleleiraLeila
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseUraniumUI()
                .UseUraniumUIMaterial()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");

                    fonts.AddMaterialSymbolsFonts();
                });


            builder.Services.AddHttpClient<IApiService, ApiService>(client =>
            {
                client.BaseAddress = new Uri("https://apicabelereiraleila.trictv.com.br/api/");
            })
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                var handler = new HttpClientHandler();
                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;

                return handler;
            });

            builder.Services.AddSingleton<
                INavigationService,
                NavigationService>();

            builder.Services.AddTransient<CabeleleiraLeila.Views.InicioPage>();
            builder.Services.AddTransient<CabeleleiraLeila.ViewModels.InicioViewModel>();

            builder.Services.AddTransient<CabeleleiraLeila.Views.AgendarPage>();
            builder.Services.AddTransient<CabeleleiraLeila.ViewModels.AgendarViewModel>();

            builder.Services.AddTransient<CabeleleiraLeila.Views.LoginPage>();
            builder.Services.AddTransient<CabeleleiraLeila.ViewModels.LoginViewModel>();

            builder.Services.AddTransient<CabeleleiraLeila.Views.Admin.HomeAdminPage>();
            builder.Services.AddTransient<CabeleleiraLeila.ViewModels.Admin.HomeAdminViewModel>();

            builder.Services.AddTransient<CabeleleiraLeila.Views.Admin.CategoriasPage>();
            builder.Services.AddTransient<CabeleleiraLeila.ViewModels.Admin.CategoriasViewModel>();

            builder.Services.AddTransient<CabeleleiraLeila.Views.Admin.CategoriaFormPage>();
            builder.Services.AddTransient<CabeleleiraLeila.ViewModels.Admin.CategoriaFormViewModel>();

            builder.Services.AddTransient<CabeleleiraLeila.Views.Admin.ServicosPage>();
            builder.Services.AddTransient<CabeleleiraLeila.ViewModels.Admin.ServicosViewModel>();

            builder.Services.AddTransient<CabeleleiraLeila.Views.Admin.ServicoFormPage>();
            builder.Services.AddTransient<CabeleleiraLeila.ViewModels.Admin.ServicoFormViewModel>();

            builder.Services.AddTransient<CabeleleiraLeila.Views.Admin.ClientesPage>();
            builder.Services.AddTransient<CabeleleiraLeila.ViewModels.Admin.ClientesViewModel>();

            builder.Services.AddTransient<CabeleleiraLeila.Views.Admin.ClienteFormPage>();
            builder.Services.AddTransient<CabeleleiraLeila.ViewModels.Admin.ClienteFormViewModel>();

            builder.Services.AddTransient<CabeleleiraLeila.Views.Admin.ExpedientePage>();
            builder.Services.AddTransient<CabeleleiraLeila.ViewModels.Admin.ExpedienteViewModel>();

            builder.Services.AddTransient<CabeleleiraLeila.Views.Admin.ExpedienteFormPage>();
            builder.Services.AddTransient<CabeleleiraLeila.ViewModels.Admin.ExpedienteFormViewModel>();

            builder.Services.AddTransient<CabeleleiraLeila.Views.Admin.AgendaPage>();
            builder.Services.AddTransient<CabeleleiraLeila.ViewModels.Admin.AgendaViewModel>();

            builder.Services.AddTransient<CabeleleiraLeila.Views.Admin.AgendaDetalhePage>();
            builder.Services.AddTransient<CabeleleiraLeila.ViewModels.Admin.AgendaDetalheViewModel>();

            builder.Services.AddTransient<CabeleleiraLeila.Views.Admin.RelatoriosPage>();
            builder.Services.AddTransient<CabeleleiraLeila.ViewModels.Admin.RelatoriosViewModel>();


            builder.Services.AddTransient<CabeleleiraLeila.Views.Cliente.HomeClientePage>();
            builder.Services.AddTransient<CabeleleiraLeila.ViewModels.Cliente.HomeClienteViewModel>();

            builder.Services.AddTransient<CabeleleiraLeila.Views.Cliente.AgendamentoDetalheClientePage>();
            builder.Services.AddTransient<CabeleleiraLeila.ViewModels.Cliente.AgendamentoDetalheClienteViewModel>();

            builder.Services.AddTransient<CabeleleiraLeila.Views.Cliente.HistoricoClientePage>();
            builder.Services.AddTransient<CabeleleiraLeila.ViewModels.Cliente.HistoricoClienteViewModel>();

            builder.Services.AddTransient<CabeleleiraLeila.Views.RedefinirSenhaPage>();
            builder.Services.AddTransient<CabeleleiraLeila.ViewModels.RedefinirSenhaViewModel>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
