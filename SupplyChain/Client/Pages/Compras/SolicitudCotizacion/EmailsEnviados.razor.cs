using Microsoft.AspNetCore.Components;
using SupplyChain.Client.RepositoryHttp;
using SupplyChain.Shared;
using SupplyChain.Shared.Models;
using Syncfusion.Blazor.Grids;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace SupplyChain.Client.Pages.Compras.SolicitudCotizacion
{
    public class EmailEnviadosBase : ComponentBase
    {
        [Inject] public IRepositoryHttp Http { get; set; }
        [Parameter] public bool CargarDatosInternamente { get; set; } = false;
        [Parameter] public List<SolCotEmail> Data { get; set; } = new();
        protected SfGrid<SolCotEmail> refGrid;
        protected bool mostrarSpinnerCargando = false;


        protected async override Task OnInitializedAsync()
        {
            if (CargarDatosInternamente)
            {
                await GetEmailEnviados(null);
            }
        }

        public async Task<List<SolCotEmail>> GetEmailEnviados(Compra[] sugerenciasSeleccionadas)
        {
            if (sugerenciasSeleccionadas is not null && sugerenciasSeleccionadas.Length > 0)
            {
                List<Compra> lista = sugerenciasSeleccionadas.ToList();
                var response = await Http.Post<List<Compra>, List<SolCotEmail>>("api/SolCotEmail/BySugerenciasCompras", lista);

                if (response.Error)
                {
                    Console.WriteLine(response.HttpResponseMessage.ReasonPhrase);
                    return Data = new();
                }
                else
                {
                    return Data = response.Response;
                }
            }
            else
            {
                var response = await Http.GetFromJsonAsync<List<SolCotEmail>>("api/SolCotEmail");

                if (response.Error)
                {
                    Console.WriteLine(response.HttpResponseMessage.ReasonPhrase);
                    return Data = new();
                }
                else
                {
                    return Data = response.Response;
                }
            }
        }




    }
}
