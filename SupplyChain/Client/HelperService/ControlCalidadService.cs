using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SupplyChain.Client.HelperService.Base;
using SupplyChain.Client.RepositoryHttp;
using SupplyChain.Shared;

//MODELO Procesos
//Public Class Cargavalores service = control de calidad / llamar todo como control de calidd

namespace SupplyChain.Client.HelperService;

public class ControlCalidadService : BaseService<Procesos, int>
{
    private const string API = "api/Procesos";
    private readonly IRepositoryHttp httpClient2;

    public ControlCalidadService(IRepositoryHttp httpClient) : base(httpClient, API)
    {
        httpClient2 = httpClient;
    }

    public async Task<bool> Existe(int VALE)
    {
        var response = await http.GetFromJsonAsync<bool>($"{API}/Existe/{VALE}");
        if (response.Error)
        {
            Console.WriteLine(await response.HttpResponseMessage.Content.ReadAsStringAsync());
            return false;
        }

        return response.Response;
    }

    public async Task<bool> Eliminar(List<Procesos> valores)
    {
        var response = await http.PostAsJsonAsync($"{API}/PostList", valores);
        if (response.Error)
        {
            Console.Write(await response.HttpResponseMessage.Content.ReadAsStringAsync());
            return false;
        }

        return true;
    }

    public async Task<HttpResponseWrapper<List<vControlCalidadPendientes>>> GetControlCalidad(int registro)
    {
        var response =
            await httpClient2.GetFromJsonAsync<List<vControlCalidadPendientes>>(
                $"api/ControlCalidad/byRegistro/{registro}");

        return response;
    }

    public async Task<bool> Agregar(int VALE)
    {
        var response = await http.GetFromJsonAsync<bool>($"{API}/Existe/{VALE}");
        if (response.Error)
        {
            Console.WriteLine(await response.HttpResponseMessage.Content.ReadAsStringAsync());
            return false;
        }

        return response.Response;
    }
}