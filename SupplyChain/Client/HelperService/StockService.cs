using Microsoft.JSInterop;
using SupplyChain.Client.HelperService.Base;
using SupplyChain.Client.RepositoryHttp;
using SupplyChain.Shared;
using SupplyChain.Shared.Enum;
using SupplyChain.Shared.Logística;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace SupplyChain.Client.HelperService
{

    /// <summary>
    /// Servicio que admininstra los movimientos de stock que se encuentra en la tabla pedidos
    /// </summary>
    public class StockService : BaseService<Pedidos, int?>
    {
        private const string API = "api/Stock";

        public StockService(IRepositoryHttp httpClient): base(httpClient, API)
        {
        }

        public async Task<HttpResponseWrapper<List<Pedidos>>> GetRemitos(TipoFiltro tipoFiltro = TipoFiltro.Todos)
        {
            return await http.GetFromJsonAsync<List<Pedidos>>($"{API}/GetRemitos/{tipoFiltro}");
        }

        public async Task<HttpResponseWrapper<PedidoEncabezado>> GetPedidoEncabezadoById(int id)
        {
            return await http.GetFromJsonAsync<PedidoEncabezado>($"{API}/GetPedidoEncabezadoById/{id}");
        }

        public async Task<HttpResponseWrapper<List<Pedidos>>> GuardarLista(List<Pedidos> lista)
        {
            return await http.PostAsJsonAsync($"{API}/PostList", lista);
        }
    }
}
