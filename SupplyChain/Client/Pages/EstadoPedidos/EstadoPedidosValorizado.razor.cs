using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SupplyChain.Client.HelperService;
using SupplyChain.Client.RepositoryHttp;
using SupplyChain.Client.Shared;
using SupplyChain.Shared;
using SupplyChain.Shared.PCP;
using Syncfusion.Blazor.DropDowns;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.LinearGauge;
using Syncfusion.Blazor.Navigations;
using Syncfusion.Blazor.Notifications;
using Syncfusion.Pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Syncfusion.Pdf.Parsing;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Grid;
using System.IO;
using Microsoft.AspNetCore.Mvc;

namespace SupplyChain.Client.Pages.EstadoPedidos
{
    public class EstadoPedidosValorizadoBase : ComponentBase
    {
        [Inject] protected IRepositoryHttp Http { get; set; }
        [Inject] protected PdfService PdfService { get; set; }
        [Inject] protected IJSRuntime JS { get; set; }
        [Inject] protected EstadoPedidoService EstadoPedidoService { get; set; }
        [CascadingParameter] public MainLayout MainLayout { get; set; }

        protected SfToast ToastObj;
        protected SfGrid<vEstadoPedido> refSfGrid;
        protected List<vEstadoPedido> DataEstadoPedidoValorizado = new();
        protected vEstadoPedido PedidoSeleccionadoValorizado = new();
        protected bool SpinnerVisible = false;

        protected bool VisibleDialog = false;
        protected List<Object> Toolbaritems = new List<Object>()
        {
            "Search",
        new ItemModel { Text = "Ver Todos", TooltipText = "Seleccionar Columnas", Id = "VerTodos" },
        new ItemModel { Text = "Ver Pendientes", TooltipText = "Ver Pendientes", Id = "VerPendiente" },
        new ItemModel { Text = "Exportar Excel", TooltipText = "Exportar Excel", Id = "ExportarExcel" }
          };

        protected const string APPNAME = "grdEstadoPedidosValorizado";
        protected string state;

        protected async override Task OnInitializedAsync()
        {
            MainLayout.Titulo = "Estados de Pedidos Valorizado";
            SpinnerVisible = true;
            await GetPedidos(SupplyChain.Shared.Enum.EstadoPedido.TodosPendientes);
            var response = await Http.GetFromJsonAsync<List<vEstadoPedido>>
                ("api/EstadoPedidos");
            if (response.Error)
            {
                Console.WriteLine("E R R O R !!!");
                Console.WriteLine(response.HttpResponseMessage.ReasonPhrase);
            }
            else
            {
                DataEstadoPedidoValorizado = response.Response;
                refSfGrid?.PreventRender();
            }
            SpinnerVisible = false;
        }

        public void RowBound(RowDataBoundEventArgs<vEstadoPedido> Args)
        {
            if (string.IsNullOrEmpty(Args.Data.REMITO))
            {
                Args.Row.AddClass(new string[] { "e-removeEditcommand" });
            }
            if (string.IsNullOrEmpty(Args.Data.FACTURA))
            {
                Args.Row.AddClass(new string[] { "e-removeDeletedcommand" });
            }
        }

        public async Task CommandClickHandler(CommandClickEventArgs<vEstadoPedido> args)
        {
            if (args.CommandColumn.ID == "btnDescargarRemito")
            {
                await descargarRemito(args.RowData);
            }
            if (args.CommandColumn.ID == "btnDescargarFactura")
            {
                await descargarFactura(args.RowData);
            }
            if (args.CommandColumn.ID == "btnDescargarCertificado")
            {
                await descargarCertificado(args.RowData.PEDIDO);
            }
        }

        private async Task descargarCertificado(int pedido)
        {
            await JS.InvokeVoidAsync("descargarCertificado", pedido);
        }

        private async Task descargarRemito(vEstadoPedido pedido)
        {
            var responseFactura = await Http.GetFromJsonAsync<List<Archivo>>($"api/AdministracionArchivos/ByParamRuta/RUTAREMITO/{pedido.REMITO}");
            if (responseFactura.Error)
            {
                Console.WriteLine("ERROR AL OBTENER REMITO");
                Console.WriteLine(await responseFactura.HttpResponseMessage.Content.ReadAsStringAsync());
            }
            else
            {
                if (responseFactura.Response.Count > 0)
                {
                    await JS.SaveAs(responseFactura.Response[0].Nombre, responseFactura.Response[0].ContenidoByte);
                }
                else
                {

                }
            }
        }

        private async Task descargarFactura(vEstadoPedido pedido)
        {
            var formatoFacturaBuscar = pedido.LETRA_FACTURA + pedido.FACTURA;
            var responseFactura = await Http.GetFromJsonAsync<List<Archivo>>($"api/AdministracionArchivos/ByParamRuta/RUTAFACTURA/{formatoFacturaBuscar}");
            if (responseFactura.Error)
            {
                Console.WriteLine("ERROR AL OBTENER FACTURA");
                Console.WriteLine(await responseFactura.HttpResponseMessage.Content.ReadAsStringAsync());
            }
            else
            {
                if (responseFactura.Response.Count > 0)
                {
                    await JS.SaveAs(responseFactura.Response[0].Nombre, responseFactura.Response[0].ContenidoByte);
                }
                else
                {

                }
            }

        }

        public void QueryCellInfoHandler(QueryCellInfoEventArgs<vEstadoPedido> args)
        {
            if (args.Data.ESTADO_PEDIDO == 1)
            {
                args.Cell.AddClass(new string[] { "color-estado-pedido-a-confirmar" });
            }
            else if (args.Data.ESTADO_PEDIDO == 2) /*"PEDIDO CONFIRMADO"*/
            {
                args.Cell.AddClass(new string[] { "color-estado-pedido-confirmado" });
            }
            else if (args.Data.ESTADO_PEDIDO == 3) /*"EN PRODUCCION"*/
            {
                args.Cell.AddClass(new string[] { "color-estado-pedido-produccion" });
            }
            else if (args.Data.ESTADO_PEDIDO == 4) /*"CON TOTALIDAD DE COMPONENTES"*/
            {
                args.Cell.AddClass(new string[] { "color-estado-pedido-componentes" });
            }
            else if (args.Data.ESTADO_PEDIDO == 5) /*"ARMADO Y CALIBRACION"*/
            {
                args.Cell.AddClass(new string[] { "color-estado-pedido-armado" });
            }
            else if (args.Data.ESTADO_PEDIDO == 6) /*"PENDIENTE DE REMITIR"*/
            {
                args.Cell.AddClass(new string[] { "color-estado-pedido-pendiente-remitir" });
            }
            else if (args.Data.ESTADO_PEDIDO == 7) /*"A ENTREGAR"*/
            {
                args.Cell.AddClass(new string[] { "color-estado-pedido-entregar" });
            }
            else if (args.Data.ESTADO_PEDIDO == 8) /*"ENTREGADO"*/
            {
                args.Cell.AddClass(new string[] { "color-estado-pedido-entregado" });
            }
            else if (args.Data.ESTADO_PEDIDO == 9) /*"FACTURADO"*/
            {
                args.Cell.AddClass(new string[] { "color-estado-pedido-facturado" });
            }
            else if (args.Data.ESTADO_PEDIDO == 10) /*ANULADO*/
            {
                args.Cell.AddClass(new string[] { "color-estado-pedido-anulado" });
            }
        }

        protected async Task OnRowSelected(RowSelectEventArgs<vEstadoPedido> arg)
        {
            arg.PreventRender = true;

        }

        protected async Task OnRowSelectedDouble_Click(RecordDoubleClickEventArgs<vEstadoPedido> arg)
        {
            arg.PreventRender = true;
            PedidoSeleccionadoValorizado = arg.RowData;
            VisibleDialog = true;
        }

        public async Task OnVistaSeleccionada(VistasGrillas vistasGrillas)
        {
            await refSfGrid.SetPersistData(vistasGrillas.Layout);
        }
        public async Task OnReiniciarGrilla()
        {
            await refSfGrid.ResetPersistData();
        }
        public async Task BeginHandler(ActionEventArgs<vEstadoPedido> args)
        {
            if(args.RequestType==Syncfusion.Blazor.Grids.Action.Grouping ||
                args.RequestType== Syncfusion.Blazor.Grids.Action.UnGrouping ||
                args.RequestType==Syncfusion.Blazor.Grids.Action.ClearFiltering ||
                args.RequestType== Syncfusion.Blazor.Grids.Action.CollapseAllComplete ||
                args.RequestType==Syncfusion.Blazor.Grids.Action.ColumnState ||
                args.RequestType == Syncfusion.Blazor.Grids.Action.ClearFiltering ||
                args.RequestType == Syncfusion.Blazor.Grids.Action.Reorder ||
                args.RequestType == Syncfusion.Blazor.Grids.Action.Sorting
                )
            {
                refSfGrid.PreventRender();
                refSfGrid.Refresh();

                state = await refSfGrid.GetPersistData();
                await refSfGrid.AutoFitColumnsAsync();
                await refSfGrid.RefreshColumns();
                await refSfGrid.RefreshHeader();
            }
        }
        protected async Task GetPedidos(SupplyChain.Shared.Enum.EstadoPedido estadoPedido = SupplyChain.Shared.Enum.EstadoPedido.Todos)
        {
            var response = await EstadoPedidoService.ByEstado(estadoPedido);
            if (response.Error)
            {

            }
            else
            {
                DataEstadoPedidoValorizado = response.Response.OrderByDescending(p => p.ESTADO_PEDIDO).ToList();
            }
        }

        public async Task ClickHandler(Syncfusion.Blazor.Navigations.ClickEventArgs args)
        {
            if(args.Item.Text == "Seleccionar Columnas")
            {
                await refSfGrid.OpenColumnChooser(200, 50);
            }
            else if(args.Item.Id == "VerTodos"){
                SpinnerVisible = true;
                await GetPedidos();
                SpinnerVisible = false;
            }
            else if (args.Item.Id == "VerPendiente")
            {
                SpinnerVisible = true;
                await GetPedidos(SupplyChain.Shared.Enum.EstadoPedido.TodosPendientes);
                SpinnerVisible = false;
            }
            else if (args.Item.Id == "ExportarExcel")
            {
                await refSfGrid.ExportToExcelAsync();
            }
        }

        protected void AxisLabelChange(AxisLabelRenderEventArgs args)
        {
            if (args.Text == "1")
                args.Text = "PEDIDO A CONFIRMAR";
            else if (args.Text == "2")
                args.Text = "PEDIDO CONFIRMADO";
            else if (args.Text == "3")
                args.Text = "EN PROCESO";
            //else if (args.Text == "4")
            //    args.Text = "CON TOTALIDAD DE COMPONENTES";
            else if (args.Text == "4")
                args.Text = "ARMADO Y PRUEBA";
            else if (args.Text == "5")
                args.Text = "A REMITIR";
            else if (args.Text == "6")
                args.Text = "A ENTREGAR";
            else if (args.Text == "7")
                args.Text = "ENTREGADO";
            //else if (args.Text == "9")
            //    args.Text = "FACTURADO";
            else
                args.Text = " ";
        }

    }

}