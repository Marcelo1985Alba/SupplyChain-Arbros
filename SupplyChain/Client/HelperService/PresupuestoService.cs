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
    public class PresupuestoService : BaseService<Presupuesto,int>
    {
        private readonly IRepositoryHttp Http;
        private const string API = "api/Presupuestos";
        public PresupuestoService(IRepositoryHttp Http):base(Http, API)
        {
            this.Http = Http;
        }

        protected async Task<HttpResponseWrapper<List<PresupuestoAnterior>>> GetPresupuestosAnteriores()
        {
            return await Http.GetFromJsonAsync<List<PresupuestoAnterior>>("api/PresupuestosAnterior");
        }

        public async Task<HttpResponseWrapper<List<vPresupuestos>>> GetVistaParaGrilla()
        {
            return await http.GetFromJsonAsync<List<vPresupuestos>>(API+ "/GetPresupuestoVista");

        }
    }
}
