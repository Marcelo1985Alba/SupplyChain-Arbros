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
        private readonly IJSRuntime js;

        public StockService(IJSRuntime js, IRepositoryHttp httpClient): base(httpClient, API)
        {
            this.js = js;
        }

        public async Task<HttpResponseWrapper<List<Pedidos>>> GetRemitos(TipoFiltro tipoFiltro = TipoFiltro.Todos)
        {
            return await http.GetFromJsonAsync<List<Pedidos>>($"{API}/GetRemitos/{tipoFiltro}");
        }
        
        public async Task<HttpResponseWrapper<PedidoEncabezado>> GetPedidoEncabezadoByRemito(string remito)
        {
            return await http.GetFromJsonAsync<PedidoEncabezado>($"{API}/GetRemito/{remito}");
        }

        public async Task<HttpResponseWrapper<PedidoEncabezado>> GetPedidoEncabezadoById(int id)
        {
            return await http.GetFromJsonAsync<PedidoEncabezado>($"{API}/GetPedidoEncabezadoById/{id}");
        }

        public async Task<HttpResponseWrapper<PedidoEncabezado>> GetPedidoEncabezadoByLista(List<int> pedidoIds)
        {
            var baseUri = $"{API}/GetListaByPedidos?";
            var queryString = "";
            for (int i = 0; i < pedidoIds.Count; i++)
            {
                if (i == 0)
                {
                    queryString = $"pedidoIds={pedidoIds[i]}";
                }
                else
                {
                    queryString += $"&pedidoIds={pedidoIds[i]}";
                }

                
            }
            var url = $"{baseUri}{queryString}";
            return await http.GetFromJsonAsync<PedidoEncabezado>(url);
        }

        public async Task<HttpResponseWrapper<List<Pedidos>>> GuardarLista(List<Pedidos> lista)
        {
            return await http.PostAsJsonAsync($"api/Pedidos/PostList", lista);
        }

        internal async ValueTask Imprimir(string remito, bool conEtiqueta)
        {
            await js.InvokeVoidAsync("descargarRemito", remito);
            if (conEtiqueta)
            {
                await ImprimirEtiquetaDeRemito(remito);
            }
        }

        internal async ValueTask ImprimirEtiquetaDeRemito(string remito)
        {
            await js.InvokeVoidAsync("descargarEtiquetaDeRemito", remito);
        }
    }
}
