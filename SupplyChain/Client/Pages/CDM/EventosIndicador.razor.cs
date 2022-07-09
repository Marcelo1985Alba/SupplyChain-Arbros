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

namespace SupplyChain.Client.Pages
{

    public class EventosIndicadorCopia : ComponentBase
    {
        [Inject] public HttpClient Http { get; set; }
        [CascadingParameter] public MainLayout MainLayout { get; set; }

        protected SfDashboardLayout dashboardObject;
        protected SfChart refChartDetalle;
        protected SfSpinner refSpinner;
        protected bool VisibleSpinner = false;

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

            await GetEventos();
            //await GetProyectos();
            VisibleSpinner = false;
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
            .Where(v => v.MES == Convert.ToInt32(mes) && v.ANIO == Convert.ToInt32(añoEventoSeleccionado))
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
                    && !string.IsNullOrEmpty(v.DES_PROVE) && v.MES == Convert.ToInt32(mes))
            .GroupBy(g => new { g.DES_PROVE }).Select(d => new ChartData()
            {
                XSerieName = d.Key.DES_PROVE.Trim(),
                YSerieName = Math.Round(Convert.ToDouble(d.Count()))
            }).ToList();

            StateHasChanged();
            await refChartDetalleEventosMesTipo.RefreshAsync();
            await refChartDetalleEventosProveedor.RefreshAsync();
        }
    }
}
