using Microsoft.JSInterop;
using SupplyChain.Client.RepositoryHttp;
using SupplyChain.Shared;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SupplyChain.Client.HelperService
{
    public class SemaforoService
    {
        private readonly IRepositoryHttp Http;
        private readonly IJSRuntime _js;
        private const string API = "api/Semaforo";

        public SemaforoService(IRepositoryHttp Http, IJSRuntime js)
        {
            this.Http = Http;
            this._js = js;
        }

        public async Task<HttpResponseWrapper<List<Semaforo>>> GetSemaforo()
        {
            return await Http.GetFromJsonAsync<List<Semaforo>>("api/Semaforo");
        }
    }
}
