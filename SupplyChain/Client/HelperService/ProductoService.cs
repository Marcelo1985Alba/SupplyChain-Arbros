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
    public class ProductoService : BaseService<Producto, string>
    {
<<<<<<< HEAD
<<<<<<< HEAD
=======
        private const string API = "api/Prod";

        public ProductoService(IRepositoryHttp httpClient) : base(httpClient, API)
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

        public async Task<HttpResponseWrapper<object>> Actualizar(Producto producto)
        {
            var response = await http.PutAsJsonAsync<Producto>($"{API}", producto);
            return response;
        }

>>>>>>> parent of 9c2d7ab (07092023 ProductoService con metodo Search y Get, FormProducto metod Actualizar actualizado)
    }
=======
        private const string API = "api/Prod";
>>>>>>> parent of 1c4fe0e (08092023 - agregue celdas y operaciones con boton de enviar al archivo php)

        public ProductoService(IRepositoryHttp httpClient) : base(httpClient, API)
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

        public async Task<HttpResponseWrapper<List<Producto>>> Search(string idProd, string Des_producto)
        {
            // Des_producto = string.IsNullOrEmpty(Des_producto) ? "VACIO" : Des_producto;
            //return await http.GetFromJsonAsync<List<Producto>>($"api/Prod/Search/{idProd}/{Des_producto}");
            if (string.IsNullOrEmpty(idProd))
            {
                idProd = "VACIO";
            }

            if (string.IsNullOrEmpty(Des_producto))
            {
                Des_producto = "VACIO";
            }
            return await http.GetFromJsonAsync<List<Producto>>($"{API}/Search/{idProd}/{Des_producto}");
        }



        public async Task<HttpResponseWrapper<List<Producto>>> Get(bool conMP = true, bool conSE = true, bool conPT = true)
        {
            return await http.GetFromJsonAsync<List<Producto>>($"api/Prod/ByTipo/{conMP}/{conSE}/{conPT}");

        }

    }
}
