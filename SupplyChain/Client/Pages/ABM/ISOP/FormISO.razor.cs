using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using SupplyChain.Client.HelperService;
using Syncfusion.Blazor.DropDowns;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Notifications;
using Syncfusion.Blazor.Spinner;

namespace SupplyChain.Client.Pages.ABM.ISOP;

public class FormIsoBase : ComponentBase
{
    protected List<AspAmb> aspAmbientales = new();

    protected List<BaseOption> ControlData = new()
    {
        new BaseOption() { Text = "Directa" },
        new BaseOption() { Text = "Indirecta" }
    };

    protected bool edit = true;

    protected List<BaseOption> FactorData = new()
    {
        new BaseOption() { Text = "Politico" },
        new BaseOption() { Text = "Social" },
        new BaseOption() { Text = "Economico" },
        new BaseOption() { Text = "Tecnologico" },
        new BaseOption() { Text = "Ambiental" }
    };

    protected List<BaseOption> FODAData = new()
    {
        new BaseOption() { Text = "Fortaleza" },
        new BaseOption() { Text = "Debilidad" },
        new BaseOption() { Text = "Oportunidad" },
        new BaseOption() { Text = "Amenaza" }
    };

    protected List<BaseOption> FrecuenciaData = new()
    {
        new BaseOption() { Text = "Muy baja" },
        new BaseOption() { Text = "Baja" },
        new BaseOption() { Text = "Media" },
        new BaseOption() { Text = "Alta" },
        new BaseOption() { Text = "Muy alta" }
    };

    protected List<BaseOption> GestionData = new()
    {
        new BaseOption() { Text = "Optima" },
        new BaseOption() { Text = "Parcial" },
        new BaseOption() { Text = "Sin gestion" }
    };

    protected Dictionary<string, object> HtmlAttributeSubmit = new()
    {
        { "type", "submit" }
    };

    protected List<BaseOption> ImpactoData = new()
    {
        new BaseOption() { Text = "Muy poco" },
        new BaseOption() { Text = "Poco" },
        new BaseOption() { Text = "Moderado" },
        new BaseOption() { Text = "Alto" },
        new BaseOption() { Text = "Muy alto" }
    };

    protected List<BaseOption> ImpAmbData = new()
    {
        new BaseOption() { Text = "AIRE" },
        new BaseOption() { Text = "AGUA" },
        new BaseOption() { Text = "SUELO" },
        new BaseOption() { Text = "RRNN" },
        new BaseOption() { Text = "BIOTA" },
        new BaseOption() { Text = "QVIDA" },
        new BaseOption() { Text = "RIESGO" },
        new BaseOption() { Text = "OPORTUNIDAD" }
    };

    protected List<BaseOption> NatDelImpData = new()
    {
        new BaseOption() { Text = "Beneficioso" },
        new BaseOption() { Text = "Adverso" }
    };

    protected List<BaseOption> OperacionData = new()
    {
        new BaseOption() { Text = "Normal" },
        new BaseOption() { Text = "Anormal" },
        new BaseOption() { Text = "Emergencia" }
    };

    protected SfGrid<ISO> refGridItems;
    protected SfSpinner refSpinnerCli;
    protected List<TipoArea> roles = new();
    protected bool SpinnerVisible = false;
    protected SfToast ToastObj;
    [Inject] protected HttpClient Http { get; set; }
    [Inject] public ISOService isoService { get; set; }
    [Inject] protected AspAmbService AspAmpService { get; set; }
    [Inject] protected TipoAreaService TipoAreaService { get; set; }
    [Inject] protected AspAmbService AspAmbService { get; set; }
    [Parameter] public ISO isos { get; set; } = new();
    [Parameter] public bool Show { get; set; }
    [Parameter] public EventCallback<ISO> OnGuardar { get; set; }
    [Parameter] public EventCallback<ISO> OnEliminar { get; set; }
    [Parameter] public EventCallback OnCerrar { get; set; }
    protected bool IsAdd { get; set; }

    protected override async Task OnInitializedAsync()
    {
        var response = await AspAmbService.Get();
        if (!response.Error) aspAmbientales = response.Response;
        var response2 = await TipoAreaService.Get();
        if (!response2.Error) roles = response2.Response;
    }

    protected async Task<bool> Agregar(ISO iso)
    {
        var response = await isoService.Existe(iso.Id);
        if (!response)
        {
            var response_2 = await isoService.Agregar(iso);
            if (response_2.Error)
            {
                Console.WriteLine(await response_2.HttpResponseMessage.Content.ReadAsStringAsync());
                await ToastMensajeError("Error al intentar Guardar el ISO.");
                return false;
            }

            iso = response_2.Response;
            return true;
        }

        await ToastMensajeError($"El ISO {iso.Id} ya existe.\n\rO el ISO no es permitido.");
        return false;
    }

    protected async Task<bool> Actualizar(ISO iso)
    {
        var response = await isoService.Actualizar(iso.Id, iso);
        if (response.Error)
        {
            await ToastMensajeError("Error al intentar Guardar el iso.");
            return false;
        }

        isos = iso;
        return true;
    }

    protected async Task GuardarIso()
    {
        var guardado = false;
        if (isos.ESNUEVO)
            guardado = await Agregar(isos);
        else
            guardado = await Actualizar(isos);

        if (guardado)
        {
            Show = false;
            isos.GUARDADO = guardado;
            await OnGuardar.InvokeAsync(isos);
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

    protected void OnChange(ChangeEventArgs<string, BaseOption> args)
    {
        if (args.Value == "RIESGO" || args.Value == "OPORTUNIDAD")
            edit = false;
        else
            edit = true;
    }

    protected class BaseOption
    {
        public string Text { get; set; }
    }
}