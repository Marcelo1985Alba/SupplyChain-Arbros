using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SupplyChain.Client.HelperService.Base;
using SupplyChain.Client.RepositoryHttp;

namespace SupplyChain.Client.HelperService;

public class AreasService : BaseService<Areas, int>
{
    private const string API = "api/Areas";

    public AreasService(IRepositoryHttp httpClient) : base(httpClient, API)
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

    public async Task<bool> Eliminar(List<Areas> areas)
    {
        var response = await http.PostAsJsonAsync($"{API}/PostList", areas);
        if (response.Error)
        {
            Console.WriteLine(await response.HttpResponseMessage.Content.ReadAsStringAsync());
            return false;
        }

        return true;
    }
}