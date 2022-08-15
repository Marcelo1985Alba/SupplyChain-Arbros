using Microsoft.JSInterop;
using SupplyChain.Client.HelperService.Base;
using SupplyChain.Client.RepositoryHttp;
using SupplyChain.Shared;
using SupplyChain.Shared.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace SupplyChain.Client.HelperService
{
    /// <summary>
    /// Servicio que admininstra el estado de pedidos refenricia a la vistas VEstadoPedido
    /// </summary>
    public class EstadoPedidoService : BaseService<vEstadoPedido, int>
    {
        private const string API = "api/EstadoPedidos";

        public EstadoPedidoService(IRepositoryHttp httpClient): base(httpClient, API)
        {
        }

        /// <summary>
        /// Carga una vista con los nombres de producto y cliente
        /// </summary>
        /// <returns></returns>
        public async Task<HttpResponseWrapper<List<vEstadoPedido>>> ByEstado(EstadoPedido estado = EstadoPedido.Todos)
        {
            return await http.GetFromJsonAsync<List<vEstadoPedido>>($"{API}/ByEstado/{estado}");
            
        }


        public async Task<HttpResponseWrapper<PedCli>> GetByOCI(int oci)
        {
            return await http.GetFromJsonAsync<PedCli>($"{API}/GetByOCI/{oci}");
        }

    }
}
