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
    public class ProductoService : BaseService<Producto, string>
    {
        private const string API = "api/Prod";

        public ProductoService(IRepositoryHttp httpClient): base(httpClient, API)
        {
        }

        public async Task<HttpResponseWrapper<List<Producto>>> GetProdAndReparaciones()
        {
            return await http.GetFromJsonAsync<List<Producto>>($"{API}/GetProdAndReparaciones");
        }
    }
}
