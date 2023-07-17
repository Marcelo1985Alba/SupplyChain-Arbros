using Microsoft.JSInterop;
using SupplyChain.Client.HelperService.Base;
using SupplyChain.Client.RepositoryHttp;
using SupplyChain.Shared;

namespace SupplyChain.Client.HelperService
{
    public class MotivosPresupuestoService : BaseService<MotivosPresupuesto, int>
    {
        private readonly IRepositoryHttp Http;
        private readonly IJSRuntime _js;
        private const string api = "api/MotivosPresupuesto";

        public MotivosPresupuestoService(IRepositoryHttp Http, IJSRuntime js) : base(Http, api)
        {
            this.Http = Http;
            this._js = js;
        }
    }
}
