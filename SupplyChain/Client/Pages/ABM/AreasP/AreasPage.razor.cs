using System.Collections.Generic;
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

namespace SupplyChain.Client.Pages.ABM.AreasP;

public class AreaPageBase : ComponentBase
{
    protected List<Areas> areas = new();
    protected Areas areaSeleccionada = new();

    protected bool popupFormVisible;
    protected SfGrid<Areas> refGrid;
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
        new ItemModel { Text = "Copia", TooltipText = "Copiar un area", PrefixIcon = "e-copy", Id = "Copy" },
        "ExcelExport"
    };

    [Inject] protected HttpClient Http { get; set; }
    [Inject] protected IJSRuntime jSRuntime { get; set; }
    [Inject] protected AreasService AreasService { get; set; }

    [CascadingParameter] private MainLayout MainLayout { get; set; }

    protected override async Task OnInitializedAsync()
    {
        MainLayout.Titulo = "Areas";

        SpinnerVisible = true;
        var response = await AreasService.Get();
        if (!response.Error) areas = response.Response;
        SpinnerVisible = false;
    }

    protected async Task OnToolbarHandler(ClickEventArgs args)
    {
        if (args.Item.Id == "Copy")
        {
            await CopiarArea();
        }
        else if (args.Item.Id == "grdAreas_delete")
        {
            if ((await refGrid.GetSelectedRecordsAsync()).Count > 0)
            {
                var isConfirmed = await jSRuntime.InvokeAsync<bool>("confirm", "Seguro de que desea eliminar el area?");
                if (isConfirmed)
                {
                    var areasABorrar = await refGrid.GetSelectedRecordsAsync();
                    var response = AreasService.Eliminar(areasABorrar);
                    if (!response.IsCompletedSuccessfully)
                        await ToastObj.Show(new ToastModel
                        {
                            Title = "EXITO!",
                            Content = "las areas seleccionadas fueron eliminadas correctamente.",
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
        else if (args.Item.Id == "grdArea_excelexport")
        {
            await refGrid.ExportToExcelAsync();
        }
    }

    private async Task CopiarArea()
    {
        if (refGrid.SelectedRecords.Count == 1)
        {
            areaSeleccionada = new Areas();
            var selectedRecord = refGrid.SelectedRecords[0];
            var isConfirmed = await jSRuntime.InvokeAsync<bool>("confirm", "Seguro de que desea copiar el area?");
            if (isConfirmed)
            {
                areaSeleccionada.ESNUEVO = true;
                areaSeleccionada.CG_CIA = selectedRecord.CG_CIA;
                areaSeleccionada.CG_PROVE = selectedRecord.CG_PROVE;
                areaSeleccionada.CG_TIPOAREA = selectedRecord.CG_TIPOAREA;
                areaSeleccionada.CONTROLES = selectedRecord.CONTROLES;
                areaSeleccionada.DES_AREA = selectedRecord.DES_AREA;
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

    protected async Task OnActionBeginHandler(ActionEventArgs<Areas> args)
    {
        if (args.RequestType == Action.Add ||
            args.RequestType == Action.BeginEdit)
        {
            args.Cancel = true;
            args.PreventRender = false;
            popupFormVisible = true;
            areaSeleccionada = new Areas();
            areaSeleccionada.ESNUEVO = true;
        }

        if (args.RequestType == Action.BeginEdit)
        {
            areaSeleccionada = args.Data;
            areaSeleccionada.ESNUEVO = false;
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

    protected async Task OnActionCompleteHandler(ActionEventArgs<Areas> args)
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

    protected async Task Guardar(Areas area)
    {
        if (area.GUARDADO)
        {
            await ToastMensajeExito();
            popupFormVisible = false;
            if (area.ESNUEVO)
            {
                areas.Add(area);
            }
            else
            {
                //actualizar datos sin ir a la base de datos
                var areaSinModificar = areas.Where(p => p.Id == area.Id).FirstOrDefault();
                areaSinModificar.Id = area.Id;
                areaSinModificar.CG_CIA = area.CG_CIA;
                areaSinModificar.CG_PROVE = area.CG_PROVE;
                areaSinModificar.CG_TIPOAREA = area.CG_TIPOAREA;
                areaSinModificar.CONTROLES = area.CONTROLES;
                areaSinModificar.DES_AREA = area.DES_AREA;
                areas.OrderByDescending(p => p.Id);
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

    #region "Vista Grilla"

    protected const string APPNAME = "grdAreasABM";
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