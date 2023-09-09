using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SupplyChain.Client.HelperService.Base;
using SupplyChain.Client.RepositoryHttp;

namespace SupplyChain.Client.HelperService;

public class TipoAreaService : BaseService<TipoArea, int>
{
    private const string API = "api/TipoArea";

    public TipoAreaService(IRepositoryHttp httpClient) : base(httpClient, API)
    {
    }

    public async Task<bool> Existe(int id)
    {
        var response = await http.GetFromJsonAsync<bool>($"{API}/Existe/{id}");
        if (response.Error)
        {
            Console.WriteLine(await response.HttpResponseMessage.Content.ReadAsStringAsync());
            return false;
        }

        return response.Response;
    }

    public async Task<bool> Eliminar(List<TipoArea> tipoArea)
    {
        var response = await http.PostAsJsonAsync($"{API}/PostList", tipoArea);
        if (response.Error)
        {
            Console.WriteLine(await response.HttpResponseMessage.Content.ReadAsStringAsync());
            return false;
        }

        return true;
    }
}