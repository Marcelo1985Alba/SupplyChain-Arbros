using Microsoft.AspNetCore.Components;
using SupplyChain.Client.Shared;
using SupplyChain.Client.Shared.BuscadorCliente;
using SupplyChain.Shared;
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

namespace SupplyChain.Client.Pages.Ventas._4_Solicitudes
{
    public class SolicitudesBase : ComponentBase
    {
        [Inject] public HttpClient Http { get; set; }
        [CascadingParameter] public MainLayout MainLayout { get; set; }
        protected SfGrid<vSolicitudes> refGrid;
        protected SfSpinner refSpinner;
        protected SfToast ToastObj;
        protected vSolicitudes SolicitudSeleccionada = new();
        protected List<vSolicitudes> Solicitudes = new();
        protected List<ClienteExterno> Clientes = new();
        protected bool SpinnerVisible = true;
        protected bool SpinnerVisiblePresupuesto = false;
        protected ClientesDialog refClienteDialog;
        protected bool PopupBuscadorVisible = false;
        protected Dictionary<string, object> HtmlAttribute = new Dictionary<string, object>()
        {
           {"type", "button" }
        };
        protected List<Object> Toolbaritems = new(){
            "Search",
            "Add",
            "Edit",
            "Delete",
            "Print",
            //new ItemModel { Text = "Copy", TooltipText = "Copy", PrefixIcon = "e-copy", Id = "copy" },
            "ExcelExport"
        };

        protected Presupuesto presupuesto = new();
        protected async override Task OnInitializedAsync()
        {
            MainLayout.Titulo = "Solicitudes";
            await GetSolicitudes();
            SpinnerVisible = false;
        }

        protected async Task GetSolicitudes()
        {
            Solicitudes = await Http.GetFromJsonAsync<List<vSolicitudes>>("api/Solicitudes");
        }


        protected async Task BuscarCliente()
        {
            await refClienteDialog.Show();
            PopupBuscadorVisible = true;
        }

        protected async Task GeneraPresupuesto()
        {
            SpinnerVisiblePresupuesto = true;
            presupuesto = new Presupuesto
            {
                CG_ART = SolicitudSeleccionada.Producto,
                CANTENT = SolicitudSeleccionada.Cantidad,
                CG_CLI = SolicitudSeleccionada.CG_CLI,
                DES_CLI = SolicitudSeleccionada.DES_CLI
            };
            var response = await Http.PostAsJsonAsync("api/Presupuestos/PostFromSolicitud", presupuesto);
            if (response.IsSuccessStatusCode)
            {
                SpinnerVisiblePresupuesto = false;
                SolicitudSeleccionada.TienePresupuesto = true;
                await refGrid.EndEditAsync();
                await ToastMensajeExito();
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine(error);
                SpinnerVisiblePresupuesto = false;
                await ToastMensajeError();
            }
            
        }

        private async Task ToastMensajeExito()
        {
            await this.ToastObj.Show(new ToastModel
            {
                Title = "EXITO!",
                Content = "Guardado Correctamente.",
                CssClass = "e-toast-success",
                Icon = "e-success toast-icons",
                ShowCloseButton = true,
                ShowProgressBar = true
            });
        }
        private async Task ToastMensajeError()
        {
            await this.ToastObj.Show(new ToastModel
            {
                Title = "Error!",
                Content = "Ocurrio un Error. Verifique si el producto existe en la base de datos",
                CssClass = "e-toast-warning",
                Icon = "e-warning toast-icons",
                ShowCloseButton = true,
                ShowProgressBar = true
            });
        }

        protected async Task ClienteExternoSelected(ClienteExterno clienteSelected)
        {
            await refSpinner.ShowAsync();
            PopupBuscadorVisible = false;
            SolicitudSeleccionada.CG_CLI = Convert.ToInt32(clienteSelected.CG_CLI);
            SolicitudSeleccionada.DES_CLI = clienteSelected.DESCRIPCION.Trim();
            SolicitudSeleccionada.Cuit = clienteSelected.CUIT;
            await refSpinner.HideAsync();
        }

        protected async Task OnActionBeginHandler(ActionEventArgs<vSolicitudes> args)
        {
            if (args.RequestType == Syncfusion.Blazor.Grids.Action.BeginEdit)
            {
                SolicitudSeleccionada = args.Data;
                args.PreventRender = false;
            }
        }

        protected async Task OnActionCompleteHandler(ActionEventArgs<vSolicitudes> args)
        {
            if (args.RequestType == Syncfusion.Blazor.Grids.Action.BeginEdit)
            {
                SolicitudSeleccionada = args.Data;
                args.PreventRender = false;
            }
        }
    }
}
