﻿using Microsoft.JSInterop;
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
        public async Task<HttpResponseWrapper<List<Procun>>> ActualizaProceso(decimal numeroprocun, string proceso)
        {
            return await http.GetFromJsonAsync<List<Procun>>($"api/Procun/ActualizaCelda/{numeroprocun}/{proceso}");
        }
        public async Task<HttpResponseWrapper<List<Procun>>> PostProcun(Procun proc)
        {
            return await http.GetFromJsonAsync<List<Procun>>($"api/Procun/PostProcun/{proc}");
        }


        public async Task<HttpResponseWrapper<List<Procun>>> Search(string idProd ="VACIO", string Des_Prod = "VACIO")
        {
            //Des_Prod = string.IsNullOrEmpty(Des_Prod) ? "VACIO" : Des_Prod;
            //return await http.GetFromJsonAsync<List<Procun>>($"api/Procun/Search{idProd}/{Des_Prod}");
            if (string.IsNullOrEmpty(idProd))
            {
                idProd = "VACIO";
            }
            if (string.IsNullOrEmpty(Des_Prod))
            {
                Des_Prod = "VACIO";
            }
            return await http.GetFromJsonAsync<List<Procun>>($"api/Procun/Search/{idProd}/{Des_Prod}");
        }


    }
}
