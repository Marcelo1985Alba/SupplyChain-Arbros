using System;
using System.Threading.Tasks;
using SupplyChain.Client.HelperService.Base;
using SupplyChain.Client.RepositoryHttp;
using SupplyChain.Shared;

namespace SupplyChain.Client.HelperService;

/// <summary>
///     administracion de tipos de cambio. Tanto internos como externos.
/// </summary>
public class TipoCambioService : BaseService<vTipoCambio, int>
{
    private const string API = "api/TiposCambio";

    public TipoCambioService(IRepositoryHttp httpClient) : base(httpClient, API)
    {
    }

    public async Task<decimal> GetValorDolarHoy()
    {
        var response = await http.GetFromJsonAsync<decimal>($"{API}/GetUltimaCotizacionDolar");
        if (response.Error)
        {
            Console.WriteLine(await response.HttpResponseMessage.Content.ReadAsStringAsync());
            return 0;
        }

        return response.Response;
    }
}