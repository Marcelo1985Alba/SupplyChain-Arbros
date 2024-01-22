using Microsoft.JSInterop;
using SupplyChain.Client.HelperService.Base;
using SupplyChain.Client.RepositoryHttp;
using SupplyChain.Shared;
using SupplyChain.Shared.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Timers;

namespace SupplyChain.Client.HelperService
{
    /// <summary>
    /// Servicio que admininstra las solicitudes
    /// </summary>
    public class ServicioService : BaseService<Service, int>
    {
        private const string API = "api/Servicios";

        public ServicioService(IRepositoryHttp httpClient): base(httpClient, API)
        {
        }

        public async Task<HttpResponseWrapper<List<Service>>> GetByFilter(TipoFiltro tipoFiltro = TipoFiltro.Todos)
        {
            return await http.GetFromJsonAsync<List<Service>>($"{API}/GetByFiltro/{tipoFiltro}");
        }

        public async Task<HttpResponseWrapper<Service>> GetByPedido(int pedido)
        {
            return await http.GetFromJsonAsync<Service>($"{API}/GetByPedido/{pedido}");
        }

        public async Task<HttpResponseMessage> Eliminar(int id)
        {
            return await http.DeleteAsync($"{API}/{id}");
        }
    }
}
