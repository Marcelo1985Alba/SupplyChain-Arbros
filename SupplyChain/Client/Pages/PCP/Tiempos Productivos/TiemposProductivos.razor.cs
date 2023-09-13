using Microsoft.AspNetCore.Components;
using SupplyChain.Client.Shared;
using SupplyChain.Shared.PCP;
using Syncfusion.Blazor.Charts;
using Syncfusion.Blazor.PivotView;
using Syncfusion.Blazor.Spinner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SupplyChain.Client.Pages.PCP.Tiempos_Productivos
{
    public class TiemposProductivosBase: ComponentBase
    {
        [Inject] protected HttpClient Http { get; set; }
        protected SfChart refChartDetalle;
        protected List<vProdMaquinaDataCore> vProdMaquinaOriginal = new();
        protected List<vProdMaquinaDataCore> vProdMaquinaDetalle = new();
        protected List<ChartData> vProdMaquinaCM1 = new();
        protected List<ChartData> vProdMaquinaCN1 = new();
        protected List<ChartData> vProdMaquinaCN2 = new();
        protected List<ChartData> vProdMaquinaCN3 = new();
        protected List<ChartData> vProdMaquinaCN4 = new();

        protected List<ChartData> vProdMaquinaMes = new();
        protected List<ChartData> vProdMaquinaMesSetup = new();
        protected List<ChartData> vProdMaquinaMesParadas = new();
        protected SfSpinner SpinnerObj;
        protected bool SpinnerVisible = false;

        protected string TituloGraficoMensual = "";
        protected string maquinaSeleccionada = "";
        protected int añoSeleccionado;
        [CascadingParameter] public MainLayout Mainlayout { get; set; }


        protected class ChartData
        {
            public string XSerieName { get; set; }
            public double YSerieName { get; set; }
        }

        protected async override Task OnInitializedAsync()
        {
            Mainlayout.Titulo = "Tiempos Productivos";
            SpinnerVisible = true;
            vProdMaquinaOriginal =  await Http.GetFromJsonAsync<List<vProdMaquinaDataCore>>("api/TiemposProdcutivosDataCore");


            vProdMaquinaCM1= vProdMaquinaOriginal.Where(d=> d.Maquina.Trim() == "CM1")
                .GroupBy(c=> new { c.Año }).Select(d=> new ChartData() {
                XSerieName = d.Key.Año.ToString(),
                YSerieName = Convert.ToDouble(d.Sum(p=> p.TiempoNetoHoras))
            }).ToList();
            
            vProdMaquinaCN1= vProdMaquinaOriginal.Where(d=> d.Maquina.Trim() == "CN1")
                .GroupBy(c=> new { c.Año }).Select(d=> new ChartData() {
                XSerieName = d.Key.Año.ToString(),
                YSerieName = Convert.ToDouble(d.Sum(p=> p.TiempoNetoHoras))
            }).ToList();
            vProdMaquinaCN2= vProdMaquinaOriginal.Where(d=> d.Maquina.Trim() == "CN2")
                .GroupBy(c=> new { c.Año }).Select(d=> new ChartData() {
                XSerieName = d.Key.Año.ToString(),
                YSerieName = Convert.ToDouble(d.Sum(p=> p.TiempoNetoHoras))
            }).ToList();
            vProdMaquinaCN3= vProdMaquinaOriginal.Where(d=> d.Maquina.Trim() == "CN3")
                .GroupBy(c=> new { c.Año }).Select(d=> new ChartData() {
                XSerieName = d.Key.Año.ToString(),
                YSerieName = Convert.ToDouble(d.Sum(p=> p.TiempoNetoHoras))
            }).ToList();
            vProdMaquinaCN4= vProdMaquinaOriginal.Where(d=> d.Maquina.Trim() == "CN4")
                .GroupBy(c=> new { c.Año }).Select(d=> new ChartData() {
                XSerieName = d.Key.Año.ToString(),
                YSerieName = Convert.ToDouble(d.Sum(p=> p.TiempoNetoHoras))
            }).ToList();

            
            SpinnerVisible = false;
        }

        protected async Task MostrarDetalle(Syncfusion.Blazor.Charts.PointEventArgs args)
        {
            añoSeleccionado = Convert.ToInt32(args.Point.X);
            maquinaSeleccionada = args.Series.Name;
            TituloGraficoMensual = $"Tiempos Productivos Mensual {maquinaSeleccionada}";

            vProdMaquinaDetalle = new();

            vProdMaquinaMes = vProdMaquinaOriginal
                .Where(v=> v.Año == añoSeleccionado && v.Maquina.Trim() == maquinaSeleccionada.Trim())
                .OrderBy(o=> o.Mes)
                .GroupBy(g=> new { g.Mes }).Select(d => new ChartData()
                {
                    XSerieName = d.Key.Mes.ToString(),
                    YSerieName = Math.Round(Convert.ToDouble(d.Sum(p => p.TiempoNetoHoras)))
                }).ToList();


            vProdMaquinaMesParadas = vProdMaquinaOriginal
                .Where(v => v.Año == Convert.ToInt32(añoSeleccionado) && v.Maquina.Trim() == maquinaSeleccionada.Trim())
                .OrderBy(o => o.Mes)
                .GroupBy(g => new { g.Mes }).Select(d => new ChartData()
                {
                    XSerieName = d.Key.Mes.ToString(),
                    YSerieName = Math.Round(Convert.ToDouble(d.Sum(p => p.ParadasPlanHoras)))
                }).ToList();
            
            vProdMaquinaMesSetup = vProdMaquinaOriginal
                .Where(v => v.Año == Convert.ToInt32(añoSeleccionado) && v.Maquina.Trim() == maquinaSeleccionada.Trim())
                .OrderBy(o => o.Mes)
                .GroupBy(g => new { g.Mes }).Select(d => new ChartData()
                {
                    XSerieName = d.Key.Mes.ToString(),
                    YSerieName = Math.Round(Convert.ToDouble(d.Sum(p => p.SetupRealHoras)),2)
                }).ToList();


            await InvokeAsync(StateHasChanged);
            await refChartDetalle.RefreshAsync();
            await refChartDetalle.RefreshAsync();


        }

        protected async Task MostrarDetalleMes(Syncfusion.Blazor.Charts.PointEventArgs args)
        {
            var mes = Convert.ToInt32(args.Point.X);


            vProdMaquinaDetalle = vProdMaquinaOriginal
                .Where(v => v.Año == añoSeleccionado && v.Mes == mes && v.Maquina.Trim() == maquinaSeleccionada.Trim())
                .OrderBy(o => o.FechaFin)
                .ToList();

            await InvokeAsync(StateHasChanged);
        }

    }
}
