using Microsoft.AspNetCore.Components;
using static SupplyChain.Client.Pages.ChatApp.ChatBase;
using System.Collections.Generic;
using Syncfusion.Blazor.Inputs;
using SupplyChain.Shared.Models;
using static System.Net.WebRequestMethods;
using System.Threading.Tasks;
using System;
using SupplyChain.Client.RepositoryHttp;
using SupplyChain.Client.Shared;
using SupplyChain.Shared;
using System.Linq;
using System.Text.Json;
using SupplyChain.Client.Pages.Compras.SolicitudCotizacion;

namespace SupplyChain.Client.Pages.Compras
{
    public class SolicitudCotizacionBase: ComponentBase
    {
        [Inject] public IRepositoryHttp Http { get; set; }
        [CascadingParameter] public MainLayout MainLayout { get; set; }

        protected bool mostrarSpinner = false;
        protected ProveedoresMateriaPrima refProveedoresMateriaPrima;
        protected Proveedores refProveedores;

        protected int[] idsProveedores = Array.Empty<int>();
        protected async override Task OnInitializedAsync()
        {
            MainLayout.Titulo = "Solicitud de Cotizacion";
        }

        protected async Task OnRecibirItemsSeleccionados(Compra[] sugerenciasSeleccionadas)
        {
            mostrarSpinner = true;
            if (sugerenciasSeleccionadas is not null && sugerenciasSeleccionadas.Length > 0)
            {
                List<Compra> lista = sugerenciasSeleccionadas.ToList();
                var urlGetProveFromMatProve = $"api/MatProve/GetProveedores?sugerencias={JsonSerializer.Serialize(lista)}";
                var response = await Http.Post<List<Compra>, List<vProveedorItris>>("api/MatProve/GetProveedores", lista);

                if (response.Error)
                {
                    Console.WriteLine(response.HttpResponseMessage.ReasonPhrase);
                }
                else
                {
                    idsProveedores = response.Response.Select(s => s.Id).ToArray();
                    EnviarProveedoresMp();
                } 
            }
            else
            {
                idsProveedores = Array.Empty<int>();
                EnviarProveedoresMp();
            }
            mostrarSpinner = false;
        }

        protected void EnviarProveedoresMp()
        {
            refProveedoresMateriaPrima.SetIntArray(idsProveedores);
            refProveedores.SetIntArray(idsProveedores);
        }

    }
}
