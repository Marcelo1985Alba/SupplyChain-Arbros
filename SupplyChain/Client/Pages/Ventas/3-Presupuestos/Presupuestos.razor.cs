using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using SupplyChain.Client.HelperService;
using SupplyChain.Client.Pages.Ventas._4_Solicitudes;
using SupplyChain.Client.Shared;
using SupplyChain.Shared;
using SupplyChain.Shared.Enum;
using Syncfusion.Blazor.DropDowns;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Navigations;
using Syncfusion.Blazor.Notifications;
using Syncfusion.Blazor.Spinner;
using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;


namespace SupplyChain.Client.Pages.Ventas._3_Presupuestos
{
    public class PresupuestosBase : ComponentBase
    {
        [Inject] public IJSRuntime Js { get; set; }
        [Inject] public PresupuestoService PresupuestoService { get; set; }
        [Inject] public SemaforoService SemaforoService { get; set; }
        [CascadingParameter] public MainLayout MainLayout { get; set; }
        [CascadingParameter] public Task<AuthenticationState> authenticationState { get; set; }
        [Parameter] public Semaforo Semaforo { get; set; } = new();
        [Parameter] public Presupuesto Presupuesto { get; set; } = new();
        [Parameter] public EventCallback<Semaforo> OnSelectedChanged { get; set; }

        public AuthenticationState authState;
        protected SfGrid<vPresupuestos> refGrid;
        protected SfGrid<FormPresupuesto> refGridForm;
        protected SfGrid<Presupuestos> refGridPre;
        protected SfSpinner refSpinner;
        protected SfToast ToastObj;
        protected FormPresupuesto refFormPresupuesto;
        protected Presupuesto PresupuestoSeleccionado = new();
        protected vPresupuestos presup = new();
        protected List<vPresupuestos> Presupuestos = new();
        protected List<Semaforo> datasemaforo = new List<Semaforo>();
        protected List<Presupuesto> datapresupuesto = new List<Presupuesto>();
        protected bool SpinnerVisible = true;
        protected bool SpinnerVisiblePresupuesto = false;

        protected bool popupFormVisible = false;
        protected List<Object> Toolbaritems = new()
        {

            new ItemModel { Text = "Agregar", Type = ItemType.Button, Id ="Agregar"},
            new ItemModel { Text = "Editar", Type = ItemType.Button, Id ="Editar"},
            new ItemModel { Text = "Eliminar", Type = ItemType.Button, Id ="Eliminar"},
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
            await GetSemaforo();
            await GetPresupuestos(TipoFiltro.Pendientes);
            SpinnerVisible = false;

        }

        public async Task Actualizar(Syncfusion.Blazor.DropDowns.ChangeEventArgs<string, Semaforo> args)
        {
            var presupuesto = await PresupuestoService.ActualizarColor(presup.Id, args.Value);
            
        }

        public async Task GuardarComentario(int id, string comentario)
        {
            try
            {
                var presup = await PresupuestoService.EnviarComentario(id, comentario);

            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public void RowSelectHandler(RowSelectEventArgs<vPresupuestos> args)
        {
            presup = args.Data;
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

        protected async Task GetSemaforo()
        {
            var response = await SemaforoService.Get();
            if (response.Error)
            {
                await ToastMensajeError("Erro al obtener un color.");
            }
            else
            {
                datasemaforo = response.Response;
            }
        }

        //protected async Task OnActionBeginHandler(ActionEventArgs<vPresupuestos> args)
        //{
        //    if (args.RequestType == Syncfusion.Blazor.Grids.Action.Add ||
        //        //args.RequestType == Syncfusion.Blazor.Grids.Action.BeginEdit ||
        //        args.RequestType == Syncfusion.Blazor.Grids.Action.Delete )
        //    {
        //        args.Cancel = true;
        //        args.PreventRender = false;
        //    }

        //    if (args.RequestType == Syncfusion.Blazor.Grids.Action.Add)
        //    {
        //        SpinnerVisible = true;
        //        PresupuestoSeleccionado = new();
        //        PresupuestoSeleccionado.DIRENT = "";
        //        await refFormPresupuesto.ShowAsync(0);
        //        popupFormVisible = true;
        //        SpinnerVisible = false;
        //    }

        //if (args.RequestType == Syncfusion.Blazor.Grids.Action.BeginEdit)
        //{
        //    SpinnerVisible = true;
        //    var response = await PresupuestoService.GetById(args.Data.Id);
        //    if (response.Error)
        //    {
        //        await ToastMensajeError();
        //    }
        //    else
        //    {
        //        PresupuestoSeleccionado = response.Response;
        //        await refFormPresupuesto.ShowAsync(args.Data.Id);
        //        popupFormVisible = true;
        //    }
        //    SpinnerVisible = true;
        //}

        //    if (args.RequestType == Syncfusion.Blazor.Grids.Action.Delete)
        //    {
        //        if (args.RowData.TIENEPEDIDO)
        //        {
        //            await ToastMensajeError("El presupuesto tiene pedido.\r\nNose puede eliminar.");
        //        }
        //        else
        //        {
        //            PresupuestoSeleccionado.Id = args.RowData.Id;
        //            await ConfirmacionEliminarDialog.ShowAsync();
        //        }

        //    }
        //}

        protected async Task CellSelectHandler(CellSelectEventArgs<vPresupuestos> args)
        {
            var CellValue = await refGrid.GetSelectedRowCellIndexesAsync();

            var CurrentEditRow = CellValue[0].Item1;
            var CurrentEditCell = (int)CellValue[0].Item2;

            var fields = await refGrid.GetColumnFieldNames();
            await refGrid.EditCellAsync(CurrentEditRow, fields[CurrentEditCell]);

        }
        protected async Task OnActionCompleteHandler(ActionEventArgs<vPresupuestos> args)
        {
            if (args.RequestType == Syncfusion.Blazor.Grids.Action.Add ||
                //args.RequestType == Syncfusion.Blazor.Grids.Action.BeginEdit ||
                args.RequestType == Syncfusion.Blazor.Grids.Action.Delete )
            {
                args.Cancel = true;
                args.PreventRender = false;
            }

        }

        protected async Task OnToolbarHandler(ClickEventArgs  args)
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
            else if (args.Item.Id == "Editar")
            {
                SpinnerVisible = true;
                var seleccionado = await refGrid.GetSelectedRecordsAsync();
               if(seleccionado.Count > 0)
                {
                    var response = await PresupuestoService.GetById(seleccionado[0].Id);

                    if (response.Error)
                    {
                        await ToastMensajeError();
                    }
                    else
                    {
                        PresupuestoSeleccionado = response.Response;
                        await refFormPresupuesto.ShowAsync(seleccionado[0].Id);
                        popupFormVisible = true;
                    }
                    SpinnerVisible = true;

                }
            }
            else if (args.Item.Id == "Agregar")
            {
                SpinnerVisible = true;
                PresupuestoSeleccionado = new();
                PresupuestoSeleccionado.DIRENT = "";
                await refFormPresupuesto.ShowAsync(0);
                popupFormVisible = true;
                SpinnerVisible = false;

            }
            else if (args.Item.Id == "Eliminar")
            {
              var seleccionado = await PresupuestoService.TienePedido(PresupuestoSeleccionado.Id);
                   
                if (PresupuestoSeleccionado.TienePedido)
                {
                   await ToastMensajeError("El presupuesto tiene pedido.\r\nNose puede eliminar.");
                }
                else
                {
                  PresupuestoSeleccionado.Id = presup.Id;
                  await ConfirmacionEliminarDialog.ShowAsync();
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
                        COMENTARIO = presupuesto.COMENTARIO,
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
                    presupActualizado.COMENTARIO = presupuesto.COMENTARIO;
                }


                await refGrid.RefreshHeaderAsync();
                await refGrid.Refresh();
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
