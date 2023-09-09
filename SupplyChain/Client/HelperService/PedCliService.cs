using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using SupplyChain.Client.HelperService.Base;
using SupplyChain.Client.RepositoryHttp;
using SupplyChain.Shared.Logística;

namespace SupplyChain.Client.HelperService;

/// <summary>
///     Servicio que admininstra los pedidos
/// </summary>
public class PedCliService : BaseService<PedCli, int>
{
    private const string API = "api/PedCli";
    private readonly IJSRuntime js;

    public PedCliService(IJSRuntime js, IRepositoryHttp httpClient) : base(httpClient, API)
    {
        this.js = js;
    }

    public async Task<HttpResponseWrapper<PedCliEncabezado>> GetPedidoEncabezadoById(int id)
    {
        return await http.GetFromJsonAsync<PedCliEncabezado>($"{API}/GetPedidoEncabezadoById/{id}");
    }

    public async Task<HttpResponseWrapper<PedCliEncabezado>> GetPedidoEncabezadoByNumOci(int numOci)
    {
        return await http.GetFromJsonAsync<PedCliEncabezado>($"{API}/GetPedidoEncabezadoByNumOci/{numOci}");
    }


    public async Task<HttpResponseWrapper<List<PedCli>>> GuardarLista(List<PedCli> lista)
    {
        return await http.PostAsJsonAsync($"{API}/PostList", lista);
    }

    internal async ValueTask ImprimirNumOci(int numOci)
    {
        await js.InvokeVoidAsync("descargarPedido", numOci);
    }

    public async Task<HttpResponseMessage> EliminarPedido(int id)
    {
        return await http.DeleteAsync($"{API}/{id}");
    }

    public async Task<HttpResponseMessage> EliminarOCI(int oci)
    {
        return await http.DeleteAsync($"{API}/PorOci/{oci}");
    }
}