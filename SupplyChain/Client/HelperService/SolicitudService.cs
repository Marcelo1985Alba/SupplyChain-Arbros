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
    /// Servicio que admininstra las solicitudes
    /// </summary>
    public class SolicitudService : BaseService<Solicitud, int>
    {
        private const string API = "api/Solicitudes";

        public SolicitudService(IRepositoryHttp httpClient): base(httpClient, API)
        {
        }

        /// <summary>
        /// Carga una vista con los nombres de producto y cliente
        /// </summary>
        /// <returns></returns>
        public async Task<HttpResponseWrapper<List<vSolicitudes>>> GetVistaParaGrilla(TipoFiltro tipoFiltro = TipoFiltro.Todos)
        {
            return await http.GetFromJsonAsync<List<vSolicitudes>>($"{API}/Vista/{tipoFiltro}");
            
        }


    }
}
