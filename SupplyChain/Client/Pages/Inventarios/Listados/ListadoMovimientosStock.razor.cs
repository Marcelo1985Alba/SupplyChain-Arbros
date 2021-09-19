using Microsoft.AspNetCore.Components;
using SupplyChain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SupplyChain.Client.Pages.Inventarios
{
    public class ListadoMovimientosStockBase : ComponentBase
    {
        [Inject] public HttpClient Http { get; set; }
        protected List<MovimientoStockSP> DataSource;
        protected bool spinnerVisible = false;


        protected async override Task OnInitializedAsync()
        {
            spinnerVisible = true;
            DataSource = await Http.GetFromJsonAsync<List<MovimientoStockSP>>("api/Stock/MovimientosStock");
            spinnerVisible = false;
        }
    }
}
