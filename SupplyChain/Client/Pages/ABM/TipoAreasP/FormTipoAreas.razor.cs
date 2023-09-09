using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using SupplyChain.Client.HelperService;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Notifications;
using Syncfusion.Blazor.Spinner;

namespace SupplyChain.Client.Pages.ABM.TipoAreasP;

public class FormTipoAreasBase : ComponentBase
{
    protected Dictionary<string, object> HtmlAttributeSubmit = new()
    {
        { "type", "submit" }
    };

    protected SfGrid<TipoArea> refGridItems;
    protected SfSpinner refSpinnerCli;
    protected bool SpinnerVisible = false;
    protected SfToast ToastObj;
    [Inject] protected HttpClient Http { get; set; }
    [Inject] public TipoAreaService TipoAreaService { get; set; }
    [Parameter] public TipoArea tipoAreas { get; set; } = new();
    [Parameter] public bool Show { get; set; }
    [Parameter] public EventCallback<TipoArea> OnGuardar { get; set; }
    [Parameter] public EventCallback<TipoArea> OnEliminar { get; set; }
    [Parameter] public EventCallback OnCerrar { get; set; }
    protected bool IsAdd { get; set; }

    protected override async Task OnInitializedAsync()
    {
    }

    protected async Task<bool> Agregar(TipoArea tipoArea)
    {
        var response = await TipoAreaService.Existe(tipoArea.Id);
        if (!response)
        {
            var response_2 = await TipoAreaService.Agregar(tipoArea);
            if (response_2.Error)
            {
                Console.WriteLine(await response_2.HttpResponseMessage.Content.ReadAsStringAsync());
                await ToastMensajeError("Error al intentar Guardar el tipo de Area.");
                return false;
            }

            tipoAreas = response_2.Response;
            return true;
        }

        await ToastMensajeError(
            $"El tipo de Area con codigo {tipoArea.Id} ya existe.\n\rO el tipo de Area no es permitido.");
        return false;
    }

    protected async Task<bool> Actualizar(TipoArea tipoArea)
    {
        var response = await TipoAreaService.Actualizar(tipoArea.Id, tipoArea);
        if (response.Error)
        {
            await ToastMensajeError("Error al intentar Guardar el tipo de Area.");
            return false;
        }

        tipoAreas = tipoArea;
        return true;
    }

    protected async Task GuardarTipoArea()
    {
        var guardado = false;
        if (tipoAreas.ESNUEVO)
            guardado = await Agregar(tipoAreas);
        else
            guardado = await Actualizar(tipoAreas);

        if (guardado)
        {
            Show = false;
            tipoAreas.GUARDADO = guardado;
            await OnGuardar.InvokeAsync(tipoAreas);
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