using Microsoft.AspNetCore.Components;
using SupplyChain.Client.RepositoryHttp;
using SupplyChain.Client.Shared;
using SupplyChain.Shared.Models;
using Syncfusion.Blazor.Inputs;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Syncfusion.Blazor.DropDowns;

namespace SupplyChain.Client.Pages.Compras.SolicitudCotizacion
{
    public class SugerenciasComprasBase : ComponentBase
    {
        [Inject] protected IRepositoryHttp Http { get; set; }
        [Parameter] public EventCallback<Compra[]> OnItemsSeleccionados { get; set; }

        protected SfListBox<Compra[], Compra> refSfListBox;
        protected List<Compra> ListData = new();
        protected List<Compra> DataSource = new();

        protected bool mostrarSpinnerSeleccionSugerencia = false;
        protected async override Task OnInitializedAsync()
        {
            mostrarSpinnerSeleccionSugerencia = true;
            await CargaSugerencia();
            mostrarSpinnerSeleccionSugerencia = false;
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

        protected async Task SelectedChange(ListBoxChangeEventArgs<Compra[],Compra> args)
        {
            if (OnItemsSeleccionados.HasDelegate)
            {
                await OnItemsSeleccionados.InvokeAsync(args.Value);
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
