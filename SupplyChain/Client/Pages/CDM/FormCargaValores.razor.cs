using Microsoft.AspNetCore.Components;
using SupplyChain.Client.HelperService;
using Syncfusion.Blazor.Grids;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Syncfusion.Blazor.Spinner;
using Syncfusion.Blazor.Notifications;
using SupplyChain.Shared;
using Microsoft.JSInterop;
using System.Security.Authentication.ExtendedProtection;

namespace SupplyChain.Client.Pages.CDM
{
    public class FormCargaValoresBase : ComponentBase
    {
        [Inject] protected HttpClient Http { get; set; }
        //[Inject] public CargaValoresService CargaValoresService { get; set; }
        [Inject] public InventarioService InventarioService { get; set; }
        [Inject] protected IJSRuntime jSRuntime { get; set; }
        //[Parameter] public Pedidos pedidos { get; set; } = new();
        [Parameter] public Procesos procesos { get; set; } = new();
        [Parameter] public ProcalsMP procalsMP { get; set; } = new();
        [Parameter] public bool Show { get; set; } = false;
        [Parameter] public EventCallback<Procesos> OnGuardar { get; set; }
        [Parameter] public EventCallback<Procesos> OnEliminar { get; set; }
        [Parameter] public EventCallback OnCerrar { get; set; }

        protected SfGrid<CargaValoresDetalles> refGridItems;
        protected SfGrid<ProcalsMP> refGrid;
        protected SfSpinner refSpinnerCli;
        protected SfSpinner refSpinner;
        protected bool SpinnerVisible = false;
        protected SfToast ToastObj;
        protected ProcalsMP ProcalSeleccionada = new();
        protected List<ProcalsMP> valor = new();
        protected List<CargaValoresDetalles> valor2 = new();
        protected bool popupFormVisible = false;
        protected string state;

        protected Dictionary<string, object> HtmlAttributeSubmint = new()
        {
            {"type", "submit" }
        };
        private async Task CopiarProcalMPValores()
        {
            if (refGridItems.SelectedRecords.Count == 1)
            {
                ProcalSeleccionada = new();
                ProcalsMP selectedRecord = refGrid.SelectedRecords[0];
                bool isConfirmed = await jSRuntime.InvokeAsync<bool>("confirm", "Seguro que desea copiar la materia?");
                if (isConfirmed)
                {
                    ProcalSeleccionada.ESNUEVO = true;
                    ProcalSeleccionada.DESCAL = selectedRecord.DESCAL;
                    ProcalSeleccionada.CARCAL = selectedRecord.CARCAL;
                    ProcalSeleccionada.UNIDADM = selectedRecord.UNIDADM;
                    ProcalSeleccionada.AVISO = selectedRecord.AVISO;
                }
                popupFormVisible = true;
            }
            else
            {
                await this.ToastObj.Show(new ToastModel
                {
                    Title = "ERROR!",
                    Content = "Solo se puede copiar un item",
                    CssClass = "e-toast-danger",
                    Icon = "e-error toast-icons",

                    ShowCloseButton = true,
                    ShowProgressBar = true
                });
            }
        }

        protected async Task OnActionBeginHandler(ActionEventArgs<ProcalsMP> args)
        {
            if (args.RequestType == Syncfusion.Blazor.Grids.Action.Add ||
                args.RequestType == Syncfusion.Blazor.Grids.Action.BeginEdit)
            {
                args.Cancel = true;
                args.PreventRender = false;
                popupFormVisible = true;
                ProcalSeleccionada = new();
            }
            if (args.RequestType == Syncfusion.Blazor.Grids.Action.BeginEdit)
            {
                ProcalSeleccionada = args.Data;
            }
            if (args.RequestType == Syncfusion.Blazor.Grids.Action.Grouping
                || args.RequestType == Syncfusion.Blazor.Grids.Action.UnGrouping
                || args.RequestType == Syncfusion.Blazor.Grids.Action.ClearFiltering
                || args.RequestType == Syncfusion.Blazor.Grids.Action.CollapseAllComplete
                || args.RequestType == Syncfusion.Blazor.Grids.Action.ColumnState
                || args.RequestType == Syncfusion.Blazor.Grids.Action.Reorder
                || args.RequestType == Syncfusion.Blazor.Grids.Action.Sorting
                )
            {
                refGridItems.PreventRender();
                refGridItems.Refresh();

                state = await refGridItems.GetPersistData();
                await refGridItems.AutoFitColumnsAsync();
                await refGridItems.RefreshColumns();
                await refGridItems.RefreshHeader();
            }
        } 

        protected async Task OnActionCompleteHandler(ActionEventArgs<ProcalsMP> args)
        {
            if (args.RequestType == Syncfusion.Blazor.Grids.Action.BeginEdit)
            {
                args.Cancel = true;
                args.PreventRender = false;
                popupFormVisible = true;
            }
        }

        protected void OnCerrarDialog()
        {
            popupFormVisible = false;
        }
        protected async Task Guardar(ProcalsMP procals)
        {

        }

        protected bool IsAdd { get; set; }

        //protected async Task<bool> Agregar(Procesos procesos)
        //{
        //    var response = await InventarioService.Existe(procesos.Id);
        //    if (!response)
        //    {
        //        var response2 = await InventarioService.Agregar(procesos);
        //        if (response2.Error)
        //        {
        //            Console.WriteLine(await response2.HttpResponseMessage.Content.ReadAsStringAsync());
        //            await ToastMensajeError("Error al intentar Guardar un Proceso.");
        //            return false;
        //        }
        //        procesos = response2.Response;
        //        return true;
        //    }
        //    await ToastMensajeError($"El proceso con codigo{procesos.Id} ya existe.\n\rO el proceso no es permitido.");
        //    return false;
        //}

        //protected async Task<bool> Actualizar(Procesos procesos)
        //{
        //    var response = await InventarioService.Actualizar(procesos.Id, procesos);
        //    if (response.Error)
        //    {
        //        await ToastMensajeError("Error al intenar guardar el Proceso.");
        //        return false;
        //    }
        //    return true;
        //}

        protected async Task GuardarProcalMP()
        {

        }
        protected async Task GuardarProceso()
        {
            //bool guardado = false;
            //if (procesos.ESNUEVO)
            //{
            //    guardado = await Agregar(procesos);
            //}
            //else
            //{
            //    guardado = await Actualizar(procesos);
            //}
            //if (guardado)
            //{
            //    Show = false;
            //    procesos.GUARDADO = guardado;
            //    await OnGuardar.InvokeAsync(procesos);
            //}
        }

        //COMENTADO
        protected async Task GetItem(int id)
        {
        //    var response = await InventarioService.GetById(id);
        //    if (response.Error)
        //    {
        //        await ToastMensajeError("Error ******");
        //    }
        //    else
        //    {
        //        pedidos = response.Response;
        //        foreach (var item in pedidos)
        //        {
        //            item.Estado = SupplyChain.Shared.Enum.EstadoItem.Modificado;
        //        }
        //    }
        }
        //ANTES BeforeBatchDeleteArgs<CargaValoresDetalles>
        protected async Task BatchDeleteHandler(BeforeBatchDeleteArgs<CargaValoresDetalles> args)
        {
            //var item = pedidos.Items.Find(i => i.Id == args.RowData.Id);
            //item.Estado = SupplyChain.Shared.Enum.EstadoItem.Eliminado;
        }
        //ANTES BeforeBatchAddArgs<CargaValoresDetalles>
        public void BatchAddHandler(BeforeBatchAddArgs<CargaValoresDetalles> args)
        {
            IsAdd = true;
        }
        //ANTES BeforeBatchSaveArgs<CargaValoresDetalles>
        public void BatchSaveHandler(BeforeBatchSaveArgs<CargaValoresDetalles> args)
        {
            IsAdd=false;
        }
        //ANTES CellSaveArgs<CargaValoresDetalles>
        public async Task CellSavedHandler(CellSaveArgs<CargaValoresDetalles> args)
        {
            //var index = await refGridItems.GetRowIndexByPrimaryKey(args.RowData.Id);
            //if (args.ColumnName == "SINMON")
            //{

            //}
        }
            public async Task Hide()
            {
                Show = false;
            }
         
            private async Task ToastMensajeExito(string content = "Guardado Correctamente.")
            {
                await this.ToastObj.Show(new ToastModel
                {
                    Title = "EXITO!",
                    Content = content,
                    CssClass = "e-toast-success",
                    Icon = "e-warning toast-icons",
                    ShowCloseButton = true,
                    ShowProgressBar = true
                });
            }
            private async Task ToastMensajeError(string content = "Ocurrio un error.")
            {
                await ToastObj.Show(new ToastModel
                {
                    Title = "EXITO!",
                    Content = content,
                    CssClass = "e-toast-success",
                    Icon = "e-warning toast-icons",
                    ShowCloseButton = true,
                    ShowProgressBar = true
                });

            }
        }
    }

