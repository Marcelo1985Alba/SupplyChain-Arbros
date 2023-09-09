using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using SupplyChain.Client.RepositoryHttp;
using SupplyChain.Client.Shared;
using SupplyChain.Shared;

namespace SupplyChain.Client.Pages.Inventarios.Listados;

public class StockInventariosBase : ComponentBase
{
    protected List<StockSP> DataSource;
    protected FilterMovimientosStock filter = new();
    protected DateTime hasta = DateTime.Now;
    protected string[] InitialGroup = { "Tipo_Insumo" };
    protected bool spinnerVisible;
    [Inject] public IRepositoryHttp Http { get; set; }
    [CascadingParameter] public MainLayout ML { get; set; }

    protected override async Task OnInitializedAsync()
    {
        ML.Titulo = "Listado de Stock de Inventarios";
    }

    protected async Task Buscar()
    {
        spinnerVisible = true;
        //DataSource = await Http.GetFromJsonAsync<List<StockSP>>(GeneraUrl());
        var response = await Http.GetFromJsonAsync<List<StockSP>>(GeneraUrl());
        if (response.Error)
        {
            Console.WriteLine(response.HttpResponseMessage.ReasonPhrase);
            Console.WriteLine(await response.HttpResponseMessage.Content.ReadAsStringAsync());
        }
        else
        {
            DataSource = response.Response;
        }

        spinnerVisible = false;
    }

    private string GeneraUrl()
    {
        filter.Hasta = hasta.ToString("yyyyMMdd");
        var api = "api/Stock/StockInventario";

        api += $"?Deposito={filter.Deposito}&Hasta={filter.Hasta}";
        Console.WriteLine(api);
        return api;
    }

    protected async Task ChangeDeposito(Deposito deposito)
    {
        filter.Deposito = deposito.CG_DEP;
    }

    protected async Task LimpiarFiltros()
    {
        hasta = DateTime.Now;

        filter = new FilterMovimientosStock
        {
            Deposito = 0,
            Hasta = hasta.ToString("dd/MM/yyyy")
        };

        DataSource = new List<StockSP>();
    }
}