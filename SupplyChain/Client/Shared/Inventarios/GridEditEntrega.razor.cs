using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SupplyChain.Shared;
using SupplyChain.Shared.Models;
using SupplyChain.Shared.PCP;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Navigations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SupplyChain.Client.Shared.Inventarios
{
    public class GridEditEntregaBase: ComponentBase
    {
        [Inject] public IJSRuntime JsRuntime { get; set; }
        [Inject] HttpClient Http { get; set; }
        protected ConfirmacionDialog ConfirmacionDialog;
        protected BuscadorEmergente<Pedidos> BuscadorProducto;
        protected Pedidos stock;
        protected Pedidos stockCopiado;
        protected bool confirmaCopy = false;
        protected bool bAgregarInsumo = false;
        protected SupplyChain.Client.Shared.BuscadorEmergenteResumenStock BuscadorEmergenteRS;
        protected Producto[] DataSourceProductos;
        [Parameter] public string Titulo { get; set; } = null!;
        [Parameter] public List<Pedidos> DataSource { get; set; } = null!;
        [Parameter] public bool PermiteAgregar { get; set; } = false;
        [Parameter] public bool PermiteEditar { get; set; } = false;
        [Parameter] public bool PermiteEliminar { get; set; } = false;
        [Parameter] public EventCallback<Pedidos> OnGuardar { get; set; }
        [Parameter] public EventCallback<List<Pedidos>> OnItemsDataSource { get; set; }
        [Parameter] public SelectionType TipoSeleccion { get; set; } = SelectionType.Single;
        [CascadingParameter] public PedidoEncabezado RegistroGenerado { get; set; }
        protected Dictionary<string, object> HtmlAttribute = new Dictionary<string, object>()
        {
           {"type", "button" }
        };

        protected List<ItemModel> Toolbaritems = new List<ItemModel>(){
        new ItemModel { CssClass="", Text = "Agregar Insumo", ShowTextOn = DisplayMode.Both, Type = ItemType.Button,
            TooltipText = "Agregar Insumo", SuffixIcon = "fa fa-search", Id = "AgregarInsumo" }
        };

        protected DialogSettings DialogParams = new DialogSettings { MinHeight = "400px", Width = "900px" };

        protected SfGrid<Pedidos> Grid;

        private bool deshabilitaBotonBuscar = false;
        protected bool DeshabilitaBotonBuscar
        {
            get { return deshabilitaBotonBuscar; }
            set
            {
                deshabilitaBotonBuscar = DataSource.Any(s => s.TIPOO == 9);

            }
        }



        protected List<Deposito> depositos = new();

        protected override async Task OnInitializedAsync()
        {
            depositos = await Http.GetFromJsonAsync<List<Deposito>>("api/Deposito");

        }


        public async Task ClickHandler(Syncfusion.Blazor.Navigations.ClickEventArgs args)
        {
            if (args.Item.Id == "AgregarInsumo")
            {
                await AgregarInsumo();
            }
        }

        protected async Task OnActionBeginHandler(ActionEventArgs<Pedidos> args)
        {
            if (args.RequestType == Syncfusion.Blazor.Grids.Action.BeginEdit)
            {
                args.PreventRender = false;
                stock = args.Data;
            }
        }

        protected async Task AgregarInsumo()
        {
            if (RegistroGenerado.TIPOO == 9) //movim entre deo
            {
                TipoSeleccion = SelectionType.Multiple;
            }

            PopupBuscadorStockVisible = true;
            bAgregarInsumo = true;
            Items = null;
            await BuscadorEmergenteRS.ShowAsync();
            tituloBuscador = "Listado de Insumos en Stock";

            ColumnasBuscador = new string[] { "CG_ART", "DEPOSITO", "DESPACHO", "SERIE", "LOTE", "STOCK" };

            Items = await Http.GetFromJsonAsync<vResumenStock[]>("api/ResumenStock/GetResumenStockPositivo");

            
        }

        public void RowBound(RowDataBoundEventArgs<Pedidos> args)
        {
            if (args.Data.STOCK <= 0)
            {
                args.Row.AddClass(new string[] { "row-red" });
            }
            //else if (args.Data.Freight < 35)
            //{
            //    args.Row.AddClass(new string[] { "below-35" });
            //}
            //else
            //{
            //    args.Row.AddClass(new string[] { "above-35" });
            //}
        }

        public async Task BeginEditHandler(BeginEditArgs<Pedidos> args)
        {
            stock = args.RowData;
            //ir al servidor y cargar el stock del item
            if (stock.TIPOO == 6 || stock.TIPOO == 9) //DEVolucion y Momvim entre deposito
            {
                if (stock == null) return;

                if (!string.IsNullOrEmpty(stock.CG_ART))
                {
                    var cg_art = stock.CG_ART;
                    var cg_dep = stock.CG_DEP;
                    var despacho = stock.DESPACHO == null ? "" : stock.DESPACHO;
                    var lote = stock.LOTE == null ? "" : stock.LOTE;
                    var serie = stock.SERIE == null ? "" : stock.SERIE;

                    var resumen = await Http
                        .GetFromJsonAsync<vResumenStock>($"api/ResumenStock/GetByStock?CG_ART={cg_art}" +
                        $"&CG_DEP={cg_dep}&DESPACHO={despacho}&LOTE={lote}&SERIE={serie}");


                    stock.ResumenStock = resumen;
                }

            }
        }


        public async Task OnActionComplete(ActionEventArgs<Pedidos> Args)
        {
            Args.PreventRender = false;
            if (Args.RequestType == Syncfusion.Blazor.Grids.Action.Save)
            {
                await Grid.RefreshColumns();
                Grid.Refresh();
                await Grid.RefreshHeader();
            }

        }


        void OnRowRemoving(Pedidos pedido)
        {
            DataSource = DataSource.Where(v => v != pedido).ToList();
            InvokeAsync(StateHasChanged);
        }

        protected string tituloBuscador { get; set; } = "";
        private bool popupBuscadorVisible = false;
        protected bool PopupBuscadorStockVisible { get => popupBuscadorVisible; set { popupBuscadorVisible = value; InvokeAsync(StateHasChanged); } }
        protected bool PopupBuscadorProdVisible { get => popupBuscadorVisible; set { popupBuscadorVisible = value; InvokeAsync(StateHasChanged); } }
        protected string[] ColumnasBuscador = new string[] { "CG_MAT" };
        protected vResumenStock[] Items;

        protected async Task Buscar()
        {

            if (RegistroGenerado.TIPOO == 21) //Ajuste
            {
                await BuscarProductosStock();
            }

            if (RegistroGenerado.TIPOO == 9 || RegistroGenerado.TIPOO == 10 || RegistroGenerado.TIPOO == 6) //Devol y Movim
            {

                await BuscarProductosStockEnPositivo();
            }


        }

        protected async Task BuscarProductos()
        {
            PopupBuscadorStockVisible = false;

            Items = null;
            tituloBuscador = "Listado de Productos";
            PopupBuscadorProdVisible = true;
            ColumnasBuscador = new string[] { "CG_PROD", "DES_PROD", "UNID" };
            //DataSourceProductos = await Http.GetFromJsonAsync<Producto[]>($"api/Productos/GetProductos/{cg_orden}");


            await InvokeAsync(StateHasChanged);
        }

        protected async Task BuscarProductosStock()
        {
            PopupBuscadorProdVisible = false;

            Items = null;
            tituloBuscador = "Listado de Productos en Stock";
            PopupBuscadorStockVisible = true;
            ColumnasBuscador = new string[] { "CG_ART", "PRODUCTO", "DEPOSITO", "DESPACHO", "SERIE", "LOTE", "STOCK" };
            //int cg_orden = TipoInsumoCodigo;
            Items = await Http.GetFromJsonAsync<vResumenStock[]>("api/ResumenStock");

            await InvokeAsync(StateHasChanged);
        }

        protected async Task BuscarProductosStockEnPositivo()
        {
            PopupBuscadorStockVisible = true;
            Items = null;
            tituloBuscador = $"Listado de Insumo {stock.CG_ART} con Stock";

            ColumnasBuscador = new string[] { "CG_ART", "PRODUCTO", "CG_DEP", "DEPOSITO", "DESPACHO", "SERIE", "LOTE", "STOCK" };
            //int cg_orden = TipoInsumoCodigo;
            Items = await Http.GetFromJsonAsync<vResumenStock[]>($"api/ResumenStock/ByCodigo?Codigo={stock.CG_ART.Trim()}" +
                "&Descripcion=");

            Items = Items == null ? new List<vResumenStock>().ToArray() : Items;

            await BuscadorEmergenteRS.ShowAsync();
            //await InvokeAsync(StateHasChanged);
        }


        private async Task onStockSelected(Pedidos obj)
        {
            PopupBuscadorStockVisible = false;
            var tipoEntidad = obj.GetType().Name;
            if (tipoEntidad == "Stock")
            {
                var pedido = (Pedidos)obj;
                //Get items del vale
                var vale = pedido.VALE;
                DataSource = await Http.GetFromJsonAsync<List<Pedidos>>($"api/Stock/AbriVale/{pedido.VALE}");

                //NumeroVale = ItemsVale[0].VALE;
                //Fecha = ItemsVale[0].FE_MOV;
            }

        }



        private async Task BuscarItem()
        {
            await BuscadorProducto.ShowAsync();
        }

        protected async Task OnSelectedItem(Pedidos item)
        {
            stock.CG_ART = item.CG_ART;
            stock.DES_ART = item.DES_ART;
            stock.UNID = item.UNID;
            stock.CG_DEP = 4;
            stock.STOCK = item.STOCK;
            stock.STOCKA = item.STOCKA;
            stock.IMPORTE1 = item.IMPORTE1;


            DataSource.Add(stock);


            await Grid.RefreshColumns();
            Grid.Refresh();
            await Grid.RefreshHeader();
            await BuscadorProducto.HideAsync();
        }

        protected async Task OnResumenStockSelected(List<vResumenStock> lista)
        {
            await BuscadorEmergenteRS.HideAsync();
            foreach (var vresumenStock in lista)
            {
                bAgregarInsumo = true;
                await GetSeleccionResumenStock(vresumenStock);
            }

            await InvokeAsync(StateHasChanged);
        }
        protected async Task OnResumenStockSelected(vResumenStock resumenStock)
        {
            await BuscadorEmergenteRS.HideAsync();

            await GetSeleccionResumenStock(resumenStock);

            
        }

        private async Task GetSeleccionResumenStock(vResumenStock resumenStock)
        {
            //buscar registro mas reciente con las caracteristicas de resumen stock
            var cg_art = resumenStock.CG_ART;
            var cg_dep = resumenStock.CG_DEP;
            var despacho = resumenStock.DESPACHO == null ? "" : resumenStock.DESPACHO;
            var lote = resumenStock.LOTE == null ? "" : resumenStock.LOTE;
            var serie = resumenStock.SERIE == null ? "" : resumenStock.SERIE;

            var registroCompleto = await Http.GetFromJsonAsync<Pedidos>($"api/Stock/GetByResumenStock?CG_ART={cg_art}" +
                $"&CG_DEP={cg_dep}" +
                $"&DESPACHO={despacho}" +
                $"&LOTE={lote}" +
                $"&SERIE={serie}");

            registroCompleto.Id = stock?.Id ?? 0;
            registroCompleto.TIPOO = RegistroGenerado.TIPOO;
            registroCompleto.ResumenStock = resumenStock;

            Grid.PreventRender();
            if (bAgregarInsumo)
            {
                registroCompleto.PEDIDO = DataSource.Count > 0 ? DataSource[0].PEDIDO : 0;
                registroCompleto.CG_ORDF = DataSource.Count > 0 ? DataSource[0].CG_ORDF : 0;
                registroCompleto.STOCK = (decimal?)1.0000;
                registroCompleto.PENDIENTEOC = resumenStock.STOCK;
                registroCompleto.Id = DataSource.Count == 0 ? -1 : DataSource.Min(t => t.Id) - 1;
                DataSource.Add(registroCompleto);
                await Grid.RefreshColumns();
                await Grid.RefreshHeader();
                Grid.Refresh();
                bAgregarInsumo = false;
            }
            else
            {
                stock.PEDIDO = DataSource.Count > 0 ? DataSource[0].PEDIDO : 0;
                stock.CG_ORDF = DataSource.Count > 0 ? DataSource[0].CG_ORDF : 0;
                stock.ResumenStock = resumenStock;
                stock.PENDIENTEOC = resumenStock.STOCK - stock.STOCK;
                stock.CG_DEP = resumenStock.CG_DEP;
                stock.SERIE = resumenStock.SERIE;
                stock.DESPACHO = resumenStock.DESPACHO;
                stock.LOTE = resumenStock.LOTE;
            }
        }

        protected void CantChange()
        {
            if (stock.TIPOO == 10 || stock.TIPOO == 27 || stock.TIPOO == 28) //entregas con o sin of y entrega OA
            {
                stock.PENDIENTEOC = stock.ResumenStock.STOCK - stock.STOCK;
            }

            if (stock.TIPOO == 21) //ajuste inventario
            {
                if (stock.STOCK < 0)
                {
                    stock.PENDIENTEOC = stock.ResumenStock.STOCK - (stock.STOCK * -1);
                }
                if (stock.STOCK > 1)
                {
                    stock.PENDIENTEOC = stock.ResumenStock.STOCK + stock.STOCK;
                }
            }
            
        }


        public async Task QueryCellInfoHandler(QueryCellInfoEventArgs<Pedidos> args)
        {
            args.PreventRender = true;
            if (args.Data.TIPOO == 10 || args.Data.TIPOO == 28)
            {
                args.Data.STOCKA = Math.Round((decimal)(args.Data.STOCK / args.Data.CG_DEN), 4);
                if (args.Data.ResumenStock?.STOCK < args.Data.STOCK)
                {
                    args.Cell.AddClass(new string[] { "sin-stock" });
                }
            }
            
        }
    }
}


