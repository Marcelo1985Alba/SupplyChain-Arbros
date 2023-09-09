using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using SupplyChain.Client.HelperService.Base;
using SupplyChain.Client.RepositoryHttp;
using SupplyChain.Shared;

namespace SupplyChain.Client.HelperService;

public class ProcesoService : BaseService<Procesos, int>
{
    private const string API = "api/Proceso";
    private readonly HttpClient Http;
    private readonly IRepositoryHttp iHttp;

    public ProcesoService(IRepositoryHttp httpClient) : base(httpClient, API)
    {
        iHttp = httpClient;
    }

    public async Task<List<vControlCalidadPendientes>> GetSegundaGrilla()
    {
        return await Http.GetFromJsonAsync<List<vControlCalidadPendientes>>("api/Stock/GetSegundaGrilla/");
    }

    public async Task<HttpResponseWrapper<List<Procesos>>> GuardarLista(List<Procesos> list)
    {
        return await iHttp.PostAsJsonAsync($"{API}/PostList", list);
    }
}