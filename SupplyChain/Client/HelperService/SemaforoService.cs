using Microsoft.JSInterop;
using SupplyChain.Client.HelperService.Base;
using SupplyChain.Client.RepositoryHttp;
using SupplyChain.Shared;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SupplyChain.Client.HelperService
{
    public class SemaforoService : BaseService<Semaforo, int>
    {
        private readonly IRepositoryHttp Http;
        private readonly IJSRuntime _js;
        private const string API = "api/Semaforo";

        public SemaforoService(IRepositoryHttp Http, IJSRuntime js) : base( Http, API)
        {
            this.Http = Http;
            this._js= js;
        }

    }
}
