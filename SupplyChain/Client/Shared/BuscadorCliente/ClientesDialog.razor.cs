using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SupplyChain.Client.Shared.BuscadorCliente
{
    public class ClientesDialogBase : ComponentBase
    {
        [Inject] public RepositoryHttp.IRepositoryHttp Http { get; set; }
        [Parameter] public bool PopupBuscadorVisible { get; set; } = false;
        [Parameter] public EventCallback<ClienteExterno> OnObjectSelected { get; set; }

        protected List<ClienteExterno> clientes = new();

        public async Task Show()
        {
            var response = await Http.GetFromJsonAsync<List<ClienteExterno>>("api/Cliente/GetClienteExterno");
            if (response.Error)
            {
                Console.WriteLine(await response.HttpResponseMessage.Content.ReadAsStringAsync());
            }
            else
            {
                clientes = response.Response;
                PopupBuscadorVisible = true;
            }
        }
        public async Task Hide()
        {
            PopupBuscadorVisible = false;
        }
        protected async Task SendObjectSelected(ClienteExterno obj)
        {
            await OnObjectSelected.InvokeAsync(obj);
            await Hide();
        }
    }
}
