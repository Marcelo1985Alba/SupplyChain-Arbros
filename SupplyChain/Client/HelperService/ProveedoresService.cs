//using Microsoft.AspNetCore.Mvc;
//using Microsoft.JSInterop;
//using SupplyChain.Client.HelperService.Base;
//using SupplyChain.Client.RepositoryHttp;
//using SupplyChain.Shared;
//using SupplyChain.Shared.Enum;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net.Http;
//using System.Threading.Tasks;
//using System.Timers;

//namespace SupplyChain.Client.HelperService
//{
//    public class ProveedoresService : BaseService<vProveedorItris, int>
//    {
//        private readonly IRepositoryHttp _httpClient;
//        private readonly HttpClient Http;
//        private const string api = "API/Prove";
//        public ProveedoresService(IRepositoryHttp httpClient, string api) : base(httpClient, api)
//        {
//            _httpClient = httpClient;
            
//        }
//        [HttpGet("Get")]
//        public async Task<HttpResponseWrapper<List<vProveedorItris>>> Get()
//        {
//            return await http.GetFromJsonAsync<List<vProveedorItris>>($"api/Proveedores/Get");
//        }
        
        
//    }  
//}
