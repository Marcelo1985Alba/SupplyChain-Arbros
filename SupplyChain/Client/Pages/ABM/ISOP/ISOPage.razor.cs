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

namespace SupplyChain.Client.Pages.ABM.ISOP
{
    public class IsoPageBase : ComponentBase
    {
        [Inject] protected HttpClient Http { get; set; }
        [Inject] protected IJSRuntime jSRuntime { get; set; }
        [Inject] protected ISOService isoService { get; set; }
        #region "Vista Grilla"
        protected const string APPNAME = "grdISOABM";
        protected string state;
        #endregion
        protected List<Object> Toolbaritems = new List<Object>(){
        "Search",
        "Add",
        "Edit",
        "Delete",
        "Print",
        new ItemModel { Text = "Copia", TooltipText = "Copiar un ISO", PrefixIcon = "e-copy", Id = "Copy" },
        "ExcelExport"
        };
        protected List<ISO> isos = new();

        protected SfToast ToastObj;
        protected SfSpinner refSpinner;
        protected SfGrid<ISO> refGrid;
        protected ISO isoSeleccionado = new();
        protected bool SpinnerVisible = false;

        protected bool popupFormVisible = false;

        [CascadingParameter] MainLayout MainLayout { get; set; }
        protected override async Task OnInitializedAsync()
        {
            MainLayout.Titulo = "ISO";

            SpinnerVisible = true;
            var response = await isoService.Get();
            if (!response.Error)
            {
                isos = response.Response;
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
                await CopiarIso();
            }
            else if (args.Item.Id == "grdISO_delete")
            {
                if ((await refGrid.GetSelectedRecordsAsync()).Count > 0)
                {
                    bool isConfirmed = await jSRuntime.InvokeAsync<bool>("confirm", "Seguro de que desea eliminar el ISO?");
                    if (isConfirmed)
                    {
                        List<ISO> isosABorrar = await refGrid.GetSelectedRecordsAsync();
                        var response = isoService.Eliminar(isosABorrar);
                        if (!response.IsCompletedSuccessfully)
                        {
                            await this.ToastObj.Show(new ToastModel
                            {
                                Title = "EXITO!",
                                Content = "los isos seleccionadas fueron eliminadas correctamente.",
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
            }
            else if (args.Item.Id == "grdISO_excelexport")
            {
                await refGrid.ExportToExcelAsync();
            }
        }

        private async Task CopiarIso()
        {
            if (refGrid.SelectedRecords.Count == 1)
            {
                isoSeleccionado = new();
                ISO selectedRecord = refGrid.SelectedRecords[0];
                bool isConfirmed = await jSRuntime.InvokeAsync<bool>("confirm", "Seguro de que desea copiar el ISO?");
                if (isConfirmed)
                {
                    isoSeleccionado.ESNUEVO = true;
                    isoSeleccionado.Fecha = selectedRecord.Fecha;
                    isoSeleccionado.Descripcion = selectedRecord.Descripcion;
                    isoSeleccionado.Detalle = selectedRecord.Detalle;
                    isoSeleccionado.Factor = selectedRecord.Factor;
                    isoSeleccionado.Proceso = selectedRecord.Proceso;
                    isoSeleccionado.FODA = selectedRecord.FODA;
                    isoSeleccionado.ImpAmb = selectedRecord.ImpAmb;
                    isoSeleccionado.AspAmb = selectedRecord.AspAmb;
                    isoSeleccionado.Frecuencia = selectedRecord.Frecuencia;
                    isoSeleccionado.Impacto = selectedRecord.Impacto;
                    isoSeleccionado.CondOperacion = selectedRecord.CondOperacion;
                    isoSeleccionado.CondControl = selectedRecord.CondControl;
                    isoSeleccionado.NaturalezaDelImpacto = selectedRecord.NaturalezaDelImpacto;
                    isoSeleccionado.Gestion = selectedRecord.Gestion;
                    isoSeleccionado.Comentarios = selectedRecord.Comentarios;
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

        protected async Task OnActionBeginHandler(ActionEventArgs<ISO> args)
        {
            if (args.RequestType == Syncfusion.Blazor.Grids.Action.Add ||
                args.RequestType == Syncfusion.Blazor.Grids.Action.BeginEdit)
            {
                args.Cancel = true;
                args.PreventRender = false;
                popupFormVisible = true;
                isoSeleccionado = new();
                isoSeleccionado.ESNUEVO = true;
            }
            if (args.RequestType == Syncfusion.Blazor.Grids.Action.BeginEdit)
            {
                isoSeleccionado = args.Data;
                isoSeleccionado.ESNUEVO = false;
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

        protected async Task OnActionCompleteHandler(ActionEventArgs<ISO> args)
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

        protected async Task Guardar(ISO iso)
        {
            if (iso.GUARDADO)
            {
                await ToastMensajeExito();
                popupFormVisible = false;
                if (iso.ESNUEVO)
                {
                    isos.Add(iso);
                }
                else
                {
                    //actualizar datos sin ir a la base de datos
                    var isoSinModificar = isos.Where(p => p.Id == iso.Id).FirstOrDefault();
                    isoSinModificar.Id = iso.Id;
                    isoSinModificar.Fecha = iso.Fecha;
                    isoSinModificar.Descripcion = iso.Descripcion;
                    isoSinModificar.Detalle = iso.Detalle;
                    isoSinModificar.Factor = iso.Factor;
                    isoSinModificar.Proceso = iso.Proceso;
                    isoSinModificar.FODA = iso.FODA;
                    isoSinModificar.ImpAmb = iso.ImpAmb;
                    isoSinModificar.AspAmb = iso.AspAmb;
                    isoSinModificar.Frecuencia = iso.Frecuencia;
                    isoSinModificar.Impacto = iso.Impacto;
                    isoSinModificar.CondOperacion = iso.CondOperacion;
                    isoSinModificar.CondControl = iso.CondControl;
                    isoSinModificar.NaturalezaDelImpacto = iso.NaturalezaDelImpacto;
                    isoSinModificar.Gestion = iso.Gestion;
                    isoSinModificar.Comentarios = iso.Comentarios;
                    isos.OrderByDescending(p => p.Id);
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
