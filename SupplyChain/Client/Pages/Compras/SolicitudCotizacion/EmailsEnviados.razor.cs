using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using SupplyChain.Client.RepositoryHttp;
using SupplyChain.Shared;
using SupplyChain.Shared.Models;
using Syncfusion.Blazor.Grids;

namespace SupplyChain.Client.Pages.Compras.SolicitudCotizacion;

public class EmailEnviadosBase : ComponentBase
{
    protected bool mostrarSpinnerCargando = false;
    protected SfGrid<SolCotEmail> refGrid;
    [Inject] public IRepositoryHttp Http { get; set; }
    [Parameter] public bool CargarDatosInternamente { get; set; }
    [Parameter] public List<SolCotEmail> Data { get; set; } = new();


    protected override async Task OnInitializedAsync()
    {
        if (CargarDatosInternamente) await GetEmailEnviados(null);
    }

    public async Task<List<SolCotEmail>> GetEmailEnviados(Compra[] sugerenciasSeleccionadas)
    {
        if (sugerenciasSeleccionadas is not null && sugerenciasSeleccionadas.Length > 0)
        {
            var lista = sugerenciasSeleccionadas.ToList();
            var response =
                await Http.Post<List<Compra>, List<SolCotEmail>>("api/SolCotEmail/BySugerenciasCompras", lista);

            if (response.Error)
            {
                Console.WriteLine(response.HttpResponseMessage.ReasonPhrase);
                return Data = new List<SolCotEmail>();
            }

            return Data = response.Response.OrderByDescending(s => s.Id).ToList();
        }
        else
        {
            var response = await Http.GetFromJsonAsync<List<SolCotEmail>>("api/SolCotEmail");

            if (response.Error)
            {
                Console.WriteLine(response.HttpResponseMessage.ReasonPhrase);
                return Data = new List<SolCotEmail>();
            }

            return Data = response.Response;
        }
    }


    public async Task Refrescar(List<SolCotEmail> emails)
    {
        Data.AddRange(emails);
        Data = Data.OrderByDescending(s => s.Id).ToList();
        await refGrid.Refresh();
    }
}