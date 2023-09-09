using SupplyChain.Client.HelperService.Base;
using SupplyChain.Client.RepositoryHttp;
using SupplyChain.Shared;

namespace SupplyChain.Client.HelperService;

/// <summary>
///     administracion de transportes
/// </summary>
public class TransporteService : BaseService<vTransporte, int>
{
    private const string API = "api/Transportes";

    public TransporteService(IRepositoryHttp httpClient) : base(httpClient, API)
    {
    }
}