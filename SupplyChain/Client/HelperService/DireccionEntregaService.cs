using System.Collections.Generic;
using System.Threading.Tasks;
using SupplyChain.Client.HelperService.Base;
using SupplyChain.Client.RepositoryHttp;
using SupplyChain.Shared;

namespace SupplyChain.Client.HelperService;

public class DireccionEntregaService : BaseService<vDireccionesEntrega, int>
{
    private const string API = "api/DireccionesEntrega";

    public DireccionEntregaService(IRepositoryHttp httpClient) : base(httpClient, API)
    {
    }

    public async Task<HttpResponseWrapper<List<vDireccionesEntrega>>> GetByNumeroCliente(int idCliente)
    {
        return await http.GetFromJsonAsync<List<vDireccionesEntrega>>($"{API}/GetByNumeroCliente/{idCliente}");
    }
}