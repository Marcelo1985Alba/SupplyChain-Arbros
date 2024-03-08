using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SupplyChain.Client.Auth;
using SupplyChain.Client.HelperService;
using SupplyChain.Client.RepositoryHttp;
using Syncfusion.Blazor;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Syncfusion.Blazor.Popups;
using SupplyChain.Shared.Models;

namespace SupplyChain.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
    
            Syncfusion.Licensing.SyncfusionLicenseProvider
                .RegisterLicense("MTA0Njk4NUAzMjMwMmUzNDJlMzBaRFRIcHVVaVJ2K1ZFVTd6TXA4dnRXQ01EVlRTMEpXdXpkTnFUTGdMem9rPQ==");

            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.Services.AddMemoryCache();
            builder.RootComponents.Add<App>("#app");
            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            ConfigureServices(builder.Services);
            builder.Services.AddSyncfusionBlazor();
            // Register the Syncfusion locale service to customize the  SyncfusionBlazor component locale culture
            builder.Services.AddSingleton(typeof(ISyncfusionStringLocalizer), typeof(SyncfusionLocalizer));

            // Set the default culture of the application
            //CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("es");
            //CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("es");
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");
            CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("en-US");
            builder.Services.AddScoped<SfDialogService>();
            builder.Services.AddSyncfusionBlazor(options => { options.IgnoreScriptIsolation = true; });
            await builder.Build().RunAsync();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();//Sistema de Autorizacion
            services.AddAuthorizationCore();
            services.AddScoped<IRepositoryHttp, RepositoryHttp.RepositoryHttp>();
            services.AddScoped<ProveedorAutenticacion>();
            services.AddScoped<AuthenticationStateProvider, ProveedorAutenticacion>(
                provider => provider.GetRequiredService<ProveedorAutenticacion>());

            services.AddScoped<ILoginService, ProveedorAutenticacion>(
                provider => provider.GetRequiredService<ProveedorAutenticacion>());

            //JWT
            services.AddScoped<ProveedorAutenticacionJWT>();
            services.AddScoped<AuthenticationStateProvider, ProveedorAutenticacionJWT>(
                provider => provider.GetRequiredService<ProveedorAutenticacionJWT>());

            services.AddScoped<ILoginServiceJWT, ProveedorAutenticacionJWT>(
                provider => provider.GetRequiredService<ProveedorAutenticacionJWT>());

            services.AddScoped<ProductoService>();
            services.AddScoped<ClienteService>();
            services.AddScoped<DireccionEntregaService>();
            services.AddScoped<SolicitudService>();
            services.AddScoped<PrecioArticuloService>();
            services.AddScoped<PresupuestoService>();
            services.AddScoped<InventarioService>();
            services.AddScoped<CondicionPagoService>();
            services.AddScoped<CondicionEntregaService>();
            services.AddScoped<TipoCambioService>();
            services.AddScoped<AreasService>();
            services.AddScoped<CeldasService>();
            services.AddScoped<TipoAreaService>();
            services.AddScoped<LineasService>();
            services.AddScoped<UnidadesService>();
            services.AddScoped<ProyectosService>();
            services.AddScoped<PedCliService>();
            services.AddScoped<ServicioService>();
            services.AddScoped<TransporteService>();
            services.AddScoped<StockService>();
            services.AddScoped<MantCeldasService>();
            services.AddScoped<ProcunService>();
            services.AddScoped<EstadoPedidoService>();
            services.AddScoped<ChatService>();
            services.AddScoped<PdfService>();
            services.AddScoped<ExcelService>();
            services.AddScoped<RenovadorToken>();
            services.AddScoped<ISOService>();
            services.AddScoped<AspAmbService>();
            services.AddScoped<AspNetRolesService>();

            services.AddSingleton<ToastService>();
            services.AddScoped<ProcalMPService>();
            services.AddScoped<ControlCalidadService>();
            services.AddScoped<ProcesoService>();
            services.AddScoped<EstadoComprasService>();

            services.AddScoped<ModulosUsuarioService>();
            services.AddScoped<ProveedoresService>();
            services.AddScoped<SemaforoService>();
            services.AddScoped<MotivosPresupuestoService>();
            services.AddScoped<CampoComService>();
            services.AddScoped<ProcunProcesosService>();
            services.AddScoped<ProcedimientosService>();
        }
    }
}
