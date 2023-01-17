//using Microsoft.AspNetCore.Components;
//using SupplyChain.Client.HelperService;
//using SupplyChain.Client.Shared;
//using SupplyChain.Shared;
//using Syncfusion.Blazor.Grids;
//using Syncfusion.Blazor.Notifications;
//using Syncfusion.Blazor.Spinner;
//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;

//namespace SupplyChain.Client.Pages.PCP.Carga_Procesos
//{
//    public class CargaProcesosPageBase : ComponentBase
//    {
//        [Inject] public InventarioService InventarioService { get; set; }
        
//        #region "Vista Grilla"
//        protected const string APPNAME = "grdCargaProcesos";
//        protected string state;
//        #endregion

//        protected List<Object> Toolbaritems = new List<Object>()
//        {
//            "Search"
//        };

//        protected Pedidos pendientesSeleccionado = new();
//        protected SfToast ToatObj;
//        protected SfSpinner refSpinner;
//        protected bool SpinnerVisible=false;
//        protected bool popupFormVisible=false;
//        protected List<Pedidos> procesos= new();
//        protected SfGrid<Pedidos> refGrid;

//        [CascadingParameter] MainLayout MainLayout { get; set; }
//        protected override async Task OnInitializedAsync()
//        {
//            MainLayout.Titulo = "Procesos";

//            SpinnerVisible=true;
//            procesos= await InventarioService.GetControlCalidad();
//            SpinnerVisible=false;
//        }
//        protected async Task OnVistaSeleccionada(VistasGrillas vistasGrillas)
//        {
//            await refGrid.SetPersistDataAsync(vistasGrillas.Layout);
//        }
//        protected async Task OnReiniciarGrilla()
//        {
//            await refGrid.ResetPersistData();
//        }

//        protected async Task OnActionBeginHandler(ActionEventArgs<Pedidos> args)
//        {
//            if(args.RequestType == Syncfusion.Blazor.Grids.Action.BeginEdit)
//            {
//                args.Cancel = true;
//                args.PreventRender = false;
//                popupFormVisible= true;
//            }
//            if (args.RequestType == Syncfusion.Blazor.Grids.Action.Grouping
//                || args.RequestType == Syncfusion.Blazor.Grids.Action.UnGrouping
//                || args.RequestType == Syncfusion.Blazor.Grids.Action.ClearFiltering
//                || args.RequestType == Syncfusion.Blazor.Grids.Action.CollapseAllComplete
//                || args.RequestType == Syncfusion.Blazor.Grids.Action.ColumnState
//                || args.RequestType == Syncfusion.Blazor.Grids.Action.ClearFiltering
//                || args.RequestType == Syncfusion.Blazor.Grids.Action.Reorder
//                || args.RequestType == Syncfusion.Blazor.Grids.Action.Sorting
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
//        protected async Task OnActionCompleteHandler(ActionEventArgs<Pedidos> args)
//        {
//            if (args.RequestType == Syncfusion.Blazor.Grids.Action.BeginEdit)
//            {
//                args.Cancel = true;
//                args.PreventRender = false;
//                popupFormVisible = true;
//            }
//        }
//        protected void OnCerrarDialog()
//        {
//            popupFormVisible = false;
//        }
//        protected async Task rowSelected(RowSelectEventArgs<Pedidos> args)
//        {
//            pendientesSeleccionado = args.Data;
//        }
//    }
//}
