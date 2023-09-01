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
    public class ProductoService : BaseService<Producto, string>
    {
        private const string API = "api/Prod";

        public ProductoService(IRepositoryHttp httpClient): base(httpClient, API)
        {
        }

        public async Task<HttpResponseWrapper<List<Producto>>> GetProdAndReparaciones()
        {
            return await http.GetFromJsonAsync<List<Producto>>($"{API}/GetProdAndReparaciones");
        }

        public async Task<bool> Eliminar(List<Producto> productos)
        {
            var response = await http.PostAsJsonAsync<List<Producto>>($"{API}/PostList", productos);
            if (response.Error)
            {
                Console.WriteLine(await response.HttpResponseMessage.Content.ReadAsStringAsync());
                return false;
            }

            return true;
        }

        public async Task<HttpResponseWrapper<List<Producto>>> Search( string idProd, string Des_producto)
        {
            Des_producto = string.IsNullOrEmpty(Des_producto) ? "VACIO" : Des_producto;
            return await http.GetFromJsonAsync<List<Producto>>($"api/Prod/Search/{idProd}/{Des_producto}");
            //if (string.IsNullOrEmpty(idProd))
            //{
            //    idProd = null;
            //}
            //if(string.IsNullOrEmpty(des_prod))
            //{
            //    des_prod = null;
            //}
            //return await http.GetFromJsonAsync<List<Producto>>($"{API}/Search/{idProd}/{des_prod}");
        }

        public async Task<HttpResponseWrapper<List<Producto>>> Get(bool conMP=true, bool conSE = true, bool conPT = true)
        {
            return await http.GetFromJsonAsync<List<Producto>>($"api/Prod/ByTipo/{conMP}/{conSE}/{conPT}");

        }

    }
}
