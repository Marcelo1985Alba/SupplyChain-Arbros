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

namespace SupplyChain.Client.Pages.PCP.Unidades_Equivalentes
{
    public class BaseEstadUnidEquiv : ComponentBase
    {
        [Inject] public HttpClient Http { get; set; }
        [CascadingParameter] public MainLayout MainLayout { get; set; }


        protected SfSpinner refSpinner;
        protected bool VisibleSpinner = false;

        public List<vEstadPedidosAlta> DataPedidosAltas { get; set; }
        protected int AñoMax = DateTime.Now.Year;


        public class ChartData
        {
            public string XSerieName { get; set; }
            public double YSerieName { get; set; }
        }
        protected SfChart refChartDetalle;
        public List<vEstadPedidosIngresados> DataPedidosIngresados { get; set; }
        protected List<ChartData> PedidosIngresadosAnuales { get; set; } = new List<ChartData>();
        protected List<vEstadPedidosIngresados> PedidosIngresadosAnualesDetalle { get; set; } = new List<vEstadPedidosIngresados>();
        protected List<ChartData> PedidosIngresadosMensuales { get; set; } = new List<ChartData>();
        protected string TituloGraficoMensual = "";
        protected string SerieSelecciona = "";
        protected int PromedioPedidosIngresadosMensuales = 0;

        protected SfChart refChartDetallePedidosAlta;
        protected List<ChartData> PedidosAltasAnuales { get; set; } = new List<ChartData>();
        protected List<ChartData> PedidosAltasMensuales { get; set; } = new List<ChartData>();
        protected List<vEstadPedidosAlta> PedidosAltaAnualesDetalle { get; set; } = new List<vEstadPedidosAlta>();
        protected string TituloGraficoMensualPedidosAlta = "";
        protected string SerieSeleccionaPedidosAlta = "";
        protected int PromedioPedidosAltaMensuales = 0;

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
            this.DataPedidosIngresados = await Http.GetFromJsonAsync<List<vEstadPedidosIngresados>>("api/EstadisticaVentas/PedidosIngresados");

            PedidosIngresadosAnuales = DataPedidosIngresados.GroupBy(g => new { g.ANIO })
            .Select(d => new ChartData()
            {
                XSerieName = d.Key.ANIO.ToString(),
                YSerieName = Convert.ToDouble(d.Sum(p => p.UNIDEQUI))
            }).OrderBy(c => c.XSerieName)
            .ToList();
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
        }

        protected async Task MostrarDetalle(Syncfusion.Blazor.Charts.PointEventArgs args)
        {
            var año = args.Point.X;
            SerieSelecciona = args.Series.Name;
            TituloGraficoMensual = $"Unidades Equivalentes Mensuales {año}";

            PedidosIngresadosAnualesDetalle = DataPedidosIngresados.Where(p => p.ANIO == Convert.ToInt32(año)).ToList();


            PedidosIngresadosMensuales = DataPedidosIngresados
            .Where(v => v.ANIO == Convert.ToInt32(año))
            .OrderBy(o => o.MES)
            .GroupBy(g => new { g.MES }).Select(d => new ChartData()
            {
                XSerieName = d.Key.MES.ToString(),
                YSerieName = Math.Round(Convert.ToDouble(d.Sum(p => p.UNIDEQUI)))
            }).ToList();


            PromedioPedidosIngresadosMensuales = Convert.ToInt32(PedidosIngresadosMensuales.Average(p => p.YSerieName));

            await InvokeAsync(StateHasChanged);
            await refChartDetalle.RefreshAsync();
            await refChartDetalle.RefreshAsync();


        }

        protected async Task MostrarDetallePedidosAlta(Syncfusion.Blazor.Charts.PointEventArgs args)
        {
            var año = args.Point.X;
            SerieSeleccionaPedidosAlta = args.Series.Name;
            TituloGraficoMensualPedidosAlta = $"Unidades Equivalentes Mensual {año}";

            PedidosAltaAnualesDetalle = DataPedidosAltas.Where(p => p.ANIO == Convert.ToInt32(año)).ToList();

            PedidosAltasMensuales = DataPedidosAltas
            .Where(v => v.ANIO == Convert.ToInt32(año))
            .OrderBy(o => o.MES)
            .GroupBy(g => new { g.MES }).Select(d => new ChartData()
            {
                XSerieName = d.Key.MES.ToString(),
                YSerieName = Math.Round(Convert.ToDouble(d.Sum(p => p.UNIDEQUI)))
            }).ToList();

            PromedioPedidosAltaMensuales = Convert.ToInt32(PedidosAltasMensuales.Average(p => p.YSerieName));

            await InvokeAsync(StateHasChanged);
            await refChartDetallePedidosAlta.RefreshAsync();
            await refChartDetallePedidosAlta.RefreshAsync();


        }
    }
}
