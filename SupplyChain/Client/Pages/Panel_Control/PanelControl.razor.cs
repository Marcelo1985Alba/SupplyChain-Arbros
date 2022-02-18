using Microsoft.AspNetCore.Components;
using SupplyChain.Client.Shared;
using SupplyChain.Shared;
using Syncfusion.Blazor.Charts;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Spinner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SupplyChain.Client.Pages.Panel_Control
{

    public class BasePanelControl : ComponentBase
    {
        [Inject] public HttpClient Http { get; set; }
        [CascadingParameter] public MainLayout MainLayout { get; set; }
        protected SfChart refChartDetalle;
        protected SfSpinner refSpinner;
        protected bool VisibleSpinner = false;

        protected List<vEstadFacturacion> DataFacturacionOriginal = new();
        protected List<vEstadCompras> DataComprasOriginal = new();
        protected List<vEstadFacturacion> DataFacturacionDetalle = new();
        protected List<ChartData> FacturacionAnual= new();
        protected List<ChartData> FacturacionMensual= new();
        protected string TituloGraficoFacturacionMensual = "";
        protected string SerieSeleccionaFacturacion = "";
        protected int PromedioFacturacionMensual = 0;

        protected SfGrid<vEstadCompras> gridDetalleCompras;
        protected SfChart refChartDetalleCompras;
        protected SfAccumulationChart refAccChartDetalleComprasTipo;
        protected SfChart refChartDetalleComprasMesTipo;
        protected List<vEstadCompras> DataComprasDetalle = new();
        protected List<ChartData> ComprasAnual = new();
        protected List<ChartData> ComprasMensual = new();
        protected List<ChartData> ComprasMensualTipo = new();
        protected string TituloGraficoComprasMensual = "";
        protected string SerieSeleccionaCompras = "";
        protected int PromedioComprasMensual = 0;

        protected SfChart refChartDetallePedidos;
        protected SfGrid<vEstadPedidosIngresados> grdPedIngresados;
        protected List<vEstadPedidosIngresados> DataPedidosIngresados { get; set; } = new ();
        protected List<vEstadPedidosIngresados> PedidosIngresadosAnualesDetalle { get; set; } = new ();
        protected List<ChartData> PedidosIngresadosAnuales { get; set; } = new ();
        protected List<ChartData> PedidosIngresadosMensuales { get; set; } = new ();
        protected string TituloGraficoPedidosMensual = "";
        protected string SerieSeleccionaPedidos = "";
        protected int PromedioPedidosIngresadosMensuales = 0;


        protected SfChart refChartDetallePresupuestos;
        protected SfGrid<vEstadPresupuestos> grdPresupuestos;
        protected List<vEstadPresupuestos> DataPresupuestos { get; set; } = new();
        protected List<vEstadPresupuestos> PresupuestosAnualesDetalle { get; set; } = new();
        protected List<ChartData> PresupuestosAnuales { get; set; } = new();
        protected List<ChartData> PresupuestosMensuales { get; set; } = new();
        protected string TituloGraficoPresupuestosMensual = "";
        protected string SerieSeleccionaPresupuestos = "";
        protected int PromedioPresupuestosMensuales = 0;

        protected string[] palettes = new string[] {"#38610B", "#688A08", "#86B404", "#74DF00", 
            "#40FF00", "#2EFE2E", "#81F781", "#D0FA58", "#D7DF01","#DBA901", "#2EFE9A" };

        public class ChartData
        {
            public string XSerieName { get; set; }
            public double YSerieName { get; set; }
        }

        protected override async Task OnInitializedAsync()
        {
            MainLayout.Titulo = "Panel de Control";
            VisibleSpinner = true;

            await GetFacturacion();
            await GetCompras();
            await GetPedidos();
            await GetPresupuestos();
            VisibleSpinner = false;
        }

        
        protected async Task GetFacturacion()
        {
            this.DataFacturacionOriginal = await Http.GetFromJsonAsync<List<vEstadFacturacion>>("api/EstadisticaVentas/Facturacion");
            FacturacionAnual = DataFacturacionOriginal.GroupBy(g => new { g.ANIO })
            .Select(d => new ChartData()
            {
                XSerieName = d.Key.ANIO.ToString(),
                YSerieName = Convert.ToDouble(d.Sum(p => p.TOTAL_DOL))
            }).OrderBy(c => c.XSerieName)
            .ToList();
        }

        protected async Task GetCompras()
        {
            this.DataComprasOriginal = await Http.GetFromJsonAsync<List<vEstadCompras>>("api/EstadisticaVentas/Compras");

            ComprasAnual = DataComprasOriginal.GroupBy(g => new { g.ANIO })
            .Select(d => new ChartData()
            {
                XSerieName = d.Key.ANIO.ToString(),
                YSerieName = Convert.ToDouble(d.Sum(p => p.TOTAL_DOL))
            }).OrderBy(c => c.XSerieName)
            .ToList();

            gridDetalleCompras?.PreventRender();
        }


        protected async Task GetPedidos()
        {
            this.DataPedidosIngresados = await Http.GetFromJsonAsync<List<vEstadPedidosIngresados>>("api/EstadisticaVentas/PedidosIngresados");


            PedidosIngresadosAnuales = DataPedidosIngresados.GroupBy(g => new { g.ANIO })
            .Select(d => new ChartData()
            {
                XSerieName = d.Key.ANIO.ToString(),
                YSerieName = d.Sum(p => p.TOT_DOL)
            })
            .OrderBy(c => c.XSerieName)
            .ToList();

            grdPedIngresados?.PreventRender();
        }

        protected async Task GetPresupuestos()
        {
            this.DataPresupuestos = await Http.GetFromJsonAsync<List<vEstadPresupuestos>>("api/EstadisticaVentas/Presupuestos");


            PresupuestosAnuales = DataPresupuestos.GroupBy(g => new { g.ANIO })
            .Select(d => new ChartData()
            {
                XSerieName = d.Key.ANIO.ToString(),
                YSerieName = d.Sum(p => p.TOT_DOL)
            })
            .OrderBy(c => c.XSerieName)
            .ToList();

            grdPresupuestos?.PreventRender();
        }

        protected async Task MostrarDetalle(Syncfusion.Blazor.Charts.PointEventArgs args)
        {
            var año = args.Point.X;
            SerieSeleccionaFacturacion = args.Series.Name;
            TituloGraficoFacturacionMensual = $"Facturación Mensual {año}";

            //para grilla de detalle
            DataFacturacionDetalle = DataFacturacionOriginal.Where(p => p.ANIO == Convert.ToInt32(año))
                //.OrderBy(o => new { o.ANIO, o.MES })
                .ToList();


            FacturacionMensual = DataFacturacionOriginal
            .Where(v => v.ANIO == Convert.ToInt32(año))
            .OrderBy(o => o.MES)
            .GroupBy(g => new { g.MES }).Select(d => new ChartData()
            {
                XSerieName = d.Key.MES.ToString(),
                YSerieName = Math.Round(Convert.ToDouble(d.Sum(p => p.TOTAL_DOL)), 2)
            }).ToList();


            PromedioFacturacionMensual = Convert.ToInt32(FacturacionMensual.Average(p => p.YSerieName));

            await InvokeAsync(StateHasChanged);
            await refChartDetalle.RefreshAsync();
            await refChartDetalle.RefreshAsync();


        }

        protected async Task MostrarDetalleCompras(Syncfusion.Blazor.Charts.PointEventArgs args)
        {
            var año = args.Point.X;
            SerieSeleccionaCompras = args.Series.Name;
            TituloGraficoComprasMensual = $"Compra Mensual {año}";

            //para grilla de detalle
            gridDetalleCompras.PreventRender();
            DataComprasDetalle = new();
            DataComprasDetalle = DataComprasOriginal.Where(p => p.ANIO == Convert.ToInt32(año))
                //.OrderBy(o => new { o.ANIO, o.MES })
                .ToList();


            ComprasMensual = DataComprasOriginal
            .Where(v => v.ANIO == Convert.ToInt32(año))
            .OrderBy(o => o.MES)
            .GroupBy(g => new { g.MES }).Select(d => new ChartData()
            {
                XSerieName = d.Key.MES.ToString(),
                YSerieName = Math.Round(Convert.ToDouble(d.Sum(p => p.TOTAL_DOL)), 2)
            }).ToList();


            PromedioComprasMensual = Convert.ToInt32(ComprasMensual.Average(p => p.YSerieName));

            StateHasChanged();
            //gridDetalleCompras.Refresh();
            await refChartDetalleCompras.RefreshAsync();
            await refChartDetalleCompras.RefreshAsync();
        }

        protected async Task MostrarDetalleComprasMesTipo(Syncfusion.Blazor.Charts.PointEventArgs args)
        {
            var mes = args.Point.X;
            SerieSeleccionaCompras = args.Series.Name;
            TituloGraficoComprasMensual = $"Compra Mensual {mes}";

            //POR TIPO
            ComprasMensualTipo = DataComprasOriginal
            .Where(v => v.MES == Convert.ToInt32(mes))
            .OrderBy(o => o.MES)
            .GroupBy(g => new { g.TIPO }).Select(d => new ChartData()
            {
                XSerieName = d.Key.TIPO,
                YSerieName = Math.Round(Convert.ToDouble(d.Sum(p => p.TOTAL_DOL)), 2)
            }).ToList();

            PromedioComprasMensual = Convert.ToInt32(ComprasMensual.Average(p => p.YSerieName));

            StateHasChanged();
            //gridDetalleCompras.Refresh();
            await refChartDetalleComprasMesTipo.RefreshAsync();
            await refChartDetalleComprasMesTipo.RefreshAsync();
        }

        protected async Task MostrarDetallePedidosAnuales(Syncfusion.Blazor.Charts.PointEventArgs args)
        {
            var año = args.Point.X;
            SerieSeleccionaPedidos = args.Series.Name;
            TituloGraficoPedidosMensual = $"U$S Mensuales {año}";

            PedidosIngresadosAnualesDetalle = DataPedidosIngresados.Where(p => p.ANIO == Convert.ToInt32(año))
                //.OrderBy(p => new { p.ANIO_PREV, p.MES })
                .ToList();


            PedidosIngresadosMensuales = DataPedidosIngresados
            .Where(v => v.ANIO == Convert.ToInt32(año))
            .OrderBy(o => o.MES)
            .GroupBy(g => new { g.MES }).Select(d => new ChartData()
            {
                XSerieName = d.Key.MES.ToString(),
                YSerieName = Math.Round(d.Sum(p => p.TOT_DOL))
            }).ToList();


            PromedioPedidosIngresadosMensuales = Convert.ToInt32(PedidosIngresadosMensuales.Average(p => p.YSerieName));
            await InvokeAsync(StateHasChanged);
            await refChartDetallePedidos.RefreshAsync();
            await refChartDetallePedidos.RefreshAsync();


        }

        protected async Task MostrarDetallePresupuestosAnuales(Syncfusion.Blazor.Charts.PointEventArgs args)
        {
            var año = args.Point.X;
            SerieSeleccionaPresupuestos = args.Series.Name;
            TituloGraficoPresupuestosMensual = $"U$S Mensuales {año}";

            PresupuestosAnualesDetalle = DataPresupuestos.Where(p => p.ANIO == Convert.ToInt32(año))
                //.OrderBy(p => new { p.ANIO_PREV, p.MES })
                .ToList();


            PresupuestosMensuales = DataPresupuestos
                .Where(v => v.ANIO == Convert.ToInt32(año))
                .OrderBy(o => o.MES)
                .GroupBy(g => new { g.MES }).Select(d => new ChartData()
                {
                    XSerieName = d.Key.MES.ToString(),
                    YSerieName = Math.Round(d.Sum(p => p.TOT_DOL))
                }).ToList();


            PromedioPresupuestosMensuales = Convert.ToInt32(PresupuestosMensuales.Average(p => p.YSerieName));
            await InvokeAsync(StateHasChanged);
            await refChartDetallePresupuestos.RefreshAsync();
            await refChartDetallePresupuestos.RefreshAsync();


        }
    }
}
