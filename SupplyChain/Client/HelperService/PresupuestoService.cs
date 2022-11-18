using Microsoft.JSInterop;
using SupplyChain.Client.HelperService.Base;
using SupplyChain.Client.RepositoryHttp;
using SupplyChain.Shared;
using SupplyChain.Shared.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Timers;

namespace SupplyChain.Client.HelperService
{
    public class PresupuestoService : BaseService<Presupuesto,int>
    {
        private readonly IRepositoryHttp Http;
        private readonly IJSRuntime _js;
        private const string API = "api/Presupuestos";
        public PresupuestoService(IRepositoryHttp Http, IJSRuntime js):base(Http, API)
        {
            this.Http = Http;
            this._js = js;
        }

        protected async Task<HttpResponseWrapper<List<PresupuestoAnterior>>> GetPresupuestosAnteriores()
        {
            return await Http.GetFromJsonAsync<List<PresupuestoAnterior>>("api/PresupuestosAnterior");
        }

        public async Task<HttpResponseWrapper<List<vPresupuestos>>> GetVistaParaGrilla(TipoFiltro tipoFiltro = TipoFiltro.Todos) 
        {
            return await http.GetFromJsonAsync<List<vPresupuestos>>($"{API}/GetPresupuestoVista/{tipoFiltro}");

        }

        public async Task Imprimir(int presupuestoId)
        {
            await _js.InvokeVoidAsync("descargarPresupuestDataSheet", presupuestoId);
        }

        public async Task<HttpResponseMessage> Eliminar(int id)
        {
            return await http.DeleteAsync($"{API}/{id}");
        }

        public async Task<HttpResponseWrapper<bool>> TienePedido(int id)
        {
            return await http.GetFromJsonAsync<bool>($"{API}/TienePedido/{id}");
        }
    }
}
