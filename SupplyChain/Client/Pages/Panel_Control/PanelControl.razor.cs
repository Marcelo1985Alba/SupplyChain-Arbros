using Microsoft.AspNetCore.Components;
using SupplyChain.Client.Shared;
using SupplyChain.Shared;
using Syncfusion.Blazor.Charts;
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
        protected List<vEstadFacturacion> DataFacturacionDetalle = new();
        protected List<ChartData> FacturacionAnual= new();
        protected List<ChartData> FacturacionMensual= new();
        protected string TituloGraficoFacturacionMensual = "";
        protected string SerieSeleccionaFacturacion = "";
        protected int PromedioFacturacionMensual = 0;
        protected override async Task OnInitializedAsync()
        {
            MainLayout.Titulo = "Panel de Control";
            VisibleSpinner = true;

            await GetFacturacion();

            VisibleSpinner = false;
        }

        public class ChartData
        {
            public string XSerieName { get; set; }
            public double YSerieName { get; set; }
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

        protected async Task MostrarDetalle(Syncfusion.Blazor.Charts.PointEventArgs args)
        {
            var año = args.Point.X;
            SerieSeleccionaFacturacion = args.Series.Name;
            TituloGraficoFacturacionMensual = $"Facturación Mensual {año}";

            //para grilla de detalle
            DataFacturacionDetalle = DataFacturacionOriginal.Where(p => p.ANIO == Convert.ToInt32(año)).ToList();


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
    }
}
