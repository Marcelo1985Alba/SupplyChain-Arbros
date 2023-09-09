using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using SupplyChain.Client.Shared;
using SupplyChain.Shared;
using Syncfusion.Blazor.Charts;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Spinner;

namespace SupplyChain.Client.Pages.PCP.Unidades_Equivalentes;

public class BaseEstadUnidEquiv : ComponentBase
{
    protected int AñoMax = DateTime.Now.Year;
    protected SfGrid<vEstadPedidosIngresados> grdPedIngresados;
    protected int PromedioPedidosAltaMensuales;
    protected int PromedioPedidosIngresadosMensuales;
    protected int PromedioPedidosingresadosPrev = 0;
    protected SfChart refChartDetalle;

    protected SfChart refChartDetallePedidosAlta;


    protected SfChart refChartDetallePedidosIngresadosPrev;


    protected SfSpinner refSpinner;
    protected string SerieSelecciona = "";
    protected string SerieSeleccionaingresadosPrev = "";
    protected string SerieSeleccionaPedidosAlta = "";
    protected string TituloGraficoMensual = "";
    protected string TituloGraficoMensualPedidosAlta = "";
    protected string TituloGraficoMensualPedidosingresadosPrev = "";
    protected bool VisibleSpinner;
    [Inject] public HttpClient Http { get; set; }
    [CascadingParameter] public MainLayout MainLayout { get; set; }

    public List<vEstadPedidosAlta> DataPedidosAltas { get; set; }
    public List<vEstadPedidosIngresados> DataPedidosIngresados { get; set; }
    public List<vEstadPedidosIngresados> DataPedidosPendientes { get; set; } = new();
    protected List<ChartData> PedidosIngresadosAnuales { get; set; } = new();


    protected List<ChartData> PedidosIngresadosMensuales { get; set; } = new();
    protected List<vEstadPedidosIngresados> PedidosIngresadosAnualesDetalle { get; set; } = new();
    protected List<ChartData> PedidosAltasAnuales { get; set; } = new();
    protected List<ChartData> PedidosAltasMensuales { get; set; } = new();
    protected List<vEstadPedidosAlta> PedidosAltaAnualesDetalle { get; set; } = new();
    protected List<ChartData> PedidosPendientesPrevMensuales { get; set; } = new();
    protected List<ChartData> PedidosIngresadosPrevSemanales { get; set; } = new();

    protected List<vEstadPedidosIngresados> PedidosIngresadosPrevDetalle { get; set; } = new();

    protected override async Task OnInitializedAsync()
    {
        MainLayout.Titulo = "Unidades Equivalentes";
        VisibleSpinner = true;

        await GetPedidosIngresados();
        await GetPedidosAltas();

        VisibleSpinner = false;
    }


    protected async Task GetPedidosIngresados()
    {
        DataPedidosIngresados =
            await Http.GetFromJsonAsync<List<vEstadPedidosIngresados>>("api/EstadisticaVentas/PedidosIngresados");

        DataPedidosPendientes = DataPedidosIngresados
            .Where(p => p.ESTADO && p.FECHA > new DateTime(2021, 8, 1)).ToList();

        // pedidos pendientes por fecha prevista
        PedidosPendientesPrevMensuales = DataPedidosPendientes //pedidos pendiendos
            .OrderBy(c => c.MES_PREV)
            .GroupBy(g => new { g.MES_PREV })
            .Select(d => new ChartData
            {
                XSerieName = d.Key.MES_PREV.ToString(),
                YSerieName = Convert.ToDouble(d.Sum(p => p.UNIDEQUI))
            })
            //.OrderBy(c => c.XSerieName)
            .ToList();


        PedidosIngresadosAnuales = DataPedidosIngresados.GroupBy(g => new { g.ANIO })
            .Select(d => new ChartData
            {
                XSerieName = d.Key.ANIO.ToString(),
                YSerieName = Convert.ToDouble(d.Sum(p => p.UNIDEQUI))
            }).OrderBy(c => c.XSerieName)
            .ToList();
    }

    protected async Task GetPedidosAltas()
    {
        DataPedidosAltas = await Http.GetFromJsonAsync<List<vEstadPedidosAlta>>("api/EstadisticaVentas/PedidosAltas");

        PedidosAltasAnuales = DataPedidosAltas.GroupBy(g => new { g.ANIO })
            .Select(d => new ChartData
            {
                XSerieName = d.Key.ANIO.ToString(),
                YSerieName = Convert.ToDouble(d.Sum(p => p.UNIDEQUI))
            }).OrderBy(c => c.XSerieName)
            .ToList();
    }

    protected async Task MostrarDetalle(PointEventArgs args)
    {
        var año = args.Point.X;
        SerieSelecciona = args.Series.Name;
        TituloGraficoMensual = $"Unidades Equivalentes Mensuales {año}";

        PedidosIngresadosAnualesDetalle = DataPedidosIngresados.Where(p => p.ANIO == Convert.ToInt32(año)).ToList();


        PedidosIngresadosMensuales = DataPedidosIngresados
            .Where(v => v.ANIO == Convert.ToInt32(año))
            .OrderBy(o => o.MES)
            .GroupBy(g => new { g.MES }).Select(d => new ChartData
            {
                XSerieName = d.Key.MES.ToString(),
                YSerieName = Math.Round(Convert.ToDouble(d.Sum(p => p.UNIDEQUI)))
            }).ToList();


        PromedioPedidosIngresadosMensuales = Convert.ToInt32(PedidosIngresadosMensuales.Average(p => p.YSerieName));
        //await grdPedIngresados.AutoFitColumnsAsync();
        await InvokeAsync(StateHasChanged);
        await refChartDetalle.RefreshAsync();
        await refChartDetalle.RefreshAsync();
    }

    protected async Task MostrarDetallePedidosAlta(PointEventArgs args)
    {
        var año = args.Point.X;
        SerieSeleccionaPedidosAlta = args.Series.Name;
        TituloGraficoMensualPedidosAlta = $"Unidades Equivalentes Mensual {año}";

        PedidosAltaAnualesDetalle = DataPedidosAltas.Where(p => p.ANIO == Convert.ToInt32(año)).ToList();

        PedidosAltasMensuales = DataPedidosAltas
            .Where(v => v.ANIO == Convert.ToInt32(año))
            .OrderBy(o => o.MES)
            .GroupBy(g => new { g.MES }).Select(d => new ChartData
            {
                XSerieName = d.Key.MES.ToString(),
                YSerieName = Math.Round(Convert.ToDouble(d.Sum(p => p.UNIDEQUI)))
            }).ToList();

        PromedioPedidosAltaMensuales = Convert.ToInt32(PedidosAltasMensuales.Average(p => p.YSerieName));

        await InvokeAsync(StateHasChanged);
        await refChartDetallePedidosAlta.RefreshAsync();
        await refChartDetallePedidosAlta.RefreshAsync();
    }

    protected async Task MostrarDetallePrev(PointEventArgs args)
    {
        var mes = args.Point.X;
        SerieSeleccionaPedidosAlta = args.Series.Name;
        TituloGraficoMensualPedidosingresadosPrev = $"Unidades Equivalentes Semanal del Mes {mes}";

        PedidosIngresadosPrevDetalle = DataPedidosPendientes.Where(p => p.MES_PREV == Convert.ToInt32(mes)).ToList();

        PedidosIngresadosPrevSemanales = DataPedidosPendientes
            .Where(v => v.MES_PREV == Convert.ToInt32(mes))
            .OrderBy(o => o.SEMANA_PREV)
            .GroupBy(g => new { g.SEMANA_PREV }).Select(d => new ChartData
            {
                XSerieName = d.Key.SEMANA_PREV.ToString(),
                YSerieName = Math.Round(Convert.ToDouble(d.Sum(p => p.UNIDEQUI)))
            }).ToList();

        //PromedioPedidosAltaMensuales = Convert.ToInt32(PedidosAltasMensuales.Average(p => p.YSerieName));

        await InvokeAsync(StateHasChanged);
        await refChartDetallePedidosIngresadosPrev.RefreshAsync();
        await refChartDetallePedidosIngresadosPrev.RefreshAsync();
    }


    public class ChartData
    {
        public string XSerieName { get; set; }
        public double YSerieName { get; set; }
    }
}