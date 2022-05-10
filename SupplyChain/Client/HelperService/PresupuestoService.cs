using Microsoft.JSInterop;
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
    public class PresupuestoService : IDisposable
    {
        private readonly IRepositoryHttp Http;

        public PresupuestoService(IRepositoryHttp httpClient)
        {
            this.Http = httpClient;
        }

        protected async Task<HttpResponseWrapper<List<PresupuestoAnterior>>> GetPresupuestosAnteriores()
        {
            return await Http.GetFromJsonAsync<List<PresupuestoAnterior>>("api/PresupuestosAnterior");
        }

        protected async Task<HttpResponseWrapper<List<PresupuestoAnterior>>> Get()
        {
            return await Http.GetFromJsonAsync<List<PresupuestoAnterior>>("api/Presupuestos");
        }

        protected async Task<HttpResponseWrapper<Presupuesto>> Agregar(Presupuesto presupuesto)
        {
            return await Http.PostAsJsonAsync("api/Presupuestos", presupuesto);
        }

        protected async Task<HttpResponseWrapper<object>> Actualizar(Presupuesto presupuesto)
        {
            return await Http.PutAsJsonAsync($"api/Presupuestos/{presupuesto.Id}", presupuesto);
        }


        public void Dispose()
        {
            
        }
    }
}
