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
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.LinearGauge;
using Syncfusion.Blazor.Navigations;
using Syncfusion.Blazor.Notifications;
using Action = Syncfusion.Blazor.Grids.Action;

namespace SupplyChain.Client.Pages.EstadoCompras;

public class EstadoComprasBase : ComponentBase
{
    protected const string APPNAME = "grdEstadoCompras";
    protected vESTADOS_COMPRAS ComprasSeleccionado = new();
    protected List<vESTADOS_COMPRAS> DataEstadosCompras = new();
    protected SfGrid<vESTADOS_COMPRAS> refSfGrid;
    protected bool SpinnerVisible;
    protected string state;

    protected SfToast ToastObj;

    protected List<object> Toolbaritems = new()
    {
        "Search",
        new ItemModel
            { Text = "Excel Export", TooltipText = "Excel Export", PrefixIcon = "e-excelexport", Id = "Excel Export" },
        new ItemModel { Text = "Ver Todos", Id = "Todos" },
        new ItemModel { Text = "Ver Pendiente", Id = "Pendiente" }
    };

    protected bool VisibleDialog;
    [Inject] protected IRepositoryHttp Http { get; set; }
    [Inject] protected PdfService PdfService { get; set; }
    [Inject] protected IJSRuntime JS { get; set; }

    [Inject] protected EstadoComprasService EstadoComprasService { get; set; }

    // [Inject] protected EstadoComprasController EstadoComprasController { get; set; }
    [CascadingParameter] public MainLayout MainLayout { get; set; }

    public async Task OnVistaSeleccionada(VistasGrillas vistasGrillas)
    {
        await refSfGrid.SetPersistData(vistasGrillas.Layout);
    }

    public async Task OnReiniciarGrilla()
    {
        await refSfGrid.ResetPersistData();
    }


    protected override async Task OnInitializedAsync()
    {
        MainLayout.Titulo = "Estado de Compras";
        SpinnerVisible = true;
        await GetCompras(SupplyChain.Shared.Enum.EstadoCompras.TodosPendientes);
        SpinnerVisible = false;
    }

    public async Task BeginHandler(ActionEventArgs<vESTADOS_COMPRAS> args)
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

    protected async Task OnRowSelected(RowSelectEventArgs<vESTADOS_COMPRAS> arg)
    {
        arg.PreventRender = true;
    }

    protected async Task OnRowSelectedDouble_Click(RecordDoubleClickEventArgs<vESTADOS_COMPRAS> arg)
    {
        arg.PreventRender = true;
        ComprasSeleccionado = arg.RowData;
        VisibleDialog = true;
    }

    public async Task ClickHandler(ClickEventArgs args)
    {
        if (args.Item.Text == "Seleccionar Columnas") await refSfGrid.OpenColumnChooser(200, 50);
        if (args.Item.Text == "Exportar grilla en Excel") await refSfGrid.ExcelExport();
    }
    //  1.Solicitar cotizacion
    //2.Pendiente de generar compra
    //3.A la esperad de cotizacion
    //4.Pendiente de entrega
    //5.Pagada
    //6.Vencida
    //7.Cerrada

    public void QueryCellInfoHandler(QueryCellInfoEventArgs<vESTADOS_COMPRAS> args)
    {
        if (args.Data.ESTADOS_COMPRA == "Pendiente Em.Sol Cotizacion") //SOLICITAR COTIZACION
            args.Cell.AddClass(new[] { "color-estado-compra-pendiente-emitir-solcot" });
        else if (args.Data.ESTADOS_COMPRA == "Pendiente Emision OC") //PENDIENTE A COTIZACION
            args.Cell.AddClass(new[] { "color-estado-compra-pendiente-emision-oc" });
        else if (args.Data.ESTADOS_COMPRA == "Pendiente Ent. en Fecha")
            args.Cell.AddClass(new[] { "color-estado-compra-pendiente-entrega-fecha" });
        else if (args.Data.ESTADOS_COMPRA == "Pendiente Ent.Vencida")
            args.Cell.AddClass(new[] { "color-estado-compra-pendiente-entrega-vencida" });
        //else if (args.Data.ESTADOS_COMPRA == "Entrega Parcial")
        //{
        //    args.Cell.AddClass(new string[] { "color-estado-entrega-parcial" });
        //}
        else if (args.Data.ESTADOS_COMPRA == "Recibida Parcial-Pendiente Pago") //PENDIENTE DE ENTREGA
            args.Cell.AddClass(new[] { "color-estado-compra-recibida-parcial-pendiente-pago" });
        else if (args.Data.ESTADOS_COMPRA == "Recibida Total-Pendiente Pago") //VENCIDA
            args.Cell.AddClass(new[] { "color-estado-compra-recibida-total-pendiente-pago" });
        else if (args.Data.ESTADOS_COMPRA == "Pagada-Recibida") //cerrada
            args.Cell.AddClass(new[] { "color-estado-compra-pagada-recibida" });

        else if (args.Data.ESTADOS_COMPRA == "Cerrada") //PENDIENTE DE PAGO
            args.Cell.AddClass(new[] { "color-estado-compra-cerrada" });
        //else if (args.Data.ESTADOS_COMPRA == 8) //PAGADA  
        //{
        //    args.Cell.AddClass(new string[] { "color-estado-compra-pagada" });
        //}
        //else if (args.Data.ESTADOS_COMPRA == 9)//ANULADA
        //{
        //    args.Cell.AddClass(new string[] { "color-estado-compra-anulada" });
        //}
    }

    protected void AxisLabelChange(AxisLabelRenderEventArgs args)

    {
        //  1.Solicitar cotizacion
        //2.Pendiente de cotizacion
        //3.Pendiente de autorizar compra
        //4.Pendiente de entrega
        //5.Vencida
        //6.Cerrada
        //7.Pendiente de Pago
        //8.Pagada(tiene recibo
        //9.Anulada

        if (args.Text == "1")
            args.Text = "Pendiente Em.Sol Cotizacion";
        else if (args.Text == "2")
            args.Text = "Pendiente Emision OC";
        else if (args.Text == "3")
            args.Text = "Pendiente Ent. en Fecha";
        else if (args.Text == "4")
            args.Text = "Pendiente Ent.Vencida";
        else if (args.Text == "5")
            args.Text = "Recibida Parcial-Pendiente Pago";
        else if (args.Text == "6")
            args.Text = "Recibida Total-Pendiente Pago";
        else if (args.Text == "7")
            args.Text = "Pagada-Recibida";
        else if (args.Text == "8")
            args.Text = "Cerrada";
        else
            args.Text = " ";
    }

    public async Task CommandClickHandler(CommandClickEventArgs<vESTADOS_COMPRAS> args)
    {
        if (args.CommandColumn.ID == "btnDescargarRemito") await DescargarRemito(args.RowData);
        if (args.CommandColumn.ID == "btnDescargarFactura") await DescargarFactura(args.RowData);
    }

    private async Task DescargarRemito(vESTADOS_COMPRAS compras)
    {
        var responseFactura = await Http
            .GetFromJsonAsync<List<Archivo>>(
                $"api/AdministracionArchivosCompras/ByParamRuta/RUTAREMITO/{compras.REMITO}");
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
        }
    }

    private async Task DescargarFactura(vESTADOS_COMPRAS compras)
    {
        var formatoFacturaBuscar = compras.LETRA_FACTURA + compras.FACTURA;
        var responseFactura = await Http
            .GetFromJsonAsync<List<Archivo>>(
                $"api/AdministracionArchivosCompras/ByParamRuta/RUTAFACTURA/{formatoFacturaBuscar}");
        if (responseFactura.Error)
        {
            Console.WriteLine("ERROR AL OBTENER LA FACTURA");
            Console.WriteLine(await responseFactura.HttpResponseMessage.Content.ReadAsStringAsync());
        }
        else
        {
            if (responseFactura.Response.Count > 0)
            {
                await JS.SaveAs(responseFactura.Response[0].Nombre, responseFactura.Response[0].ContenidoByte);
            }
        }
    }

    public void RowBound(RowDataBoundEventArgs<vESTADOS_COMPRAS> Args)
    {
        if (string.IsNullOrEmpty(Args.Data.REMITO)) Args.Row.AddClass(new[] { "e-removedEditCommand" });
        if (string.IsNullOrEmpty(Args.Data.FACTURA)) Args.Row.AddClass(new[] { "e-removedDeleteCommand" });
    }

    protected async Task GetCompras(
        SupplyChain.Shared.Enum.EstadoCompras estadoCompras = SupplyChain.Shared.Enum.EstadoCompras.Todos)
    {
        var response = await EstadoComprasService.ByEstado(estadoCompras);
        if (response.Error)
        {
        }
        else
        {
            DataEstadosCompras = response.Response.OrderByDescending(P => P.ESTADOS_COMPRA).ToList();
        }
    }

    protected async Task OnToolBarHandler(ClickEventArgs args)
    {
        {
            if (args.Item.Id == "Excel Export")
            {
                await refSfGrid.ExportToExcelAsync();
            }
            else if (args.Item.Id == "Pendiente")
            {
                SpinnerVisible = true;
                await GetCompras(SupplyChain.Shared.Enum.EstadoCompras.TodosPendientes);

                SpinnerVisible = false;
            }
            else if (args.Item.Id == "Todos")
            {
                SpinnerVisible = true;
                await GetCompras();
                SpinnerVisible = false;
            }
        }
    }
}