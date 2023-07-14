using Microsoft.AspNetCore.Mvc;
using Microsoft.JSInterop;
using SupplyChain.Client.HelperService.Base;
using SupplyChain.Client.Pages.Ventas._3_Presupuestos;
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
    public class PresupuestoService : BaseService<Presupuesto, int>
    {
        private readonly IRepositoryHttp Http;
        private readonly IJSRuntime _js;
        private const string API = "api/Presupuestos";
        public PresupuestoService(IRepositoryHttp Http, IJSRuntime js) : base(Http, API)
        {
            this.Http = Http;
            this._js = js;
        }

        
        public async Task<HttpResponseWrapper<List<Presupuesto>>> ActualizarColor(int numpresupuesto, string color)
        {
            return await Http.GetFromJsonAsync<List<Presupuesto>>($"api/Presupuestos/ActualizarColor/{numpresupuesto}/{color}");
        }

        public async Task<HttpResponseWrapper<List<Presupuesto>>> EnviarComentario(int numpresupuesto, string comentario)
        {
            return await Http.GetFromJsonAsync<List<Presupuesto>>($"api/Presupuestos/EnviarComentario/{numpresupuesto}/{comentario}");
        }

        public async Task<HttpResponseWrapper<List<Presupuesto>>> EnviarMotivos(int numpresupuesto, string motivo)
        {
            return await Http.GetFromJsonAsync<List<Presupuesto>>($"api/Presupuestos/EnviarMotivos/{numpresupuesto}/{motivo}");
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
            var response = await Http.GetAsync($"api/AdministracionArchivos/PresupuestoDataSheetPdf/{presupuestoId}");
            // Leer el contenido de la respuesta HTTP como un arreglo de bytes
            var bytes = await response.Content.ReadAsByteArrayAsync();

            // Crear un objeto FileContentsResult que contenga el archivo PDF
            var contentType = "application/pdf";
            var nombreArchivo = $"AR-CO-{presupuestoId}.pdf";
            var archivo = new FileContentResult(bytes, contentType) { FileDownloadName = nombreArchivo };

            // Descargar el archivo PDF en el navegador del usuario
            await _js.InvokeAsync<object>("saveAsFile", nombreArchivo, Convert.ToBase64String(bytes));
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
