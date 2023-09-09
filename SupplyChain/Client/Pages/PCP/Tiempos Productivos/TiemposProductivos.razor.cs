using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using SupplyChain.Client.Shared;
using SupplyChain.Shared.PCP;
using Syncfusion.Blazor.Charts;
using Syncfusion.Blazor.Spinner;

namespace SupplyChain.Client.Pages.PCP.Tiempos_Productivos;

public class TiemposProductivosBase : ComponentBase
{
    protected int añoSeleccionado;
    protected string maquinaSeleccionada = "";
    protected SfChart refChartDetalle;
    protected SfSpinner SpinnerObj;
    protected bool SpinnerVisible;

    protected string TituloGraficoMensual = "";
    protected List<ChartData> vProdMaquinaCM1 = new();
    protected List<ChartData> vProdMaquinaCN1 = new();
    protected List<ChartData> vProdMaquinaCN2 = new();
    protected List<ChartData> vProdMaquinaCN3 = new();
    protected List<ChartData> vProdMaquinaCN4 = new();
    protected List<vProdMaquinaDataCore> vProdMaquinaDetalle = new();

    protected List<ChartData> vProdMaquinaMes = new();
    protected List<ChartData> vProdMaquinaMesParadas = new();
    protected List<ChartData> vProdMaquinaMesSetup = new();
    protected List<vProdMaquinaDataCore> vProdMaquinaOriginal = new();
    [Inject] protected HttpClient Http { get; set; }
    [CascadingParameter] public MainLayout Mainlayout { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Mainlayout.Titulo = "Tiempos Productivos";
        SpinnerVisible = true;
        vProdMaquinaOriginal =
            await Http.GetFromJsonAsync<List<vProdMaquinaDataCore>>("api/TiemposProdcutivosDataCore");


        vProdMaquinaCM1 = vProdMaquinaOriginal.Where(d => d.Maquina.Trim() == "CM1")
            .GroupBy(c => new { c.Año }).Select(d => new ChartData
            {
                XSerieName = d.Key.Año.ToString(),
                YSerieName = Convert.ToDouble(d.Sum(p => p.TiempoNetoHoras))
            }).ToList();

        vProdMaquinaCN1 = vProdMaquinaOriginal.Where(d => d.Maquina.Trim() == "CN1")
            .GroupBy(c => new { c.Año }).Select(d => new ChartData
            {
                XSerieName = d.Key.Año.ToString(),
                YSerieName = Convert.ToDouble(d.Sum(p => p.TiempoNetoHoras))
            }).ToList();
        vProdMaquinaCN2 = vProdMaquinaOriginal.Where(d => d.Maquina.Trim() == "CN2")
            .GroupBy(c => new { c.Año }).Select(d => new ChartData
            {
                XSerieName = d.Key.Año.ToString(),
                YSerieName = Convert.ToDouble(d.Sum(p => p.TiempoNetoHoras))
            }).ToList();
        vProdMaquinaCN3 = vProdMaquinaOriginal.Where(d => d.Maquina.Trim() == "CN3")
            .GroupBy(c => new { c.Año }).Select(d => new ChartData
            {
                XSerieName = d.Key.Año.ToString(),
                YSerieName = Convert.ToDouble(d.Sum(p => p.TiempoNetoHoras))
            }).ToList();
        vProdMaquinaCN4 = vProdMaquinaOriginal.Where(d => d.Maquina.Trim() == "CN4")
            .GroupBy(c => new { c.Año }).Select(d => new ChartData
            {
                XSerieName = d.Key.Año.ToString(),
                YSerieName = Convert.ToDouble(d.Sum(p => p.TiempoNetoHoras))
            }).ToList();


        SpinnerVisible = false;
    }

    protected async Task MostrarDetalle(PointEventArgs args)
    {
        añoSeleccionado = Convert.ToInt32(args.Point.X);
        maquinaSeleccionada = args.Series.Name;
        TituloGraficoMensual = $"Tiempos Productivos Mensual {maquinaSeleccionada}";

        vProdMaquinaDetalle = new List<vProdMaquinaDataCore>();

        vProdMaquinaMes = vProdMaquinaOriginal
            .Where(v => v.Año == añoSeleccionado && v.Maquina.Trim() == maquinaSeleccionada.Trim())
            .OrderBy(o => o.Mes)
            .GroupBy(g => new { g.Mes }).Select(d => new ChartData
            {
                XSerieName = d.Key.Mes.ToString(),
                YSerieName = Math.Round(Convert.ToDouble(d.Sum(p => p.TiempoNetoHoras)))
            }).ToList();


        vProdMaquinaMesParadas = vProdMaquinaOriginal
            .Where(v => v.Año == Convert.ToInt32(añoSeleccionado) && v.Maquina.Trim() == maquinaSeleccionada.Trim())
            .OrderBy(o => o.Mes)
            .GroupBy(g => new { g.Mes }).Select(d => new ChartData
            {
                XSerieName = d.Key.Mes.ToString(),
                YSerieName = Math.Round(Convert.ToDouble(d.Sum(p => p.ParadasPlanHoras)))
            }).ToList();

        vProdMaquinaMesSetup = vProdMaquinaOriginal
            .Where(v => v.Año == Convert.ToInt32(añoSeleccionado) && v.Maquina.Trim() == maquinaSeleccionada.Trim())
            .OrderBy(o => o.Mes)
            .GroupBy(g => new { g.Mes }).Select(d => new ChartData
            {
                XSerieName = d.Key.Mes.ToString(),
                YSerieName = Math.Round(Convert.ToDouble(d.Sum(p => p.SetupRealHoras)), 2)
            }).ToList();


        await InvokeAsync(StateHasChanged);
        await refChartDetalle.RefreshAsync();
        await refChartDetalle.RefreshAsync();
    }

    protected async Task MostrarDetalleMes(PointEventArgs args)
    {
        var mes = Convert.ToInt32(args.Point.X);


        vProdMaquinaDetalle = vProdMaquinaOriginal
            .Where(v => v.Año == añoSeleccionado && v.Mes == mes && v.Maquina.Trim() == maquinaSeleccionada.Trim())
            .OrderBy(o => o.FechaFin)
            .ToList();

        await InvokeAsync(StateHasChanged);
    }


    protected class ChartData
    {
        public string XSerieName { get; set; }
        public double YSerieName { get; set; }
    }
}