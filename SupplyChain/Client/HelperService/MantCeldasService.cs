using Microsoft.JSInterop;
using SupplyChain.Client.HelperService.Base;
using SupplyChain.Client.RepositoryHttp;
using SupplyChain.Shared;
using SupplyChain.Shared.Enum;
using SupplyChain.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using System.Timers;

namespace SupplyChain.Client.HelperService
{
    public class MantCeldasService : BaseService<MantCeldas, int>
    {
        private const string API = "api/MantCeldas";

        public MantCeldasService(IRepositoryHttp httpClient): base(httpClient, API)
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

        public async Task<bool> Eliminar(List<MantCeldas> mantCeldas)
        {
            var response = await http.PostAsJsonAsync<List<MantCeldas>>($"{API}/PostList", mantCeldas);
            if (response.Error)
            {
                Console.WriteLine(await response.HttpResponseMessage.Content.ReadAsStringAsync());
                return false;
            }
            return true;
        }
        public async Task<HttpResponseWrapper<List<MantCeldas>>> ByEstado(EstadoMantCeldas estado= EstadoMantCeldas.Todos)
        {
            return await http.GetFromJsonAsync<List<MantCeldas>>($"{API}/ByEstado/{estado}");
        }
    }
}
