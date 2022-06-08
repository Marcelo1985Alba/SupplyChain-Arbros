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

        protected async Task OnActionBeginHandler(ActionEventArgs<vPresupuestos> args)
        {
            if (args.RequestType == Syncfusion.Blazor.Grids.Action.Add ||
                args.RequestType == Syncfusion.Blazor.Grids.Action.BeginEdit)
            {
                args.Cancel = true;
                args.PreventRender = false;
            }

            if (args.RequestType == Syncfusion.Blazor.Grids.Action.Add)
            {
                SpinnerVisible = true;
                PresupuestoSeleccionado = new();
                await refFormPresupuesto.ShowAsync(0);
                popupFormVisible = true;
                SpinnerVisible = false;
            }

            if (args.RequestType == Syncfusion.Blazor.Grids.Action.BeginEdit)
            {
                SpinnerVisible = true;
                var response = await PresupuestoService.GetById(args.Data.Id);
                if (response.Error)
                {
                    await ToastMensajeError();
                }
                else
                {
                    PresupuestoSeleccionado = response.Response;
                    await refFormPresupuesto.ShowAsync(args.Data.Id);
                    popupFormVisible = true;
                }
                SpinnerVisible = false;
            }
        }

        protected async Task OnActionCompleteHandler(ActionEventArgs<vPresupuestos> args)
        {
            if (args.RequestType == Syncfusion.Blazor.Grids.Action.Add ||
                args.RequestType == Syncfusion.Blazor.Grids.Action.BeginEdit)
            {
                args.Cancel = true;
                args.PreventRender = false;
            }


        }

        protected async Task Guardar(Presupuesto presupuesto)
        {
            if (presupuesto.GUARDADO)
            {
                await ToastMensajeExito($"Guardado Correctamente.\nPresupuesto Nro {presupuesto.Id}");
                popupFormVisible = false;
                if (presupuesto.ESNUEVO)
                {
                    var nuevoPresup = new vPresupuestos
                    {
                        CG_CLI = presupuesto.CG_CLI,
                        DES_CLI = presupuesto.DES_CLI,
                        Id = presupuesto.Id,
                        Fecha = presupuesto.FECHA,
                        MONEDA = presupuesto.MONEDA,
                        TOTAL = presupuesto.TOTAL
                    };
                    Presupuestos.Add(nuevoPresup);

                }
                else
                {
                    //actualizar datos sin ir a la base de datos
                    var presupActualizado = Presupuestos.Where(s => s.Id == presupuesto.Id).FirstOrDefault();
                    presupActualizado.CG_CLI = presupuesto.CG_CLI;
                    presupActualizado.DES_CLI = presupuesto.DES_CLI;
                    presupActualizado.TOTAL = presupuesto.TOTAL;

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
