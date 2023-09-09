using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using SupplyChain.Client.Shared;
using Syncfusion.Blazor.Charts;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Layouts;
using Syncfusion.Blazor.Notifications;
using Syncfusion.Blazor.Spinner;

namespace SupplyChain.Client.Pages;

public class EventosIndicadorCopia : ComponentBase
{
    protected string añoEventoSeleccionado = string.Empty;
    protected SfDashboardLayout dashboardObject;
    protected List<vEstadEventos> DataEventosDetalle = new();

    ///////*****************NO CONFORMIDADES*************////////////////////////////
    protected List<vEstadEventos> DataEventosOriginal = new();
    protected List<ChartData> EventosAnual = new();
    protected List<ChartData> EventosMensual = new();
    protected List<ChartData> EventosMensualTipo = new();
    protected List<ChartData> EventosProveedor = new();

    protected SfGrid<vEstadEventos> gridDetalleEventos;
    protected SfChart refChartDetalle;
    protected SfChart refChartDetalleEventos;
    protected SfChart refChartDetalleEventosMesTipo;
    protected SfChart refChartDetalleEventosProveedor;
    protected SfSpinner refSpinner;
    protected string SerieSeleccionaEventos = "";
    protected string TituloGraficoEventosMensual = "";
    protected string TituloGraficoEventosProveedor = "";
    protected string TituloGraficoEventosTipo = "";
    protected SfToast ToastObj;
    protected bool VisibleSpinner;
    [Inject] public HttpClient Http { get; set; }
    [CascadingParameter] public MainLayout MainLayout { get; set; }

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
        DataEventosOriginal = await Http.GetFromJsonAsync<List<vEstadEventos>>("api/NoConformidades/Eventos");

        EventosAnual = DataEventosOriginal.GroupBy(g => new { g.ANIO })
            .Select(d => new ChartData
            {
                XSerieName = d.Key.ANIO.ToString(),
                YSerieName = Convert.ToDouble(d.Count())
            }).OrderBy(c => c.XSerieName)
            .ToList();

        gridDetalleEventos?.PreventRender();
    }

    protected async Task MostrarDetalleEventos(PointEventArgs args)
    {
        añoEventoSeleccionado = args.Point.X.ToString();
        SerieSeleccionaEventos = args.Series.Name;
        TituloGraficoEventosMensual = $"Cantidad de Eventos en {añoEventoSeleccionado}";

        //para grilla de detalle
        gridDetalleEventos.PreventRender();
        DataEventosDetalle = new List<vEstadEventos>();
        DataEventosDetalle = DataEventosOriginal.Where(p => p.ANIO == Convert.ToInt32(añoEventoSeleccionado))
            //.OrderBy(o => new { o.ANIO, o.MES })
            .ToList();


        EventosMensual = DataEventosOriginal
            .Where(v => v.ANIO == Convert.ToInt32(añoEventoSeleccionado))
            .OrderBy(o => o.MES)
            .GroupBy(g => new { g.MES }).Select(d => new ChartData
            {
                XSerieName = d.Key.MES.ToString(),
                YSerieName = Math.Round(Convert.ToDouble(d.Count()))
            }).ToList();

        //POR TIPO
        EventosMensualTipo = DataEventosOriginal
            .Where(v => v.ANIO == Convert.ToInt32(añoEventoSeleccionado))
            .OrderBy(o => o.MES)
            .GroupBy(g => new { g.Des_TipoNc }).Select(d => new ChartData
            {
                XSerieName = d.Key.Des_TipoNc,
                YSerieName = Math.Round(Convert.ToDouble(d.Count()))
            }).ToList();

        //eventos por proveedor
        TituloGraficoEventosProveedor = $"Eventos por Proveedor en {añoEventoSeleccionado}";
        EventosProveedor = DataEventosOriginal
            .Where(v => v.ANIO == Convert.ToInt32(añoEventoSeleccionado) && !string.IsNullOrEmpty(v.DES_PROVE))
            .GroupBy(g => new { g.DES_PROVE }).Select(d => new ChartData
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

    protected async Task MostrarDetalleEventosMesTipo(PointEventArgs args)
    {
        var mes = args.Point.X;
        SerieSeleccionaEventos = args.Series.Name;
        TituloGraficoEventosMensual = $"Cantidad de Eventos en mes {mes}";

        //POR TIPO
        TituloGraficoEventosTipo = $"Eventos por Tipo en {mes}/{añoEventoSeleccionado} ";
        EventosMensualTipo = DataEventosOriginal
            .Where(v => v.MES == Convert.ToInt32(mes) && v.ANIO == Convert.ToInt32(añoEventoSeleccionado))
            .OrderBy(o => o.MES)
            .GroupBy(g => new { g.Des_TipoNc }).Select(d => new ChartData
            {
                XSerieName = d.Key.Des_TipoNc,
                YSerieName = Math.Round(Convert.ToDouble(d.Count()))
            }).ToList();


        //eventos por proveedor
        TituloGraficoEventosProveedor = $"Eventos por Proveedor en {mes}/{añoEventoSeleccionado} ";
        EventosProveedor = DataEventosOriginal
            .Where(v => v.ANIO == Convert.ToInt32(añoEventoSeleccionado)
                        && !string.IsNullOrEmpty(v.DES_PROVE) && v.MES == Convert.ToInt32(mes))
            .GroupBy(g => new { g.DES_PROVE }).Select(d => new ChartData
            {
                XSerieName = d.Key.DES_PROVE.Trim(),
                YSerieName = Math.Round(Convert.ToDouble(d.Count()))
            }).ToList();

        StateHasChanged();
        await refChartDetalleEventosMesTipo.RefreshAsync();
        await refChartDetalleEventosProveedor.RefreshAsync();
    }

    ///////**********************************************////////////////////////////

    public class ChartData
    {
        public string XSerieName { get; set; }
        public double YSerieName { get; set; }

        public string ZSerieName { get; set; }
    }

    #region "Vista Grilla"

    protected const string APPNAME = "grdEventos";
    protected string state;

    #endregion
}