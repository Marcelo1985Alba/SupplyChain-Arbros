using SupplyChain.Client.HelperService.Base;
using SupplyChain.Client.RepositoryHttp;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Threading.Tasks;

//MODELO Procesos
//Public Class Cargavalores service = control de calidad / llamar todo como control de calidd

namespace SupplyChain.Client.HelperService
{
    public class ControlCalidadService : BaseService<Procesos, int>
    {
        private const string API = "api/Procesos";

        public ControlCalidadService(IRepositoryHttp httpClient) : base(httpClient, API)
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

        public async Task<bool> Eliminar(List<Procesos> valores)
        {
            var response = await http.PostAsJsonAsync<List<Procesos>>($"{API}/PostList", valores);
            if (response.Error)
            {
                Console.Write(await response.HttpResponseMessage.Content.ReadAsStringAsync());
                return false;
            }
            return true;
        }

    }
}
