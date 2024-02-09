using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SupplyChain.Client.HelperService;
using System.Collections.Generic;
using System.Net.Http;
using System;
using SupplyChain.Shared;
using Syncfusion.Blazor.Notifications;
using Syncfusion.Blazor.Spinner;
using Syncfusion.Blazor.Grids;
using System.Threading.Tasks;
using SupplyChain.Client.Shared;
using SupplyChain.Shared.ABM;

namespace SupplyChain.Client.Pages.ABM.Procedimiento
{
    public class PageProcedimientosBase : ComponentBase
    {
        [Inject] protected HttpClient http { get; set; }
        [Inject] protected IJSRuntime jsRuntime { get; set; }
        [Inject] protected ProcedimientosService ProcedimientosService { get; set; }
        #region "Vista Grilla"
        protected const string APPNAME = "grdProdABM";
        protected string state;
        #endregion
        protected List<Object> Toolbaritems = new List<Object>(){
        "Search",
        "Add",
        "Edit",
        "Delete",
        "Print",
        "ExcelExport"
        };
        protected List<Procedimiento> Procedimientos = new();
        protected SfToast ToastObj;
        protected SfSpinner refSpinner;
        protected SfGrid<Procedimiento> grid;

        protected Procedimiento procedimientosSeleccionado = new();
        protected bool SpinnerVisible = false;

        protected bool popupFormVisible = false;

        [CascadingParameter] MainLayout MainLayout { get; set; }

        protected override async Task OnInitializedAsync()
        {
            MainLayout.Titulo = "Procedimientos";
            SpinnerVisible = true;
            var response = await ProcedimientosService.Get();
            if(!response.Error)
            {
                Procedimientos = response.Response;
            }
            SpinnerVisible = false;
        }

        #region "Eventos Vista Grilla"
        protected async Task OnVistaSeleccionada(VistasGrillas vistasGrillas)
        {
            await grid.SetPersistData(vistasGrillas.Layout);
        }
        protected async Task OnReiniciarGrilla()
        {
            await grid.ResetPersistData();
        }
        #endregion

        //protected async Task OnActionBeginHandler(ActionEventArgs<Procedimiento> args)
        //{
        //    if (args.RequestType == Syncfusion.Blazor.Grids.Action.Add ||
        //        args.RequestType == Syncfusion.Blazor.Grids.Action.BeginEdit)
        //    {
        //        args.Cancel = true;
        //        args.PreventRender = false;
        //        popupFormVisible = true;
        //        procedimientosSeleccionado = new();
        //        //procedimientosSeleccionado.ESNUEVO = true;
        //    }



        //    if (args.RequestType == Syncfusion.Blazor.Grids.Action.BeginEdit)
        //    {
        //        procedimientosSeleccionado = args.Data;
        //        //procedimientosSeleccionado.ESNUEVO = false;
        //        //await refFormProducto.Refrescar(procedimientosSeleccionado);
        //    }

        //    if (args.RequestType == Syncfusion.Blazor.Grids.Action.Grouping
        //        || args.RequestType == Syncfusion.Blazor.Grids.Action.UnGrouping
        //        || args.RequestType == Syncfusion.Blazor.Grids.Action.ClearFiltering
        //        || args.RequestType == Syncfusion.Blazor.Grids.Action.CollapseAllComplete
        //        || args.RequestType == Syncfusion.Blazor.Grids.Action.ColumnState
        //        || args.RequestType == Syncfusion.Blazor.Grids.Action.ClearFiltering
        //        || args.RequestType == Syncfusion.Blazor.Grids.Action.Reorder
        //        || args.RequestType == Syncfusion.Blazor.Grids.Action.Sorting
        //        )
        //    {
        //        //VisibleProperty = true;
        //        grid.PreventRender();
        //        grid.Refresh();

        //        state = await grid.GetPersistDataAsync();
        //        await grid.AutoFitColumnsAsync();
        //        await grid.RefreshColumnsAsync();
        //        await grid.RefreshHeaderAsync();
        //    }
        //}

        //protected async Task OnActionCompleteHandler(ActionEventArgs<Procedimiento> args)
        //{
        //    if (args.RequestType == Syncfusion.Blazor.Grids.Action.BeginEdit)
        //    {
        //        args.Cancel = true;
        //        args.PreventRender = false;
        //        popupFormVisible = true;
        //    }
        //}

    }
}
