using Microsoft.AspNetCore.Components;
using SupplyChain.Client.HelperService;
using SupplyChain.Client.Shared;
using SupplyChain.Shared;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Navigations;
using Syncfusion.Blazor.Notifications;
using Syncfusion.Blazor.Spinner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SupplyChain.Client.Pages.Ventas._3_Presupuestos
{
    public class PresupuestosBase : ComponentBase
    {
        [Inject] public PresupuestoService PresupuestoService { get; set; }
        [CascadingParameter] public MainLayout MainLayout { get; set; }
        protected SfGrid<vPresupuestos> refGrid;
        protected SfSpinner refSpinner;
        protected SfToast ToastObj;
        protected FormPresupuesto refFormPresupuesto;
        protected Presupuesto PresupuestoSeleccionado = new();
        protected List<vPresupuestos> Presupuestos = new();

        protected bool SpinnerVisible = true;
        protected bool SpinnerVisiblePresupuesto = false;

        protected bool popupFormVisible = false;
        protected List<Object> Toolbaritems = new()
        {
            "Search",
            //new ItemModel { Text = "Add", TooltipText = "Agregar un nuevo Presupuesto", PrefixIcon = "e-add", Id = "Add" },
            "Add",
            "Edit",
            "Delete",
            "Print",
            //new ItemModel { Text = "Copy", TooltipText = "Copy", PrefixIcon = "e-copy", Id = "copy" },
            "ExcelExport"
        };

        protected List<string> Monedas = new() { "PESOS", "DOLARES" };

        protected PresupuestoAnterior presupuesto = new();
        protected async override Task OnInitializedAsync()
        {
            MainLayout.Titulo = "Presupuestos";
            await GetPresupuestos();
            SpinnerVisible = false;
        }

        protected async Task GetPresupuestos()
        {
            var response = await PresupuestoService.GetVistaParaGrilla();
            if (response.Error)
            {
                Console.WriteLine(await response.HttpResponseMessage.Content.ReadAsStringAsync());
            }
            else
            {
                Presupuestos = response.Response.OrderByDescending(s => s.Id).ToList();
            }
        }

        protected async Task GeneraPresupuesto()
        {
            //SpinnerVisiblePresupuesto = true;
            ////Datos de la solicitud
            //presupuesto.CG_ART = PresupuestoSeleccionado.Producto;
            //presupuesto.CANTENT = PresupuestoSeleccionado.Cantidad;
            //presupuesto.CG_CLI = PresupuestoSeleccionado.CG_CLI;
            //presupuesto.DES_CLI = PresupuestoSeleccionado.Des_Cli;

            //var response = await Http.PostAsJsonAsync("api/Presupuestos", presupuesto);
            //if (response.IsSuccessStatusCode)
            //{
            //    SpinnerVisiblePresupuesto = false;
            //    PresupuestoSeleccionado.TienePresupuesto = true;
            //    await refGrid.EndEditAsync();
            //    //await ToastMensajeExito();
            //}
            //else
            //{
            //    var error = await response.Content.ReadAsStringAsync();
            //    Console.WriteLine(error);
            //    SpinnerVisiblePresupuesto = false;
            //    //await ToastMensajeError();
            //}

        }

        protected async Task OnActionBeginHandler(ActionEventArgs<vPresupuestos> args)
        {
            if (args.RequestType == Syncfusion.Blazor.Grids.Action.Add ||
                args.RequestType == Syncfusion.Blazor.Grids.Action.BeginEdit)
            {
                args.Cancel = true;
                args.PreventRender = false;
                popupFormVisible = true;
                PresupuestoSeleccionado = new();
            }

            if (args.RequestType == Syncfusion.Blazor.Grids.Action.BeginEdit)
            {
                var response = await PresupuestoService.GetById(args.Data.Id);
                if (response.Error)
                {
                    await ToastMensajeError();
                }
                else
                {
                    PresupuestoSeleccionado = response.Response;
                }
            }
        }

        protected async Task OnActionCompleteHandler(ActionEventArgs<vPresupuestos> args)
        {
            if (args.RequestType == Syncfusion.Blazor.Grids.Action.Add ||
                args.RequestType == Syncfusion.Blazor.Grids.Action.BeginEdit)
            {
                args.Cancel = true;
                args.PreventRender = false;
                popupFormVisible = true;
            }


        }

        protected async Task Guardar(Presupuesto solicitud)
        {
            if (solicitud.GUARDADO)
            {
                await ToastMensajeExito();
                popupFormVisible = false;
                if (solicitud.ESNUEVO)
                {
                    //var nuevaSol = new vSolicitudes()
                    //{
                    //    Id = solicitud.Id,
                    //    TagId = solicitud.TagId,
                    //    Fecha = solicitud.Fecha,
                    //    Cantidad = solicitud.Cantidad,
                    //    CG_CLI = solicitud.CG_CLI,
                    //    Cuit = solicitud.Cuit,
                    //    DES_CLI = solicitud.Des_Cli,
                    //    DES_PROD = solicitud.Des_Prod,
                    //    Producto = solicitud.Producto,
                    //    TienePresupuesto = solicitud.TienePresupuesto
                    //};

                    //Presupuestos.Add(nuevaSol);

                }
                else
                {
                    //actualizar datos sin ir a la base de datos
                    var sol = Presupuestos.Where(s => s.Id == solicitud.Id).FirstOrDefault();
                    //sol.Producto = solicitud.Producto;
                    //sol.DES_PROD = solicitud.Des_Prod;
                    //sol.Cantidad = solicitud.Cantidad;
                    //sol.CG_CLI = solicitud.CG_CLI;
                    //sol.DES_CLI = solicitud.Des_Cli;
                    //sol.Cuit = solicitud.Cuit;
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
            //refFormPresupuesto?.Hide();
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
