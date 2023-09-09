using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SupplyChain.Client.HelperService;
using SupplyChain.Client.RepositoryHttp;
using SupplyChain.Client.Shared;
using SupplyChain.Shared;
using SupplyChain.Shared.Enum;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.LinearGauge;
using Syncfusion.Blazor.Navigations;
using Syncfusion.Blazor.Notifications;
using Action = Syncfusion.Blazor.Grids.Action;

namespace SupplyChain.Client.Pages.EstadoPedidos;

public class EstadoPedidosBase : ComponentBase
{
    //new
    protected const string APPNAME = "grdEstadosPedidos";
    protected List<vEstadoPedido> DataEstadosPedidos = new();
    protected vEstadoPedido PedidoSeleccionado = new();
    protected SfGrid<vEstadoPedido> refSfGrid;
    protected bool SpinnerVisible;
    protected string state;
    protected SfToast ToastObj;

    protected List<object> Toolbaritems = new()
    {
        "Search",
        new ItemModel { Text = "Ver Todos", TooltipText = "Seleccionar Columnas", Id = "VerTodos" },
        new ItemModel { Text = "Ver Pendientes", TooltipText = "Ver Pendientes", Id = "VerPendiente" }
    };

    protected bool VisibleDialog;
    [Inject] protected IRepositoryHttp Http { get; set; }
    [Inject] protected PdfService PdfService { get; set; }
    [Inject] protected EstadoPedidoService EstadoPedidoService { get; set; }
    [Inject] protected IJSRuntime JS { get; set; }
    [CascadingParameter] public MainLayout MainLayout { get; set; }

    protected override async Task OnInitializedAsync()
    {
        MainLayout.Titulo = "Estados de Pedidos";
        SpinnerVisible = true;
        await GetPedidos(EstadoPedido.TodosPendientes);

        var response = await Http.GetFromJsonAsync<List<vEstadoPedido>>
            ("api/EstadoPedidos");
        if (response.Error)
        {
            Console.WriteLine("E R R O R !!!");
            Console.WriteLine(response.HttpResponseMessage.ReasonPhrase);
        }
        else
        {
            DataEstadosPedidos = response.Response;
            refSfGrid?.PreventRender();
            //await refSfGrid?.AutoFitColumnsAsync();
        }

        SpinnerVisible = false;
    }

    public void RowBound(RowDataBoundEventArgs<vEstadoPedido> Args)
    {
        if (string.IsNullOrEmpty(Args.Data.REMITO)) Args.Row.AddClass(new[] { "e-removeEditcommand" });

        if (string.IsNullOrEmpty(Args.Data.FACTURA)) Args.Row.AddClass(new[] { "e-removeDeletecommand" });
    }

    public async Task CommandClickHandler(CommandClickEventArgs<vEstadoPedido> args)
    {
        if (args.CommandColumn.ID == "btnDescargarRemito") await DescargarRemito(args.RowData);

        if (args.CommandColumn.ID == "btnDescargarFactura") await DescargarFactura(args.RowData);

        if (args.CommandColumn.ID == "btnDescargarCertificado") await DescargarCertificado(args.RowData.PEDIDO);
        /*
            //OBTENGO TRAZABILIDAD PARA PODER OBTENER LOS CERTIFICADO DE MP
            var listTrazab = new List<vTrazabilidad>();
            var listArchivosDescargar = new List<Archivo>();
            var responseTrazab = await Http.GetFromJsonAsync<List<vTrazabilidad>>($"api/Trazabilidads/MostrarTrazabilidad/{pedido}");
            if (responseTrazab.Error)
            {
                Console.WriteLine("ERROR AL OBTENER TRAZABILIDAD");
                Console.WriteLine(await responseTrazab.HttpResponseMessage.Content.ReadAsStringAsync());
            }
            else
            {
                listTrazab = responseTrazab.Response;
            }

            if (listTrazab.Count > 0 )
            {
                var lineasRoscada = new List<int>(new int[] { 8, 18, 52, 23 });
                var lineasBridada = new List<int>(new int[] { 8, 18, 23, 19 });
                List<vTrazabilidad> lineasCertif = new();
                var producto = listTrazab.FirstOrDefault(t => t.TIPOO == 1).CG_ART;
                if (producto.StartsWith("00"))//reparacion
                {

                }
                else if (producto.StartsWith("1")) //roscada
                {
                    lineasCertif = listTrazab.Where(t => lineasRoscada.Contains((int)t.CG_LINEA)).ToList();
                }
                else if (producto.StartsWith("2")) //bridada
                {
                    lineasCertif = listTrazab.Where(t=> lineasBridada.Contains((int)t.CG_LINEA)).ToList();
                }

                if (lineasCertif.Count > 0)
                {
                    foreach (var item in lineasCertif)
                    {
                        var responseMp = await Http
                            .GetFromJsonAsync<List<Archivo>>($"api/AdministracionArchivos/ByParamRuta/RUTATRAZABILIDAD/{item.DESPACHO}.pdf");
                        if (responseMp.Error)
                        {
                            Console.WriteLine("ERROR AL OBTENER CERTIFICADOS DE MATERIA PRIMA");
                            Console.WriteLine(await responseMp.HttpResponseMessage.Content.ReadAsStringAsync());
                        }
                        else
                        {
                            if (responseMp.Response.Count > 0)
                            {
                                listArchivosDescargar.Add(responseMp.Response[0]);
                            }

                        }
                    }
                }
            }

            var response = await Http
                .GetFromJsonAsync<List<Archivo>>($"api/AdministracionArchivos/ByParamRuta/RUTACERTIFICADOS/{pedido}");

            if (response.Error)
            {
                Console.WriteLine("ERROR AL OBTENER CERTIFICADOS DE PRODUCTO");
                Console.WriteLine(await response.HttpResponseMessage.Content.ReadAsStringAsync());
            }
            else
            {
                foreach (var item in response.Response)
                {
                    listArchivosDescargar.Add(item);
                }
                foreach (Archivo item in listArchivosDescargar)
                {
                    await JS.SaveAs(item.Nombre, item.ContenidoByte);
                }

            }
            */
    }

    private async Task DescargarCertificado(int pedido)
    {
        await JS.InvokeVoidAsync("descargarCertificado", pedido);
    }

    private async Task DescargarRemito(vEstadoPedido pedido)
    {
        var responseFactura = await Http
            .GetFromJsonAsync<List<Archivo>>($"api/AdministracionArchivos/ByParamRuta/RUTAREMITO/{pedido.REMITO}");
        if (responseFactura.Error)
        {
            Console.WriteLine("ERROR AL OBTENER REMITO");
            Console.WriteLine(await responseFactura.HttpResponseMessage.Content.ReadAsStringAsync());
        }
        else
        {
            if (responseFactura.Response.Count > 0)
                await JS.SaveAs(responseFactura.Response[0].Nombre, responseFactura.Response[0].ContenidoByte);
        }
    }

    private async Task DescargarFactura(vEstadoPedido pedido)
    {
        var formatoFacturaBuscar = pedido.LETRA_FACTURA + pedido.FACTURA;
        var responseFactura = await Http
            .GetFromJsonAsync<List<Archivo>>(
                $"api/AdministracionArchivos/ByParamRuta/RUTAFACTURA/{formatoFacturaBuscar}");
        if (responseFactura.Error)
        {
            Console.WriteLine("ERROR AL OBTENER FACTURA");
            Console.WriteLine(await responseFactura.HttpResponseMessage.Content.ReadAsStringAsync());
        }
        else
        {
            if (responseFactura.Response.Count > 0)
                await JS.SaveAs(responseFactura.Response[0].Nombre, responseFactura.Response[0].ContenidoByte);
        }
    }

    public void QueryCellInfoHandler(QueryCellInfoEventArgs<vEstadoPedido> args)
    {
        if (args.Data.ESTADO_PEDIDO == 1) /*"PEDIDO A CONFIRMAR"*/
            args.Cell.AddClass(new[] { "color-estado-pedido-a-confirmar" });
        else if (args.Data.ESTADO_PEDIDO == 2) /*"PEDIDO CONFIRMADO"*/
            args.Cell.AddClass(new[] { "color-estado-pedido-confirmado" });
        else if (args.Data.ESTADO_PEDIDO == 3) /*"EN PRODUCCION"*/
            args.Cell.AddClass(new[] { "color-estado-pedido-produccion" });
        else if (args.Data.ESTADO_PEDIDO == 4) /*"CON TOTALIDAD DE COMPONENTES"*/
            args.Cell.AddClass(new[] { "color-estado-pedido-componentes" });
        else if (args.Data.ESTADO_PEDIDO == 5) /*"ARMADO Y CALIBRACION"*/
            args.Cell.AddClass(new[] { "color-estado-pedido-armado" });
        else if (args.Data.ESTADO_PEDIDO == 6) /*"PENDIENTE DE REMITIR"*/
            args.Cell.AddClass(new[] { "color-estado-pedido-pendiente-remitir" });
        else if (args.Data.ESTADO_PEDIDO == 7) /*"A ENTREGAR"*/
            args.Cell.AddClass(new[] { "color-estado-pedido-entregar" });
        else if (args.Data.ESTADO_PEDIDO == 8) /*"ENTREGADO"*/
            args.Cell.AddClass(new[] { "color-estado-pedido-entregado" });
        else if (args.Data.ESTADO_PEDIDO == 9) /*"FACTURADO"*/
            args.Cell.AddClass(new[] { "color-estado-pedido-facturado" });
        else if (args.Data.ESTADO_PEDIDO == 10) /*ANULADO*/
            args.Cell.AddClass(new[] { "color-estado-pedido-anulado" });
        else if (args.Data.ESTADO_PEDIDO == 11) /*ANULADO*/ args.Cell.AddClass(new[] { "color-estado-pedido-cobrado" });
    }

    protected async Task OnRowSelected(RowSelectEventArgs<vEstadoPedido> arg)
    {
        arg.PreventRender = true;
    }

    protected async Task OnRowSelectedDouble_Click(RecordDoubleClickEventArgs<vEstadoPedido> arg)
    {
        arg.PreventRender = true;
        PedidoSeleccionado = arg.RowData;
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
        if (args.RequestType == Action.Grouping ||
            args.RequestType == Action.UnGrouping
            || args.RequestType == Action.ClearFiltering
            || args.RequestType == Action.CollapseAllComplete
            || args.RequestType == Action.ColumnState
            || args.RequestType == Action.ClearFiltering
            || args.RequestType == Action.Reorder ||
            args.RequestType == Action.Sorting
           )
        {
            //VisibleProperty = true;
            refSfGrid.PreventRender();
            refSfGrid.Refresh();

            state = await refSfGrid.GetPersistData();
            await refSfGrid.AutoFitColumnsAsync();
            await refSfGrid.RefreshColumns();
            await refSfGrid.RefreshHeader();
            //VisibleProperty = false;
        }
    }

    protected async Task GetPedidos(EstadoPedido estadoPedido = EstadoPedido.Todos)
    {
        var response = await EstadoPedidoService.ByEstado(estadoPedido);
        if (response.Error)
        {
        }
        else
        {
            DataEstadosPedidos = response.Response.OrderByDescending(p => p.ESTADO_PEDIDO).ToList();
        }
    }

    public async Task ClickHandler(ClickEventArgs args)
    {
        if (args.Item.Text == "Seleccionar Columnas") await refSfGrid.OpenColumnChooser(200, 50);
        if (args.Item.Text == "Exportar grilla en Excel")
        {
            await refSfGrid.ExcelExport();
        }
        else if (args.Item.Id == "VerTodos")
        {
            SpinnerVisible = true;
            await GetPedidos();
            SpinnerVisible = false;
        }
        else if (args.Item.Id == "VerPendiente")
        {
            SpinnerVisible = true;
            await GetPedidos(EstadoPedido.TodosPendientes);
            SpinnerVisible = false;
        }
    }

    protected void AxisLabelChange(AxisLabelRenderEventArgs args)
    {
        //1   PEDIDO A CONFIRMAR
        //2   PEDIDO CONFIRMADO
        //3   EN PROCESO
        //4   CON TOTALIDAD DE COMPONENTES
        //5   A REMITIR
        //6   A ENTREGAR
        //7   ENTREGADO

        if (args.Text == "1")
        {
            args.Text = "PEDIDO A CONFIRMAR";
        }
        else if (args.Text == "2")
        {
            args.Text = $"{PedidoSeleccionado.FE_PED.ToString("dd/MM/yyyy")} PEDIDO CONFIRMADO ";
        }
        else if (args.Text == "3")
        {
            if (PedidoSeleccionado.FE_PLAN.HasValue)
                args.Text = $"{PedidoSeleccionado.FE_PLAN.Value:dd/MM/yyyy} EN PROCESO";
            else
                args.Text = "EN PROCESO";
        }

        //else if (args.Text == "4")
        //    args.Text = "CON TOTALIDAD DE COMPONENTES";
        else if (args.Text == "4")
        {
            if (PedidoSeleccionado.FE_FIRME.HasValue)
                args.Text = $"{PedidoSeleccionado.FE_FIRME.Value:dd/MM/yyyy} ARMADO Y PRUEBA";
            else
                args.Text = "ARMADO Y PRUEBA";
        }

        else if (args.Text == "5")
        {
            if (PedidoSeleccionado.FE_CIERRE.HasValue)
                args.Text = $"{PedidoSeleccionado.FE_CIERRE.Value.ToString("dd/MM/yyyy")} A REMITIR";
            else
                args.Text = "A REMITIR";
        }

        else if (args.Text == "6")
        {
            if (PedidoSeleccionado.FE_REM.HasValue)
                args.Text = $"{PedidoSeleccionado.FE_REM.Value.ToString("dd/MM/yyyy")} A ENTREGAR";
            else
                args.Text = "A ENTREGAR";
        }

        else if (args.Text == "7")
        {
            if (PedidoSeleccionado.FE_REM.HasValue)
                args.Text = $"{PedidoSeleccionado.FE_REM.Value.ToString("dd/MM/yyyy")} ENTREGADO";
            else
                args.Text = "ENTREGADO";
        }

        else if (args.Text == "8")
        {
            if (PedidoSeleccionado.FE_FACT.HasValue)
                args.Text = $"{PedidoSeleccionado.FE_FACT.Value.ToString("dd/MM/yyyy")} FACTURADO";
            else
                args.Text = "FACTURADO";
        }

        else if (args.Text == "9")
        {
            if (PedidoSeleccionado.FE_RECIBO.HasValue)
                args.Text = $"{PedidoSeleccionado.FE_RECIBO.Value.ToString("dd/MM/yyyy")} COBRADO";
            else
                args.Text = "COBRADO";
        }
        else
        {
            args.Text = "";
        }
    }
}