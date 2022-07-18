using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using SupplyChain.Client.Auth;
using SupplyChain.Client.HelperService;
using SupplyChain.Client.RepositoryHttp;
using Syncfusion.Blazor;
using System;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace SupplyChain.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
    
            Syncfusion.Licensing.SyncfusionLicenseProvider
                .RegisterLicense("NTU5NDk0QDMxMzkyZTM0MmUzMEtURElKREJxU1B4WVhyUFZYdGRoYVJ4WHU1ZVNZN1U4QjFGNCs0cGRORmM9");

            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            builder.RootComponents.Add<App>("#app");
            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            //builder.Services.AddHttpClient<HttpClientConToken>(
            //   cliente => cliente.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
            //   .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

            //builder.Services.AddHttpClient<HttpClientSinToken>(
            //   cliente => cliente.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress));

            builder.Services.AddSyncfusionBlazor();
            // Register the Syncfusion locale service to customize the  SyncfusionBlazor component locale culture
            builder.Services.AddSingleton(typeof(ISyncfusionStringLocalizer), typeof(SyncfusionLocalizer));

            // Set the default culture of the application
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("es");
            CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("es");
            ConfigureServices(builder.Services);
            await builder.Build().RunAsync();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();//Sistema de Autorizacion
            services.AddAuthorizationCore();
            services.AddScoped<IRepositoryHttp, RepositoryHttp.RepositoryHttp>();
            services.AddScoped<ProveedorAutenticacion>();

            //Auth sin jwt
            //services.AddScoped<AuthenticationStateProvider, ProveedorAutenticacion>(
            //    provider => provider.GetRequiredService<ProveedorAutenticacion>());

            //services.AddScoped<ILoginService, ProveedorAutenticacion>(
            //    provider => provider.GetRequiredService<ProveedorAutenticacion>());


            //JWT
            services.AddScoped<ProveedorAutenticacionJWT>();
            services.AddScoped<AuthenticationStateProvider, ProveedorAutenticacionJWT>(
                provider => provider.GetRequiredService<ProveedorAutenticacionJWT>());

            services.AddScoped<ILoginServiceJWT, ProveedorAutenticacionJWT>(
                provider => provider.GetRequiredService<ProveedorAutenticacionJWT>());

            
            services.AddScoped<RenovadorToken>();
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
            services.AddScoped<PdfService>();
            services.AddScoped<ExcelService>();


            services.AddSingleton<ToastService>();
            services.AddApiAuthorization();
        }
    }
}
