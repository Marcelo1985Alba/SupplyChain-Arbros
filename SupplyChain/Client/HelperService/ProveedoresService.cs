using System.Collections.Generic;
using System.Threading.Tasks;
using SupplyChain.Client.HelperService.Base;
using SupplyChain.Client.RepositoryHttp;
using SupplyChain.Shared;

namespace SupplyChain.Client.HelperService;

public class ProveedoresService : BaseService<vProveedorItris, int>
{
    private const string API = "api/Proveedores";

    public ProveedoresService(IRepositoryHttp httpClient) : base(httpClient, API)
    {
    }

    public async Task<HttpResponseWrapper<List<vProveedorItris>>> GetProveedoresItris()
    {
        var response = await http.GetFromJsonAsync<List<vProveedorItris>>($"{API}/GetProveedoresItris");
        return response;
    }
}