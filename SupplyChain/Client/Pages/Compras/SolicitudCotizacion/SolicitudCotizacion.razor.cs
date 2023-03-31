using Microsoft.AspNetCore.Components;
using static SupplyChain.Client.Pages.ChatApp.ChatBase;
using System.Collections.Generic;
using Syncfusion.Blazor.Inputs;
using SupplyChain.Shared.Models;
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

        protected List<vProveedorItris> vProveedorItrisEnviarSolicitud = new();
        protected List<SolCotEmail> EmailsEnviados = new();

        protected int[] idsProveedores = Array.Empty<int>();
        protected async override Task OnInitializedAsync()
        {
            MainLayout.Titulo = "Solicitud de Cotizacion";
        }

        protected async Task GetProveedoresFromSugerencias(Compra[] sugerenciasSeleccionadas)
        {
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
        }
        protected async Task OnRecibirItemsSeleccionados(Compra[] sugerenciasSeleccionadas)
        {
            mostrarSpinner = true;
            await GetProveedoresFromSugerencias(sugerenciasSeleccionadas);
            await GetEmailsEnviadosFromSugerencias(sugerenciasSeleccionadas);
            mostrarSpinner = false;
        }

        protected async Task GetEmailsEnviadosFromSugerencias(Compra[] sugerenciasSeleccionadas)
        {

            if (sugerenciasSeleccionadas is not null && sugerenciasSeleccionadas.Length > 0)
            {
                List<Compra> lista = sugerenciasSeleccionadas.ToList();
                var response = await Http.Post<List<Compra>, List<SolCotEmail>>("api/SolCotEmail/BySugerenciasCompras", lista);

                if (response.Error)
                {
                    Console.WriteLine(response.HttpResponseMessage.ReasonPhrase);
                }
                else
                {
                    EmailsEnviados = response.Response;
                    EnviarProveedoresMp();
                }
            }
            else
            {
                EmailsEnviados = new();
            }
        }

        protected void EnviarProveedoresMp()
        {
            refProveedoresMateriaPrima.SetIntArray(idsProveedores);
            refProveedores.SetIntArray(idsProveedores);
        }

        protected void OnProveedorDeseleccionado(vProveedorItris proveedorItris)
        {
            vProveedorItrisEnviarSolicitud.Remove(proveedorItris);
        }
        protected void OnProveedorSeleccionado(vProveedorItris proveedorItris)
        {
            vProveedorItrisEnviarSolicitud.Add(proveedorItris);
        }
    }
}
