﻿using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SupplyChain;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Navigations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using SupplyChain.Shared.Models;
using SupplyChain.Shared;
using Syncfusion.Blazor.Spinner;

namespace SupplyChain.Client.Pages.PCP.Pedidos_Pendientes
{
    public class PedidosPendientesBase  : ComponentBase
    {
        [Inject] protected HttpClient Http { get; set; }
        [Inject] protected IJSRuntime JsRuntime { get; set; }
        protected SfGrid<ModeloPedidosPendientes> Grid;
        protected SfSpinner SpinnerObj;
        protected bool VisibleProperty { get; set; } = false;

        public bool Enabled = true;
        public bool Disabled = false;


        //protected List<CatOpe> catopes = new List<CatOpe>();
        protected List<ModeloPedidosPendientes> listaPedPend = new List<ModeloPedidosPendientes>();

        protected List<Object> Toolbaritems = new List<Object>(){
        new ItemModel(){ Type = ItemType.Separator},
        "Search",
        new ItemModel(){ Type = ItemType.Separator},
        "Print",
        new ItemModel(){ Type = ItemType.Separator},
        "ExcelExport",
        new ItemModel { Text = "Seleccionar Columnas", TooltipText = "Seleccionar Columnas", Id = "Seleccionar Columnas" }
    };

        protected const string APPNAME = "grdPedPendPCP";
        protected string state;
        protected override async Task OnInitializedAsync()
        {
            VisibleProperty = true;
            listaPedPend = await Http.GetFromJsonAsync<List<ModeloPedidosPendientes>>("api/PedidosPendientes");
            await Grid.AutoFitColumns();

        }

        public async Task Begin(ActionEventArgs<ModeloPedidosPendientes> args)
        {
            if (args.RequestType == Syncfusion.Blazor.Grids.Action.Grouping ||
                args.RequestType == Syncfusion.Blazor.Grids.Action.UnGrouping
                || args.RequestType == Syncfusion.Blazor.Grids.Action.ClearFiltering
                || args.RequestType == Syncfusion.Blazor.Grids.Action.CollapseAllComplete
                || args.RequestType == Syncfusion.Blazor.Grids.Action.ColumnState
                || args.RequestType == Syncfusion.Blazor.Grids.Action.ClearFiltering
                || args.RequestType == Syncfusion.Blazor.Grids.Action.Reorder ||
                args.RequestType == Syncfusion.Blazor.Grids.Action.Sorting
                )
            {
                //VisibleProperty = true;
                Grid.PreventRender();
                Grid.Refresh();

                state = await Grid.GetPersistData();
                await Grid.AutoFitColumnsAsync();
                await Grid.RefreshColumns();
                await Grid.RefreshHeader();
                //VisibleProperty = false;
            }

            if (args.RequestType == Syncfusion.Blazor.Grids.Action.RowDragAndDrop)
            {

            }
        }
        protected async Task OnDataBoundGrid(BeforeDataBoundArgs<ModeloPedidosPendientes> args)
        {
            await SpinnerObj.ShowAsync();
            VisibleProperty = true;
            Grid.PreventRender();
        }
        public async Task LoadGrid(object args)
        {
            await SpinnerObj.ShowAsync();
            VisibleProperty = true;
            await Grid.AutoFitColumns();
        }
        public async Task DataBoundHandler()
        {
            Grid.PreventRender();
            await Grid.AutoFitColumns();
            VisibleProperty = false;
        }

        public async Task ClickHandler(Syncfusion.Blazor.Navigations.ClickEventArgs args)
        {
            if (args.Item.Text == "Seleccionar Columnas")
            {
                await Grid.OpenColumnChooser();
            }
            if (args.Item.Text == "Excel Export")
            {
                //BORRARESTO = Grid.Columns.FirstOrDefault().Width.ToString();
                await this.Grid.ExcelExport();
            }
            if (args.Item.Text == "Print")
            {
                await this.Grid.Print();
            }
        }

        public async Task QueryCellInfoHandler(QueryCellInfoEventArgs<ModeloPedidosPendientes> args)
        {
            if (args.Data.CG_ESTADOCARGA == 2 || args.Data.CG_ESTADOCARGA == 3)
            {
                args.Cell.AddClass(new string[] { "verdes" });
            }

        }

        public async Task OnVistaSeleccionada(VistasGrillas vistasGrillas)
        {
            await Grid.SetPersistData(vistasGrillas.Layout);
        }
        public async Task OnReiniciarGrilla()
        {
            await Grid.ResetPersistData();
        }

    }
}
