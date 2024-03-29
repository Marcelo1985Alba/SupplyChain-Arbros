﻿using Microsoft.JSInterop;
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
    /// administracion de clientes. Tanto internos como externos.
    /// </summary>
    public class CondicionPagoService : BaseService<vCondicionesPago, int>
    {
        private const string API = "api/CondicionesPago";

        public CondicionPagoService(IRepositoryHttp httpClient): base(httpClient, API)
        {
        }

    }
}
