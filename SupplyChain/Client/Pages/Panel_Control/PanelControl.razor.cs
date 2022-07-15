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
using Syncfusion.Blazor.Calendars;
using Syncfusion.Blazor.LinearGauge;

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
        protected DateTime FacturacionMinDate = DateTime.Now.AddMonths(-3);
        protected DateTime FacturacionMaxDate = DateTime.Now;
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
        protected DateTime EventosMinDate = DateTime.Now.AddMonths(-3);
        protected DateTime EventosMaxDate = DateTime.Now;
        ///////**********************************************////////////////////////////

        protected string chartXAnnoation = "75px";
        protected double[] Spacing = new double[] { 15, 15 };
        protected double Ratio = 160 / 100;

        ///////*****************COMPRAS*************////////////////////////////
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
        protected DateTime ComprasMinDate = DateTime.Now.AddMonths(-3);
        protected DateTime ComprasMaxDate = DateTime.Now;
        ///////**********************************************////////////////////////////


        protected List<vEstadPedidosAlta> DataPedidosAltas { get; set; } = new();
        protected List<vEstadPedidosAlta> DataPedidosAltasDiasAtrasoDetalle { get; set; } = new();
        protected List<ChartData> PedidosAltasAnuales { get; set; } = new();
        protected List<ChartData> PedidosAltasAnualDiasAtraso { get; set; } = new();
        protected List<ChartData> PedidosAltasMensualDiasAtraso { get; set; } = new();
        protected SfGrid<vEstadPedidosAlta> grdDiasAtraso;
        protected string TituloGraficoMensualPedidosAlta = "";
        protected int PromedioPedidosAltaDiasAtrasoMensual = 0;
        protected int añoSeleccionadoPedidosAltasDiasAtraso = 0;
        protected SfChart refChartPedidosAltaDiasAtrasoMensual;
        protected DateTime DiasAtrasoMinDate = DateTime.Now.AddMonths(-3);
        protected DateTime DiasAtrasoMaxDate = DateTime.Now;


        ///////*****************PEDIDOS INGRESADOS UNIDADES EQUIVALENTES*************////////////////////////////
        protected SfChart refChartDetallePedidosIngresadosUE;
        protected SfGrid<vEstadPedidosIngresados> grdPedIngresadosUE;
        protected List<vEstadPedidosIngresados> DataPedidosIngresadosUEDetalle { get; set; } = new();
        protected List<ChartData> PedidosIngresadosUEAnuales { get; set; } = new();
        protected List<ChartData> PedidosIngresadosUEMensuales { get; set; } = new();
        protected DateTime PedidosIngresadosUEMinDate = DateTime.Now.AddMonths(-3);
        protected DateTime PedidosIngresadosUEMaxDate = DateTime.Now;
        protected int añoSeleccionadoPedidosIngresadosUE;
        protected int PromedioPedidosIngresadosMensualesUE;
        protected string TituloGraficoPedidosIngresadosUEMensual = "";
        ///////*****************PEDIDOS INGRESADOS*************////////////////////////////
        protected SfChart refChartDetallePedidos;
        protected SfGrid<vEstadPedidosIngresados> grdPedIngresados;
        protected SfLinearGauge refChartGarantia;
        protected List<vEstadPedidosIngresados> DataPedidosIngresados { get; set; } = new ();
        protected List<ChartData> PedidosIngresadosCategoria { get; set; } = new();
        protected List<ChartData> PedidosIngresadosMercado { get; set; } = new();
        protected List<vEstadPedidosIngresados> PedidosIngresadosDetalleMes { get; set; } = new ();
        protected List<ChartData> PedidosIngresadosAnuales { get; set; } = new ();
        protected List<ChartData> PedidosIngresadosMensuales { get; set; } = new ();
        protected DateTime PedidosIngresadosMinDate = DateTime.Now.AddMonths(-3);
        protected DateTime PedidosIngresadosMaxDate = DateTime.Now;
        protected List<ChartData> PedidosIngresadosMensualesCategoriaDetalle { get; set; } = new ();
        protected string TituloPedidosIngresadosCategoria = "U$S por Categoria";
        protected string TituloPedidosIngresadosCategoriaDetalle = "Categoria";
        protected string TituloGraficoPedidosMensual = "";
        protected string TituloGraficoGarantia = "";
        protected string SerieSeleccionaPedidos = "";
        protected int añoSeleccionadoPedidosIngresados;
        protected int PromedioPedidosIngresadosMensuales;
        protected double CantidadGarantiasPedidosIngresados = 0;
        ///////**********************************************////////////////////////////

        /////////////**********PEDIDOS VS PRESUPUESTO*********////////////////////
        protected List<ChartData> PedidosIngresadosAnuales_Vs { get; set; } = new();
        protected List<ChartData> PresupuestosAnuales_Vs { get; set; } = new();
        ///////**********************************************////////////////////////////
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
        protected DateTime PresupuestosMinDate = DateTime.Now.AddMonths(-3);
        protected DateTime PresupuestosMaxDate = DateTime.Now;

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
            await GetPedidosAltas();
            await GetPresupuestos();
            //await GetProyectos();
            VisibleSpinner = false;
        }

        
        protected async Task GetFacturacion()
        {
            this.DataFacturacionOriginal = await Http.GetFromJsonAsync<List<vEstadFacturacion>>("api/EstadisticaVentas/Facturacion");

            FacturacionMinDate = DataFacturacionOriginal.Min(f => f.FECHA);
            FacturacionMaxDate = DataFacturacionOriginal.Max(f => f.FECHA);

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

            ComprasMinDate = DataComprasOriginal.Min(f => f.FECHA);
            ComprasMaxDate = DataComprasOriginal.Max(f => f.FECHA);

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
            EventosMinDate = DataEventosOriginal.Min(f => f.FE_EMIT);
            EventosMaxDate = DataEventosOriginal.Max(f => f.FE_EMIT);

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
            PedidosIngresadosMinDate = DataPedidosIngresados.Min(f => f.FECHA);
            PedidosIngresadosMaxDate = DataPedidosIngresados.Max(f => f.FECHA);

            PedidosIngresadosAnuales = DataPedidosIngresados.GroupBy(g => new { g.ANIO })
            .Select(d => new ChartData()
            {
                XSerieName = d.Key.ANIO.ToString(),
                YSerieName = d.Sum(p => p.TOT_DOL)
            })
            .OrderBy(c => c.XSerieName)
            .ToList();

            PedidosIngresadosUEAnuales = DataPedidosIngresados.GroupBy(g => new { g.ANIO })
            .Select(d => new ChartData()
            {
                XSerieName = d.Key.ANIO.ToString(),
                YSerieName = Convert.ToDouble(d.Sum(p => p.UNIDEQUI))
            }).OrderBy(c => c.XSerieName)
            .ToList();

            PedidosIngresadosAnuales_Vs = PedidosIngresadosAnuales;

            grdPedIngresados?.PreventRender();
            grdPedIngresadosUE?.PreventRender();
        }

        protected async Task GetPedidosAltas()
        {
            this.DataPedidosAltas = await Http.GetFromJsonAsync<List<vEstadPedidosAlta>>("api/EstadisticaVentas/PedidosAltas");

            PedidosAltasAnuales = DataPedidosAltas.GroupBy(g => new { g.ANIO })
            .Select(d => new ChartData()
            {
                XSerieName = d.Key.ANIO.ToString(),
                YSerieName = Convert.ToDouble(d.Sum(p => p.UNIDEQUI))
            }).OrderBy(c => c.XSerieName)
            .ToList();


            DiasAtrasoMinDate = DataPedidosIngresados.Where(p => p.PEDIDO > 0).Min(f => f.FECHA);
            DiasAtrasoMaxDate = DataPedidosIngresados.Where(p => p.PEDIDO > 0).Max(f => f.FECHA);

            PedidosAltasAnualDiasAtraso = DataPedidosAltas.Where(p => p.PEDIDO > 0)
            .Select(d => new ChartData()
            {
                XSerieName = d.ANIO.ToString(),
                YSerieName = Math.Abs( (d.FECHA_PREVISTA.HasValue ? d.FECHA_PREVISTA.Value - d.FECHA_DATE : DateTime.Now - d.FECHA_DATE).TotalDays)
            })
            .GroupBy(g=> new { g.XSerieName })
            .Select(d => new ChartData()
            {
                XSerieName = d.Key.XSerieName,
                YSerieName = d.Average(p=> p.YSerieName)
            })
            .OrderBy(c => c.XSerieName)
            .ToList();

            grdDiasAtraso?.PreventRender();
        }

        protected async Task GetPresupuestos()
        {
            this.DataPresupuestos = await Http.GetFromJsonAsync<List<vEstadPresupuestos>>("api/EstadisticaVentas/Presupuestos");
            PresupuestosMinDate = DataPresupuestos.Min(f => f.FECHA);
            PresupuestosMaxDate = DataPresupuestos.Max(f => f.FECHA);

            PresupuestosAnuales = DataPresupuestos.GroupBy(g => new { g.ANIO })
            .Select(d => new ChartData()
            {
                XSerieName = d.Key.ANIO.ToString(),
                YSerieName = d.Sum(p => p.TOT_DOL)
            })
            .OrderBy(c => c.XSerieName)
            .ToList();

            PresupuestosAnuales_Vs = PresupuestosAnuales;

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

        public async Task ValueChangeFechasFacturacion(RangePickerEventArgs<DateTime> args)
        {
            // Here, you can customize your code.
            FacturacionAnual = DataFacturacionOriginal.Where(f => f.FECHA >= FacturacionMinDate && f.FECHA <= FacturacionMaxDate)
                .GroupBy(g => new { g.ANIO })
                .Select(d => new ChartData()
                {
                    XSerieName = d.Key.ANIO.ToString(),
                    YSerieName = Convert.ToDouble(d.Sum(p => p.TOTAL_DOL))
                }).OrderBy(c => c.XSerieName)
            .ToList();

            


            FacturacionMensual = DataFacturacionOriginal
                .Where(f => f.FECHA >= FacturacionMinDate && f.FECHA <= FacturacionMaxDate)
                .OrderBy(o => o.MES)
                .GroupBy(g => new { g.MES }).Select(d => new ChartData()
                {
                    XSerieName = d.Key.MES.ToString(),
                    YSerieName = Math.Round(Convert.ToDouble(d.Sum(p => p.TOTAL_DOL)), 2)
                }).ToList();


            //para grilla de detalle
            grdFacturacionDetalle.PreventRender();
            DataFacturacionDetalle = new();
            DataFacturacionDetalle = DataFacturacionOriginal.Where(f => f.FECHA >= FacturacionMinDate && f.FECHA <= FacturacionMaxDate)
                .OrderBy(o => o.FECHA)
                .ToList();

            PromedioFacturacionMensual = Convert.ToInt32(FacturacionMensual.Average(p => p.YSerieName));
            StateHasChanged();

            await refChartDetalle.RefreshAsync();
            await refChartDetalle.RefreshAsync();

            await grdFacturacionDetalle.AutoFitColumnsAsync();
            await grdFacturacionDetalle.RefreshHeaderAsync();
            grdFacturacionDetalle.Refresh();
            await grdFacturacionDetalle.RefreshColumnsAsync();

        }

        protected async Task RestablecerGraficosFacturacion()
        {

            VisibleSpinner = true;
            FacturacionAnual = new();
            FacturacionMensual = new();
            DataFacturacionDetalle = new();
            PromedioFacturacionMensual = 0;
            await GetFacturacion();
            VisibleSpinner = false;
        }

        protected async Task RestablecerGraficosCompras()
        {

            VisibleSpinner = true;
            ComprasAnual = new();
            ComprasMensual = new();
            DataComprasDetalle = new();
            ComprasMensualTipo = new();
            PromedioComprasMensual = 0;
            await GetCompras();
            VisibleSpinner = false;
        }

        protected async Task RestablecerGraficosEventos()
        {

            VisibleSpinner = true;
            EventosAnual = new();
            EventosMensual = new();
            DataEventosDetalle = new();
            EventosMensualTipo = new();
            EventosProveedor = new();
            await GetEventos();
            VisibleSpinner = false;
        }

        protected async Task RestablecerGraficosPedidosIngresados()
        {

            VisibleSpinner = true;
            PedidosIngresadosAnuales = new();
            PedidosIngresadosMensuales = new();
            PedidosIngresadosMercado = new();
            PedidosIngresadosMensualesCategoriaDetalle = new();
            PedidosIngresadosDetalleMes = new();
            PedidosIngresadosCategoria = new();
            PromedioPedidosIngresadosMensuales = 0;
            CantidadGarantiasPedidosIngresados = 0;
            TituloGraficoGarantia = "Pedidos Garantia";
            await GetPedidos();
            VisibleSpinner = false;
        }

        protected async Task RestablecerGraficosDiasAtraso()
        {

            VisibleSpinner = true;
            DataPedidosAltasDiasAtrasoDetalle = new();
            PromedioPedidosAltaDiasAtrasoMensual = 0;
            PedidosAltasAnualDiasAtraso = new();
            PedidosAltasMensualDiasAtraso = new();
            await GetPedidosAltas();
            VisibleSpinner = false;
        }

        protected async Task RestablecerGraficosPedidosIngresadosUE()
        {

            VisibleSpinner = true;
            DataPedidosIngresadosUEDetalle = new();
            PedidosIngresadosUEAnuales = new();
            PedidosIngresadosUEMensuales = new();
            PromedioPedidosIngresadosMensualesUE = 0;
            await GetPedidos();
            VisibleSpinner = false;
        }

        protected async Task RestablecerGraficosPresupuestos()
        {

            VisibleSpinner = true;
            PresupuestosAnuales = new();
            PresupuestosMensuales = new();
            PresupuestosAnualesDetalle= new();
            PromedioPresupuestosMensuales = 0;
            await GetPresupuestos();
            VisibleSpinner = false;
        }

        public async Task ValueChangeFechasCompras(RangePickerEventArgs<DateTime> args)
        {
            // Here, you can customize your code.
            //Filtra grafico anual
            ComprasAnual = DataComprasOriginal.Where(f => f.FECHA >= ComprasMinDate && f.FECHA <= ComprasMaxDate)
                .GroupBy(g => new { g.ANIO })
                .Select(d => new ChartData()
                {
                    XSerieName = d.Key.ANIO.ToString(),
                    YSerieName = Convert.ToDouble(d.Sum(p => p.TOTAL_DOL))
                }).OrderBy(c => c.XSerieName)
            .ToList();


            TituloGraficoComprasMensual = $"Compra Mensual";
            ComprasMensual = DataComprasOriginal
                .Where(f => f.FECHA >= ComprasMinDate && f.FECHA <= ComprasMaxDate)
                .OrderBy(o => o.MES)
                .GroupBy(g => new { g.MES }).Select(d => new ChartData()
                {
                    XSerieName = d.Key.MES.ToString(),
                    YSerieName = Math.Round(Convert.ToDouble(d.Sum(p => p.TOTAL_DOL)), 2)
                }).ToList();


            //POR TIPO
            ComprasMensualTipo = DataComprasOriginal
                .Where(f => f.FECHA >= ComprasMinDate && f.FECHA <= ComprasMaxDate)
                .GroupBy(g => new { g.TIPO }).Select(d => new ChartData()
                {
                    XSerieName = d.Key.TIPO,
                    YSerieName = Math.Round(Convert.ToDouble(d.Sum(p => p.TOTAL_DOL)), 2)
                }).ToList();

            PromedioComprasMensual = Convert.ToInt32(ComprasMensual.Average(p => p.YSerieName));





            //para grilla de detalle
            gridDetalleCompras.PreventRender();
            DataComprasDetalle = new();
            DataComprasDetalle = DataComprasOriginal.Where(f => f.FECHA >= ComprasMinDate && f.FECHA <= ComprasMaxDate)
                .OrderBy(o => o.FECHA)
                .ToList();

            await InvokeAsync(StateHasChanged);

            await refChartDetalleCompras.RefreshAsync();
            await refChartDetalleCompras.RefreshAsync();

            await refChartDetalleComprasMesTipo.RefreshAsync();
            await refChartDetalleComprasMesTipo.RefreshAsync();

            await gridDetalleCompras.AutoFitColumnsAsync();
            gridDetalleCompras.Refresh();

        }

        public async Task ValueChangeFechasEventos(RangePickerEventArgs<DateTime> args)
        {
            // Here, you can customize your code.
            //Filtra grafico anual
            EventosAnual = DataEventosOriginal.Where(f => f.FE_EMIT >= EventosMinDate && f.FE_EMIT <= EventosMaxDate)
                .GroupBy(g => new { g.ANIO })
                .Select(d => new ChartData()
                {
                    XSerieName = d.Key.ANIO.ToString(),
                    YSerieName = Convert.ToDouble(d.Count())
                }).OrderBy(c => c.XSerieName)
            .ToList();


            EventosMensual = DataEventosOriginal
                .Where(f => f.FE_EMIT >= EventosMinDate && f.FE_EMIT <= EventosMaxDate)
                .OrderBy(o => o.MES)
                .GroupBy(g => new { g.MES }).Select(d => new ChartData()
                {
                    XSerieName = d.Key.MES.ToString(),
                    YSerieName = Convert.ToDouble(d.Count())
                }).ToList();

            //POR TIPO
            EventosMensualTipo = DataEventosOriginal
            .Where(f => f.FE_EMIT >= EventosMinDate && f.FE_EMIT <= EventosMaxDate)
            .GroupBy(g => new { g.Des_TipoNc }).Select(d => new ChartData()
            {
                XSerieName = d.Key.Des_TipoNc,
                YSerieName = Math.Round(Convert.ToDouble(d.Count()))
            }).ToList();

            //eventos por proveedor
            TituloGraficoEventosProveedor = $"Eventos por Proveedor en {añoEventoSeleccionado}";
            EventosProveedor = DataEventosOriginal
            .Where(f => f.FE_EMIT >= EventosMinDate && f.FE_EMIT <= EventosMaxDate)
            .GroupBy(g => new { g.DES_PROVE }).Select(d => new ChartData()
            {
                XSerieName = d.Key.DES_PROVE.Trim(),
                YSerieName = Math.Round(Convert.ToDouble(d.Count()))
            }).ToList();

            //para grilla de detalle
            DataEventosDetalle = DataEventosOriginal.Where(f => f.FE_EMIT >= EventosMinDate && f.FE_EMIT <= EventosMaxDate)
                //.OrderBy(o => new { o.ANIO, o.MES })
                .ToList();

            //gridDetalleCompras.Refresh();
            await refChartDetalleEventos.RefreshAsync();
            await refChartDetalleEventos.RefreshAsync();

            await refChartDetalleEventosMesTipo.RefreshAsync();
            await refChartDetalleEventosMesTipo.RefreshAsync();

            await refChartDetalleEventosProveedor.RefreshAsync();
            await refChartDetalleEventosProveedor.RefreshAsync();
        }

        public async Task ValueChangeFechasPedidosIngresados(RangePickerEventArgs<DateTime> args)
        {
            PedidosIngresadosDetalleMes = DataPedidosIngresados
                .Where(p => p.FECHA >= PedidosIngresadosMinDate && p.FECHA <= PedidosIngresadosMaxDate)
                .ToList();

            TituloGraficoPedidosMensual = $"U$S Mensuales Desde {PedidosIngresadosMinDate:dd/MM/yyyy} Hasta {PedidosIngresadosMaxDate:dd/MM/yyyy}";
            TituloPedidosIngresadosMercado = $"Mercado Externo Desde {PedidosIngresadosMinDate:dd/MM/yyyy} Hasta {PedidosIngresadosMaxDate:dd/MM/yyyy}";
            //grafico mensual
            PedidosIngresadosMensuales = DataPedidosIngresados
                .Where(p => p.FECHA >= PedidosIngresadosMinDate && p.FECHA <= PedidosIngresadosMaxDate)
                .OrderBy(o => o.MES)
                .GroupBy(g => new { g.MES }).Select(d => new ChartData()
                {
                    XSerieName = d.Key.MES.ToString(),
                    YSerieName = Math.Round(d.Sum(p => p.TOT_DOL))
                }).ToList();

            TituloGraficoGarantia = $"Pedidos Garantia Periodo {PedidosIngresadosMinDate:dd/MM/yyyy} {PedidosIngresadosMaxDate:dd/MM/yyyy}";
            CantidadGarantiasPedidosIngresados = DataPedidosIngresados.Count(p => p.FECHA >= PedidosIngresadosMinDate 
                && p.FECHA <= PedidosIngresadosMaxDate && p.DES_ART.ToLower().Contains("garan"));
            PromedioPedidosIngresadosMensuales = Convert.ToInt32(PedidosIngresadosMensuales.Average(p => p.YSerieName));

            //categoria de cliente
            var totalPeriodoSeleccionado = Math.Round(DataPedidosIngresados
                .Where(p => p.FECHA >= PedidosIngresadosMinDate && p.FECHA <= PedidosIngresadosMaxDate)
                .Sum(s => s.TOT_DOL), 2);

            TituloPedidosIngresadosCategoria = $"% por Categoria Desde {PedidosIngresadosMinDate:dd/MM/yyyy} Hasta {PedidosIngresadosMaxDate:dd/MM/yyyy}\n" +
                $" (US$ {totalPeriodoSeleccionado})";

            PedidosIngresadosCategoria = DataPedidosIngresados
                .Where(p => p.FECHA >= PedidosIngresadosMinDate && p.FECHA <= PedidosIngresadosMaxDate)
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
                .Where(p => p.FECHA >= PedidosIngresadosMinDate && p.FECHA <= PedidosIngresadosMaxDate)
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
                .Where(p => p.FECHA >= PedidosIngresadosMinDate && p.FECHA <= PedidosIngresadosMaxDate)
                .ToList();

            await InvokeAsync(StateHasChanged);
            await refChartDetallePedidos.RefreshAsync();
            await refChartDetallePedidos.RefreshAsync();

            await grdPedIngresados.AutoFitColumnsAsync();

        }

        public async Task ValueChangeFechasPedidosIngresadosUE(RangePickerEventArgs<DateTime> args)
        {

            TituloGraficoPedidosIngresadosUEMensual = $"Desde {PedidosIngresadosMinDate:dd/MM/yyyy} Hasta {PedidosIngresadosMaxDate:dd/MM/yyyy}";
            //grafico mensual
            PedidosIngresadosUEMensuales = DataPedidosIngresados
                .Where(p => p.FECHA >= PedidosIngresadosMinDate && p.FECHA <= PedidosIngresadosMaxDate)
                .OrderBy(o => o.MES)
                .GroupBy(g => new { g.MES }).Select(d => new ChartData()
                {
                    XSerieName = d.Key.MES.ToString(),
                    YSerieName = Math.Round(d.Sum(p => p.TOT_DOL))
                }).ToList();

            PromedioPedidosIngresadosMensualesUE = Convert.ToInt32(PedidosIngresadosUEMensuales.Average(p => p.YSerieName));

            //detalle grilla
            DataPedidosIngresadosUEDetalle = DataPedidosIngresados
                .Where(p => p.FECHA >= PedidosIngresadosMinDate && p.FECHA <= PedidosIngresadosMaxDate)
                .ToList();

            await InvokeAsync(StateHasChanged);
            await refChartDetallePedidosIngresadosUE?.RefreshAsync();
            await refChartDetallePedidosIngresadosUE?.RefreshAsync();

            await grdPedIngresadosUE?.AutoFitColumnsAsync();

        }

        public async Task ValueChangeFechasPresupuestos(RangePickerEventArgs<DateTime> args)
        {
            TituloGraficoPresupuestosMensual = $"U$S Mensuales {PresupuestosMinDate:dd/MM/yyyy} {PresupuestosMaxDate:dd/MM/yyyy}";


            PresupuestosMensuales = DataPresupuestos
                .Where(p => p.FECHA >= PresupuestosMinDate && p.FECHA <= PresupuestosMaxDate)
                .OrderBy(o => o.MES)
                .GroupBy(g => new { g.MES }).Select(d => new ChartData()
                {
                    XSerieName = d.Key.MES.ToString(),
                    YSerieName = Math.Round(d.Sum(p => p.TOT_DOL))
                }).ToList();


            PromedioPresupuestosMensuales = Convert.ToInt32(PresupuestosMensuales.Average(p => p.YSerieName));


            PresupuestosAnualesDetalle = DataPresupuestos.Where(p => p.FECHA >= PresupuestosMinDate && p.FECHA <= PresupuestosMaxDate)
                .OrderBy(p => p.FECHA)
                .ToList();

            await InvokeAsync(StateHasChanged);
            await refChartDetallePresupuestos.RefreshAsync();
            await refChartDetallePresupuestos.RefreshAsync();

            grdPresupuestos.Refresh();
            await grdPresupuestos.AutoFitColumnsAsync();
        }

        public async Task ValueChangeFechasDiasAtraso(RangePickerEventArgs<DateTime> args)
        {
            PedidosAltasAnuales = DataPedidosAltas.Where(p=> p.FECHA >= DiasAtrasoMinDate && p.FECHA <= DiasAtrasoMaxDate)
                .GroupBy(g => new { g.ANIO })
            .Select(d => new ChartData()
            {
                XSerieName = d.Key.ANIO.ToString(),
                YSerieName = Convert.ToDouble(d.Sum(p => p.UNIDEQUI))
            }).OrderBy(c => c.XSerieName)
            .ToList();


            PedidosAltasAnualDiasAtraso = DataPedidosAltas.Where(p => p.PEDIDO > 0 && p.FECHA >= DiasAtrasoMinDate && p.FECHA <= DiasAtrasoMaxDate)
            .Select(d => new ChartData()
            {
                XSerieName = d.ANIO.ToString(),
                YSerieName = Math.Abs((d.FECHA_PREVISTA.HasValue ? d.FECHA_PREVISTA.Value - d.FECHA_DATE : DateTime.Now - d.FECHA_DATE).TotalDays)
            })
            .GroupBy(g => new { g.XSerieName })
            .Select(d => new ChartData()
            {
                XSerieName = d.Key.XSerieName,
                YSerieName = d.Average(p => p.YSerieName)
            })
            .OrderBy(c => c.XSerieName)
            .ToList();

            grdDiasAtraso?.PreventRender();
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

            TituloGraficoGarantia = $"Pedidos Garantia {añoSeleccionadoPedidosIngresados}";
            CantidadGarantiasPedidosIngresados = DataPedidosIngresados.Count(v => v.ANIO == añoSeleccionadoPedidosIngresados
                && v.DES_ART.ToLower().Contains("garan"));
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

            await refChartGarantia.RefreshAsync();
            await refChartGarantia.RefreshAsync();


        }

        protected async Task MostrarDetallePedidosAnualesVs(Syncfusion.Blazor.Charts.PointEventArgs args)
        {


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
            TituloGraficoGarantia = $"Pedidos Garantia de {nombreMes} del {añoSeleccionadoPedidosIngresados}";
            CantidadGarantiasPedidosIngresados = DataPedidosIngresados.Count(p => p.ANIO == añoSeleccionadoPedidosIngresados 
            && p.MES == mesSeleccionadoPedidosIngresados && p.DES_ART.ToLower().Contains("garan"));
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
        protected async Task MostrarDetalleDiasAtraso(Syncfusion.Blazor.Charts.PointEventArgs args)
        {
            añoSeleccionadoPedidosAltasDiasAtraso = Convert.ToInt32(args.Point.X);
            TituloGraficoMensualPedidosAlta = $"Promedio Dias Atraso Mensual {añoSeleccionadoPedidosAltasDiasAtraso}";
            PedidosAltasMensualDiasAtraso = DataPedidosAltas.Where(p => p.PEDIDO > 0 && p.ANIO == añoSeleccionadoPedidosAltasDiasAtraso)
            .OrderBy(p=> p.FECHA)
            .Select(d => new ChartData()
            {
                XSerieName = d.MES.ToString(),
                YSerieName = Math.Abs((d.FECHA_PREVISTA.HasValue ? d.FECHA_PREVISTA.Value - d.FECHA_DATE : DateTime.Now - d.FECHA_DATE).TotalDays)
            })
            .GroupBy(g => new { g.XSerieName })
            .Select(d => new ChartData()
            {
                XSerieName = d.Key.XSerieName,
                YSerieName = d.Average(p => p.YSerieName)
            })
            .ToList();


            PromedioPedidosAltaDiasAtrasoMensual = Convert.ToInt32(PedidosAltasMensualDiasAtraso.Average(d => d.YSerieName));

            DataPedidosAltasDiasAtrasoDetalle = DataPedidosAltas.Where(p => p.PEDIDO > 0 && p.ANIO == añoSeleccionadoPedidosAltasDiasAtraso).OrderBy(p=> p.FECHA).ToList();
            await InvokeAsync(StateHasChanged);
            await refChartPedidosAltaDiasAtrasoMensual.RefreshAsync();
            await refChartPedidosAltaDiasAtrasoMensual.RefreshAsync();
            await grdDiasAtraso.AutoFitColumnsAsync();

        }

        protected async Task MostrarDetalleDiasAtrasoMes(Syncfusion.Blazor.Charts.PointEventArgs args)
        {
            var mes = Convert.ToInt32(args.Point.X);


            DataPedidosAltasDiasAtrasoDetalle = DataPedidosAltas.Where(p => p.PEDIDO > 0 
                && p.ANIO == añoSeleccionadoPedidosAltasDiasAtraso && p.MES == mes).OrderBy(p => p.FECHA).ToList();
            await InvokeAsync(StateHasChanged);

            await grdDiasAtraso.AutoFitColumnsAsync();

        }

        protected async Task MostrarDetalleUE(Syncfusion.Blazor.Charts.PointEventArgs args)
        {
            añoSeleccionadoPedidosIngresadosUE = Convert.ToInt32(args.Point.X);

            PedidosIngresadosUEMensuales = DataPedidosIngresados
                .Where(p => p.ANIO == añoSeleccionadoPedidosIngresadosUE).OrderBy(p => p.FECHA)
                .GroupBy(g=> new { g.MES })
                .Select(d => new ChartData()
                {
                    XSerieName = d.Key.MES.ToString(),
                    YSerieName = Convert.ToDouble(d.Sum(p=> p.UNIDEQUI))
                })
                .ToList();

            DataPedidosIngresadosUEDetalle = DataPedidosIngresados
                .Where(p => p.ANIO == añoSeleccionadoPedidosIngresadosUE).OrderBy(p => p.FECHA).ToList();

            PromedioPedidosIngresadosMensualesUE = Convert.ToInt32(PedidosIngresadosUEMensuales.Average(p => p.YSerieName));

            await InvokeAsync(StateHasChanged);

            
            await refChartDetallePedidosIngresadosUE.RefreshAsync();
            await refChartDetallePedidosIngresadosUE.RefreshAsync();
            

        }

        protected async Task MostrarDetalleUEMes(Syncfusion.Blazor.Charts.PointEventArgs args)
        {
            var mes = Convert.ToInt32(args.Point.X);


            DataPedidosIngresadosUEDetalle = DataPedidosIngresados.Where(p => p.PEDIDO > 0
                && p.ANIO == añoSeleccionadoPedidosAltasDiasAtraso && p.MES == mes).OrderBy(p => p.FECHA).ToList();
            await InvokeAsync(StateHasChanged);

            

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
