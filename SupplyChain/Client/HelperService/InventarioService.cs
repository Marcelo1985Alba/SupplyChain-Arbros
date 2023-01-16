﻿using SupplyChain.Shared;
using Syncfusion.XlsIO.Implementation;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace SupplyChain.Client.HelperService
{
    public class ProcesoService : IDisposable
    {
        private const string API = "api/Proceso";
        private readonly HttpClient Http;

        public ProcesoService(HttpClient http)
        {
            this.Http = http;
        }

        public void Dispose()
        {
            ((IDisposable)Http).Dispose();
        }

        //public async Task<List<Procesos>> GetProcesos()
        //{
        //    return await Http.GetFromJsonAsync<List<Procesos>>($"api/Stock/GetProcesos/");
        //}

        public async Task<List<vControlCalidadPendientes>> GetControlCalidadPendientes()
        {
            return await Http.GetFromJsonAsync<List<vControlCalidadPendientes>>($"api/Stock/GetControlCalidadPendientes/");
        }

        public async Task<List<vControlCalidadPendientes>> GetSegundaGrilla()
        {
            return await Http.GetFromJsonAsync<List<vControlCalidadPendientes>>($"api/Stock/GetSegundaGrilla/");
        }




        //public async Task<List<Pedidos>> GetPendienteAprobacionNuevo()
        //{ 
        //    return await Http.GetFromJsonAsync<List<Pedidos>>($"api/Stock/GetPendienteAprobacionNuevo/");
        //}



        //ADD METODOS EXISTE Y ELIMINAR
        //public async Task<bool> Existe(int id)
        //{
        //    var response = await Http.GetFromJsonAsync<bool>($"{API}/Existe/{id}");
        //    if (response.Error)
        //    {
        //        Console.WriteLine(await response.HttpResponseMessage.Content.ReadAsStringAsync());
        //        return false;
        //    }
        //    return response.Response;
        //}

        //public async Task<bool> Eliminar(List<vControlCalidadPendientes> controlCalidadPendientes)
        //{
        //    var response = await Http.PostAsJsonAsync<List<vControlCalidadPendientes>>($"{API}/PostList", controlCalidadPendientes);
        //    if (response.Error)
        //    {
        //        Console.Write(await response.HttpResponseMessage.Content.ReadAsStringAsync());
        //        return false;
        //    }
        //    return true;
        //}
    }
}
