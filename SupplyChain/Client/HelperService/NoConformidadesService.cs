using SupplyChain.Client.HelperService.Base;
using SupplyChain.Client.RepositoryHttp;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SupplyChain.Client.HelperService
{
    public class NoConformidadesService : BaseService<NoConformidades,int>
    {
        private const string API="api/NoConformidades";

        public NoConformidadesService(IRepositoryHttp httpClient) : base(httpClient,API)
        {

        }

        public async Task<bool> Eliminar(List<vEstadEventos> noConformidades)
        {
            var response= await http.PostAsJsonAsync<List<vEstadEventos>>($"{API}/PostList",noConformidades);

            if (response.Error)
            {
                Console.WriteLine(await response.HttpResponseMessage.Content.ReadAsStringAsync());
                return false;
            }
            return true;
        }
    }
}
