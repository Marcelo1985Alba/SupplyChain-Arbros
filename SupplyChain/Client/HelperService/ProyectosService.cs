using SupplyChain.Client.HelperService.Base;
using SupplyChain.Client.RepositoryHttp;
using SupplyChain.Shared;

namespace SupplyChain.Client.HelperService;

public class ProyectosService : BaseService<ProyectosGBPI, int>
{
    private const string API = "api/Proyectos";

    public ProyectosService(IRepositoryHttp httpClient) : base(httpClient, API)
    {
    }
}