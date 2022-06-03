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
    public class CondicionEntregaService : BaseService<vCondicionesEntrega, int>
    {
        private const string API = "api/CondicionesEntrega";

        public CondicionEntregaService(IRepositoryHttp httpClient): base(httpClient, API)
        {
        }

    }
}
