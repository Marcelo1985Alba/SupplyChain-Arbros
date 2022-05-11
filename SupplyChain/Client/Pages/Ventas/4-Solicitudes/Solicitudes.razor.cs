using Microsoft.AspNetCore.Components;
using SupplyChain.Client.HelperService;
using SupplyChain.Client.RepositoryHttp;
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
        [Inject] public SolicitudService SolicitudService { get; set; }
        [CascadingParameter] public MainLayout MainLayout { get; set; }
        protected SfGrid<vSolicitudes> refGrid;
        protected SfSpinner refSpinner;
        protected SfToast ToastObj;
        protected Solicitud SolicitudSeleccionada = new();
        protected List<vSolicitudes> Solicitudes = new();
        
        protected bool SpinnerVisible = true;
        protected bool SpinnerVisiblePresupuesto = false;

        protected bool popupFormVisible = false;
        protected List<Object> Toolbaritems = new(){
            "Search",
            "Add",
            "Edit",
            "Delete",
            "Print",
            //new ItemModel { Text = "Copy", TooltipText = "Copy", PrefixIcon = "e-copy", Id = "copy" },
            "ExcelExport"
        };

        protected PresupuestoAnterior presupuesto = new();
        protected async override Task OnInitializedAsync()
        {
            MainLayout.Titulo = "Solicitudes";
            await GetSolicitudes();
            SpinnerVisible = false;
        }

        protected async Task GetSolicitudes()
        {
            var response = await SolicitudService.GetVistaParaGrilla();
            if (response.Error)
            {
                Console.WriteLine(await response.HttpResponseMessage.Content.ReadAsStringAsync());
            }
            else
            {
                Solicitudes = response.Response.OrderBy(s=> s.Id).ToList();
            }
        }


        

        protected async Task GeneraPresupuesto()
        {
            SpinnerVisiblePresupuesto = true;
            //Datos de la solicitud
            presupuesto.CG_ART = SolicitudSeleccionada.Producto;
            presupuesto.CANTENT = SolicitudSeleccionada.Cantidad;
            presupuesto.CG_CLI = SolicitudSeleccionada.CG_CLI;
            presupuesto.DES_CLI = SolicitudSeleccionada.Des_Cli;

            var response = await Http.PostAsJsonAsync("api/Presupuestos", presupuesto);
            if (response.IsSuccessStatusCode)
            {
                SpinnerVisiblePresupuesto = false;
                SolicitudSeleccionada.TienePresupuesto = true;
                await refGrid.EndEditAsync();
                //await ToastMensajeExito();
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine(error);
                SpinnerVisiblePresupuesto = false;
                //await ToastMensajeError();
            }
            
        }

        

        

        protected async Task OnActionBeginHandler(ActionEventArgs<vSolicitudes> args)
        {
            if (args.RequestType == Syncfusion.Blazor.Grids.Action.Add ||
                args.RequestType == Syncfusion.Blazor.Grids.Action.BeginEdit)
            {
                args.Cancel = true;
                args.PreventRender = false;
                popupFormVisible = true;
                SolicitudSeleccionada = new();
            }

            if (args.RequestType == Syncfusion.Blazor.Grids.Action.BeginEdit)
            {
                SolicitudSeleccionada.Id = args.Data.Id;
                SolicitudSeleccionada.Fecha = args.Data.Fecha;
                SolicitudSeleccionada.TagId = args.Data.TagId;
                SolicitudSeleccionada.Producto = args.Data.Producto;
                SolicitudSeleccionada.Des_Prod = args.Data.DES_PROD;
                SolicitudSeleccionada.Cantidad = args.Data.Cantidad;
                SolicitudSeleccionada.CG_CLI = args.Data.CG_CLI;
                SolicitudSeleccionada.Cuit = args.Data.Cuit;
                SolicitudSeleccionada.Des_Cli = args.Data.DES_CLI;
            }
        }

        protected async Task OnActionCompleteHandler(ActionEventArgs<vSolicitudes> args)
        {
            if (args.RequestType == Syncfusion.Blazor.Grids.Action.Add ||
                args.RequestType == Syncfusion.Blazor.Grids.Action.BeginEdit)
            {
                args.Cancel = true;
                args.PreventRender = false;
                popupFormVisible = true;
            }

            
        }

        protected async Task Guardar(Solicitud solicitud)
        {
            if (solicitud.Guardado)
            {
                await ToastMensajeExito();
                popupFormVisible = false;
                if (solicitud.EsNuevo)
                {
                    var nuevaSol = new vSolicitudes()
                    {
                        Id = solicitud.Id,
                        TagId = solicitud.TagId,
                        Fecha = solicitud.Fecha,
                        Cantidad = solicitud.Cantidad,
                        CG_CLI = solicitud.CG_CLI,
                        Cuit = solicitud.Cuit,
                        DES_CLI = solicitud.Des_Cli,
                        DES_PROD = solicitud.Des_Prod,
                        Producto = solicitud.Producto,
                        TienePresupuesto = solicitud.TienePresupuesto
                    };

                    Solicitudes.Add(nuevaSol);
                    
                }
                else
                {
                    //actualizar datos sin ir a la base de datos
                    var sol =  Solicitudes.Where(s => s.Id == solicitud.Id).FirstOrDefault();
                    sol.Producto = solicitud.Producto;
                    sol.DES_PROD = solicitud.Des_Prod;
                    sol.Cantidad = solicitud.Cantidad;
                    sol.CG_CLI = solicitud.CG_CLI;
                    sol.DES_CLI = solicitud.Des_Cli;
                    sol.Cuit = solicitud.Cuit;
                }

                await refGrid.RefreshHeaderAsync();
                refGrid.Refresh();
                await refGrid.RefreshColumnsAsync();

            }
            else
            {
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
                Content = "Ocurrio un Error.",
                CssClass = "e-toast-warning",
                Icon = "e-warning toast-icons",
                ShowCloseButton = true,
                ShowProgressBar = true
            });
        }


    }
}
