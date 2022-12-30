using Microsoft.AspNetCore.Builder;
using SupplyChain.Client.HelperService.Base;
using SupplyChain.Client.RepositoryHttp;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace SupplyChain.Client.HelperService
{
    public class ProcalMPService : BaseService<ProcalsMP, int>
    {
        private readonly HttpClient Http;
        private const string API = "api/ProMP";

        public ProcalMPService(IRepositoryHttp httpClient) : base(httpClient, API)
        {
        }

        public async Task<bool> Existe(int id)
        {
            var response = await http.GetFromJsonAsync<bool>($"{API}/Existe/{id}");
                if (response.Error)
                {
                 Console.WriteLine(await response.HttpResponseMessage.Content.ReadAsStringAsync());
                 return false;
                }
                return response.Response;
         }   

            public async Task<bool> Eliminar(List<ProcalsMP> procalMP)
            { 
            var response = await http.PostAsJsonAsync<List<ProcalsMP>>($"{API}/PostList", procalMP);
            if (response.Error)
                {
                    Console.Write(await response.HttpResponseMessage.Content.ReadAsStringAsync());
                    return false;
                }
                    return true;
            }

        public async Task<List<ProcalsMP>> GetProcalsMP()
        {
            return await Http.GetFromJsonAsync<List<ProcalsMP>>($"api/Stock/GetProcalsMP/");
        }
    }
}
                          
