using Microsoft.AspNetCore.Mvc;
using Microsoft.JSInterop;
using SupplyChain.Client.HelperService.Base;
using SupplyChain.Client.Pages.ABM.Prods;
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
    public class CampoComService : BaseService<CampoComodin, int>
    {
        private const string API = "api/CampoCom";

        public CampoComService(IRepositoryHttp httpClient) : base(httpClient, API)
        {
        }

        public async Task<bool>Eliminar(List<CampoComodin>campo)
        {
            var response = await http.PostAsJsonAsync<List<CampoComodin>>($"{API}/PostList",campo);
            if (response.Error)
            {
                Console.WriteLine(await response.HttpResponseMessage.Content.ReadAsStringAsync());
                return false;
            }
            return true;
        }

        public async Task<bool>Existe(int id)
        {
            var response = await http.GetFromJsonAsync<bool>($"{API}/Existe/{id}");
            if (response.Error)
            {
                Console.WriteLine(await response.HttpResponseMessage.Content.ReadAsStringAsync());
                return false;
            }
            return true;
        }
     
    }
}
