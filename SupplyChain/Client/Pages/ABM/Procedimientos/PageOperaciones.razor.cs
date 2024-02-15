using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SupplyChain.Client.HelperService;
using System.Collections.Generic;
using System.Net.Http;
using System;
using SupplyChain.Shared;
using Syncfusion.Blazor.Notifications;
using Syncfusion.Blazor.Spinner;
using Syncfusion.Blazor.Grids;
using System.Threading.Tasks;
using SupplyChain;
using SupplyChain.Shared.Models;
using SupplyChain.Client.Shared;
using Syncfusion.Blazor.Lists;
using Syncfusion.Blazor.Navigations;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using static System.Net.Mime.MediaTypeNames;
using SupplyChain.Client.Pages.Ventas._3_Presupuestos;

namespace SupplyChain.Client.Pages.ABM.Procedimientos
{
    public class PageProcedimientosBase : ComponentBase
    {
        [Inject] protected HttpClient http { get; set; }
        [Inject] protected IJSRuntime jsRuntime { get; set; }
        [Inject] protected ProcedimientosService ProcedimientosService { get; set; }
        #region "Vista Grilla"
        protected const string APPNAME = "grdOpeABM";
        protected string state;
        #endregion
        protected List<Object> Toolbaritems = new List<Object>(){
        "Search",
        "Add",
        "Edit",
        new ItemModel{Text="Delete",Id="Delete"},       
        new ItemModel{Text="ExcelExport", Id="ExcelExport" },
        new ItemModel{Text="Copy", Id="Copy"}

        };
        protected List<Operaciones> Operacion = new();
        protected SfToast ToastObj;
        protected SfSpinner refSpinner;
        protected SfGrid<Operaciones> refGrid;
        protected FormOperaciones refFormOperaciones;
        protected Operaciones operacionesSeleccionado = new();
        protected bool SpinnerVisible = false;

        protected bool popupFormVisible = false;

        [CascadingParameter] MainLayout MainLayout { get; set; }
        #region "Eventos Vista Grilla"
        protected async Task OnVistaSeleccionada(VistasGrillas vistasGrillas)
        {
            await refGrid.SetPersistDataAsync(vistasGrillas.Layout);
        }
        protected async Task OnReiniciarGrilla()
        {
            await refGrid.ResetPersistDataAsync();
        }
        #endregion
        protected override async Task OnInitializedAsync()
        {
            MainLayout.Titulo = "Operaciones";
            SpinnerVisible = true;
            var response = await ProcedimientosService.Get();
            if(!response.Error)
            {
                Operacion = response.Response;
            }
            SpinnerVisible = false;
        }



        private async Task ToastMensajeExito()
        {
            await this.ToastObj.ShowAsync(new ToastModel
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
            await ToastObj.ShowAsync(new ToastModel
            {
                Title = "Error!",
                Content = "Ocurrio un Error.",
                CssClass = "e-toast-warning",
                Icon = "e-warning toast-icons",
                ShowCloseButton = true,
                ShowProgressBar = true
            });
        }

        protected async Task OnToolbarHandler(ClickEventArgs args)
        {
            if (args.Item.Text == "Copy")
            {
                await CopiarProcedimiento();
            }
            else if(args.Item.Text== "ExcelExport")
            {
                await refGrid.ExportToExcelAsync();
            }
            else if(args.Item.Text== "Delete")
            {
                if((await refGrid.GetSelectedRecordsAsync()).Count > 0)
                {
                    bool isConfirmed= await jsRuntime.InvokeAsync<bool>("confirm","Seguro que desea eliminar la Operación?");
                    if (isConfirmed)
                    {
                        List<Operaciones> procedimientoABorrar= await refGrid.GetSelectedRecordsAsync();
                        var elimino = await ProcedimientosService.Eliminar(procedimientoABorrar);

                        if (elimino)
                        {
                            await this.ToastObj.ShowAsync(new ToastModel
                            {
                                Title = "EXITO!",
                                Content = "las Operaciones seleccionados fueron eliminados correctamente.",
                                CssClass = "e-toast-success",
                                Icon = "e-success toast-icons",
                                ShowCloseButton = true,
                                ShowProgressBar = true
                            });
                            Operacion= Operacion.Where(s => s.Id != operacionesSeleccionado.Id)
                            .OrderByDescending(s => s.Id)
                            .ToList();
                            await refGrid.Refresh();
                        }
                        else
                        {
                            await ToastMensajeError();
                        }
                    }
                }
            }
        }

        private async Task CopiarProcedimiento()
        {
            if (refGrid.SelectedRecords.Count == 1)
            {
                operacionesSeleccionado = new();
                Operaciones selectedRecord = refGrid.SelectedRecords[0];
                bool isConfirmed = await jsRuntime.InvokeAsync<bool>("confirm", "Seguro que desea copiar el Procedimiento?");
                if (isConfirmed)
                {
                    operacionesSeleccionado.ESNUEVO = true;
                    operacionesSeleccionado.DESPACHO = selectedRecord.DESPACHO;
                    operacionesSeleccionado.VALE= selectedRecord.VALE;
                    operacionesSeleccionado.CG_PROD = selectedRecord.CG_PROD;
                    operacionesSeleccionado.CG_ORDEN = selectedRecord.CG_ORDEN;
                    operacionesSeleccionado.DESCAL = selectedRecord.DESCAL;
                    operacionesSeleccionado.CARCAL = selectedRecord.CARCAL;
                    operacionesSeleccionado.UNIDADM = selectedRecord.UNIDADM;
                    operacionesSeleccionado.CANTMEDIDA = selectedRecord.CANTMEDIDA;
                    operacionesSeleccionado.MEDIDA = selectedRecord.MEDIDA;
                    operacionesSeleccionado.MEDIDA1 = selectedRecord.MEDIDA1;
                    operacionesSeleccionado.TOLE1 = selectedRecord.TOLE1;
                    operacionesSeleccionado.TOLE2 = selectedRecord.TOLE2;
                    operacionesSeleccionado.OBSERV1 = selectedRecord.OBSERV1;
                    operacionesSeleccionado.AVISO = selectedRecord.AVISO;
                    operacionesSeleccionado.CG_PROVE = selectedRecord.CG_PROVE;
                    operacionesSeleccionado.VALORNC = selectedRecord.VALORNC;
                    operacionesSeleccionado.LEYENDANC = selectedRecord.LEYENDANC;
                    operacionesSeleccionado.LOTE = selectedRecord.LOTE;
                    operacionesSeleccionado.UNID = selectedRecord.UNID;
                    operacionesSeleccionado.NUM_PASE = selectedRecord.NUM_PASE;
                    operacionesSeleccionado.ENSAYOS = selectedRecord.ENSAYOS;
                    operacionesSeleccionado.FE_REG = selectedRecord.FE_REG;
                    operacionesSeleccionado.APROBADO = selectedRecord.APROBADO;
                    operacionesSeleccionado.TIPO = selectedRecord.TIPO;
                    operacionesSeleccionado.ANULADO = selectedRecord.ANULADO;
                    operacionesSeleccionado.CG_CLI = selectedRecord.CG_CLI;
                    operacionesSeleccionado.UNIDADM = selectedRecord.UNIDADM;
                    operacionesSeleccionado.Usuario = selectedRecord.Usuario;

                    popupFormVisible = true;
                }
            }
            else
            {
                await this.ToastObj.ShowAsync(new ToastModel
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

        protected async Task Guardar(Operaciones operaciones)
        {
            if (operaciones.GUARDADO)
            {
                await ToastMensajeExito();
                popupFormVisible = false;
                if (operaciones.ESNUEVO)
                {
                    Operacion.Add(operaciones);
                }
                else
                {
                    var procSinModificar = Operacion.Where(p => p.Id == operaciones.Id).FirstOrDefault();
                    //procSinModificar.Id= operaciones.Id;
                    procSinModificar.DESPACHO = operaciones.DESPACHO;
                    procSinModificar.CG_PROD = operaciones.CG_PROD;
                    procSinModificar.CG_ORDEN = operaciones.CG_ORDEN;
                    procSinModificar.DESCAL = operaciones.DESCAL;
                    procSinModificar.CARCAL = operaciones.CARCAL;
                    procSinModificar.UNIDADM = operaciones.UNIDADM;
                    procSinModificar.CANTMEDIDA = operaciones.CANTMEDIDA;
                    procSinModificar.MEDIDA = operaciones.MEDIDA;
                    procSinModificar.MEDIDA1 = operaciones.MEDIDA1;
                    procSinModificar.TOLE1 = operaciones.TOLE1;
                    procSinModificar.TOLE2 = operaciones.TOLE2;
                    procSinModificar.OBSERV1 = operaciones.OBSERV1;
                    procSinModificar.AVISO = operaciones.AVISO;
                    procSinModificar.CG_PROVE = operaciones.CG_PROVE;
                    procSinModificar.VALORNC = operaciones.VALORNC;
                    procSinModificar.LEYENDANC = operaciones.LEYENDANC;
                    procSinModificar.LOTE = operaciones.LOTE;
                    procSinModificar.UNID = operaciones.UNID;
                    procSinModificar.NUM_PASE = operaciones.NUM_PASE;
                    procSinModificar.ENSAYOS = operaciones.ENSAYOS;
                    procSinModificar.FE_REG = operaciones.FE_REG;
                    procSinModificar.APROBADO = operaciones.APROBADO;
                    procSinModificar.TIPO = operaciones.TIPO;
                    procSinModificar.ANULADO = operaciones.ANULADO;
                    procSinModificar.CG_CLI = operaciones.CG_CLI;
                    procSinModificar.Usuario = operaciones.Usuario;
                    Operacion.OrderByDescending(p => p.Id);
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


        protected async Task OnActionBeginHandler(ActionEventArgs<Operaciones> args)
        {
            if(args.RequestType==Syncfusion.Blazor.Grids.Action.Add || args.RequestType == Syncfusion.Blazor.Grids.Action.BeginEdit)
            {
                args.Cancel = true;
                args.PreventRender = false;
                popupFormVisible = true;
                operacionesSeleccionado = new();
                operacionesSeleccionado.ESNUEVO = true;
                //await refFormOperaciones.Refrescar(operacionesSeleccionado);
            }

            if (args.RequestType == Syncfusion.Blazor.Grids.Action.BeginEdit)
            {
                operacionesSeleccionado = args.Data;
                operacionesSeleccionado.ESNUEVO = false;
                await refFormOperaciones.Refrescar(operacionesSeleccionado);
            }
            if(args.RequestType== Syncfusion.Blazor.Grids.Action.Grouping
                || args.RequestType==Syncfusion.Blazor.Grids.Action.UnGrouping
                || args.RequestType==Syncfusion.Blazor.Grids.Action.ClearFiltering
                || args.RequestType==Syncfusion.Blazor.Grids.Action.CollapseAllComplete
                || args.RequestType==Syncfusion.Blazor.Grids.Action.ColumnState
                || args.RequestType==Syncfusion.Blazor.Grids.Action.ClearFiltering
                || args.RequestType==Syncfusion.Blazor.Grids.Action.Reorder
                || args.RequestType == Syncfusion.Blazor.Grids.Action.Sorting)
            {
                refGrid.PreventRender();
                refGrid.Refresh();

                state= await refGrid.GetPersistDataAsync();
                await refGrid.AutoFitColumnsAsync();
                await refGrid.RefreshColumnsAsync();
                await refGrid.RefreshHeaderAsync();
            }
        }




        protected async Task OnActionCompleteHandler(ActionEventArgs<Operaciones> args)
        {
            if (args.RequestType == Syncfusion.Blazor.Grids.Action.BeginEdit)
            {
                args.Cancel = true;
                args.PreventRender = false;
                popupFormVisible = true;
            }
        }

        protected void OnCerraDialog()
        {
            popupFormVisible = false;
        }

    }
}
