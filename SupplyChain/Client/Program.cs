using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SupplyChain.Client.Auth;
using SupplyChain.Client.HelperService;
using Syncfusion.Blazor;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SupplyChain.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
    //        Syncfusion.Licensing.SyncfusionLicenseProvider
    //.               RegisterLicense("NDc2NDI4QDMxMzkyZTMyMmUzMFJORXVzWnFYMWFSU2oxbEJ0NnV6N1RyRDg1NGRVU3N4MEhIazBlUzBHbGc9");

            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("NTAyNTMyQDMxMzkyZTMyMmUzMExZSVRWeURJODNsOHRJQ2c3T1N1aGpucy9JcmhMSVRlemxNMVVBYWFEQVE9");

            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            builder.RootComponents.Add<App>("#app");
            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            ConfigureServices(builder.Services);
            builder.Services.AddSyncfusionBlazor();
            // Register the Syncfusion locale service to customize the  SyncfusionBlazor component locale culture
            builder.Services.AddSingleton(typeof(ISyncfusionStringLocalizer), typeof(SyncfusionLocalizer));

            // Set the default culture of the application
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("es");
            CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("es");
            await builder.Build().RunAsync();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();//Sistema de Autorizacion
            services.AddAuthorizationCore();
            services.AddScoped<ProveedorAutenticacion>();
            services.AddScoped<AuthenticationStateProvider, ProveedorAutenticacion>(
                provider => provider.GetRequiredService<ProveedorAutenticacion>());

            services.AddScoped<ILoginService, ProveedorAutenticacion>(
                provider => provider.GetRequiredService<ProveedorAutenticacion>());

            services.AddSingleton<ToastService>();
        }
    }
}
