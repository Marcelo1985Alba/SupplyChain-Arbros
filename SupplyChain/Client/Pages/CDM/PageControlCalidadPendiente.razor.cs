using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.JSInterop;
using SupplyChain;
using SupplyChain.Client.HelperService;
using SupplyChain.Client.Shared;
using SupplyChain.Shared;
using SupplyChain.Shared.Enum;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Kanban.Internal;
using Syncfusion.Blazor.LinearGauge.Internal;
using Syncfusion.Blazor.Navigations;
using Syncfusion.Blazor.Notifications;
using Syncfusion.Blazor.Spinner;
using Syncfusion.XlsIO.FormatParser.FormatTokens;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace SupplyChain.Client.Pages.CDM
{
    public class PageControlCalidadPendientesBase : ComponentBase
    {
        [Inject] public InventarioService InventarioService{ get; set; }
        [Inject] protected HttpClient Http { get; set; }
        [Inject] protected IJSRuntime jSRuntime { get; set; }
        #region "Vista Grilla"
        protected const string APPNAME = "grdCargaValores";
        protected string state;
        #endregion
        protected List<Object> Toolbaritems = new List<Object>()
        {
            "Search"
        };
        protected SfGrid<Pedidos> refGrid;
        protected List<Pedidos> pendientes = new();
        protected Pedidos pendienteSeleccionado = new();
        protected SfToast ToastObj;
        protected SfSpinner refSpinner;
        protected bool SpinnerVisible = false;
        protected bool popupFormVisible = false;
        [CascadingParameter] MainLayout MainLayout { get; set; }
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
            if (args.RequestType == Syncfusion.Blazor.Grids.Action.BeginEdit)
            {
                args.Cancel = true;
                args.PreventRender = false;
                popupFormVisible = true;
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
        protected async Task rowSelected(RowSelectEventArgs<Pedidos> args)
        {
            pendienteSeleccionado = args.Data;
        }
    }
}
