using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.JSInterop;
using SupplyChain.Client.HelperService;
using SupplyChain.Client.Shared;
using SupplyChain.Shared;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.LinearGauge.Internal;
using Syncfusion.Blazor.Navigations;
using Syncfusion.Blazor.Notifications;
using Syncfusion.Blazor.Spinner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace SupplyChain.Client.Pages.CDM
{
    public class PageValoresBase : ComponentBase
    {
        [Inject] protected HttpClient Http { get; set; }
        [Inject] protected IJSRuntime jSRuntime { get; set; }
        [Inject] protected CargaValoresService CargaValoresService { get; set; }
        #region "Vista Grilla"
        protected const string APPNAME = "grdCargaValores";
        protected string state;
        #endregion
        protected List<Object> Toolbaritems = new List<Object>()
        {
               "Search", "Add", "Edit", "Delete", "Print",
            new ItemModel {Text = "Copia", TooltipText="Copiar una materia", PrefixIcon="e-copy", Id="Copy"},
            "ExcelExport"
        };

        protected List<Valores> valor = new();
        protected SfToast ToastObj;
        protected SfSpinner refSpinner;
        protected SfGrid<Valores> refGrid;
        protected Valores valoresSeleccionada = new();
        protected bool SpinnerVisible = false;
        protected bool popupFormVisible = false;

        [CascadingParameter] MainLayout MainLayout { get; set; }
        protected override async Task OnInitializedAsync()
        {
            MainLayout.Titulo = "Valores";

            SpinnerVisible = true;
            var response = await CargaValoresService.Get();
            if (!response.Error)
            {
                valor = response.Response;
            }
            SpinnerVisible = false;
        }

        protected async Task OnVistaSeleccionada(VistasGrillas vistasGrillas)
        {
            await refGrid.SetPersistDataAsync(vistasGrillas.Layout);
        }

        protected async Task OnReiniciarGrilla()
        {
            await refGrid.ResetPersistData();
        }

        protected async Task OnToolHandler(ClickEventArgs args)
        {
            if (args.Item.Id == "Copy")
            {
                await CopiarCargaValores();
            }
            else if (args.Item.Id == "grdCargaValores_delete")
            {
                if ((await refGrid.GetSelectedRecordsAsync()).Count > 0)
                {
                    bool isConfirmed = await jSRuntime.InvokeAsync<bool>(
                        "confirm",
                        "Seguro que deseaa eliminar la linea?");
                    if (isConfirmed)
                    {
                        List<Valores> valoresABorrar = await refGrid.GetSelectedRecordsAsync();
                        var response = CargaValoresService.Eliminar(valoresABorrar);
                        if (!response.IsCompletedSuccessfully)
                        {
                            await this.ToastObj.Show(new ToastModel
                            {
                                Title = "EXITO!",
                                Content = "los procesos seleccionados fueron eliminados correctamente.",
                                CssClass = "e-toast-success",
                                Icon = "e-success toast-icons",
                                ShowCloseButton = true,
                                ShowProgressBar = true,
                            });
                        }
                        else
                        {
                            await ToastMensajeError();
                        }
                    }
                }
            }
            else if (args.Item.Id == "grdCargaValores_excelexport")
            {
                await refGrid.ExportToExcelAsync();
            }
        }

        private async Task CopiarCargaValores()
        {
            if (refGrid.SelectedRecords.Count == 1)
            {
                valoresSeleccionada = new();
                Valores selectedRecord = refGrid.SelectedRecords[0];
                bool isConfirmed = await jSRuntime.InvokeAsync<bool>("confirm", "Seguro que desea copiar la materia?");
                if (isConfirmed)
                {
                    valoresSeleccionada.ESNUEVO = true;
                    valoresSeleccionada.FE_ENSAYO = selectedRecord.FE_ENSAYO;
                    valoresSeleccionada.CANTMEDIDA = selectedRecord.CANTMEDIDA;
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

        protected async Task OnActionBeginHandler(ActionEventArgs<Valores> args)
        {
            if (args.RequestType == Syncfusion.Blazor.Grids.Action.Add ||
               args.RequestType == Syncfusion.Blazor.Grids.Action.BeginEdit)
            {
                args.Cancel = true;
                args.PreventRender = false;
                popupFormVisible = true;
                valoresSeleccionada = new();
                valoresSeleccionada.ESNUEVO = true;
            }
            if (args.RequestType == Syncfusion.Blazor.Grids.Action.BeginEdit)
            {
                valoresSeleccionada = args.Data;
                valoresSeleccionada.ESNUEVO = false;
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

        protected async Task OnActionCompleteHandler(ActionEventArgs<Valores> args)
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

        protected async Task Guardar(Valores valorg)
        {
            if (valorg.GUARDADO)
            {
                await ToastMensajeExito();
                popupFormVisible = false;
                if (valorg.ESNUEVO)
                {
                    valor.Add(valorg);
                }
                else
                {
                    //actualiza los datos sin ir a BD
                    var valoresSinModificar = valor.Where(v => v.Id == valorg.Id).FirstOrDefault();
                    valoresSinModificar.Id = valorg.Id;
                    valoresSinModificar.FE_ENSAYO = valorg.FE_ENSAYO;
                    valoresSinModificar.CANTMEDIDA = valorg.CANTMEDIDA;
                    valor.OrderByDescending(v => v.Id);
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