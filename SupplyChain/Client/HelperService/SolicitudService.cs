using Microsoft.AspNetCore.Mvc;
using Microsoft.JSInterop;
using SupplyChain.Client.HelperService.Base;
using SupplyChain.Client.Pages.Ventas._4_Solicitudes;
using SupplyChain.Client.RepositoryHttp;
using SupplyChain.Shared;
using SupplyChain.Shared.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Timers;
using static System.Net.WebRequestMethods;

namespace SupplyChain.Client.HelperService
{
    /// <summary>
    /// Servicio que admininstra las solicitudes http
    /// </summary>
    public class SolicitudService : BaseService<Solicitud, int>
    {
        private const string API = "api/Solicitudes";
        private readonly IJSRuntime _js;
        public SolicitudService(IRepositoryHttp httpClient, IJSRuntime js) : base(httpClient, API)
        {
            _js = js;
        }

        /// <summary>
        /// Carga una vista con los nombres de producto y cliente
        /// </summary>
        /// <returns></returns>
        public async Task<HttpResponseWrapper<List<vSolicitudes>>> GetVistaParaGrilla(TipoFiltro tipoFiltro = TipoFiltro.Todos)
        {
            return await http.GetFromJsonAsync<List<vSolicitudes>>($"{API}/Vista/{tipoFiltro}");
            
        }

        public async Task Imprimir(int solicitudId)
        {
            var response = await base.http.GetAsync($"api/AdministracionArchivos/SolicitudDatSheetPdf/{solicitudId}");
            // Leer el contenido de la respuesta HTTP como un arreglo de bytes
            var bytes = await response.Content.ReadAsByteArrayAsync();

            // Crear un objeto FileContentsResult que contenga el archivo PDF
            var contentType = "application/pdf";
            var nombreArchivo = $"AR-SOL-{solicitudId}.pdf";
            var archivo = new FileContentResult(bytes, contentType) { FileDownloadName = nombreArchivo };

            // Descargar el archivo PDF en el navegador del usuario
            await _js.InvokeAsync<object>("saveAsFile", nombreArchivo, Convert.ToBase64String(bytes));
        }

        public async Task<HttpResponseMessage> Eliminar(int id)
        {
            return await http.DeleteAsync($"{API}/{id}");
        }

    }
}
