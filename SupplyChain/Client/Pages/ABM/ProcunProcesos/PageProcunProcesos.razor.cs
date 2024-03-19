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
using SupplyChain.Client.Shared;
using Syncfusion.Blazor.Navigations;
using System.Linq;
using static System.Net.Mime.MediaTypeNames;
using SupplyChain.Client.Pages.ABM.ProcunProcesos;

namespace SupplyChain.Client.Pages.ABM.Procedimientos
{
    public class PageProcunProcesosBase : ComponentBase
    {
        [Inject] protected HttpClient http { get; set; }
        [Inject] protected IJSRuntime jsRuntime { get; set; }
        [Inject] protected ProcunProcesoService ProcunProcesosService { get; set; }
        #region "Vista Grilla"
        protected const string APPNAME = "grdOpeABM";
        protected string state;
        #endregion
        protected List<Object> Toolbaritems = new List<Object>(){
        "Search",
        "Add",
        "Edit",
        new ItemModel{Text="Delete",Id="Delete"},       
        //new ItemModel{Text="ExcelExport", Id="ExcelExport" },
        new ItemModel{Text="Copy", Id="Copy"}

        };
        //protected List<ProcunProcesos> procunProcesos = new();
        protected List<ProcunProceso> procunProcesos = new();
        protected SfToast ToastObj;
        protected SfSpinner refSpinner;
        protected SfGrid<ProcunProceso> refGrid;
       protected FormProcunProcesos refFormProcunProcesos;
        protected ProcunProceso procedimientoSeleccionado = new();
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
            MainLayout.Titulo = "Procedimientos";
            SpinnerVisible = true;
            var response = await ProcunProcesosService.Get();
            if(!response.Error)
            {
                procunProcesos = response.Response;
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
            //else if(args.Item.Text== "ExcelExport")
            //{
            //    await refGrid.ExportToExcelAsync();
            //}
            else if(args.Item.Text== "Delete")
            {
                if((await refGrid.GetSelectedRecordsAsync()).Count > 0)
                {
                    bool isConfirmed= await jsRuntime.InvokeAsync<bool>("confirm","Seguro que desea eliminar la Operación?");
                    if (isConfirmed)
                    {
                        List<ProcunProceso> procedimientoABorrar= await refGrid.GetSelectedRecordsAsync();
                        var elimino = await ProcunProcesosService.Eliminar(procedimientoABorrar);

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
                            procunProcesos= procunProcesos.Where(s => s.Id != procedimientoSeleccionado.Id)
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
                procedimientoSeleccionado = new();
                ProcunProceso selectedRecord = refGrid.SelectedRecords[0];
                bool isConfirmed = await jsRuntime.InvokeAsync<bool>("confirm", "Seguro que desea copiar el Procedimiento?");
                if (isConfirmed)
                {
                    procedimientoSeleccionado.ESNUEVO = true;
                    procedimientoSeleccionado.PROCESO = selectedRecord.PROCESO;
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

        protected async Task Guardar(ProcunProceso procunProceso)
        {
            if (procunProceso.GUARDADO)
            {
                await ToastMensajeExito();
                popupFormVisible = false;
                if (procunProceso.ESNUEVO)
                {
                    procunProcesos.Add(procunProceso);
                }
                else
                {
                    var procSinModificar = procunProcesos.Where(p => p.Id == procunProceso.Id).FirstOrDefault();
                    procSinModificar.Id=procunProceso.Id;
                    procSinModificar.PROCESO=procunProceso.PROCESO;
                                       
                    procunProcesos.OrderByDescending(p => p.Id);
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


        protected async Task OnActionBeginHandler(ActionEventArgs<ProcunProceso> args)
        {
            if(args.RequestType==Syncfusion.Blazor.Grids.Action.Add ||
                args.RequestType == Syncfusion.Blazor.Grids.Action.BeginEdit)
            {
                args.Cancel = true;
                args.PreventRender = false;
                popupFormVisible = true;
                procedimientoSeleccionado = new();
                procedimientoSeleccionado.ESNUEVO = true;
                //await refFormOperaciones.Refrescar(operacionesSeleccionado);
            }

            if (args.RequestType == Syncfusion.Blazor.Grids.Action.BeginEdit)
            {
                procedimientoSeleccionado = args.Data;
                procedimientoSeleccionado.ESNUEVO = false;
                //await refFormProcunProcesos.Refrescar(procedimientoSeleccionado);
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
                await refGrid.Refresh();

                state= await refGrid.GetPersistDataAsync();
                await refGrid.AutoFitColumnsAsync();
                await refGrid.RefreshColumnsAsync();
                await refGrid.RefreshHeaderAsync();
            }
        }




        protected async Task OnActionCompleteHandler(ActionEventArgs<ProcunProceso> args)
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
