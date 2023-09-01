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
    public class ProcunService : BaseService<Procun, decimal>
    {
        private const string API = "api/Procun";

        public ProcunService(IRepositoryHttp httpClient): base(httpClient, API)
        {
        }

        public async Task<bool> Existe(decimal id)
        {
            var response = await http.GetFromJsonAsync<bool>($"{API}/Existe/{id}");
            if (response.Error)
            {
                Console.WriteLine(await response.HttpResponseMessage.Content.ReadAsStringAsync());
                return false;
            }

            return response.Response;
        }

        public async Task<bool> Eliminar(List<Procun> procs)
        {
            var response = await http.PostAsJsonAsync<List<Procun>>($"{API}/PostList", procs);
            if (response.Error)
            {
                Console.WriteLine(await response.HttpResponseMessage.Content.ReadAsStringAsync());
                return false;
            }

            return true;
        }

        public async Task<HttpResponseWrapper<List<Procun>>> ActualizaCelda(decimal numeroprocun,string cg_celda)
        {
            return await http.GetFromJsonAsync<List<Procun>>($"api/Procun/ActualizaCelda/{numeroprocun}/{cg_celda}");
        }

    }
}
