using SupplyChain.Client.HelperService.Base;
using SupplyChain.Client.RepositoryHttp;
using SupplyChain.Shared.ABM;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SupplyChain.Client.HelperService
{
    public class ProcedimientosService : BaseService<Procedimientos,string>
    {
        private const string API = "api/Procedimientos";

        public ProcedimientosService(IRepositoryHttp httpClient) : base(httpClient, API)
        {

        }

        public async Task<HttpResponseWrapper<List<Procedimientos>>> GetProcedimientos()
        {
            return await http.GetFromJsonAsync<List<Procedimientos>>($"{API}/GetProcedimientos");
        }

        public async Task<bool> Eliminar(List<Procedimientos> procedimientos)
        {
            var response = await http.PostAsJsonAsync<List<Procedimientos>>($"{API}/PostList", procedimientos);
            if (response.Error)
            {
                Console.WriteLine(await response.HttpResponseMessage.Content.ReadAsStringAsync());
                return false;
            }
            return true;
        }


    }
}
