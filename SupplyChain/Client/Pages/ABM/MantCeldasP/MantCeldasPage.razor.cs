using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SupplyChain.Client.HelperService;
using SupplyChain.Client.Shared;
using SupplyChain.Shared;
using SupplyChain.Shared.Enum;
using SupplyChain.Shared.Models;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Navigations;
using Syncfusion.Blazor.Notifications;
using Syncfusion.Blazor.Spinner;
using Action = Syncfusion.Blazor.Grids.Action;

namespace SupplyChain.Client.Pages.ABM.MantCeldasP;

public class MantCeldaPageBase : ComponentBase
{
    protected List<Celdas> celdas = new();
    protected List<MantCeldas> mantCeldas = new();
    protected MantCeldas mantCeldaSeleccionada = new();

    protected bool popupFormVisible;
    protected SfGrid<MantCeldas> refGrid;
    protected SfSpinner refSpinner;
    protected bool SpinnerVisible;

    protected SfToast ToastObj;

    protected List<object> Toolbaritems = new()
    {
        "Search",
        "Add",
        "Edit",
        "Delete",
        "Print",
        new ItemModel { Text = "Copia", TooltipText = "Copiar una mantCelda", PrefixIcon = "e-copy", Id = "Copy" },
        "ExcelExport",
        new ItemModel { Text = "Cancelado", Id = "Cancelados" },
        new ItemModel { Text = "Programados", Id = "Programados" },
        new ItemModel { Text = "Realizados", Id = "Realizado" }
    };

    [Inject] protected HttpClient Http { get; set; }
    [Inject] protected IJSRuntime jSRuntime { get; set; }
    [Inject] protected MantCeldasService MantCeldasService { get; set; }
    [Inject] protected CeldasService CeldasService { get; set; }
    [CascadingParameter] private MainLayout MainLayout { get; set; }

    protected override async Task OnInitializedAsync()
    {
        MainLayout.Titulo = "MantCeldas";

        SpinnerVisible = true;
        var response = await MantCeldasService.Get();
        if (!response.Error) mantCeldas = response.Response;
        SpinnerVisible = true;
        var response2 = await CeldasService.Get();
        if (!response2.Error) celdas = response2.Response;
        SpinnerVisible = false;
    }

    protected async Task OnToolbarHandler(ClickEventArgs args)
    {
        if (args.Item.Id == "Copy")
        {
            await CopiarMantCelda();
        }
        else if (args.Item.Id == "grdCeldas_delete")
        {
            if ((await refGrid.GetSelectedRecordsAsync()).Count > 0)
            {
                var isConfirmed =
                    await jSRuntime.InvokeAsync<bool>("confirm", "Seguro de que desea eliminar la mantCelda?");
                if (isConfirmed)
                {
                    var mantCeldasABorrar = await refGrid.GetSelectedRecordsAsync();
                    var response = MantCeldasService.Eliminar(mantCeldasABorrar);
                    if (!response.IsCompletedSuccessfully)
                        await ToastObj.Show(new ToastModel
                        {
                            Title = "EXITO!",
                            Content = "las mantCeldas seleccionadas fueron eliminadas correctamente.",
                            CssClass = "e-toast-success",
                            Icon = "e-success toast-icons",
                            ShowCloseButton = true,
                            ShowProgressBar = true
                        });
                    else
                        await ToastMensajeError();
                }
            }
            //await EliminarCelda();
        }
        else if (args.Item.Id == "grdMantCeldas_excelexport")
        {
            await refGrid.ExportToExcelAsync();
        }
        else if (args.Item.Id == "Programados")
        {
            SpinnerVisible = true;
            await GetMantCeldas(EstadoMantCeldas.Programados);

            SpinnerVisible = false;
        }
        else if (args.Item.Id == "Cancelados")
        {
            SpinnerVisible = true;
            await GetMantCeldas(EstadoMantCeldas.Cancelado);

            SpinnerVisible = false;
        }
        else if (args.Item.Id == "Realizado")
        {
            SpinnerVisible = true;
            await GetMantCeldas(EstadoMantCeldas.Realizado);

            SpinnerVisible = false;
        }
    }

    protected async Task GetMantCeldas(EstadoMantCeldas estadoMantCeldas = EstadoMantCeldas.Todos)
    {
        var response = await MantCeldasService.ByEstado(estadoMantCeldas);
        if (response.Error)
        {
        }
        else
        {
            mantCeldas = response.Response.OrderByDescending(p => p.Estado).ToList();
        }
    }

    private async Task CopiarMantCelda()
    {
        if (refGrid.SelectedRecords.Count == 1)
        {
            mantCeldaSeleccionada = new MantCeldas();
            var selectedRecord = refGrid.SelectedRecords[0];
            var isConfirmed = await jSRuntime.InvokeAsync<bool>("confirm", "Seguro de que desea copiar la mantCelda?");
            if (isConfirmed)
            {
                mantCeldaSeleccionada.ESNUEVO = true;
                mantCeldaSeleccionada.Cg_Celda = selectedRecord.Cg_Celda;
                mantCeldaSeleccionada.Des_Celda = selectedRecord.Des_Celda;
                mantCeldaSeleccionada.Fecha = selectedRecord.Fecha;
                mantCeldaSeleccionada.Mantenimiento = selectedRecord.Mantenimiento;
                mantCeldaSeleccionada.Tarea = selectedRecord.Tarea;
                mantCeldaSeleccionada.Causa = selectedRecord.Causa;
                mantCeldaSeleccionada.TiempoParada = selectedRecord.TiempoParada;
                mantCeldaSeleccionada.Repuesto = selectedRecord.Repuesto;
                mantCeldaSeleccionada.Costo = selectedRecord.Costo;
                mantCeldaSeleccionada.Operario = selectedRecord.Operario;
                mantCeldaSeleccionada.Operador = selectedRecord.Operador;
                mantCeldaSeleccionada.Proveedor = selectedRecord.Proveedor;
                mantCeldaSeleccionada.Estado = selectedRecord.Estado;
                mantCeldaSeleccionada.FechaCumplido = selectedRecord.FechaCumplido;
                mantCeldaSeleccionada.Observaciones = selectedRecord.Observaciones;
                popupFormVisible = true;
            }
        }
        else
        {
            await ToastObj.Show(new ToastModel
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

    protected async Task OnActionBeginHandler(ActionEventArgs<MantCeldas> args)
    {
        if (args.RequestType == Action.Add ||
            args.RequestType == Action.BeginEdit)
        {
            args.Cancel = true;
            args.PreventRender = false;
            popupFormVisible = true;
            mantCeldaSeleccionada = new MantCeldas();
            mantCeldaSeleccionada.ESNUEVO = true;
        }

        if (args.RequestType == Action.BeginEdit)
        {
            mantCeldaSeleccionada = args.Data;
            mantCeldaSeleccionada.ESNUEVO = false;
        }

        if (args.RequestType == Action.Grouping
            || args.RequestType == Action.UnGrouping
            || args.RequestType == Action.ClearFiltering
            || args.RequestType == Action.CollapseAllComplete
            || args.RequestType == Action.ColumnState
            || args.RequestType == Action.ClearFiltering
            || args.RequestType == Action.Reorder
            || args.RequestType == Action.Sorting
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

    protected async Task OnActionCompleteHandler(ActionEventArgs<MantCeldas> args)
    {
        if (args.RequestType == Action.BeginEdit)
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

    protected async Task Guardar(MantCeldas mantCelda)
    {
        if (mantCelda.GUARDADO)
        {
            await ToastMensajeExito();
            popupFormVisible = false;
            if (mantCelda.ESNUEVO)
            {
                mantCelda.Des_Celda = celdas.Where(a => a.Id == mantCelda.Cg_Celda).FirstOrDefault().DES_CELDA;
                mantCeldas.Add(mantCelda);
            }
            else
            {
                //actualizar datos sin ir a la base de datos
                var mantCeldaSinModificar = mantCeldas.Where(p => p.Id == mantCelda.Id).FirstOrDefault();
                mantCeldaSinModificar.Id = mantCelda.Id;
                mantCeldaSinModificar.Cg_Celda = mantCelda.Cg_Celda;
                mantCeldaSinModificar.Des_Celda =
                    celdas.Where(a => a.Id == mantCelda.Cg_Celda).FirstOrDefault().DES_CELDA;
                mantCeldaSinModificar.Fecha = mantCelda.Fecha;
                mantCeldaSinModificar.Mantenimiento = mantCelda.Mantenimiento;
                mantCeldaSinModificar.Tarea = mantCelda.Tarea;
                mantCeldaSinModificar.Causa = mantCelda.Causa;
                mantCeldaSinModificar.TiempoParada = mantCelda.TiempoParada;
                mantCeldaSinModificar.Repuesto = mantCelda.Repuesto;
                mantCeldaSinModificar.Costo = mantCelda.Costo;
                mantCeldaSinModificar.Operario = mantCelda.Operario;
                mantCeldaSinModificar.Operador = mantCelda.Operador;
                mantCeldaSinModificar.Proveedor = mantCelda.Proveedor;
                mantCeldaSinModificar.Estado = mantCelda.Estado;
                mantCeldaSinModificar.FechaCumplido = mantCelda.FechaCumplido;
                mantCeldaSinModificar.Observaciones = mantCelda.Observaciones;
                mantCeldas.OrderByDescending(p => p.Id);
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
        await ToastObj.Show(new ToastModel
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

    protected async Task QueryCellInfoHandler(QueryCellInfoEventArgs<MantCeldas> args)
    {
        var today = DateTime.Now;
        var Diff_dates = today.Subtract((DateTime)args.Data.Fecha);
        if (args.Data.Estado == "Programado" && Diff_dates.Days > 0)
            args.Cell.AddClass(new[] { "amarillas" });
        else if (args.Data.Estado == "Programado")
            args.Cell.AddClass(new[] { "azules" });
        else if (args.Data.Estado == "Realizado")
            args.Cell.AddClass(new[] { "verdes" });
        else if (args.Data.Estado == "Cancelado") args.Cell.AddClass(new[] { "rojas" });
    }

    #region "Vista Grilla"

    protected const string APPNAME = "grdCeldasABM";
    protected string state;

    #endregion

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
}