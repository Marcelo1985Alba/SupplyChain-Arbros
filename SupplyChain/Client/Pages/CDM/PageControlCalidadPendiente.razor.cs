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

namespace SupplyChain.Client.Pages.CDM;

public class PageControlCalidadPendientesBase : ComponentBase
{
    protected List<Pedidos> pendientes = new();
    protected Pedidos pendienteSeleccionado = new();
    protected bool popupFormVisible;
    protected SfGrid<Pedidos> refGrid;
    protected SfSpinner refSpinner;
    protected bool SpinnerVisible;
    protected SfToast ToastObj;

    protected List<object> Toolbaritems = new()
    {
        "Search"
    };

    [Inject] public InventarioService InventarioService { get; set; }
    [Inject] protected HttpClient Http { get; set; }
    [Inject] protected IJSRuntime jSRuntime { get; set; }
    [CascadingParameter] private MainLayout MainLayout { get; set; }

    protected override async Task OnInitializedAsync()
    {
        MainLayout.Titulo = "Control de Calidad";

        SpinnerVisible = true;
        pendientes = await InventarioService.GetControlCalidad();
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

    protected async Task OnActionBeginHandler(ActionEventArgs<Pedidos> args)
    {
        if (args.RequestType == Action.BeginEdit)
        {
            args.Cancel = true;
            args.PreventRender = false;
            popupFormVisible = true;
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
            refGrid.PreventRender();
            refGrid.Refresh();

            state = await refGrid.GetPersistData();
            await refGrid.AutoFitColumnsAsync();
            await refGrid.RefreshColumns();
            await refGrid.RefreshHeader();
        }
    }

    protected async Task OnActionCompleteHandler(ActionEventArgs<Pedidos> args)
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

    protected async Task rowSelected(RowSelectEventArgs<Pedidos> args)
    {
        pendienteSeleccionado = args.Data;
    }

    #region "Vista Grilla"

    protected const string APPNAME = "grdCargaValores";
    protected string state;

    #endregion
}