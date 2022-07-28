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
    /// Servicio que admininstra los pedidos
    /// </summary>
    public class PedCliService : BaseService<PedCli, int>
    {
        private const string API = "api/PedCli";

        public PedCliService(IRepositoryHttp httpClient): base(httpClient, API)
        {
        }


        public async Task<HttpResponseWrapper<PedCliEncabezado>> GetPedidoEncabezadoById(int id)
        {
            return await http.GetFromJsonAsync<PedCliEncabezado>($"{API}/GetPedidoEncabezadoById/{id}");
        }
    }
}
