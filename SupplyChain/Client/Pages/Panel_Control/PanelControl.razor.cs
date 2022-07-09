using Microsoft.AspNetCore.Components;
using SupplyChain.Client.Shared;
using SupplyChain.Shared;
using Syncfusion.Blazor.Charts;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Layouts;
using Syncfusion.Blazor.Spinner;
using Syncfusion.Blazor.Gantt;
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

        protected SfDashboardLayout dashboardObject;
        protected SfChart refChartDetalle;
        protected SfSpinner refSpinner;
        protected bool VisibleSpinner = false;

        ///////*****************FACTURACION*************////////////////////////////
        protected SfGrid<vEstadFacturacion> grdFacturacionDetalle;
        protected List<vEstadFacturacion> DataFacturacionOriginal = new();
        protected List<vEstadCompras> DataComprasOriginal = new();
        protected List<vEstadFacturacion> DataFacturacionDetalle = new();
        protected List<ChartData> FacturacionAnual= new();
        protected List<ChartData> FacturacionMensual= new();
        protected string TituloGraficoFacturacionMensual = "";
        protected string SerieSeleccionaFacturacion = "";
        protected int PromedioFacturacionMensual = 0;
        ///////****************************************////////////////////////////

        ///////*****************NO CONFORMIDADES*************////////////////////////////
        protected List<vEstadEventos> DataEventosOriginal = new();

        protected SfGrid<vEstadEventos> gridDetalleEventos;
        protected SfChart refChartDetalleEventos;
        protected SfChart refChartDetalleEventosMesTipo;
        protected SfChart refChartDetalleEventosProveedor;
        protected List<vEstadEventos> DataEventosDetalle = new();
        protected List<ChartData> EventosAnual = new();
        protected List<ChartData> EventosMensual = new();
        protected List<ChartData> EventosMensualTipo = new();
        protected List<ChartData> EventosProveedor = new();
        protected string TituloGraficoEventosMensual = "";
        protected string TituloGraficoEventosTipo = "";
        protected string TituloGraficoEventosProveedor = "";
        protected string SerieSeleccionaEventos = "";
        protected string añoEventoSeleccionado = string.Empty;
        ///////**********************************************////////////////////////////

        protected string chartXAnnoation = "75px";
        protected double[] Spacing = new double[] { 15, 15 };
        protected double Ratio = 160 / 100;

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
        protected List<ChartData> PedidosIngresadosCategoria { get; set; } = new();
        protected List<ChartData> PedidosIngresadosMercado { get; set; } = new();
        protected List<vEstadPedidosIngresados> PedidosIngresadosDetalleMes { get; set; } = new ();
        protected List<ChartData> PedidosIngresadosAnuales { get; set; } = new ();
        protected List<ChartData> PedidosIngresadosMensuales { get; set; } = new ();
        protected List<ChartData> PedidosIngresadosMensualesCategoriaDetalle { get; set; } = new ();
        protected string TituloPedidosIngresadosCategoria = "U$S por Categoria";
        protected string TituloPedidosIngresadosCategoriaDetalle = "Categoria";
        protected string TituloGraficoPedidosMensual = "";
        protected string SerieSeleccionaPedidos = "";
        protected int añoSeleccionadoPedidosIngresados;
        protected int PromedioPedidosIngresadosMensuales;

        /////////////**********MERCADO EXTERNO*********////////////////////
        protected DetalleCategoria.DialogDetalle dialogDetalle;
        protected string TituloPedidosIngresadosMercado = "Mercado Externo";
        //////////////////////////////////////////////////////////////////

        /////////////**********PROYECTOS*********////////////////////
        protected DateTime ProjectStart = new DateTime(2021, 3, 25);
        protected DateTime ProjectEnd = new DateTime(2023, 7, 28);
        protected SfGantt<ProyectosGBPI> refGanttProyectos;
        //protected List<ProyectosGBPI> DataProyectos { get; set; } = new();
        /////////////////////////////////////////////////////////////


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

            private string _z;
            public string ZSerieName
            {
                get { return _z; }
                set
                {
                    // Set B to some new value
                    //_z = $"{XSerieName}: {YSerieName} %";
                    _z = value;
                }
            }
        }

        protected override async Task OnInitializedAsync()
        {
            MainLayout.Titulo = "Panel de Control";
            VisibleSpinner = true;

            await GetFacturacion();
            await GetCompras();
            await GetEventos();
            await GetPedidos();
            await GetPresupuestos();
            //await GetProyectos();
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

            grdFacturacionDetalle?.PreventRender();
            grdFacturacionDetalle?.AutoFitColumnsAsync();
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

        protected async Task GetEventos()
        {
            this.DataEventosOriginal = await Http.GetFromJsonAsync<List<vEstadEventos>>("api/NoConformidades/Eventos");

            EventosAnual = DataEventosOriginal.GroupBy(g => new { g.ANIO })
            .Select(d => new ChartData()
            {
                XSerieName = d.Key.ANIO.ToString(),
                YSerieName = Convert.ToDouble(d.Count())
            }).OrderBy(c => c.XSerieName)
            .ToList();

            gridDetalleEventos?.PreventRender();
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

        protected async Task MostrarDetalleEventos(Syncfusion.Blazor.Charts.PointEventArgs args)
        {
            añoEventoSeleccionado = args.Point.X.ToString();
            SerieSeleccionaEventos = args.Series.Name;
            TituloGraficoEventosMensual = $"Cantidad de Eventos en {añoEventoSeleccionado}";

            //para grilla de detalle
            gridDetalleEventos.PreventRender();
            DataEventosDetalle = new();
            DataEventosDetalle = DataEventosOriginal.Where(p => p.ANIO == Convert.ToInt32(añoEventoSeleccionado))
                //.OrderBy(o => new { o.ANIO, o.MES })
                .ToList();


            EventosMensual = DataEventosOriginal
            .Where(v => v.ANIO == Convert.ToInt32(añoEventoSeleccionado))
            .OrderBy(o => o.MES)
            .GroupBy(g => new { g.MES }).Select(d => new ChartData()
            {
                XSerieName = d.Key.MES.ToString(),
                YSerieName = Math.Round(Convert.ToDouble(d.Count()))
            }).ToList();

            //POR TIPO
            EventosMensualTipo = DataEventosOriginal
            .Where(v => v.ANIO == Convert.ToInt32(añoEventoSeleccionado))
            .OrderBy(o => o.MES)
            .GroupBy(g => new { g.Des_TipoNc }).Select(d => new ChartData()
            {
                XSerieName = d.Key.Des_TipoNc,
                YSerieName = Math.Round(Convert.ToDouble(d.Count()))
            }).ToList();

            //eventos por proveedor
            TituloGraficoEventosProveedor = $"Eventos por Proveedor en {añoEventoSeleccionado}";
            EventosProveedor = DataEventosOriginal
            .Where(v => v.ANIO == Convert.ToInt32(añoEventoSeleccionado) && !string.IsNullOrEmpty(v.DES_PROVE))
            .GroupBy(g => new { g.DES_PROVE }).Select(d => new ChartData()
            {
                XSerieName = d.Key.DES_PROVE.Trim(),
                YSerieName = Math.Round(Convert.ToDouble(d.Count()))
            }).ToList();

            StateHasChanged();
            await refChartDetalleEventos.RefreshAsync();
            await refChartDetalleEventos.RefreshAsync();

            await refChartDetalleEventosMesTipo.RefreshAsync();
            await refChartDetalleEventosMesTipo.RefreshAsync();
            
            await refChartDetalleEventosProveedor.RefreshAsync();
            await refChartDetalleEventosProveedor.RefreshAsync();
        }

        protected async Task MostrarDetalleEventosMesTipo(Syncfusion.Blazor.Charts.PointEventArgs args)
        {
            var mes = args.Point.X;
            SerieSeleccionaEventos = args.Series.Name;
            TituloGraficoEventosMensual = $"Cantidad de Eventos en mes {mes}";

            //POR TIPO
            TituloGraficoEventosTipo = $"Eventos por Tipo en {mes}/{añoEventoSeleccionado} ";
            EventosMensualTipo = DataEventosOriginal
            .Where(v => v.MES == Convert.ToInt32(mes) &&  v.ANIO == Convert.ToInt32(añoEventoSeleccionado) )
            .OrderBy(o => o.MES)
            .GroupBy(g => new { g.Des_TipoNc }).Select(d => new ChartData()
            {
                XSerieName = d.Key.Des_TipoNc,
                YSerieName = Math.Round(Convert.ToDouble(d.Count()))
            }).ToList();


            //eventos por proveedor
            TituloGraficoEventosProveedor = $"Eventos por Proveedor en {mes}/{añoEventoSeleccionado} ";
            EventosProveedor = DataEventosOriginal
            .Where(v => v.ANIO == Convert.ToInt32(añoEventoSeleccionado) 
                    && !string.IsNullOrEmpty(v.DES_PROVE) && v.MES == Convert.ToInt32(mes) )
            .GroupBy(g => new { g.DES_PROVE }).Select(d => new ChartData()
            {
                XSerieName = d.Key.DES_PROVE.Trim(),
                YSerieName = Math.Round(Convert.ToDouble(d.Count()))
            }).ToList();

            StateHasChanged();
            await refChartDetalleEventosMesTipo.RefreshAsync();
            await refChartDetalleEventosProveedor.RefreshAsync();
        }

        protected async Task MostrarDetallePedidosAnuales(Syncfusion.Blazor.Charts.PointEventArgs args)
        {
            añoSeleccionadoPedidosIngresados = Convert.ToInt32(args.Point.X);
            SerieSeleccionaPedidos = args.Series.Name;
            TituloGraficoPedidosMensual = $"U$S Mensuales del {añoSeleccionadoPedidosIngresados}";
            TituloPedidosIngresadosMercado = $"Mercado Externo del {añoSeleccionadoPedidosIngresados}";
            //grafico mensual
            PedidosIngresadosMensuales = DataPedidosIngresados
                .Where(v => v.ANIO == añoSeleccionadoPedidosIngresados)
                .OrderBy(o => o.MES)
                .GroupBy(g => new { g.MES }).Select(d => new ChartData()
                {
                    XSerieName = d.Key.MES.ToString(),
                    YSerieName = Math.Round(d.Sum(p => p.TOT_DOL))
                }).ToList();


            PromedioPedidosIngresadosMensuales = Convert.ToInt32(PedidosIngresadosMensuales.Average(p => p.YSerieName));

            //categoria de cliente
            var totalPeriodoSeleccionado = Math.Round(DataPedidosIngresados
                .Where(p => p.ANIO == añoSeleccionadoPedidosIngresados)
                .Sum(s => s.TOT_DOL), 2);

            TituloPedidosIngresadosCategoria = $"% por Categoria del {añoSeleccionadoPedidosIngresados} (US$ {totalPeriodoSeleccionado})";

            PedidosIngresadosCategoria = DataPedidosIngresados
                .Where(p => p.ANIO == añoSeleccionadoPedidosIngresados)
                .GroupBy(g => new { g.CATEGORIA })
            .Select(d => new ChartData()
            {
                XSerieName = d.Key.CATEGORIA,
                YSerieName = Math.Round(Math.Round(d.Sum(p => p.TOT_DOL), 2) / totalPeriodoSeleccionado, 2) * 100,
                ZSerieName = $"{Math.Round(Math.Round(d.Sum(p => p.TOT_DOL), 2) / totalPeriodoSeleccionado, 2) * 100} %\n" +
                $"({Math.Round(d.Sum(p => p.TOT_DOL), 2)} U$S)"
            })
            .OrderBy(c => c.XSerieName)
            .ToList();

            PedidosIngresadosMercado = DataPedidosIngresados
                .Where(p => p.ANIO == añoSeleccionadoPedidosIngresados)
                .GroupBy(g => new { g.MERCADO })
            .Select(d => new ChartData()
            {
                XSerieName = d.Key.MERCADO,
                YSerieName = Math.Round(d.Sum(p => p.TOT_DOL), 2),
                ZSerieName = $"{Math.Round(Math.Round(d.Sum(p => p.TOT_DOL), 2) / totalPeriodoSeleccionado, 2) * 100} %\n" +
                $"({Math.Round(d.Sum(p => p.TOT_DOL), 2)} U$S)"
            })
            .OrderBy(c => c.XSerieName)
            .ToList();

            //detalle grilla
            PedidosIngresadosDetalleMes = DataPedidosIngresados
                .Where(p => p.ANIO == añoSeleccionadoPedidosIngresados)
                .ToList();

            await InvokeAsync(StateHasChanged);
            await refChartDetallePedidos.RefreshAsync();
            await refChartDetallePedidos.RefreshAsync();


        }

        protected async Task MostrarDetallePedidosMensuales(Syncfusion.Blazor.Charts.PointEventArgs args)
        {
            var mesSeleccionadoPedidosIngresados = Convert.ToInt32(args.Point.X);
            var SerieSeleccionaPedidosMensual = args.Series.Name;

            var totalPeriodoSeleccionado = Math.Round(DataPedidosIngresados
                .Where(p => p.ANIO == añoSeleccionadoPedidosIngresados && p.MES == mesSeleccionadoPedidosIngresados)
                .Sum(s => s.TOT_DOL), 2);

            var nombreMes = new DateTime(añoSeleccionadoPedidosIngresados, mesSeleccionadoPedidosIngresados, 1).ToString("MMMM");
            TituloPedidosIngresadosCategoria = $"% por Categoria de {nombreMes} del {añoSeleccionadoPedidosIngresados} (US$ {totalPeriodoSeleccionado})";
            TituloPedidosIngresadosMercado = $"Mercado Externo de {nombreMes} del {añoSeleccionadoPedidosIngresados}";
            PedidosIngresadosDetalleMes = DataPedidosIngresados
                .Where(p => p.ANIO == añoSeleccionadoPedidosIngresados && p.MES == mesSeleccionadoPedidosIngresados)
                .ToList();
            
            //%categoria de cliente
            PedidosIngresadosCategoria = DataPedidosIngresados
                .Where(p => p.ANIO == añoSeleccionadoPedidosIngresados && p.MES == mesSeleccionadoPedidosIngresados)
                .GroupBy(g => new { g.CATEGORIA })
            .Select(d => new ChartData()
            {
                XSerieName = d.Key.CATEGORIA,
                YSerieName = Math.Round(Math.Round(d.Sum(p => p.TOT_DOL), 2) / totalPeriodoSeleccionado, 2) * 100,
                ZSerieName = $"{Math.Round(Math.Round(d.Sum(p => p.TOT_DOL), 2) / totalPeriodoSeleccionado, 2) * 100} %\n" +
                $"({Math.Round(d.Sum(p => p.TOT_DOL), 2)} U$S)"
            })
            .OrderBy(c => c.XSerieName)
            .ToList();

            PedidosIngresadosMercado = DataPedidosIngresados
                .Where(p => p.ANIO == añoSeleccionadoPedidosIngresados && p.MES == mesSeleccionadoPedidosIngresados)
                .GroupBy(g => new { g.MERCADO })
            .Select(d => new ChartData()
            {
                XSerieName = d.Key.MERCADO,
                YSerieName = Math.Round(d.Sum(p => p.TOT_DOL), 2),
                ZSerieName = $"{Math.Round(Math.Round(d.Sum(p => p.TOT_DOL), 2) / totalPeriodoSeleccionado, 2) * 100} %\n" +
                $"({Math.Round(d.Sum(p => p.TOT_DOL), 2)} U$S)"
            })
            .OrderBy(c => c.XSerieName)
            .ToList();

            await InvokeAsync(StateHasChanged);

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

        protected async Task OnFiltroByCategoria(AccumulationPointEventArgs args)
        {
            var categoria = args.Point.X.ToString().Trim();
            var name = args.Name;
            //detalle en grafico de categoria seleccionada filtrada por año y agrupada por mes
            PedidosIngresadosMensualesCategoriaDetalle = DataPedidosIngresados
                .Where(v => v.ANIO == añoSeleccionadoPedidosIngresados && v.CATEGORIA.Trim() == categoria.Trim() )
                .OrderBy(o => o.MES)
                .GroupBy(g => new { g.MES }).Select(d => new ChartData()
                {
                    XSerieName = d.Key.MES.ToString(),
                    YSerieName = Math.Round(d.Sum(p => p.TOT_DOL))
                }).ToList();

            TituloPedidosIngresadosCategoriaDetalle = $"Categoria {categoria.ToUpper()} de { añoSeleccionadoPedidosIngresados}";
            await dialogDetalle.ShowAsync();

        }

        /*protected async Task GetProyectos()
        {
            this.DataProyectos = await Http.GetFromJsonAsync<List<ProyectosGBPI>>("api/Proyectos/Proyectos");
        }*/
    }
}
