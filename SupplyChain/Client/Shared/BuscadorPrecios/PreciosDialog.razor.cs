using Microsoft.AspNetCore.Components;
using SupplyChain.Client.HelperService;
using SupplyChain.Shared.Models;
using Syncfusion.Blazor.Spinner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SupplyChain.Client.Shared.BuscadorPrecios
{
    public class PreciosDialogBase : ComponentBase
    {
        [Inject] public PrecioArticuloService PrecioArticuloService { get; set; }
        [Parameter] public bool SoloReparaciones { get; set; } = false;
        [Parameter] public bool PopupBuscadorVisible { get; set; } = false;
        [Parameter] public string Codigo { get; set; } = "VACIO";
        [Parameter] public string Descripcion { get; set; } = "VACIO";
        [Parameter] public EventCallback<PreciosArticulos> OnObjectSelected { get; set; }
        [Parameter] public EventCallback OnCerrarDialog { get; set; }
        protected SfSpinner refSpinner;
        protected List<PreciosArticulos> datasource = new();
        public async Task Show()
        {
            if (SoloReparaciones)
            {
                await GetReraparaciones();
            }
            else
            {
                await Search();
            }
        }

        private async Task Search()
        {
            var response = await PrecioArticuloService.Search(Codigo, Descripcion);
            if (response.Error)
            {
                Console.WriteLine(await response.HttpResponseMessage.Content.ReadAsStringAsync());
            }
            else
            {
                datasource = response.Response;
            }
        }

        private async Task GetReraparaciones()
        {
            var response = await PrecioArticuloService.GetReparaciones();
            if (response.Error)
            {

            }
            else
            {
                datasource = response.Response;
            }
        }

        public async Task Hide()
        {
            PopupBuscadorVisible = false;
        }
        protected async Task SendObjectSelected(PreciosArticulos obj)
        {
            if (obj != null)
            {
                await OnObjectSelected.InvokeAsync(obj);
            }
            
            await Hide();
        }

        protected async Task CerrarDialog()
        {
            await Hide();
            await OnCerrarDialog.InvokeAsync();
        }
    }

}
