using Microsoft.JSInterop;
using SupplyChain.Client.HelperService.Base;
using SupplyChain.Client.RepositoryHttp;
using SupplyChain.Shared;

namespace SupplyChain.Client.HelperService;

public class MotivosPresupuestoService : BaseService<MotivosPresupuesto, int>
{
    private const string api = "api/MotivosPresupuesto";
    private readonly IJSRuntime _js;
    private readonly IRepositoryHttp Http;

    public MotivosPresupuestoService(IRepositoryHttp Http, IJSRuntime js) : base(Http, api)
    {
        this.Http = Http;
        _js = js;
    }
}