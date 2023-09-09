using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SupplyChain.Client.HelperService;
using SupplyChain.Client.Shared;
using SupplyChain.Shared;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Notifications;
using Syncfusion.Blazor.Spinner;

namespace SupplyChain.Client.Pages.Compras.GrillaProveedores;

public class GrillaProveBase : ComponentBase
{
    protected bool popupFormVisible;

    protected List<vProveedorItris> proveedorItris = new();
    protected vProveedorItris proveedorSeleccionado = new();
    protected SfGrid<vProveedorItris> refGrid;
    protected SfSpinner refSpinner;
    protected bool SpinnerVisible;
    protected SfToast ToastObj;

    protected List<object> Toolbaritems = new()
    {
        "Search"
    };

    [Inject] protected HttpClient Http { get; set; }
    [Inject] protected IJSRuntime jSRuntime { get; set; }
    [Inject] protected ProveedoresService ProveedoresService { get; set; }

    [CascadingParameter] private MainLayout MainLayout { get; set; }

    protected override async Task OnInitializedAsync()
    {
        MainLayout.Titulo = "Proveedores";

        SpinnerVisible = true;
        var response = await ProveedoresService.GetProveedoresItris();
        if (!response.Error) proveedorItris = response.Response;
        SpinnerVisible = false;
    }

    protected async Task OnVistaSeleccionada(VistasGrillas vistasGrillas)
    {
        await refGrid.SetPersistDataAsync(vistasGrillas.Layout);
    }

    protected async Task OnReinciarGrilla()
    {
        await refGrid.ResetPersistData();
    }

    protected void OnCerrarDialog()
    {
        popupFormVisible = false;
    }

    protected async Task OnActionBeginHandler(ActionEventArgs<vProveedorItris> args)
    {
        if (args.RequestType == Action.Add ||
            args.RequestType == Action.BeginEdit)
        {
            args.Cancel = true;
            args.PreventRender = false;
            popupFormVisible = true;
            proveedorSeleccionado = new vProveedorItris();
        }

        if (args.RequestType == Action.BeginEdit) proveedorSeleccionado = args.Data;
        if (args.RequestType == Action.Grouping
            || args.RequestType == Action.UnGrouping
            || args.RequestType == Action.ClearFiltering
            || args.RequestType == Action.CollapseAllComplete
            || args.RequestType == Action.ColumnState
            || args.RequestType == Action.Reorder
            || args.RequestType == Action.Sorting
           )
        {
            refGrid.PreventRender();
            refGrid.Refresh();

            state = await refGrid.GetPersistData();
            await refGrid.AutoFitColumnsAsync();
            await refGrid.RefreshColumns();
            await refGrid.RefreshHeader();
        }
    }

    protected async Task OnActionCompleteHandler(ActionEventArgs<vProveedorItris> args)
    {
        if (args.RequestType == Action.BeginEdit)
        {
            args.Cancel = true;
            args.PreventRender = false;
            popupFormVisible = true;
        }
    }

    #region "VISTA GRILLA"

    protected const string APPNAME = "grdProveedores";
    protected string state;

    #endregion
}