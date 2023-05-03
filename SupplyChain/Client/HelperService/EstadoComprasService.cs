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
using static System.Net.WebRequestMethods;

namespace SupplyChain.Client.HelperService
{
    public class EstadoComprasService : BaseService<vESTADOS_COMPRAS, int>
    {
        private const string API = "api/EstadoCompras";
        private readonly IJSRuntime js;

        public EstadoComprasService(IJSRuntime js, IRepositoryHttp httpClient) : base(httpClient, API)
        {
            this.js = js;
        }

        public async Task<HttpResponseWrapper<List<vESTADOS_COMPRAS>>> ByEstado(EstadoCompras estado = EstadoCompras.Todos)
        {
            return await http.GetFromJsonAsync<List<vESTADOS_COMPRAS>>($"{API}/ByEstado/{estado}");
        }

        public async Task<HttpResponseWrapper<Compras>> GetByOCI(int oci)
        {
            return await http.GetFromJsonAsync<Compras>($"{API}/GetByOCI/{oci}");
        }
    }
}
