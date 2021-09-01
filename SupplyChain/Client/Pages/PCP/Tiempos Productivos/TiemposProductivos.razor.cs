using Microsoft.AspNetCore.Components;
using SupplyChain.Client.Shared;
using SupplyChain.Shared.PCP;
using Syncfusion.Blazor.PivotView;
using Syncfusion.Blazor.Spinner;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SupplyChain.Client.Pages.PCP.Tiempos_Productivos
{
    public class TiemposProductivosBase: ComponentBase
    {
        [Inject] protected HttpClient Http { get; set; }

        protected List<vProdMaquinaDataCore> vProdMaquinaDatas;
        public ChartSeriesType ChartType = ChartSeriesType.Column;
        protected List<DropDownData> ChartTypes = new List<DropDownData>() {
        new DropDownData { Name = "Column", Value = ChartSeriesType.Column },
        new DropDownData { Name = "Bar", Value = ChartSeriesType.Bar },
        new DropDownData { Name = "Line", Value = ChartSeriesType.Line },
        new DropDownData { Name = "Spline", Value = ChartSeriesType.Spline },
        new DropDownData { Name = "Area", Value = ChartSeriesType.Area },
        new DropDownData { Name = "SplineArea", Value = ChartSeriesType.SplineArea },
        new DropDownData { Name = "StepLine", Value = ChartSeriesType.StepLine },
        new DropDownData { Name = "StepArea", Value = ChartSeriesType.StepArea },
        new DropDownData { Name = "StackingColumn", Value = ChartSeriesType.StackingColumn },
        new DropDownData { Name = "StackingBar", Value = ChartSeriesType.StackingBar },
        new DropDownData { Name = "StackingArea", Value = ChartSeriesType.StackingArea },
        new DropDownData { Name = "StackingColumn100", Value = ChartSeriesType.StackingColumn100 },
        new DropDownData { Name = "StackingBar100", Value = ChartSeriesType.StackingBar100 },
        new DropDownData { Name = "StackingArea100", Value = ChartSeriesType.StackingArea100 },
        new DropDownData { Name = "Scatter", Value = ChartSeriesType.Scatter },
        new DropDownData { Name = "Bubble", Value = ChartSeriesType.Bubble },
        new DropDownData { Name = "Polar", Value = ChartSeriesType.Polar },
        new DropDownData { Name = "Radar", Value = ChartSeriesType.Radar },
        new DropDownData { Name = "Pareto", Value = ChartSeriesType.Pareto },
        new DropDownData { Name = "Pie", Value = ChartSeriesType.Pie },
        new DropDownData { Name = "Doughnut", Value = ChartSeriesType.Doughnut },
        new DropDownData { Name = "Funnel", Value = ChartSeriesType.Funnel },
        new DropDownData { Name = "Pyramid", Value = ChartSeriesType.Pyramid }
    };
        protected List<ToolbarItems> PivotToolbar = new List<ToolbarItems> {
        ToolbarItems.New,
        ToolbarItems.Save,
        ToolbarItems.SaveAs,
        ToolbarItems.Rename,
        ToolbarItems.Remove,
        ToolbarItems.Load,
        ToolbarItems.Grid,
        ToolbarItems.Chart,
        ToolbarItems.Export,
        ToolbarItems.SubTotal,
        ToolbarItems.GrandTotal,
        ToolbarItems.Formatting,
        ToolbarItems.FieldList
    };
        protected SfSpinner SpinnerObj;
        protected bool SpinnerVisible = false;
        protected class DropDownData
        {
            public string Name { get; set; }
            public ChartSeriesType Value { get; set; }
        }

        [CascadingParameter] public MainLayout Mainlayout { get; set; }
        protected async override Task OnInitializedAsync()
        {
            Mainlayout.Titulo = "Tiempos Productivos";
            vProdMaquinaDatas =  await Http.GetFromJsonAsync<List<vProdMaquinaDataCore>>("api/TiemposProdcutivosDataCore");

        }

        //protected async Task enginePopulating(EnginePopulatingEventArgs args)
        //{
        //    if (SpinnerObj != null) await SpinnerObj.ShowAsync();
            
        //}
    }
}
