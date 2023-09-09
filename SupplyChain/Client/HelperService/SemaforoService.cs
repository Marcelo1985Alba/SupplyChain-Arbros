using Microsoft.JSInterop;
using SupplyChain.Client.HelperService.Base;
using SupplyChain.Client.RepositoryHttp;
using SupplyChain.Shared;

namespace SupplyChain.Client.HelperService;

public class SemaforoService : BaseService<Semaforo, int>
{
    private const string api = "api/Semaforo";
    private readonly IJSRuntime _js;
    private readonly IRepositoryHttp Http;

    public SemaforoService(IRepositoryHttp Http, IJSRuntime js) : base(Http, api)
    {
        this.Http = Http;
        _js = js;
    }
}