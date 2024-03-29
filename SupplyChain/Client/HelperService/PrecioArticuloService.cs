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
    /// <summary>
    /// Servicio que admininstra las solicitudes
    /// </summary>
    public class PrecioArticuloService : BaseService<PreciosArticulos, string>
    {
        private const string API = "api/PrecioArticulos";

        public PrecioArticuloService(IRepositoryHttp httpClient): base(httpClient, API)
        {
        }


        public async Task<bool> Existe(string id)
        {
            var response = await http.GetFromJsonAsync<bool>($"{API}/Existe/{id}");
            if (response.Error)
            {
                Console.WriteLine(await response.HttpResponseMessage.Content.ReadAsStringAsync());
                return false;
            }

            return response.Response;
        }

        public async Task<HttpResponseWrapper<List<PreciosArticulos>>> GetReparaciones()
        {
            return await http.GetFromJsonAsync<List<PreciosArticulos>>($"{API}/Reparaciones");
        }

        public async Task<HttpResponseWrapper<List<PreciosArticulos>>> Search(string codigo = "VACIO", string descripcion = "VACIO")
        {
            if (string.IsNullOrEmpty(codigo))
            {
                codigo = "VACIO";
            }

            if (string.IsNullOrEmpty(descripcion))
            {
                descripcion = "VACIO";
            }

            return await http.GetFromJsonAsync<List<PreciosArticulos>>($"{API}/Search/{codigo}/{descripcion}");
        }

    }
}
