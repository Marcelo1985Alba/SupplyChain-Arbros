using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using SupplyChain.Client.HelperService;
using SupplyChain.Shared.Models;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Notifications;
using Syncfusion.Blazor.Spinner;

namespace SupplyChain.Client.Pages.ABM.CeldasP;

public class FormCeldasBase : ComponentBase
{
    protected Dictionary<string, object> HtmlAttributeSubmit = new()
    {
        { "type", "submit" }
    };

    protected SfGrid<Celdas> refGridItems;
    protected SfSpinner refSpinnerCli;
    protected bool SpinnerVisible = false;
    protected SfToast ToastObj;
    [Inject] protected HttpClient Http { get; set; }
    [Inject] public CeldasService CeldasService { get; set; }
    [Parameter] public Celdas celdas { get; set; } = new();
    [Parameter] public bool Show { get; set; }
    [Parameter] public EventCallback<Celdas> OnGuardar { get; set; }
    [Parameter] public EventCallback<Celdas> OnEliminar { get; set; }
    [Parameter] public EventCallback OnCerrar { get; set; }
    protected bool IsAdd { get; set; }

    protected override async Task OnInitializedAsync()
    {
    }

    protected async Task<bool> Agregar(Celdas celda)
    {
        var response = await CeldasService.Existe(celda.Id);
        if (!response)
        {
            var response_2 = await CeldasService.Agregar(celda);
            if (response_2.Error)
            {
                Console.WriteLine(await response_2.HttpResponseMessage.Content.ReadAsStringAsync());
                await ToastMensajeError("Error al intentar Guardar la celda.");
                return false;
            }

            celdas = response_2.Response;
            return true;
        }

        await ToastMensajeError($"La celda con codigo {celda.Id} ya existe.\n\rO la celda no es permitida.");
        return false;
    }

    protected async Task<bool> Actualizar(Celdas celda)
    {
        var response = await CeldasService.Actualizar(celda.Id, celda);
        if (response.Error)
        {
            await ToastMensajeError("Error al intentar Guardar la celda.");
            return false;
        }

        celdas = celda;
        return true;
    }

    protected async Task GuardarCelda()
    {
        var guardado = false;
        if (celdas.ESNUEVO)
            guardado = await Agregar(celdas);
        else
            guardado = await Actualizar(celdas);

        if (guardado)
        {
            Show = false;
            celdas.GUARDADO = guardado;
            await OnGuardar.InvokeAsync(celdas);
        }
    }

    public async Task Hide()
    {
        Show = false;
    }

    protected async Task OnCerrarDialog()
    {
        await OnCerrar.InvokeAsync();
    }

    private async Task ToastMensajeExito(string content = "Guardado Correctamente.")
    {
        await ToastObj.Show(new ToastModel
        {
            Title = "EXITO!",
            Content = content,
            CssClass = "e-toast-success",
            Icon = "e-success toast-icons",
            ShowCloseButton = true,
            ShowProgressBar = true
        });
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