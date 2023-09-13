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
    public class ModulosUsuarioService : BaseService<ModulosUsuario, int>
    {
        private const string API = "api/ModulosUsuario";
        public ModulosUsuarioService(IRepositoryHttp httpClient): base(httpClient, API)
        {
        }

        public async Task<HttpResponseWrapper<List<Modulo>>> GetModulosFomUserId(string userId)
        {
            return await base.http.GetFromJsonAsync<List<Modulo>>($"{API}/GetModulosFromUserId/{userId}");
        }


        public async Task<HttpResponseWrapper<List<Modulo>>> GuardarLista(List<ModulosUsuario> lista)
        {
            return await http.Post<List<ModulosUsuario>,List<Modulo>>($"{API}/PostList", lista);
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

        public async Task<bool> Eliminar(List<Areas> areas)
        {
            var response = await http.PostAsJsonAsync<List<Areas>>($"{API}/PostList", areas);
            if (response.Error)
            {
                Console.WriteLine(await response.HttpResponseMessage.Content.ReadAsStringAsync());
                return false;
            }
            return true;
        }
    }
}
