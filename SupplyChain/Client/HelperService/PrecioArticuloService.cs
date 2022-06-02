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
    /// <summary>
    /// Servicio que admininstra las solicitudes
    /// </summary>
    public class PrecioArticuloService : BaseService<PreciosArticulos, string>
    {
        private const string API = "api/PrecioArticulos";

        public PrecioArticuloService(IRepositoryHttp httpClient): base(httpClient, API)
        {
        }



    }
}
