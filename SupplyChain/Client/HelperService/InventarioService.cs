using Syncfusion.XlsIO.Implementation;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace SupplyChain.Client.HelperService
{
    public class InventarioService : IDisposable
    {
        private readonly HttpClient Http;

        public InventarioService(HttpClient http)
        {
            this.Http = http;
        }

        public void Dispose()
        {
            ((IDisposable)Http).Dispose();
        }

        public async Task<int> GetProximoVale()
        {
            int vale = await Http.GetFromJsonAsync<int>("api/Stock/GetMaxVale");
            return vale;
        }
        public async Task<List<Pedidos>> GetVale(int vale)
        {
            return await Http.GetFromJsonAsync<List<Pedidos>>($"api/Stock/ByNumeroVale/{vale}");
        }

        public async Task<List<Procesos>> GetPendienteAprobacion()
        {
            return await Http.GetFromJsonAsync<List<Procesos>>($"api/Stock/GetPendienteAprobacion/");
        }
        public async Task<List<ProcalsMP>> GetProcalsMP()
        {
            return await Http.GetFromJsonAsync<List<ProcalsMP>>($"api/Stock/GetProcalsMP/");
        }

        //public async Task<List<Pedidos>> GetPendienteAprobacionNuevo()
        //{
        //    return await Http.GetFromJsonAsync<List<Pedidos>>($"api/Stock/GetPendienteAprobacionNuevo/");
        //}



        //ADD METODOS EXISTE Y ELIMINAR
        //public async Task<bool> Existe(int id)
        //{
        //    var response = await http.GetFromJsonAsync<bool>($"{API}/Existe/{id}");
        //    if (response.Error)
        //    {
        //        Console.WriteLine(await response.HttpResponseMessage.Content.ReadAsStringAsync());
        //        return false;
        //    }
        //    return response.Response;
        //}

        //public async Task<bool> Eliminar(List<Pedidos> valores)
        //{
        //    var response = await http.PostAsJsonAsync<List<Pedidos>>($"{API}/PostList", valores);
        //    if (response.Error)
        //    {
        //        Console.Write(await response.HttpResponseMessage.Content.ReadAsStringAsync());
        //        return false;
        //    }
        //    return true;
        //}
    }
}
