using Microsoft.AspNetCore.Builder;
using SupplyChain.Client.HelperService.Base;
using SupplyChain.Client.RepositoryHttp;
using Syncfusion.XlsIO.Implementation;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;
namespace SupplyChain.Client.HelperService
{
    public class DepositoService : BaseService<Deposito, int>
    {
        private readonly HttpClient Http;
        private const string API = "api/Depos";

        public DepositoService(IRepositoryHttp httpClient): base(httpClient, API)
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
        public async Task <bool> Eliminar(List<Deposito> deposito)
        {
            var response = await http.PostAsJsonAsync<List<Deposito>>($"{API}/PostList", deposito);
                if (response.Error)
                {
                    Console.Write(await response.HttpResponseMessage.Content.ReadAsStringAsync());
                    return false;
                }
                return true;
            }
        }
        public async Task<List<Deposito>> GetDeposito()
        {
        return await Http.GetFromJsonAsync<List<Deposito>>($"api/Stock/GetDeposito/");
    }
    }

