using SupplyChain.Client.HelperService.Base;
using SupplyChain.Client.RepositoryHttp;
using SupplyChain.Shared;
using SupplyChain.Shared.Models;
using Syncfusion.XlsIO.Implementation;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace SupplyChain.Client.HelperService
{
    public class ProcesoService : BaseService<Procesos, int>
    {
        private const string API = "api/Proceso";
        private readonly HttpClient Http;

        public ProcesoService(IRepositoryHttp httpClient) : base(httpClient, API)
        {
        }

        public async Task<List<vControlCalidadPendientes>> GetSegundaGrilla()
        {
            return await Http.GetFromJsonAsync<List<vControlCalidadPendientes>>($"api/Stock/GetSegundaGrilla/");
        }
    }
}
