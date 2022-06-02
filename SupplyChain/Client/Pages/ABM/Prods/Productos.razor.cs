using Microsoft.AspNetCore.Components;
using SupplyChain.Client.Shared;
using SupplyChain.Shared;
using SupplyChain.Shared.Models;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Notifications;
using Syncfusion.Blazor.Spinner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
namespace SupplyChain.Client.Pages.ABM.Prods
{
    public class ProductosPageBase : ComponentBase
    {
        [Inject] protected HttpClient Http { get; set; }

        #region "Vista Grilla"
        protected const string APPNAME = "grdProdABM";
        protected string state;
        #endregion

        protected List<Producto> Productos  = new();
        protected List<SupplyChain.Unidades> unidades = new();
        protected List<Moneda> monedas = new();
        protected List<SupplyChain.Celdas> celda = new();
        protected List<SupplyChain.Areas> area = new();
        protected List<SupplyChain.Lineas> linea = new();
        protected List<TipoArea> tipoarea = new();
        protected List<Cat> cat = new();
        protected SfToast ToastObj;
        protected SfSpinner refSpinner;
        protected SfGrid<Producto> refGrid;

        protected bool SpinnerVisible = false;
        [CascadingParameter] MainLayout MainLayout { get; set; }
        protected override async Task OnInitializedAsync()
        {
            MainLayout.Titulo = "Productos";

            SpinnerVisible = true;
            Productos = (await Http.GetFromJsonAsync<List<Producto>>("api/Prod")).OrderBy(s => s.CG_ORDEN).ToList();

            //unidades = await Http.GetFromJsonAsync<List<Unidades>>("api/unidades");
            //monedas = await Http.GetFromJsonAsync<List<Moneda>>("api/Monedas");
            //celda = await Http.GetFromJsonAsync<List<Celdas>>("api/Celdas");
            //area = await Http.GetFromJsonAsync<List<Areas>>("api/Areas");
            //linea = await Http.GetFromJsonAsync<List<Lineas>>("api/Lineas");
            //tipoarea = await Http.GetFromJsonAsync<List<TipoArea>>("api/TipoArea");
            //cat = await Http.GetFromJsonAsync<List<Cat>>("api/Cat");

            SpinnerVisible = false;
        }


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

        protected async Task OnActionBeginHandler(ActionEventArgs<Producto> args)
        {
            if (args.RequestType == Syncfusion.Blazor.Grids.Action.BeginEdit)
            {
                //SolicitudSeleccionada = args.Data;
                args.PreventRender = false;
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
                //VisibleProperty = true;
                refGrid.PreventRender();
                refGrid.Refresh();

                state = await refGrid.GetPersistData();
                await refGrid.AutoFitColumnsAsync();
                await refGrid.RefreshColumns();
                await refGrid.RefreshHeader();
            }
        }

        protected async Task OnActionCompleteHandler(ActionEventArgs<Producto> args)
        {
            if (args.RequestType == Syncfusion.Blazor.Grids.Action.BeginEdit)
            {
                //SolicitudSeleccionada = args.Data;
                args.PreventRender = false;
            }
        }
    }
}
