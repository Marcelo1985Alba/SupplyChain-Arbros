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
        protected FormSolicitud refFormSolicitud;
        protected Solicitud SolicitudSeleccionada = new();
        protected List<vSolicitudes> Solicitudes = new();
        protected string heightPopup = "420px";
        protected bool SpinnerVisible = true;
        protected bool SpinnerVisiblePresupuesto = false;

        protected bool popupFormVisible = false;
        protected List<Object> Toolbaritems = new(){
            "Search",
            "Add",
            "Edit",
            "Delete",
            new ItemModel { Text = "Copia", TooltipText = "Copiar un item para generar una nueva solicitud", PrefixIcon = "e-copy", Id = "Copy" },
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

        private async Task GetSolicitudFromDB(int id, vSolicitudes vSolicitud)
        {
            var response = await SolicitudService.GetById(id);
            if (response.Error)
            {
                await ToastMensajeError();
            }
            else
            {
                SolicitudSeleccionada = response.Response;
                SolicitudSeleccionada.Cuit = vSolicitud.Cuit;
                SolicitudSeleccionada.Des_Cli = vSolicitud.DES_CLI;
                SolicitudSeleccionada.Des_Prod = vSolicitud.Descripcion;
                //if (!SolicitudSeleccionada.Producto.StartsWith("00"))
                //{
                //    heightPopup = "600px";
                //}
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
                //heightPopup = "600px";
            }

            if (args.RequestType == Syncfusion.Blazor.Grids.Action.BeginEdit)
            {
                //TRAER DATOS QUE NO ESTAN ENLA GRILLA
                await GetSolicitudFromDB(args.Data.Id, args.Data);
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

        protected async Task OnToolbarHandler(ClickEventArgs args)
        {
            if (args.Item.Id == "Copy")
            {
                await CopiarSolicitud();
            }
            else
            {
                if (args.Item.Id == "grdSol_excelexport")
                {
                    await refGrid.ExportToExcelAsync();
                }
            }
        }

        private async Task CopiarSolicitud()
        {
            if (refGrid.SelectedRecords.Count > 0)
            {
                var vSoli = refGrid.SelectedRecords[0];
                await GetSolicitudFromDB(vSoli.Id, vSoli);
                SolicitudSeleccionada.Id = 0;
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
                        Descripcion = solicitud.Des_Prod,
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
                    sol.Descripcion = solicitud.Des_Prod;
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

        protected void OnCerraDialog()
        {
            popupFormVisible = false;
            refFormSolicitud?.Hide();
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
            await ToastObj.Show(new ToastModel
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
