//using Microsoft.AspNetCore.Components;
//using Microsoft.JSInterop;
//using SupplyChain.Client.HelperService;
//using SupplyChain.Client.Shared;
//using SupplyChain.Shared.Models;
//using SupplyChain.Shared;
//using Syncfusion.Blazor.Grids;
//using Syncfusion.Blazor.Navigations;
//using Syncfusion.Blazor.Notifications;
//using Syncfusion.Blazor.Spinner;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net.Http;
//using System.Net.Http.Json;
//using System.Threading.Tasks;
//using Syncfusion.Blazor.LinearGauge.Internal;
//using System.Drawing.Printing;


//namespace SupplyChain.Client.Pages.Compras.GrillaProveedores
//{
//    public class GrillaProveBase : ComponentBase
//    {
//        [Inject] protected HttpClient Http { get; set; }
//        [Inject] protected IJSRuntime jSRuntime { get; set; }
//        #region "VISTA GRILLA"
//        protected const string APPNAME = "grdProveedores";
//        protected string state;
//        #endregion
//        [Inject] protected ProveedoresService ProveedoresService { get; set; }
//        protected List<object> Toolbaritems = new List<object>()
//        {
//            new ItemModel{ Text="Buscar", TooltipText="Buscar Proveedor", PrefixIcon="e-search", Id="Buscar"}
//        };

//        protected List<vProveedorItris> proveedorItris = new();
//        protected SfToast ToastObj;
//        protected SfGrid<vProveedorItris> refGrid;
//        protected vProveedorItris proveedorSeleccionado = new();
//        protected bool SpinnerVisible = false;
//        protected bool popupFormVisible = false;
//        protected SfSpinner refSpinner;

//        [CascadingParameter] MainLayout MainLayout {get; set;}

//        protected override async Task OnInitializedAsync()
//        {
//            MainLayout.Titulo = "Proveedores";

//            SpinnerVisible = true;
//            var response = await ProveedoresService.Get();
//            if (!response.Error)
//            {
//                proveedorItris = response.Response;
//            }
//            SpinnerVisible = false;
//        }
//        protected async Task OnVistaSeleccionada(VistasGrillas vistasGrillas)
//        {
//            await refGrid.SetPersistDataAsync(vistasGrillas.Layout);
//        }
//        protected async Task OnReinciarGrilla()
//        {
//            await refGrid.ResetPersistData();
//        }

//        protected void OnCerrarDialog()
//        {
//            popupFormVisible= false;
//        }
//        protected async Task OnActionBeginHandler(ActionEventArgs<vProveedorItris> args)
//        {
//            if(args.RequestType==Syncfusion.Blazor.Grids.Action.Add||
//                args.RequestType == Syncfusion.Blazor.Grids.Action.BeginEdit)
//            {
//                args.Cancel = true;
//                args.PreventRender = false;
//                popupFormVisible= true;
//                proveedorSeleccionado = new();
//            }
//            if (args.RequestType == Syncfusion.Blazor.Grids.Action.BeginEdit)
//            {
//                proveedorSeleccionado = args.Data;
//            }
//            if(args.RequestType==Syncfusion.Blazor.Grids.Action.Grouping
//                || args.RequestType==Syncfusion.Blazor.Grids.Action.UnGrouping
//                || args.RequestType==Syncfusion.Blazor.Grids.Action.ClearFiltering
//                || args.RequestType==Syncfusion.Blazor.Grids.Action.CollapseAllComplete
//                || args.RequestType==Syncfusion.Blazor.Grids.Action.ColumnState
//                || args.RequestType==Syncfusion.Blazor.Grids.Action.Reorder
//                || args.RequestType==Syncfusion.Blazor.Grids.Action.Sorting
//                )
//            {
//                refGrid.PreventRender();
//                refGrid.Refresh();

//                state = await refGrid.GetPersistData();
//                await refGrid.AutoFitColumnsAsync();
//                await refGrid.RefreshColumns();
//                await refGrid.RefreshHeader();
//            }
//        }
//        protected async Task OnActionCompleteHandler(ActionEventArgs<vProveedorItris> args)
//        {
//            if (args.RequestType == Syncfusion.Blazor.Grids.Action.BeginEdit)
//            {
//                args.Cancel=true;
//                args.PreventRender = false;
//                popupFormVisible= true;
//            }
//        }

//    }
//}
