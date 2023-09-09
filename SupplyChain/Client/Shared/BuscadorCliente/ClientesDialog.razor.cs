﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using SupplyChain.Client.HelperService;
using SupplyChain.Client.RepositoryHttp;
using Syncfusion.Blazor.Notifications;
using Syncfusion.Blazor.Spinner;

namespace SupplyChain.Client.Shared.BuscadorCliente;

public class ClientesDialogBase : ComponentBase
{
    protected List<ClienteExterno> clientes = new();
    protected SfSpinner refSpinner;
    protected SfToast ToastObj;
    [Inject] public IRepositoryHttp Http { get; set; }
    [Inject] public ClienteService ClienteService { get; set; }
    [Parameter] public bool PopupBuscadorVisible { get; set; }
    [Parameter] public EventCallback<ClienteExterno> OnObjectSelected { get; set; }
    [Parameter] public EventCallback OnCerrarDialog { get; set; }
    [Parameter] public bool CompararCliente { get; set; }
    [Parameter] public int Cg_Cli_Comparar { get; set; }
    [Parameter] public int Cg_Cli { get; set; }
    [Parameter] public string Des_Cli { get; set; } = "";

    public async Task Show()
    {
        refSpinner?.ShowAsync();
        await Search();
    }


    protected async Task BuscarTodos()
    {
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


    protected async Task Search()
    {
        var response = await ClienteService.Search(Cg_Cli, Des_Cli);
        if (response.Error)
        {
            refSpinner?.HideAsync();
            Console.WriteLine(await response.HttpResponseMessage.Content.ReadAsStringAsync());
        }
        else
        {
            clientes = response.Response;

            if (clientes.Count == 1)
                await SendObjectSelected(clientes[0]);
            else
                PopupBuscadorVisible = true;

            refSpinner?.HideAsync();
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