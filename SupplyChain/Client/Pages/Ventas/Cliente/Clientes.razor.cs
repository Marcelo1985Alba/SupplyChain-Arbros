using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SupplyChain.Client.HelperService;
using SupplyChain.Client.Shared;
using SupplyChain.Shared;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Notifications;
using Syncfusion.Blazor.Spinner;
using System.Collections.Generic;
using System.Drawing;
using System.Net.Http;
using System.Threading.Tasks;

namespace SupplyChain.Client.Pages.Ventas._5_Clientes
{
    public class GrillaClientesBase : ComponentBase
    {
        [Inject] protected HttpClient Http { get; set; }
        [Inject] protected IJSRuntime jsRuntime { get; set; }
        [Inject] protected ClienteService ClienteService { get; set; }
        protected List<object> Toolbaritems = new()
        {
            "Search"
        };
        #region
        protected const string APPNAME = "grdClienteExterno";
        protected string state;
        #endregion
        protected List<ClienteExterno> clientes = new();
        protected SfToast ToastObj;
        protected SfGrid<ClienteExterno> refGrid;
        protected ClienteExterno clienteSeleccionado = new();
        protected bool SpinnerVisible = false;
        protected bool popupFormVisible = false;
        protected SfSpinner refSpinner;

        [CascadingParameter] MainLayout MainLayout { get; set; }

        protected override async Task OnInitializedAsync()
        {
            MainLayout.Titulo = "Clientes";
            SpinnerVisible = true;
            var response = await ClienteService.GetClientesExterno();
            if (!response.Error)
            {
                clientes = response.Response;
            }
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

        protected async Task OnActionBeginHandler(ActionEventArgs<ClienteExterno> args)
        {
            if (args.RequestType == Syncfusion.Blazor.Grids.Action.Add ||
                args.RequestType == Syncfusion.Blazor.Grids.Action.BeginEdit)
            {
                args.Cancel = true;
                args.PreventRender = false;
                popupFormVisible = true;
                clienteSeleccionado = new();
            }
            if (args.RequestType == Syncfusion.Blazor.Grids.Action.BeginEdit)
            {
                clienteSeleccionado = args.Data;
            }
            if (args.RequestType == Syncfusion.Blazor.Grids.Action.Grouping
                || args.RequestType == Syncfusion.Blazor.Grids.Action.UnGrouping
                || args.RequestType == Syncfusion.Blazor.Grids.Action.ClearFiltering
                || args.RequestType == Syncfusion.Blazor.Grids.Action.CollapseAllComplete
                || args.RequestType == Syncfusion.Blazor.Grids.Action.ColumnState
                || args.RequestType == Syncfusion.Blazor.Grids.Action.Reorder
                || args.RequestType == Syncfusion.Blazor.Grids.Action.Sorting
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
        protected async Task OnActionCompleteHandler(ActionEventArgs<ClienteExterno> args)
        {
            if (args.RequestType == Syncfusion.Blazor.Grids.Action.BeginEdit)
            {
                args.Cancel = true;
                args.PreventRender = false;
                popupFormVisible = true;
            }
        }
    }
}
