using Microsoft.JSInterop;
using SupplyChain.Client.HelperService.Base;
using SupplyChain.Client.RepositoryHttp;
using SupplyChain.Shared;
using SupplyChain.Shared.Enum;
using SupplyChain.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace SupplyChain.Client.HelperService
{
    public class ISOService : BaseService<ISO, int>
    {
        private const string API = "api/ISO";

        public ISOService(IRepositoryHttp httpClient): base(httpClient, API)
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

        public async Task<bool> Eliminar(List<ISO> iso)
        {
            var response = await http.PostAsJsonAsync<List<ISO>>($"{API}/PostList", iso);
            if (response.Error)
            {
                Console.WriteLine(await response.HttpResponseMessage.Content.ReadAsStringAsync());
                return false;
            }
            return true;
        }
    }
}
