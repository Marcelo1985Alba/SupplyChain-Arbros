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
using SupplyChain.Client.Shared;

namespace SupplyChain.Client.Pages.CDM
{
    public class FormCargaValoresBase : ComponentBase
        {
        [Inject] protected HttpClient Http { get; set; }
        //[Inject] public CargaValoresService CargaValoresService { get; set; }
        [Inject] protected InventarioService InventarioService { get; set; }
        [Inject] protected ControlCalidadService ControlCalidadService{ get; set; }

        [Inject] protected IJSRuntime jSRuntime { get; set; }
        //[Parameter] public Pedidos pedidos { get; set; } = new();
        [Parameter] public Pedidos pedidos { get; set; }
        [Parameter] public List<vControlCalidadPendientes> controlCalidadPendientes { get; set; } = new(); 
        //[Parameter] public Procesos procesos { get; set; } = new();
        [Parameter] public ProcalsMP procalsMP { get; set; } = new();
        [Parameter] public bool Show { get; set; } = false;
        [Parameter] public EventCallback<vControlCalidadPendientes> OnGuardar { get; set; }
        [Parameter] public EventCallback<vControlCalidadPendientes> OnEliminar { get; set; }
        [Parameter] public EventCallback OnCerrar { get; set; }

        //AGREGADO NUEVO
        protected const string APPNAME = "grdCargaProcesos";
        [CascadingParameter] MainLayout MainLayout { get; set; }

        protected SfGrid<vControlCalidadPendientes> refGridItems;

        //protected SfGrid<Procesos> refGrid;
        protected SfGrid<vControlCalidadPendientes> refGrid;
        protected SfSpinner refSpinnerCli;
        protected SfSpinner refSpinner;
        protected SfToast ToastObj;
        protected vControlCalidadPendientes ControlCalidadSeleccionado= new();
        //protected Procesos ProcesoSeleccionada = new();
        protected List<vControlCalidadPendientes> control = new();
        //protected List<ControlCalidadService> control2 = new();
        //protected List<Procesos> pro = new();
        protected List<CargaValoresDetalles> valor2 = new();
        protected bool popupFormVisible = false;
        protected bool SpinnerVisible = false;
        protected string state;


        protected Dictionary<string, object> HtmlAttributeSubmint = new()
        {
            {"type", "submit" }
        };
        private async Task CopiarProcesoValores()
        {
            if (refGridItems.SelectedRecords.Count == 1)
            {
                ControlCalidadSeleccionado = new();
                vControlCalidadPendientes selectedRecord = refGrid.SelectedRecords[0];
                bool isConfirmed = await jSRuntime.InvokeAsync<bool>("confirm", "Seguro que desea copiar la materia?");
                if (isConfirmed)
                {
                    //ControlCalidadSeleccionado.ESNUEVO = true;
                    ControlCalidadSeleccionado.DESCAL = selectedRecord.DESCAL;
                    ControlCalidadSeleccionado.CARCAL = selectedRecord.CARCAL;
                    ControlCalidadSeleccionado.UNIDADM = selectedRecord.UNIDADM;
                    ControlCalidadSeleccionado.AVISO = selectedRecord.AVISO;
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
        //AGREGADO 5/1
        public async Task ShowAsync(int id)
        {

        }
        protected async Task OnActionBeginHandler(ActionEventArgs<vControlCalidadPendientes> args)
        {
            if (args.RequestType == Syncfusion.Blazor.Grids.Action.Add ||
                args.RequestType == Syncfusion.Blazor.Grids.Action.BeginEdit)
            {
                args.Cancel = true;
                args.PreventRender = false;
                popupFormVisible = true;
                ControlCalidadSeleccionado = new();
            }
            if (args.RequestType == Syncfusion.Blazor.Grids.Action.BeginEdit)
            {
                ControlCalidadSeleccionado = args.Data;
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

        protected async Task OnActionCompleteHandler(ActionEventArgs<vControlCalidadPendientes> args)
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
        protected async Task Guardar(vControlCalidadPendientes controlCalidad)
        {

        }

        protected bool IsAdd { get; set; }

        //protected async Task<bool> Agregar(vControlCalidadPendientes controlCalidadPendientes)
        //{
        //    var response = await InventarioService.Existe(controlCalidadPendientes.VALE);
        //    if (!response)
        //    {
        //        var response2 = await InventarioService.Agregar(controlCalidadPendientes);
        //        if (response2.Error)
        //        {
        //            Console.WriteLine(await response2.HttpResponseMessage.Content.ReadAsStringAsync());
        //            await ToastMensajeError("Error al intentar Guardar un Proceso.");
        //            return false;
        //        }
        //        controlCalidadPendientes = response2.Response;
        //        return true;
        //    }
        //    await ToastMensajeError($"El proceso con codigo{controlCalidadPendientes.VALE} ya existe.\n\rO el proceso no es permitido.");
        //    return false;
        //}

        //protected async Task<bool> Actualizar(vControlCalidadPendientes controlCalidadPendientes)
        //{
        //    //var response = await InventarioService.Actualizar(controlCalidadPendientes.VALE, controlCalidadPendientes);
        //    //if (response.Error)
        //    //{
        //    //    await ToastMensajeError("Error al intenar guardar el Proceso.");
        //    //    return false;
        //    //}
        //    //return true;
        //}
        protected override async Task OnInitializedAsync()
        {
            MainLayout.Titulo = "Control de Calidad";

            SpinnerVisible = true;
            //CAMBIAR A INVENTARIOSERVICE
            //var response = await ControlCalidadService.GetControlCalidad(controlCalidadPendientes.REGISTRO);
            //if (!response.Error)
            //{
            //    control = response.Response;
            //}
            SpinnerVisible = false;
        }

        protected async Task GuardarProceso()
        {
            //bool guardado = false;
            //if (controlCalidadPendientes.ESNUEVO)
            //{
            //  //  guardado = await Agregar(procesos);
            //}
            //else
            //{
            //    //guardado = await Actualizar(procesos);
            //}
            //if (guardado)
            //{
            //    Show = false;
            //    controlCalidadPendientes.GUARDADO = guardado;
            //    await OnGuardar.InvokeAsync(controlCalidadPendientes);
            //}
        }

        //COMENTADO
        protected async Task GetItem(int vale)
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
        protected async Task BatchDeleteHandler(BeforeBatchDeleteArgs<vControlCalidadPendientes> args)
        {
            //var item = controlCalidadPendientes.Items.Find(i => i.VALE == args.RowData.VALE);
            //item.Estado = SupplyChain.Shared.Enum.EstadoItem.Eliminado;
        }
        //ANTES BeforeBatchAddArgs<CargaValoresDetalles>
        public void BatchAddHandler(BeforeBatchAddArgs<vControlCalidadPendientes> args)
        {
            IsAdd = true;
        }
        //ANTES BeforeBatchSaveArgs<CargaValoresDetalles>
        public void BatchSaveHandler(BeforeBatchSaveArgs<vControlCalidadPendientes> args)
        {
            IsAdd=false;
        }
        //ANTES CellSaveArgs<CargaValoresDetalles>
        public async Task CellSavedHandler(CellSaveArgs<vControlCalidadPendientes> args)
        {
            var index = await refGridItems.GetRowIndexByPrimaryKey(args.RowData.VALE);
            if (args.ColumnName == "SINMON")
            {

            }
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

