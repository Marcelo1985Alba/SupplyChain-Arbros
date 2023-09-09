using SupplyChain.Client.HelperService.Base;
using SupplyChain.Client.RepositoryHttp;
using SupplyChain.Shared;

namespace SupplyChain.Client.HelperService;

/// <summary>
///     administracion de clientes. Tanto internos como externos.
/// </summary>
public class CondicionPagoService : BaseService<vCondicionesPago, int>
{
    private const string API = "api/CondicionesPago";

    public CondicionPagoService(IRepositoryHttp httpClient) : base(httpClient, API)
    {
    }
}