using SupplyChain.Client.HelperService.Base;
using SupplyChain.Client.RepositoryHttp;
using SupplyChain.Shared;

namespace SupplyChain.Client.HelperService;

public class CondicionEntregaService : BaseService<vCondicionesEntrega, int>
{
    private const string API = "api/CondicionesEntrega";

    public CondicionEntregaService(IRepositoryHttp httpClient) : base(httpClient, API)
    {
    }
}