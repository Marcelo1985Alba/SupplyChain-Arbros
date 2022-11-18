using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using SupplyChain.Client.HelperService;
using SupplyChain.Client.Pages.Ventas._4_Solicitudes;
using SupplyChain.Client.Shared;
using SupplyChain.Shared;
using SupplyChain.Shared.Enum;
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
        [CascadingParameter] public Task<AuthenticationState> authenticationState { get; set; }
        public AuthenticationState authState;
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
            new ItemModel { Text = "Imprimir", TooltipText = "Imprimir presupuesto, codiciones comerciales y datasheet", PrefixIcon = "e-print", Id = "Imprimir", Type = ItemType.Button },
            //new ItemModel { Text = "Copy", TooltipText = "Copy", PrefixIcon = "e-copy", Id = "copy" },
            "ExcelExport",
            new ItemModel { Text = "", TooltipText = "Actualizar Grilla", PrefixIcon = "e-refresh", Id = "refresh", Type = ItemType.Button},
            new ItemModel { Text = "Ver Todos", Type = ItemType.Button, Id = "VerTodos", PrefixIcon = "e-icons e-eye" },
            new ItemModel { Text = "Ver Pendientes", Type = ItemType.Button, Id = "VerPendientes" },
        };

        protected List<string> Monedas = new() { "PESOS", "DOLARES" };

        protected PresupuestoAnterior presupuesto = new();
        protected ConfirmacionDialog ConfirmacionEliminarDialog;
        protected async override Task OnInitializedAsync()
        {
            MainLayout.Titulo = "Presupuestos";
            await GetPresupuestos(TipoFiltro.Pendientes);
            SpinnerVisible = false;
        }

        protected async Task GetPresupuestos(TipoFiltro tipoFiltro = TipoFiltro.Todos )
        {
            var response = await PresupuestoService.GetVistaParaGrilla(tipoFiltro);
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
                args.RequestType == Syncfusion.Blazor.Grids.Action.BeginEdit ||
                args.RequestType == Syncfusion.Blazor.Grids.Action.Delete )
            {
                args.Cancel = true;
                args.PreventRender = false;
            }

            if (args.RequestType == Syncfusion.Blazor.Grids.Action.Add)
            {
                SpinnerVisible = true;
                PresupuestoSeleccionado = new();
                PresupuestoSeleccionado.DIRENT = "";
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

            if (args.RequestType == Syncfusion.Blazor.Grids.Action.Delete)
            {
                if (args.RowData.TIENEPEDIDO)
                {
                    await ToastMensajeError("El presupuesto tiene pedido.\r\nNose puede eliminar.");
                }
                else
                {
                    PresupuestoSeleccionado.Id = args.RowData.Id;
                    await ConfirmacionEliminarDialog.ShowAsync();
                }
                
            }
        }

        protected async Task OnActionCompleteHandler(ActionEventArgs<vPresupuestos> args)
        {
            if (args.RequestType == Syncfusion.Blazor.Grids.Action.Add ||
                args.RequestType == Syncfusion.Blazor.Grids.Action.BeginEdit ||
                args.RequestType == Syncfusion.Blazor.Grids.Action.Delete )
            {
                args.Cancel = true;
                args.PreventRender = false;
            }


        }

        protected async Task OnToolbarHandler(ClickEventArgs args)
        {
            if (args.Item.Id == "refresh")
            {
                SpinnerVisible = true;
                await GetPresupuestos();
                SpinnerVisible = false;
            }
            else if (args.Item.Id == "VerPendientes")
            {
                SpinnerVisible = true;
                await GetPresupuestos(TipoFiltro.Pendientes);
                SpinnerVisible = false;
            }
            else if (args.Item.Id == "VerTodos")
            {
                SpinnerVisible = true;
                await GetPresupuestos(TipoFiltro.Todos);
                SpinnerVisible = false;
            }
            else if (args.Item.Id == "Imprimir")
            {
                var seleccionado = await refGrid.GetSelectedRecordsAsync();
                if (seleccionado.Count > 0)
                {
                    SpinnerVisible = true;
                    await PresupuestoService.Imprimir(seleccionado[0].Id);
                    SpinnerVisible = false;
                }
                
            }
        }

        protected async Task Eliminar()
        {
            SpinnerVisible = true;
            await ConfirmacionEliminarDialog.HideAsync();

            var responseTp = await PresupuestoService.TienePedido(PresupuestoSeleccionado.Id);
            if (responseTp.Error)
            {
                await ToastMensajeError("Error al verificar si tiene pedido asociado");
            }
            else if (!responseTp.Response)
            {
                var response = await PresupuestoService.Eliminar(PresupuestoSeleccionado.Id);
                if (response.IsSuccessStatusCode)
                {
                    await ToastMensajeExito("Eliminado Correctamente!");
                    Presupuestos = Presupuestos.Where(s => s.Id != PresupuestoSeleccionado.Id)
                        .OrderByDescending(s => s.Id)
                        .ToList();
                }
                else
                {
                    await ToastMensajeError();
                }

            }
            else
            {
                await ToastMensajeError("El Presupuesto tiene pedido asociado.");
            }

            SpinnerVisible = false;
        }

        protected async Task Guardar(Presupuesto presupuesto)
        {
            if (presupuesto.GUARDADO)
            {
                var auth = await authenticationState;
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
                        TOTAL = presupuesto.TOTAL,
                        USUARIO = auth.User.Identity.Name
                    };
                    Presupuestos.Add(nuevoPresup);
                    Presupuestos = Presupuestos.OrderByDescending(p => p.Id).ToList();
                }
                else
                {
                    //actualizar datos sin ir a la base de datos
                    var presupActualizado = Presupuestos.Where(s => s.Id == presupuesto.Id).FirstOrDefault();
                    presupActualizado.MONEDA = presupuesto.MONEDA;
                    presupActualizado.CG_CLI = presupuesto.CG_CLI;
                    presupActualizado.DES_CLI = presupuesto.DES_CLI;
                    presupActualizado.TOTAL = presupuesto.TOTAL;
                    presupActualizado.USUARIO = presupuesto.USUARIO;
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
