using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SupplyChain.Client.HelperService.Base;
using SupplyChain.Client.RepositoryHttp;

namespace SupplyChain.Client.HelperService;

public class AspNetRolesService : BaseService<AspNetRoles, string>
{
    private const string API = "api/AspNetRoles";

    public AspNetRolesService(IRepositoryHttp httpClient) : base(httpClient, API)
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

    public async Task<bool> Eliminar(List<AspAmb> aspAmb)
    {
        var response = await http.PostAsJsonAsync($"{API}/PostList", aspAmb);
        if (response.Error)
        {
            Console.WriteLine(await response.HttpResponseMessage.Content.ReadAsStringAsync());
            return false;
        }

        return true;
    }
}