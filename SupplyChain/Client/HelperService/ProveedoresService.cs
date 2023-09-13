using Microsoft.JSInterop;
using SupplyChain.Client.HelperService.Base;
using SupplyChain.Client.RepositoryHttp;
using SupplyChain.Shared;
using SupplyChain.Shared.Enum;
using SupplyChain.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace SupplyChain.Client.HelperService
{
    public class ProveedoresService : BaseService<vProveedorItris, int>
    {
        private const string API = "api/Proveedores";

        public ProveedoresService(IRepositoryHttp httpClient): base(httpClient, API)
        {
        }

        public async Task<HttpResponseWrapper<List<vProveedorItris>>> GetProveedoresItris()
        {
            var response = await base.http.GetFromJsonAsync<List<vProveedorItris>>($"{API}/GetProveedoresItris");
            return response;
        }
    }
}
