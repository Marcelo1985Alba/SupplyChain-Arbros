using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using SupplyChain.Client.Shared;
using SupplyChain.Shared;
using SupplyChain.Shared.Models;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Navigations;

namespace SupplyChain.Client.Pages.Inventarios;

public class ListadoMovimientosStockBase : ComponentBase
{
    protected List<MovimientoStockSP> DataSource;
    protected DateTime desde = DateTime.Now.AddMonths(-1);
    protected FilterMovimientosStock filter = new();
    protected SfGrid<MovimientoStockSP> Grid;
    protected DateTime hasta = DateTime.Now;
    protected bool spinnerVisible;

    protected List<object> Toolbaritems = new()
    {
        "Search",
        //new ItemModel { Text = "Certificado", TooltipText = "Certificado", PrefixIcon = "e-copy", Id = "Certificado" },
        "ExcelExport"
        //new ItemModel { Text = "Search", TooltipText = "OPDS", Id = "Search" }
    };

    [Inject] public HttpClient Http { get; set; }
    [CascadingParameter] public MainLayout ML { get; set; }

    protected override async Task OnInitializedAsync()
    {
        ML.Titulo = "Listado de Movimientos de Stock";
    }

    protected async Task Buscar()
    {
        spinnerVisible = true;
        DataSource = await Http.GetFromJsonAsync<List<MovimientoStockSP>>(GeneraUrl());
        spinnerVisible = false;
    }

    private string GeneraUrl()
    {
        filter.Desde = desde.ToString("yyyyMMdd");
        filter.Hasta = hasta.ToString("yyyyMMdd");
        var api = "api/Stock/MovimientosStock";

        api += $"?Tipoo={filter.Tipoo}&Desde={filter.Desde}&Hasta={filter.Hasta}";

        return api;
    }

    protected async Task ChangeTipoo(Tire tire)
    {
        filter.Tipoo = tire.Tipoo;
    }

    protected async Task ChangeDeposito(Deposito deposito)
    {
        filter.Deposito = deposito.CG_DEP;
    }

    protected async Task LimpiarFiltros()
    {
        desde = DateTime.Now.AddMonths(-1);
        hasta = DateTime.Now;

        filter = new FilterMovimientosStock
        {
            Tipoo = 0,
            Desde = desde.ToString("dd/MM/yyyy"),
            Hasta = hasta.ToString("dd/MM/yyyy")
        };

        DataSource = new List<MovimientoStockSP>();
    }

    protected async Task ClickHandler(ClickEventArgs args)
    {
        if (args.Item.Id == "GridMovStock_excelexport") // id grid + excel
        {
            ExcelExportProperties excelExportProperties = new();
            excelExportProperties.IncludeTemplateColumn = true;
            await Grid.ExcelExport(excelExportProperties);
        }
    }
}