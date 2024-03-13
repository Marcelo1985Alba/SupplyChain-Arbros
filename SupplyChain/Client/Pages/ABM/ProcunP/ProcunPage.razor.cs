using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SupplyChain.Client.HelperService;
using SupplyChain.Client.Shared;
using SupplyChain.Shared.Models;
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

namespace SupplyChain.Client.Pages.ABM.ProcunP
{
    public class ProcunPageBase : ComponentBase
    {
        [Inject] protected HttpClient Http { get; set; }
        [Inject] protected IJSRuntime jSRuntime { get; set; }
        [Inject] protected ProcunService ProcunService { get; set; }
        #region "Vista Grilla"
        protected const string APPNAME = "grdCeldasABM";
        protected string state;
        #endregion
        protected List<Object> Toolbaritems = new List<Object>(){
        "Search",
        "Add",
        //new ItemModel { Text = "Editar", TooltipText = "Editar", PrefixIcon = "e-edit", Id = "Editar" },
        "Edit",
        "Delete",
        new ItemModel { Text = "Copia", TooltipText = "Copiar una celda", PrefixIcon = "e-copy", Id = "Copy" },
        new ItemModel{Text="Excel Export", Id="ExcelExport"}
        };

        protected FormProcun refFormProcun;
        protected List<Procun> procuns = new();
        protected List<vProcun> vprocuns = new();
        protected SfToast ToastObj;
        protected SfSpinner refSpinner;
        protected SfGrid<Procun> refGrid;
        protected SfGrid<vProcun> refGrid2;
        protected Procun procSeleccionado = new();
        protected vProcun vprocSeleccionado = new();
        protected bool SpinnerVisible = false;

        protected bool popupFormVisible = false;
        protected ConfirmacionDialog ConfirmacionEliminarDialog;


        [CascadingParameter] MainLayout MainLayout { get; set; }
        protected override async Task OnInitializedAsync()
        {
            MainLayout.Titulo = "Procun";

            SpinnerVisible = true;
            var response = await ProcunService.GetvProcuns();
            if (!response.Error)
            {
                vprocuns = response.Response;
            }
            SpinnerVisible = false;
        }

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



        protected async Task OnToolbarHandler(ClickEventArgs args)
        {   
            if (args.Item.Id == "Copy")
            {
                await CopiarProcun();
            }
            else if (args.Item.Id == "grdProcun_delete")
            {
                if((await refGrid2.GetSelectedRecordsAsync()).Count > 0)
                {
                    bool isConfirmed = await jSRuntime.InvokeAsync<bool>("confirm", "Seguro que desela eliminar el proceso?");
                    if (isConfirmed)
                    {
                        List<vProcun> procunsABorrar= await refGrid2.GetSelectedRecordsAsync();
                        var response = ProcunService.Eliminar2(procunsABorrar);
                        if (!response.IsCompletedSuccessfully)
                        {
                            await this.ToastObj.ShowAsync(new ToastModel
                            {
                                Title="EXITO!",
                                Content="Los procesos eliminados fueron eliminados correctamente.",
                                Icon="e-success toast-icons",
                                ShowCloseButton=true,
                                ShowProgressBar=true
                            });
                        }
                        else
                        {
                            await ToastMensajeError();
                        }
                        await refGrid2.Refresh();

                        //await refGrid.Refresh();

                        // novaawait refGrid.RefreshColumnsAsync();    
                    }
                }
            }
            else if (args.Item.Id == "ExcelExport")
            {
                await refGrid2.ExportToExcelAsync();
            }
          
         
        }


        private async Task CopiarProcun()
        {
            if (refGrid2.SelectedRecords.Count == 1)
            {
                procSeleccionado = new();
                vProcun selectedRecord = refGrid2.SelectedRecords[0];
                bool isConfirmed = await jSRuntime.InvokeAsync<bool>("confirm", "Seguro de que desea copiar el procun?");
                if (isConfirmed)
                {

                    vprocSeleccionado.ESNUEVO = true;
                    vprocSeleccionado.ORDEN = selectedRecord.ORDEN;
                    vprocSeleccionado.CG_PROD = selectedRecord.CG_PROD;
                    vprocSeleccionado.DES_PROD= selectedRecord.DES_PROD;
                    vprocSeleccionado.CG_AREA = selectedRecord.CG_AREA;
                    vprocSeleccionado.CG_LINEA = selectedRecord.CG_LINEA;
                    vprocSeleccionado.CG_CELDA = selectedRecord.CG_CELDA;
                    vprocSeleccionado.PROCESO = selectedRecord.PROCESO;
                    vprocSeleccionado.TIEMPO1 = selectedRecord.TIEMPO1;
                    vprocSeleccionado.TS1 = selectedRecord.TS1;
                    vprocSeleccionado.PROPORC = selectedRecord.PROPORC;
                    vprocSeleccionado.AUTORIZA = selectedRecord.AUTORIZA;
                    vprocSeleccionado.USUARIO= selectedRecord.USUARIO;
                    vprocSeleccionado.CG_CATEOP=selectedRecord.CG_CATEOP;
                    vprocSeleccionado.TAREAPROC= selectedRecord.TAREAPROC;
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

        protected async Task OnActionBeginHandler(ActionEventArgs<vProcun> args)
        {
            if (args.RequestType == Syncfusion.Blazor.Grids.Action.Add ||
                args.RequestType == Syncfusion.Blazor.Grids.Action.BeginEdit)
            {
                args.Cancel = true;
                args.PreventRender = false;
                popupFormVisible = true;
                vprocSeleccionado = new();
                vprocSeleccionado.ESNUEVO = true;
            }
            if (args.RequestType == Syncfusion.Blazor.Grids.Action.BeginEdit)
            {
                vprocSeleccionado = args.Data;
                vprocSeleccionado.ESNUEVO = false;
                //await refFormProcun.Refrescar(procSeleccionado);
            }
            if (args.RequestType == Syncfusion.Blazor.Grids.Action.Grouping
                || args.RequestType == Syncfusion.Blazor.Grids.Action.UnGrouping
                || args.RequestType == Syncfusion.Blazor.Grids.Action.ClearFiltering
                || args.RequestType == Syncfusion.Blazor.Grids.Action.CollapseAllComplete
                || args.RequestType == Syncfusion.Blazor.Grids.Action.ColumnState
                || args.RequestType == Syncfusion.Blazor.Grids.Action.ClearFiltering
                || args.RequestType == Syncfusion.Blazor.Grids.Action.Reorder
                || args.RequestType == Syncfusion.Blazor.Grids.Action.Sorting
                )
            {
                //VisibleProperty = true;
                refGrid2.PreventRender();
                await refGrid2.Refresh();

                state = await refGrid2.GetPersistDataAsync();
                await refGrid2.AutoFitColumnsAsync();
                await refGrid2.RefreshColumnsAsync();
                await refGrid2.RefreshHeaderAsync();
            }
          
        }

        protected async Task OnActionCompleteHandler(ActionEventArgs<vProcun> args)
        {
            if (args.RequestType == Syncfusion.Blazor.Grids.Action.BeginEdit)
            {
                args.Cancel = true;
                args.PreventRender = false;
                popupFormVisible = true;
            }
        }

        protected async Task CellSelectedHandler(CellSelectEventArgs<vProcun> args)
        {
            var CellValue= await refGrid2.GetSelectedRowCellIndexesAsync();
            var CurrentEditRow = CellValue[0].Item1;
            var CurrentEditCell = (int)CellValue[0].Item2;

            var fields= await refGrid2.GetColumnFieldNamesAsync();
            await refGrid2.EditCellAsync(CurrentEditRow, fields[CurrentEditCell]);

        }

        public void RowSelectHandler(RowSelectEventArgs<vProcun> args)
        {
            vprocSeleccionado = args.Data;
        }

        protected void OnCerrarDialog()
        {
            popupFormVisible = false;
        }

        protected async Task Guardar(Procun proc)
        {
            if (proc.GUARDADO)
            {
                await ToastMensajeExito();
                popupFormVisible = false;
                if (proc.ESNUEVO)
                {
                    procuns.Add(proc);
                }
                else
                {
                    //actualizar datos sin ir a la base de datos
                    var procunSinModificar = procuns.Where(p => p.Id == proc.Id).FirstOrDefault();
                    procunSinModificar.Id = proc.Id;
                    procunSinModificar.ORDEN = proc.ORDEN;
                    procunSinModificar.CG_PROD = proc.CG_PROD;
                    procunSinModificar.Des_Prod = proc.Des_Prod;
                    procunSinModificar.CG_FORM = proc.CG_FORM;
                    procunSinModificar.CG_AREA = proc.CG_AREA;
                    procunSinModificar.CG_LINEA = proc.CG_LINEA;
                    procunSinModificar.CG_CELDA = proc.CG_CELDA;
                    procunSinModificar.PROCESO = proc.PROCESO;
                    procunSinModificar.DESCRIP = proc.DESCRIP;
                    procunSinModificar.OBSERV = proc.OBSERV;
                    procunSinModificar.DESPROC = proc.DESPROC;
                    procunSinModificar.TIEMPO1 = proc.TIEMPO1;
                    procunSinModificar.TS1 = proc.TS1;
                    procunSinModificar.FRECU = proc.FRECU;
                    procunSinModificar.CG_CALI1 = proc.CG_CALI1;
                    procunSinModificar.PROPORC = proc.PROPORC;
                    procunSinModificar.TOLE1 = proc.TOLE1;
                    procunSinModificar.CG_CALI2 = proc.CG_CALI2;
                    procunSinModificar.VALOR1 = proc.VALOR1;
                    procunSinModificar.TOLE2 = proc.TOLE2;
                    procunSinModificar.CG_CALI3 = proc.CG_CALI3;
                    procunSinModificar.VALOR2 = proc.VALOR2;
                    procunSinModificar.TOLE3 = proc.TOLE3;
                    procunSinModificar.CG_CALI4 = proc.CG_CALI4;
                    procunSinModificar.VALOR3 = proc.VALOR3;
                    procunSinModificar.TOLE4 = proc.TOLE4;
                    procunSinModificar.CG_CALI5 = proc.CG_CALI5;
                    procunSinModificar.CG_CALI6 = proc.CG_CALI6;
                    procunSinModificar.CG_CALI7 = proc.CG_CALI7;
                    procunSinModificar.CG_OPER = proc.CG_OPER;
                    procunSinModificar.FECHA = proc.FECHA;
                    procunSinModificar.COSTO = proc.COSTO;
                    procunSinModificar.VALOR4 = proc.VALOR4;
                    procunSinModificar.COSTOCOMB = proc.COSTOCOMB;
                    procunSinModificar.COSTOENERG = proc.COSTOENERG;
                    procunSinModificar.PLANTEL = proc.PLANTEL;
                    procunSinModificar.CG_CATEOP = proc.CG_CATEOP;
                    procunSinModificar.COSTAC = proc.COSTAC;
                    procunSinModificar.OCUPACION = proc.OCUPACION;
                    procunSinModificar.COEFI = proc.COEFI;
                    procunSinModificar.TAREAPROC = proc.TAREAPROC;
                    procunSinModificar.ESTANDAR = proc.ESTANDAR;
                    procunSinModificar.RELEVAN = proc.RELEVAN;
                    procunSinModificar.REVISION = proc.REVISION;
                    procunSinModificar.USUARIO = proc.USUARIO;
                    procunSinModificar.AUTORIZA = proc.AUTORIZA;
                    vprocuns.OrderByDescending(p => p.Id);
                }
                await refGrid2.RefreshHeaderAsync();
                await refGrid2.Refresh();
                await refGrid2.RefreshColumnsAsync();
            }
            else
            {
                await ToastMensajeError();
            }
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
    }
}
