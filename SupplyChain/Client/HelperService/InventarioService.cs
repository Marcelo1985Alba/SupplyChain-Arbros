using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using SupplyChain.Shared;

namespace SupplyChain.Client.HelperService;

public class InventarioService : IDisposable
{
    private const string API = "api/Controlcalidad";
    private readonly HttpClient Http;

    public InventarioService(HttpClient http)
    {
        Http = http;
    }

    public void Dispose()
    {
        ((IDisposable)Http).Dispose();
    }

    public async Task<int> GetProximoVale()
    {
        var vale = await Http.GetFromJsonAsync<int>("api/Stock/GetMaxVale");
        return vale;
    }

    public async Task<List<Pedidos>> GetVale(int vale)
    {
        return await Http.GetFromJsonAsync<List<Pedidos>>($"api/Stock/ByNumeroVale/{vale}");
    }

    public async Task<List<Pedidos>> GetPendienteAprobacion()
    {
        return await Http.GetFromJsonAsync<List<Pedidos>>("api/Stock/GetPendienteAprobacion/");
    }

    public async Task<List<Pedidos>> GetControlCalidad()
    {
        return await Http.GetFromJsonAsync<List<Pedidos>>("api/Stock/GetControlCalidad/");
    }

    public async Task<List<vControlCalidadPendientes>> GetControlCalidadPendientes()
    {
        return await Http.GetFromJsonAsync<List<vControlCalidadPendientes>>("api/Stock/GetControlCalidadPendientes/");
    }

    public async Task<List<vControlCalidadPendientes>> GetSegundaGrilla()
    {
        return await Http.GetFromJsonAsync<List<vControlCalidadPendientes>>("api/Stock/GetSegundaGrilla/");
    }
    //ADD METODOS EXISTE Y ELIMINAR
    //public async Task<bool> Existe(int id)
    //{
    //    var response = await Http.GetFromJsonAsync<bool>($"{API}/Existe/{id}");
    //    if (response.Error)
    //    {
    //        Console.WriteLine(await response.HttpResponseMessage.Content.ReadAsStringAsync());
    //        return false;
    //    }
    //    return response.Response;
    //}
}