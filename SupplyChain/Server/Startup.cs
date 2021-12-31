using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SupplyChain.Server.Data.Repository;
using SupplyChain.Server.Repositorios;
using Syncfusion.Blazor;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using static SupplyChain.Server.Startup;

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

            services.AddDbContext<AppDbContext>(options =>
            {
                options.EnableSensitiveDataLogging();
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });

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
            services.AddMvc(options=> options.AddCommaSeparatedArrayModelBinderProvider())
                .AddControllersAsServices();
                //.ConfigureApiBehaviorOptions(options =>
                //{
                //    options.SuppressModelStateInvalidFilter = true;
                //    options.SuppressConsumesConstraintForFormFileParameters = true;
                //    options.SuppressInferBindingSourcesForParameters = true;
                //});

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
            app.UseAuthentication();
            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                //endpoints.Map("api/{**slug}", HandleApiFallback);
                endpoints.MapFallbackToFile("index.html");
            });
        }

        private Task HandleApiFallback(HttpContext context)
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            return Task.CompletedTask;
        }

        public class CommaSeparatedArrayModelBinderProvider : IModelBinderProvider
        {
            public IModelBinder GetBinder(ModelBinderProviderContext context)
            {
                if (context == null)
                    throw new ArgumentNullException(nameof(context));

                return CommaSeparatedArrayModelBinder.IsSupportedModelType(context.Metadata.ModelType) ? new CommaSeparatedArrayModelBinder() : null;
            }
        }

        public class CommaSeparatedArrayModelBinder : IModelBinder
        {
            private static Task CompletedTask => Task.CompletedTask;

            private static readonly Type[] supportedElementTypes = {
            typeof(int), typeof(long), typeof(short), typeof(byte),
            typeof(uint), typeof(ulong), typeof(ushort), typeof(Guid)
        };

            public Task BindModelAsync(ModelBindingContext bindingContext)
            {
                if (!IsSupportedModelType(bindingContext.ModelType)) return CompletedTask;

                var providerValue = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

                if (providerValue == ValueProviderResult.None) return CompletedTask;

                // Each value self may contains a series of actual values, split it with comma
                var strs = providerValue.Values.SelectMany(s => s.Split(',', StringSplitOptions.RemoveEmptyEntries)).ToList();

                if (!strs.Any() || strs.Any(s => String.IsNullOrWhiteSpace(s)))
                    return CompletedTask;

                var elementType = bindingContext.ModelType.GetElementType();
                if (elementType == null) return CompletedTask;

                var realResult = CopyAndConvertArray(strs, elementType);

                bindingContext.Result = ModelBindingResult.Success(realResult);

                return CompletedTask;
            }

            internal static bool IsSupportedModelType(Type modelType)
            {
                return modelType.IsArray && modelType.GetArrayRank() == 1
                        && modelType.HasElementType
                        && supportedElementTypes.Contains(modelType.GetElementType());
            }

            private static Array CopyAndConvertArray(IList<string> sourceArray, Type elementType)
            {
                var targetArray = Array.CreateInstance(elementType, sourceArray.Count);
                if (sourceArray.Count > 0)
                {
                    var converter = TypeDescriptor.GetConverter(elementType);
                    for (var i = 0; i < sourceArray.Count; i++)
                        targetArray.SetValue(converter.ConvertFromString(sourceArray[i]), i);
                }
                return targetArray;
            }
        }
    }

    public static class CommaSeparatedArrayModelBinderServiceCollectionExtensions
    {
        private static int FirstIndexOfOrDefault<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            var result = 0;

            foreach (var item in source)
            {
                if (predicate(item))
                    return result;

                result++;
            }

            return -1;
        }

        private static int FindModelBinderProviderInsertLocation(this IList<IModelBinderProvider> modelBinderProviders)
        {
            var index = modelBinderProviders.FirstIndexOfOrDefault(i => i is FloatingPointTypeModelBinderProvider);
            return index < 0 ? index : index + 1;
        }

        public static void InsertCommaSeparatedArrayModelBinderProvider(this IList<IModelBinderProvider> modelBinderProviders)
        {
            // Argument Check
            if (modelBinderProviders == null)
                throw new ArgumentNullException(nameof(modelBinderProviders));

            var providerToInsert = new CommaSeparatedArrayModelBinderProvider();

            // Find the location of SimpleTypeModelBinder, the CommaSeparatedArrayModelBinder must be inserted before it.
            var index = modelBinderProviders.FindModelBinderProviderInsertLocation();

            if (index != -1)
                modelBinderProviders.Insert(index, providerToInsert);
            else
                modelBinderProviders.Add(providerToInsert);
        }

        public static MvcOptions AddCommaSeparatedArrayModelBinderProvider(this MvcOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            options.ModelBinderProviders.InsertCommaSeparatedArrayModelBinderProvider();
            return options;
        }

        public static IMvcBuilder AddCommaSeparatedArrayModelBinderProvider(this IMvcBuilder builder)
        {
            builder.AddMvcOptions(options => AddCommaSeparatedArrayModelBinderProvider(options));
            return builder;
        }
    }
}
