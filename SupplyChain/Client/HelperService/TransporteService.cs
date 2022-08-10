using Microsoft.JSInterop;
using SupplyChain.Client.HelperService.Base;
using SupplyChain.Client.RepositoryHttp;
using SupplyChain.Shared;
using SupplyChain.Shared.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace SupplyChain.Client.HelperService
{
    /// <summary>
    /// administracion de transportes
    /// </summary>
    public class TransporteService : BaseService<vTransporte, int>
    {
        private const string API = "api/Transportes";

        public TransporteService(IRepositoryHttp httpClient): base(httpClient, API)
        {
        }

    }
}
