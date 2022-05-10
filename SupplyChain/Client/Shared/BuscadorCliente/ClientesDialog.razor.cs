﻿using Microsoft.AspNetCore.Components;
using SupplyChain.Client.HelperService;
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

        protected List<ClienteExterno> clientes = new();
        protected SfSpinner refSpinner;
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
            await OnObjectSelected.InvokeAsync(obj);
            await Hide();
        }
    }
}
