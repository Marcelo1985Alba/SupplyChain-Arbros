using Microsoft.AspNetCore.Components;
using SupplyChain.Client.HelperService;
using Syncfusion.Blazor.Notifications;
using Syncfusion.Blazor.Spinner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SupplyChain.Client.Shared.BuscadorCliente
{
    public class ClientesDialogBase : ComponentBase
    {
        [Inject] public RepositoryHttp.IRepositoryHttp Http { get; set; }
        [Inject] public ClienteService ClienteService { get; set; }
        [Parameter] public bool PopupBuscadorVisible { get; set; } = false;
        [Parameter] public EventCallback<ClienteExterno> OnObjectSelected { get; set; }
        [Parameter] public EventCallback OnCerrarDialog { get; set; }
        [Parameter] public bool CompararCliente { get; set; } = false;
        [Parameter] public int Cg_Cli_Comparar { get; set; } = 0;

        protected List<ClienteExterno> clientes = new();
        protected SfSpinner refSpinner;
        protected SfToast ToastObj;
        public async Task Show()
        {
            refSpinner?.ShowAsync();
            var response = await ClienteService.GetClientesExterno();
            if (response.Error)
            {
                refSpinner?.HideAsync();
                Console.WriteLine(await response.HttpResponseMessage.Content.ReadAsStringAsync());
            }
            else
            {
                clientes = response.Response;
                refSpinner?.HideAsync();
                PopupBuscadorVisible = true;
            }
        }
        public async Task Hide()
        {
            PopupBuscadorVisible = false;
        }
        protected async Task SendObjectSelected(ClienteExterno obj)
        {
            if (obj != null)
            {
                if (!CompararCliente)
                {
                    await OnObjectSelected.InvokeAsync(obj);
                    await Hide();
                }
                else
                {
                    if (CompararCliente && obj.CG_CLI == Cg_Cli_Comparar.ToString())
                    {
                        await OnObjectSelected.InvokeAsync(obj);
                        await Hide();
                    }
                    else
                    {

                    }

                }
            }
            


            
        }

        protected async Task CerrarDialog()
        {
            await Hide();
            await OnCerrarDialog.InvokeAsync();
        }

        private async Task ToastMensajeError(string content = "Ocurrio un Error.")
        {
            await ToastObj.Show(new ToastModel
            {
                Title = "Error!",
                Content = content,
                CssClass = "e-toast-warning",
                Icon = "e-warning toast-icons",
                ShowCloseButton = true,
                ShowProgressBar = true
            });
        }
    }
}
