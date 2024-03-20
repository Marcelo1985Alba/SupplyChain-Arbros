using SupplyChain.Client.HelperService.Base;
using SupplyChain.Client.RepositoryHttp;
using SupplyChain.Shared;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SupplyChain.Client.HelperService
{
    public class ProcunProcesoService : BaseService<Protab,string>
    {
        private const string API = "api/ProcunProcesos";

        public ProcunProcesoService(IRepositoryHttp httpClient) : base(httpClient, API)
        {

        }

        public async Task<HttpResponseWrapper<List<Protab>>> GetProcedimientos()
        {
            return await http.GetFromJsonAsync<List<Protab>>($"{API}/GetProcunProcesos");
        }

        public async Task<bool> Eliminar(List<Protab> protab)
        {
            var response = await http.PostAsJsonAsync<List<Protab>>($"{API}/PostList", protab);
            if (response.Error)
            {
                Console.WriteLine(await response.HttpResponseMessage.Content.ReadAsStringAsync());
                return false;
            }
            return true;
        }

        public async Task<bool> Existe(string id)
        {
            var response = await http.GetFromJsonAsync<bool>($"{API}/Existe{id}");
            if (response.Error)
            {
                Console.WriteLine(await response.HttpResponseMessage.Content.ReadAsStringAsync());
                return false;
            }
            return response.Response;
        }

        public async Task<HttpResponseWrapper<List<Protab>>> GetProcunProcesos()
        {
            return await http.GetFromJsonAsync<List<Protab>>($"{API}/GetProcunProcesos");
        }

        public async Task<HttpResponseWrapper<List<Protab>>> PostProcunProcesos(Protab protab)
        {
            return await http.GetFromJsonAsync<List<Protab>>($"{API}/PostProcunProcesos/{protab}");
        }
    }
}
