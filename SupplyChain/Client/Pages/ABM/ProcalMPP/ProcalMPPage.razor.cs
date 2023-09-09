﻿using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SupplyChain.Client.HelperService;
using SupplyChain.Client.Shared;
using SupplyChain.Shared;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Navigations;
using Syncfusion.Blazor.Notifications;
using Syncfusion.Blazor.Spinner;

namespace SupplyChain.Client.Pages.ABM.ProcalMPP;

public class ProcalMPPageBase : ComponentBase
{
    protected bool popupFormVisible;

    protected List<ProcalsMP> procalMP = new();
    protected ProcalsMP procalMPSeleccionada = new();
    protected SfGrid<ProcalsMP> refGrid;
    protected SfSpinner refSpinner;
    protected bool SpinnerVisible;

    protected SfToast ToastObj;

    protected List<object> Toolbaritems = new()
    {
        "Search", "Add", "Edit", "Delete", "Print",
        new ItemModel { Text = "Copia", TooltipText = "Copiar una materia", PrefixIcon = "e-copy", Id = "Copy" },
        "ExcelExport"
    };

    [Inject] protected HttpClient Http { get; set; }
    [Inject] protected IJSRuntime jSRuntime { get; set; }
    [Inject] protected ProcalMPService ProcalMPService { get; set; }

    [CascadingParameter] private MainLayout MainLayout { get; set; }

    protected override async Task OnInitializedAsync()
    {
        MainLayout.Titulo = "ProcalMP";

        SpinnerVisible = true;
        var response = await ProcalMPService.Get();
        if (!response.Error) procalMP = response.Response;
        SpinnerVisible = false;
    }

    protected async Task OnToolBarHandler(ClickEventArgs args)
    {
        if (args.Item.Id == "Copy")
        {
            await CopiarProcalMP();
        }
        else if (args.Item.Id == "grdProcalMP_delete")
        {
            if ((await refGrid.GetSelectedRecordsAsync()).Count > 0)
            {
                var isConfirmed = await jSRuntime.InvokeAsync<bool>("confirm", "Seguro que desea elimnar la linea?");
                if (isConfirmed)
                {
                    var procalMPABorrar = await refGrid.GetSelectedRecordsAsync();
                    var response = ProcalMPService.Eliminar(procalMPABorrar);
                    if (!response.IsCompletedSuccessfully)
                        await ToastObj.Show(new ToastModel
                        {
                            Title = "EXITO!",
                            Content = "las materias seleccionadas fueron eliminadas correctamente.",
                            CssClass = "e-toast-success",
                            Icon = "e-success toast-icons",
                            ShowCloseButton = true,
                            ShowProgressBar = true
                        });
                    else
                        await ToastMensajeError();
                }
            }
        }
        else if (args.Item.Id == "grdProcalMP_excelexport")
        {
            await refGrid.ExportToExcelAsync();
        }
    }

    private async Task CopiarProcalMP()
    {
        if (refGrid.SelectedRecords.Count == 1)
        {
            procalMPSeleccionada = new ProcalsMP();
            var selectedRecord = refGrid.SelectedRecords[0];
            var isConfirmed = await jSRuntime.InvokeAsync<bool>("confirm", "Seguro que desea copiar la materia?");
            if (isConfirmed)
            {
                procalMPSeleccionada.ESNUEVO = true;
                procalMPSeleccionada.DESCAL = selectedRecord.DESCAL;
                procalMPSeleccionada.CARCAL = selectedRecord.CARCAL;
                procalMPSeleccionada.UNIDADM = selectedRecord.UNIDADM;
                procalMPSeleccionada.MEDIDA = selectedRecord.MEDIDA;
                procalMPSeleccionada.TOLE1 = selectedRecord.TOLE1;
                procalMPSeleccionada.TOLE2 = selectedRecord.TOLE2;
                procalMPSeleccionada.OBSERV = selectedRecord.OBSERV;
                procalMPSeleccionada.DESCAL2 = selectedRecord.DESCAL2;
                procalMPSeleccionada.CARCAL2 = selectedRecord.CARCAL2;
                procalMPSeleccionada.OBSERV2 = selectedRecord.OBSERV2;
                procalMPSeleccionada.PRIORIDAD = selectedRecord.PRIORIDAD;
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

    protected async Task OnActionBeginHandler(ActionEventArgs<ProcalsMP> args)
    {
        if (args.RequestType == Action.Add ||
            args.RequestType == Action.BeginEdit)
        {
            args.Cancel = true;
            args.PreventRender = false;
            popupFormVisible = true;
            procalMPSeleccionada = new ProcalsMP();
            procalMPSeleccionada.ESNUEVO = true;
        }

        if (args.RequestType == Action.BeginEdit)
        {
            procalMPSeleccionada = args.Data;
            procalMPSeleccionada.ESNUEVO = false;
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

    protected async Task OnActionCompleteHandler(ActionEventArgs<ProcalsMP> args)
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

    protected async Task Guardar(ProcalsMP procMP)
    {
        if (procMP.GUARDADO)
        {
            await ToastMensajeExito();
            popupFormVisible = false;
            if (procMP.ESNUEVO)
            {
                procalMP.Add(procMP);
            }
            else
            {
                //actualiza los datos sin ir a BD
                var procalMPSinModificar = procalMP.Where(p => p.Id == procMP.Id).FirstOrDefault();
                procalMPSinModificar.Id = procMP.Id;
                procalMPSinModificar.DESCAL = procMP.DESCAL;
                procalMPSinModificar.CARCAL = procMP.CARCAL;
                procalMPSinModificar.UNIDADM = procMP.UNIDADM;
                procalMPSinModificar.TOLE1 = procMP.TOLE1;
                procalMPSinModificar.TOLE2 = procMP.TOLE2;
                procalMPSinModificar.OBSERV = procMP.OBSERV;
                procalMPSinModificar.DESCAL2 = procMP.DESCAL2;
                procalMPSinModificar.CARCAL2 = procMP.CARCAL2;
                procalMPSinModificar.OBSERV2 = procMP.OBSERV2;
                procalMPSinModificar.PRIORIDAD = procMP.PRIORIDAD;
                procalMP.OrderByDescending(p => p.Id);
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
            Icon = "e-succe q q ss toast-icons",
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

    #region "Vista Grilla"

    protected const string APPNAME = "grdProcalMP";
    protected string state;

    #endregion

    #region //EVENTOS VISTA DE LA GRITA

    protected async Task OnVistaSeleccionada(VistasGrillas vistasGrillas)
    {
        await refGrid.SetPersistDataAsync(vistasGrillas.Layout);
    }

    protected async Task OnReinciarGrilla()
    {
        await refGrid.ResetPersistData();
    }

    #endregion
}