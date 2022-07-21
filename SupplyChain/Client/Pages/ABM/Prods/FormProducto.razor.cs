using Microsoft.AspNetCore.Components;
using SupplyChain.Client.HelperService;
using SupplyChain.Shared.Models;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Notifications;
using Syncfusion.Blazor.Spinner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SupplyChain.Client.Pages.ABM.Prods
{
    public class FormProductoBase : ComponentBase
    {
        [Inject] protected HttpClient Http { get; set; }
        [Inject] public ProductoService ProductoService { get; set; }
        [Inject] protected CeldasService CeldasService { get; set; }
        [Parameter] public Producto Producto { get; set; } = new();
        [Parameter] public bool Show { get; set; } = false;
        [Parameter] public EventCallback<Producto> OnGuardar { get; set; }
        [Parameter] public EventCallback OnCerrar { get; set; }

        protected SfGrid<Producto> refGridItems;
        protected SfSpinner refSpinnerCli;
        protected bool SpinnerVisible = false;
        protected SfToast ToastObj;
        protected List<SupplyChain.Unidades> unidades = new();
        protected List<Moneda> monedas = new();
        protected List<Celdas> celda = new();
        protected List<SupplyChain.Areas> area = new();
        protected List<SupplyChain.Lineas> linea = new();
        protected List<TipoArea> tipoarea = new();
        protected List<Cat> cat = new();
        protected List<SupplyChain.TipoMat> tipomat = new();
        protected Dictionary<string, object> HtmlAttributeSubmit = new()
        {
            { "type", "submit" }
        };
        protected bool camposConf = true;
        protected bool IsAdd { get; set; }

        protected class TipoOptions
        {
            public string Text { get; set; }
        }
        protected List<TipoOptions> TipoData = new List<TipoOptions> {
            new TipoOptions() {Text= "CONVENCIONAL" },
            new TipoOptions() {Text= "BALANCEADA" },
            new TipoOptions() {Text= "DOBLE ANILLO" },
            new TipoOptions() {Text= "PILOTADA" },
        };

        protected async override Task OnInitializedAsync()
        {
            unidades = await Http.GetFromJsonAsync<List<SupplyChain.Unidades>>("api/unidades");
            monedas = await Http.GetFromJsonAsync<List<Moneda>>("api/Monedas");
            var response = await CeldasService.Get();
            if (!response.Error)
            {
                celda = response.Response;
            }
            //celda = await Http.GetFromJsonAsync<List<SupplyChain.Celdas>>("api/Celdas");
            area = await Http.GetFromJsonAsync<List<SupplyChain.Areas>>("api/Areas");
            linea = await Http.GetFromJsonAsync<List<SupplyChain.Lineas>>("api/Lineas");
            tipoarea = await Http.GetFromJsonAsync<List<TipoArea>>("api/TipoArea");
            cat = await Http.GetFromJsonAsync<List<Cat>>("api/Cat");
            //tipomat = await Http.GetFromJsonAsync<List<SupplyChain.TipoMat>>("api/TipoMat");
        }

        private async Task<bool> PermiteGuardarNuevoInsumo()
        {
            var existe = await Http.GetFromJsonAsync<bool>($"api/Prod/Existe/{Producto.Id}");
            if (!existe && Producto.CG_ORDEN != 1 && Producto.CG_ORDEN != 3)
            {
                switch (Producto.CG_ORDEN)
                {
                    case 1:
                        Producto.EXIGESERIE = true;
                        Producto.EXIGEOA = true;
                        break;
                    case 3:
                        Producto.EXIGELOTE = true;
                        break;
                    case 4:
                        Producto.EXIGEDESPACHO = true;
                        break;
                }

                return true;
            }

            return false;
        }
        protected async Task<bool> Agregar(Producto producto)
        {
            if (await PermiteGuardarNuevoInsumo())
            {
                var response = await ProductoService.Agregar(producto);
                if (response.Error)
                {
                    Console.WriteLine(await response.HttpResponseMessage.Content.ReadAsStringAsync());
                    await ToastMensajeError("Error al intentar Guardar el producto.");
                    return false;
                }
                Producto = response.Response;
                return true;
            }

            await ToastMensajeError($"El insumo con codigo {producto.Id} ya existe.\n\rO El tipo de insumo no es permitidio.");


            return false;
        }

        protected async Task<bool> Actualizar(Producto producto)
        {
            var response = await ProductoService.Actualizar(producto.Id, producto);
            if (response.Error)
            {
                await ToastMensajeError("Error al intentar Guardar el producto.");
                return false;
            }

            Producto = producto;
            return true;
        }

        protected async Task GuardarProd()
        {
            bool guardado = false;
            if (Producto.ESNUEVO)
            {
                guardado = await Agregar(Producto);
            }
            else
            {
                guardado = await Actualizar(Producto);
            }

            if (guardado)
            {
                Show = false;
                Producto.GUARDADO = guardado;
                await OnGuardar.InvokeAsync(Producto);
            }
            
        }

        public async Task Hide()
        {
            Show = false;
        }

        protected async Task OnCerrarDialog()
        {
            await OnCerrar.InvokeAsync();
        }

        private async Task ToastMensajeExito(string content = "Guardado Correctamente.")
        {
            await this.ToastObj.Show(new ToastModel
            {
                Title = "EXITO!",
                Content = content,
                CssClass = "e-toast-success",
                Icon = "e-success toast-icons",
                ShowCloseButton = true,
                ShowProgressBar = true
            });
        }
        private async Task ToastMensajeError(string content = "Ocurrio un Error.")
        {
            await ToastObj.Show(new ToastModel
            {
                Title = "Error!",
                Content = content,
                CssClass = "e-toast-warning",
                Icon = "e-warning toast-icons",
                ShowCloseButton = true,
                ShowProgressBar = true
            });
        }
    }
}
