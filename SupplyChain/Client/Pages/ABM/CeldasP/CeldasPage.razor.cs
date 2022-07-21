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

namespace SupplyChain.Client.Pages.ABM.CeldasP
{
    public class CeldaPageBase : ComponentBase
    {
        [Inject] protected HttpClient Http { get; set; }
        [Inject] protected IJSRuntime jSRuntime { get; set; }
        [Inject] protected CeldasService CeldasService { get; set; }
        #region "Vista Grilla"
        protected const string APPNAME = "grdCeldasABM";
        protected string state;
        #endregion
        protected List<Object> Toolbaritems = new List<Object>(){
        "Search",
        "Add",
        "Edit",
        "Delete",
        "Print",
        new ItemModel { Text = "Copia", TooltipText = "Copiar una celda", PrefixIcon = "e-copy", Id = "Copy" },
        "ExcelExport"
        };
        protected List<Celdas> celdas = new();

        protected SfToast ToastObj;
        protected SfSpinner refSpinner;
        protected SfGrid<Celdas> refGrid;
        protected Celdas celdaSeleccionada = new();
        protected bool SpinnerVisible = false;

        protected bool popupFormVisible = false;

        [CascadingParameter] MainLayout MainLayout { get; set; }
        protected override async Task OnInitializedAsync()
        {
            MainLayout.Titulo = "Celdas";

            SpinnerVisible = true;
            var response = await CeldasService.Get();
            if (!response.Error)
            {
                celdas = response.Response;
            }
            SpinnerVisible = false;
        }

        #region "Eventos Vista Grilla"
        protected async Task OnVistaSeleccionada(VistasGrillas vistasGrillas)
        {
            await refGrid.SetPersistData(vistasGrillas.Layout);
        }
        protected async Task OnReiniciarGrilla()
        {
            await refGrid.ResetPersistData();
        }
        #endregion

        protected async Task OnToolbarHandler(ClickEventArgs args)
        {
            if (args.Item.Id == "Copy")
            {
                await CopiarCelda();
            }
            else if (args.Item.Id == "grdCeldas_delete")
            {
                if ((await refGrid.GetSelectedRecordsAsync()).Count > 0)
                {
                    bool isConfirmed = await jSRuntime.InvokeAsync<bool>("confirm", "Seguro de que desea eliminar el precio de articulo?");
                    if (isConfirmed)
                    {
                        List<Celdas> celdasABorrar = await refGrid.GetSelectedRecordsAsync();
                        var response = CeldasService.Eliminar(celdasABorrar);
                        if (!response.IsCompletedSuccessfully)
                        {
                            await this.ToastObj.Show(new ToastModel
                            {
                                Title = "EXITO!",
                                Content = "las celdas seleccionadas fueron eliminadas correctamente.",
                                CssClass = "e-toast-success",
                                Icon = "e-success toast-icons",
                                ShowCloseButton = true,
                                ShowProgressBar = true
                            });
                        }
                        else
                        {
                            await ToastMensajeError();
                        }
                    }
                }
                //await EliminarCelda();
            }
            else if (args.Item.Id == "grdCelda_excelexport")
            {
                await refGrid.ExportToExcelAsync();
            }
        }

        private async Task CopiarCelda()
        {
            if (refGrid.SelectedRecords.Count == 1)
            {
                celdaSeleccionada = new();
                Celdas selectedRecord = refGrid.SelectedRecords[0];
                bool isConfirmed = await jSRuntime.InvokeAsync<bool>("confirm", "Seguro de que desea copiar la celda?");
                if (isConfirmed)
                {
                    celdaSeleccionada.ESNUEVO = true;
                    celdaSeleccionada.DES_CELDA= selectedRecord.DES_CELDA;
                    popupFormVisible = true;
                }
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

        protected async Task OnActionBeginHandler(ActionEventArgs<Celdas> args)
        {
            if (args.RequestType == Syncfusion.Blazor.Grids.Action.Add ||
                args.RequestType == Syncfusion.Blazor.Grids.Action.BeginEdit)
            {
                args.Cancel = true;
                args.PreventRender = false;
                popupFormVisible = true;
                celdaSeleccionada = new();
                celdaSeleccionada.ESNUEVO = true;
            }
            if (args.RequestType == Syncfusion.Blazor.Grids.Action.BeginEdit)
            {
                celdaSeleccionada = args.Data;
                celdaSeleccionada.ESNUEVO = false;
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
                refGrid.PreventRender();
                refGrid.Refresh();

                state = await refGrid.GetPersistData();
                await refGrid.AutoFitColumnsAsync();
                await refGrid.RefreshColumns();
                await refGrid.RefreshHeader();
            }
        }

        protected async Task OnActionCompleteHandler(ActionEventArgs<Celdas> args)
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

        protected async Task Guardar(Celdas celda)
        {
            if (celda.GUARDADO)
            {
                await ToastMensajeExito();
                popupFormVisible = false;
                if (celda.ESNUEVO)
                {
                    celdas.Add(celda);
                }
                else
                {
                    //actualizar datos sin ir a la base de datos
                    var celdaSinModificar = celdas.Where(p => p.Id == celda.Id).FirstOrDefault();
                    celdaSinModificar.Id = celda.Id;
                    celdaSinModificar.DES_CELDA = celda.DES_CELDA;
                    celdas.OrderByDescending(p => p.Id);
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
