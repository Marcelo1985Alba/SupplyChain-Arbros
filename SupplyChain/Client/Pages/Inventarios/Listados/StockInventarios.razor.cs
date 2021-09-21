using Microsoft.AspNetCore.Components;
using SupplyChain.Client.Shared;
using SupplyChain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SupplyChain.Client.Pages.Inventarios.Listados
{
    public class StockInventariosBase : ComponentBase
    {
        [Inject] public HttpClient Http { get; set; }
        [CascadingParameter] public MainLayout ML { get; set; }
        protected List<StockSP> DataSource;
        protected bool spinnerVisible = false;
        protected DateTime hasta = DateTime.Now;
        protected FilterMovimientosStock filter = new();
        protected string[] InitialGroup = (new string[] { "Tipo_Insumo" });
        protected async override Task OnInitializedAsync()
        {
            ML.Titulo = "Listado de Stock";
        }

        protected async Task Buscar()
        {

            spinnerVisible = true;
            DataSource = await Http.GetFromJsonAsync<List<StockSP>>(GeneraUrl());
            spinnerVisible = false;
        }

        private string GeneraUrl()
        {
            filter.Hasta = hasta.ToString("dd/MM/yyyy");
            string api = "api/Stock/Stocks";

            api += $"?Deposito={filter.Deposito}&Hasta={filter.Hasta}";

            return api;
        }

        protected async Task ChangeDeposito(Deposito deposito)
        {
            filter.Deposito = deposito.CG_DEP;
        }

        protected async Task LimpiarFiltros()
        {
            hasta = DateTime.Now;

            filter = new()
            {
                Deposito = 0,
                Hasta = hasta.ToString("dd/MM/yyyy")
            };

            DataSource = new();
        }
    }
}
