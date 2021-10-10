using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SupplyChain.Client.HelperService
{
    public class InventarioService
    {
        private readonly HttpClient Http;

        public InventarioService(HttpClient http )
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


    }
}
