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
    /// administracion de clientes. Tanto internos como externos.
    /// </summary>
    public class ClienteService : BaseService<Cliente, int>
    {
        private const string API = "api/Cliente";

        public ClienteService(IRepositoryHttp httpClient): base(httpClient, API)
        {
        }

        /// <summary>
        /// Obtiene clientes de una vista que esta en otra base de datos
        /// </summary>
        /// <returns></returns>
        public async Task<HttpResponseWrapper<List<ClienteExterno>>> GetClientesExterno()
        {
            return await http.GetFromJsonAsync<List<ClienteExterno>>("api/Cliente/GetClienteExterno");
        }

        public async Task<HttpResponseWrapper<List<ClienteExterno>>> Search(int idSolutiion, string descripcion)
        {
            descripcion = string.IsNullOrEmpty(descripcion) ? "VACIO" : descripcion;

            return await http.GetFromJsonAsync<List<ClienteExterno>>($"api/Cliente/Search/{idSolutiion}/{descripcion}");
        }

    }
}
