using SupplyChain.Client.HelperService.Base;
using SupplyChain.Client.RepositoryHttp;
using SupplyChain.Shared.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SupplyChain.Client.HelperService
{
    public class ProcedimientosService : BaseService<Operaciones,int>
    {
        private const string API = "api/Procedimientos";

        public ProcedimientosService(IRepositoryHttp httpClient) : base(httpClient, API)
        {

        }

        public async Task<HttpResponseWrapper<List<Operaciones>>> GetProcedimientos()
        {
            return await http.GetFromJsonAsync<List<Operaciones>>($"{API}/GetProcedimientos");
        }

        public async Task<bool> Eliminar(List<Operaciones> operaciones)
        {
            var response = await http.PostAsJsonAsync<List<Operaciones>>($"{API}/PostList", operaciones);
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
