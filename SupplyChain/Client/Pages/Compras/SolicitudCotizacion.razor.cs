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

namespace SupplyChain.Client.Pages.Compras
{
    public class SolicitudCotizacionBase: ComponentBase
    {
        [Inject] protected IRepositoryHttp Http { get; set; }
        [CascadingParameter] public MainLayout MainLayout { get; set; }
        protected List<Compra> ListData = new();
        protected List<Compra> DataSource = new();

        protected async override Task OnInitializedAsync()
        {
            MainLayout.Titulo = "Solicitud de Cotizacion";
            await CargaSugerencia();
        }
        public async Task CargaSugerencia()
        {
            var response = await Http.GetFromJsonAsync<List<Compra>>("api/Compras/GetSugerencia/");
            if (response.Error)
            {
                Console.WriteLine(response.HttpResponseMessage.ReasonPhrase); return;
            }
            else
            {
                ListData = DataSource = response.Response;
            }
        }

        protected void OnInput(InputEventArgs eventArgs)
        {
            if (string.IsNullOrEmpty(eventArgs.Value))
            {
                ListData = DataSource;
            }
            else
            {
                ListData = DataSource.FindAll(e => e.CG_MAT.ToLower().Contains(eventArgs.Value));
            }
            
        }

    }
}
