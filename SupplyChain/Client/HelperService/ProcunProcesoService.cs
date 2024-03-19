using SupplyChain.Client.HelperService.Base;
using SupplyChain.Client.RepositoryHttp;
using SupplyChain.Shared;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SupplyChain.Client.HelperService
{
    public class ProcunProcesoService : BaseService<ProcunProceso,int>
    {
        private const string API = "api/ProcunProcesos";

        public ProcunProcesoService(IRepositoryHttp httpClient) : base(httpClient, API)
        {

        }

        public async Task<HttpResponseWrapper<List<ProcunProceso>>> GetProcedimientos()
        {
            return await http.GetFromJsonAsync<List<ProcunProceso>>($"{API}/GetProcedimientos");
        }

        public async Task<bool> Eliminar(List<ProcunProceso> procunProcesos)
        {
            var response = await http.PostAsJsonAsync<List<ProcunProceso>>($"{API}/PostList", procunProcesos);
            if (response.Error)
            {
                Console.WriteLine(await response.HttpResponseMessage.Content.ReadAsStringAsync());
                return false;
            }
            return true;
        }

        public async Task<bool> Existe(int id)
        {
            var response = await http.GetFromJsonAsync<bool>($"{API}/Existe{id}");
            if (response.Error)
            {
                Console.WriteLine(await response.HttpResponseMessage.Content.ReadAsStringAsync());
                return false;
            }
            return response.Response;
        }

    }
}
