using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SupplyChain.Client.HelperService.Base;
using SupplyChain.Client.RepositoryHttp;
using SupplyChain.Shared.Models;

namespace SupplyChain.Client.HelperService;

public class CeldasService : BaseService<Celdas, string>
{
    private const string API = "api/Celdas";

    public CeldasService(IRepositoryHttp httpClient) : base(httpClient, API)
    {
    }

    public async Task<bool> Existe(string id)
    {
        var response = await http.GetFromJsonAsync<bool>($"{API}/Existe/{id}");
        if (response.Error)
        {
            Console.WriteLine(await response.HttpResponseMessage.Content.ReadAsStringAsync());
            return false;
        }

        return response.Response;
    }

    public async Task<bool> Eliminar(List<Celdas> celdas)
    {
        var response = await http.PostAsJsonAsync($"{API}/PostList", celdas);
        if (response.Error)
        {
            Console.WriteLine(await response.HttpResponseMessage.Content.ReadAsStringAsync());
            return false;
        }

        return true;
    }
}