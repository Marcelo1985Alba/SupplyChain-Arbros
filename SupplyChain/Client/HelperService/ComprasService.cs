using Microsoft.JSInterop;
using SupplyChain.Client.HelperService.Base;
using SupplyChain.Client.RepositoryHttp;
using SupplyChain.Shared.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SupplyChain.Client.HelperService
{
    public class ComprasService : BaseService<Compra, int>
    {
        private const string API = "api/Compra";
        private readonly IJSRuntime js;

        public ComprasService(IJSRuntime js, IRepositoryHttp httpClient) : base(httpClient, API)
        {
            this.js = js;
        }

        public async Task<HttpResponseWrapper<Compra>> Anular(int item)
        {
            return await http.GetFromJsonAsync<Compra>($"{API}/Anular/{item}");
        }
    }
}
