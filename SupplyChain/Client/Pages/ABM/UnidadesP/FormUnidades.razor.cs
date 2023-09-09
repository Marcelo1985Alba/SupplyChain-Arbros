using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using SupplyChain.Client.HelperService;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Notifications;
using Syncfusion.Blazor.Spinner;

namespace SupplyChain.Client.Pages.ABM.UnidadesP;

public class FormUnidadesBase : ComponentBase
{
    protected Dictionary<string, object> HtmlAttributeSubmit = new()
    {
        { "type", "submit" }
    };

    protected SfGrid<Unidades> refGridItems;
    protected SfSpinner refSpinnerCli;
    protected bool SpinnerVisible = false;
    protected SfToast ToastObj;
    [Inject] protected HttpClient Http { get; set; }
    [Inject] public UnidadesService UnidadesService { get; set; }
    [Parameter] public Unidades unidades { get; set; } = new();
    [Parameter] public bool Show { get; set; }
    [Parameter] public EventCallback<Unidades> OnGuardar { get; set; }
    [Parameter] public EventCallback<Unidades> OnEliminar { get; set; }
    [Parameter] public EventCallback OnCerrar { get; set; }
    protected bool IsAdd { get; set; }

    protected override async Task OnInitializedAsync()
    {
    }

    protected async Task<bool> Agregar(Unidades unidad)
    {
        var response = await UnidadesService.Existe(unidad.Id);
        if (!response)
        {
            var response_2 = await UnidadesService.Agregar(unidad);
            if (response_2.Error)
            {
                Console.WriteLine(await response_2.HttpResponseMessage.Content.ReadAsStringAsync());
                await ToastMensajeError("Error al intentar Guardar la unidad.");
                return false;
            }

            unidades = response_2.Response;
            return true;
        }

        await ToastMensajeError($"La unidad con codigo {unidad.Id} ya existe.\n\rO la unidad no es permitida.");
        return false;
    }

    protected async Task<bool> Actualizar(Unidades unidad)
    {
        var response = await UnidadesService.Actualizar(unidad.Id, unidad);
        if (response.Error)
        {
            await ToastMensajeError("Error al intentar Guardar la unidad.");
            return false;
        }

        unidades = unidad;
        return true;
    }

    protected async Task GuardarUnidad()
    {
        var guardado = false;
        if (unidades.ESNUEVO)
            guardado = await Agregar(unidades);
        else
            guardado = await Actualizar(unidades);

        if (guardado)
        {
            Show = false;
            unidades.GUARDADO = guardado;
            await OnGuardar.InvokeAsync(unidades);
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