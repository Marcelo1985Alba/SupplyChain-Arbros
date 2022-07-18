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
    public class ProyectosService : BaseService<ProyectosGBPI, int>
    {
        private const string API = "api/Proyectos";

        public ProyectosService(IRepositoryHttp httpClient): base(httpClient, API)
        {
        }
    }
}
