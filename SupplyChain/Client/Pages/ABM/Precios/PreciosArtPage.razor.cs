using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SupplyChain.Client.HelperService;
using SupplyChain.Client.Shared;
using SupplyChain.Shared;
using SupplyChain.Shared.Models;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Navigations;
using Syncfusion.Blazor.Notifications;
using Syncfusion.Blazor.Spinner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SupplyChain.Client.Pages.ABM.Precios
{
    public class PreciosArtPageBase: ComponentBase
    {
        [Inject] protected PrecioArticuloService PrecioArticuloService { get; set; }
        [Inject] public IJSRuntime JsRuntime { get; set; }
        [Inject] public HttpClient Http { get; set; }
        protected SfSpinner refSpinner;
        protected SfGrid<PreciosArticulos> Grid;
        protected SfToast ToastObj;
        public bool SpinnerVisible = false;
        protected bool popupFormVisible { get; set; } = false;

        public bool Enabled = true;
        public bool Disabled = false;
        public bool isAdding = false;


        protected List<PreciosArticulos> preciosArts = new();
        public PreciosArticulos preciosArticuloSeleccionado = new();

        protected List<Moneda> monedas = new();

        protected const string APPNAME = "grdPreciosArtABM";
        protected string state;

        protected List<Object> Toolbaritems = new List<Object>(){
        "Search",
        "Add",
        "Edit",
        "Delete",
        "Print",
        new ItemModel { Text = "Copia", TooltipText = "Copy", PrefixIcon = "e-copy", Id = "copy" },
        "ExcelExport"
        };
        [CascadingParameter] MainLayout MainLayout { get; set; }

        protected override async Task OnInitializedAsync()
        {
            MainLayout.Titulo = "Precios de Articulos";

            SpinnerVisible = true;
            await GetPrecioArticulos();

            SpinnerVisible = false;
        }

        protected async Task OnActionBeginHandler(ActionEventArgs<PreciosArticulos> args)
        {
            if (args.RequestType == Syncfusion.Blazor.Grids.Action.Add ||
                args.RequestType == Syncfusion.Blazor.Grids.Action.BeginEdit)
            {
                args.Cancel = true;
                args.PreventRender = false;
                popupFormVisible = true;
                preciosArticuloSeleccionado = new();
                preciosArticuloSeleccionado.ESNUEVO = true;
            }



            if (args.RequestType == Syncfusion.Blazor.Grids.Action.BeginEdit)
            {
                preciosArticuloSeleccionado = args.Data;
                preciosArticuloSeleccionado.ESNUEVO = false;
            }

            if (args.RequestType == Syncfusion.Blazor.Grids.Action.Grouping
                || args.RequestType == Syncfusion.Blazor.Grids.Action.UnGrouping
                || args.RequestType == Syncfusion.Blazor.Grids.Action.ClearFiltering
                || args.RequestType == Syncfusion.Blazor.Grids.Action.CollapseAllComplete
                || args.RequestType == Syncfusion.Blazor.Grids.Action.ColumnState
                || args.RequestType == Syncfusion.Blazor.Grids.Action.ClearFiltering
                || args.RequestType == Syncfusion.Blazor.Grids.Action.Reorder
                || args.RequestType == Syncfusion.Blazor.Grids.Action.Sorting
                )
            {
                //VisibleProperty = true;
                Grid.PreventRender();
                Grid.Refresh();

                state = await Grid.GetPersistData();
                await Grid.AutoFitColumnsAsync();
                await Grid.RefreshColumns();
                await Grid.RefreshHeader();
            }
        }

        protected async Task OnActionCompleteHandler(ActionEventArgs<PreciosArticulos> args)
        {
            if (args.RequestType == Syncfusion.Blazor.Grids.Action.BeginEdit)
            {
                args.Cancel = true;
                args.PreventRender = false;
                popupFormVisible = true;
            }
        }
        protected async Task GetPrecioArticulos()
        {
            var respose = await PrecioArticuloService.Get();
            if (respose.Error)
            {
                Console.WriteLine(await respose.HttpResponseMessage.Content.ReadAsStringAsync());
                await ToastMensajeError();
            }
            else
            {
                preciosArts = respose.Response.OrderBy(p=> p.Id).ToList();
                monedas = await Http.GetFromJsonAsync<List<Moneda>>("api/Monedas");
            }
        }

        protected async Task OnVistaSeleccionada(VistasGrillas vistasGrillas)
        {
            await Grid.SetPersistData(vistasGrillas.Layout);
        }
        protected async Task OnReiniciarGrilla()
        {
            await Grid.ResetPersistData();
        }
        protected void OnCerraDialog()
        {
            popupFormVisible = false;
        }
        protected async Task Guardar(PreciosArticulos precio)
        {
            if (precio.GUARDADO)
            {
                await ToastMensajeExito();
                popupFormVisible = false;
                if (precio.ESNUEVO)
                {
                    preciosArts.Add(precio);
                }
                else
                {
                    //actualizar datos sin ir a la base de datos
                    var prec = preciosArts.FirstOrDefault(p=> p.Id == precio.Id);
                    prec.Descripcion = precio.Descripcion;
                    prec.Construccion = precio.Construccion;
                    prec.Marca = precio.Marca;
                    prec.Moneda = precio.Moneda;
                    prec.Precio = precio.Precio;
                }
            }
            else
            {
                await ToastMensajeError();
            }
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
