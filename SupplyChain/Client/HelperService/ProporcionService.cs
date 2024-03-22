using Microsoft.JSInterop;
using SupplyChain.Client.HelperService.Base;
using SupplyChain.Client.RepositoryHttp;
using SupplyChain.Shared;

namespace SupplyChain.Client.HelperService
{
    public class ProporcionService : BaseService<Proporcion,int>
    {
        private const string API = "api/Proporcion";
        private readonly IJSRuntime _js;
        private readonly IRepositoryHttp httpClient;

        public ProporcionService(IRepositoryHttp httpClient, IJSRuntime jS): base(httpClient,API) 
        {
            this.httpClient = httpClient;
            this._js = jS;
        }
    }
}
