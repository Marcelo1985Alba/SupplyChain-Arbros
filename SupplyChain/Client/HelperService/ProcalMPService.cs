using Microsoft.AspNetCore.Builder;
using SupplyChain.Client.HelperService.Base;
using SupplyChain.Client.RepositoryHttp;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SupplyChain.Client.HelperService
{
    public class ProcalMPService : BaseService<ProcalsMP, int>
    {
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
    }
}
                          
