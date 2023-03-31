using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using SupplyChain.Server.Data.Repository;
using SupplyChain.Server.Hubs;
using SupplyChain.Server.Repositorios;
using SupplyChain.Shared;
using SupplyChain.Shared.Context;
using Syncfusion.Blazor;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupplyChain.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication();
            services.AddAuthorization();
            Bold.Licensing.BoldLicenseProvider.RegisterLicense("W2XPSOfZsRxe8tNwDyzLLeSXWmY4ye6r0x2hfMcYxds=");
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MTA0Njk4NUAzMjMwMmUzNDJlMzBaRFRIcHVVaVJ2K1ZFVTd6TXA4dnRXQ01EVlRTMEpXdXpkTnFUTGdMem9rPQ==");


            services.AddDbContext<AppDbContext>(options =>
            {
                options.EnableSensitiveDataLogging();
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });

            //especial para el gantt
            services.AddDbContext<GanttContext>(options =>
            {
                options.EnableSensitiveDataLogging();
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });
            
            services.AddCors(op=> 
            {
                op.AddDefaultPolicy(builder =>
                {
                    builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                });
            });
            #region "identity"
            services.AddIdentity<ApplicationUser, IdentityRole>(op =>
            {
                op.Password.RequireLowercase = false;
                op.Password.RequireNonAlphanumeric = false;
                op.Password.RequireUppercase = false;
                op.Password.RequireDigit = false;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                 options.TokenValidationParameters = new TokenValidationParameters()
                 {
                     ValidateIssuer = false,
                     ValidateAudience = false,
                     ValidateLifetime = true,
                     ValidateIssuerSigningKey = true,
                     IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                         Encoding.UTF8.GetBytes(Configuration["jwt:llave"]))
                 });

            #endregion


            #region "Repositorios"
            services.AddTransient<UsuariosRepository>();
            services.AddTransient<ServiciosRepository>();
            services.AddTransient<PedCliRepository>();
            services.AddTransient<ProgramaRepository>();
            services.AddTransient<ProductoRepository>();
            services.AddTransient<PrevisionRepository>();
            services.AddTransient<CompraRepository>();
            services.AddTransient<ProveedorRepository>();
            services.AddTransient<PedidosRepository>();
            services.AddTransient<FormulaRepository>();
            services.AddTransient<SolutionRepository>();
            services.AddTransient<StockCorregidoRepository>();
            services.AddTransient<SolicitudRepository>();
            services.AddTransient<PresupuestoAnteriorRepository>();
            services.AddTransient<PresupuestoRepository>();
            services.AddTransient<PrecioArticulosRepository>();
            services.AddTransient<GeneraRepository>();
            services.AddTransient<vDireccionesEntregaRepository>();
            services.AddTransient<NotificacionRepository>();
            services.AddTransient<vCondicionesPagoRepository>();
            services.AddTransient<vCondicionesEntregaRepository>();
            services.AddTransient<vTipoCambioRepository>();
            services.AddTransient<ProyectosGBPIRepository>();
            services.AddTransient<AreasRepository>();
            services.AddTransient<CeldasRepository>();
            services.AddTransient<ClienteRepository>();
            services.AddTransient<ClienteExternoRepository>();
            services.AddTransient<TipoAreaRepository>();
            services.AddTransient<LineasRepository>();
            services.AddTransient<UnidadesRepository>();
            services.AddTransient<vTransportesRepository>();
            services.AddTransient<MantCeldasRepository>();
            services.AddTransient<ProcunRepository>();
            services.AddTransient<ISORepository>();
            services.AddTransient<AspNetRolesRepository>();
            services.AddTransient<AspAmbRepository>();
            services.AddTransient<ProcalMPRepository>();
            services.AddTransient<ProcesoRepository>();
            services.AddTransient<ModulosUsuarioRepository>();
            services.AddTransient<ModuloRepository>();
            services.AddTransient<SolCotEmailRepository>();
            //services.AddTransient<CargaValoresRepository>();
            //services.AddTransient<MatproveRepository>();
            #endregion

            services.AddServerSideBlazor().AddCircuitOptions(options => { options.DetailedErrors = true; });
            services.AddControllersWithViews()
                .AddNewtonsoftJson(options =>
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore)
            .AddJsonOptions(options =>
             {
                 // Use the default property (Pascal) casing.
                 options.JsonSerializerOptions.PropertyNamingPolicy = null;
             });
            services.AddRazorPages();


            //Para invocar controladores en otros controladores a traves de DI
            services.AddMvc()
                .AddControllersAsServices();
            //.ConfigureApiBehaviorOptions(options =>
            //{
            //    options.SuppressModelStateInvalidFilter = true;
            //    options.SuppressConsumesConstraintForFormFileParameters = true;
            //    options.SuppressInferBindingSourcesForParameters = true;
            //});
            services.AddSignalR();
            services.AddResponseCompression(opts =>
            {
                opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                    new[] { "application/octet-stream" });
            });
            //services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            
            if (env.IsDevelopment() || env.IsProduction())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebAssemblyDebugging();
                //app.UseResponseCompression();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseRequestLocalization();
            app.UseHttpsRedirection();
            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();
            app.UseCors();
            app.UseAuthentication();
            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                //endpoints.Map("api/{**slug}", HandleApiFallback);
                endpoints.MapHub<SolicitudHub>("/solicitudhub");
                //endpoints.MapHub<OnlineUsersHub>("/onlinehub");
                
                endpoints.MapFallbackToFile("index.html");
                endpoints.MapHub<ChatHub>("/chathub");
            });
        }   

        private Task HandleApiFallback(HttpContext context)
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            return Task.CompletedTask;
        }

    }
}
