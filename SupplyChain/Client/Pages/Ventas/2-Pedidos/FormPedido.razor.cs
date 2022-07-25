using Microsoft.AspNetCore.Components;
using SupplyChain.Client.HelperService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SupplyChain.Client.Pages.Ventas._2_Pedidos
{
    public class FormPedidoBase: ComponentBase
    {
        [Inject] public PedCliService PedCliService { get; set; }

        [Parameter] public PedCli Pedido { get; set; } = new();
        [Parameter] public bool Show { get; set; } = false;
        [Parameter] public EventCallback<PedCli> OnGuardar { get; set; }
        [Parameter] public EventCallback OnCerrar { get; set; }

        public async Task ShowAsync(int id)
        {

        }

        protected async Task OnCerrarDialog()
        {
            await OnCerrar.InvokeAsync();
        }
    }
}
