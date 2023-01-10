using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using SupplyChain.Client.HelperService;
using SupplyChain.Shared.Models;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Notifications;
using Syncfusion.Blazor.Spinner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SupplyChain.Client.Pages.ABM.ISOP
{
    public class FormIsoBase : ComponentBase
    {
        [Inject] protected HttpClient Http { get; set; }
        [Inject] public ISOService isoService { get; set; }
        [Inject] protected AspAmbService AspAmpService { get; set; }
        [Inject] protected TipoAreaService TipoAreaService { get; set; }
        [Inject] protected AspAmbService AspAmbService { get; set; }
        [Parameter] public ISO isos { get; set; } = new();
        [Parameter] public bool Show { get; set; } = false;
        [Parameter] public EventCallback<ISO> OnGuardar { get; set; }
        [Parameter] public EventCallback<ISO> OnEliminar { get; set; }
        [Parameter] public EventCallback OnCerrar { get; set; }

        protected Boolean edit = true;
        protected SfGrid<ISO> refGridItems;
        protected SfSpinner refSpinnerCli;
        protected bool SpinnerVisible = false;
        protected SfToast ToastObj;
        protected List<TipoArea> roles = new();
        protected List<AspAmb> aspAmbientales = new();
        protected Dictionary<string, object> HtmlAttributeSubmit = new()
        {
            { "type", "submit" }
        };
        protected bool IsAdd { get; set; }

        protected class BaseOption
        {
            public string Text { get; set; }
        }
        protected List<BaseOption> FactorData = new List<BaseOption> {
            new BaseOption() {Text= "Politico" },
            new BaseOption() {Text= "Social" },
            new BaseOption() {Text= "Economico" },
            new BaseOption() {Text= "Tecnologico" },
            new BaseOption() {Text= "Ambiental" },
        };
        protected List<BaseOption> FODAData = new List<BaseOption> {
            new BaseOption() {Text= "Fortaleza" },
            new BaseOption() {Text= "Debilidad" },
            new BaseOption() {Text= "Oportunidad" },
            new BaseOption() {Text= "Amenaza" },
        };
        protected List<BaseOption> ImpAmbData = new List<BaseOption> {
            new BaseOption() {Text= "AIRE" },
            new BaseOption() {Text= "AGUA" },
            new BaseOption() {Text= "SUELO" },
            new BaseOption() {Text= "RRNN" },
            new BaseOption() {Text= "BIOTA" },
            new BaseOption() {Text= "QVIDA" },
            new BaseOption() {Text= "RIESGO" },
            new BaseOption() {Text= "OPORTUNIDAD" },
        };
       protected class FrecuenciaOpcions
        {
            public string Text { get;set; }
        }
        protected List<FrecuenciaOpcions> FrecuenciaData = new List<FrecuenciaOpcions> {
            new FrecuenciaOpcions() {Text= "Muy baja" },
            new FrecuenciaOpcions() {Text= "Baja" },
            new FrecuenciaOpcions() {Text= "Media" },
            new FrecuenciaOpcions() {Text= "Alta" },
            new FrecuenciaOpcions() {Text= "Muy alta" },
        };
        protected class ImpactoOptions
        {
            public string Text { get;set; }
        }   
        protected List<ImpactoOptions> ImpactoData = new List<ImpactoOptions> {
            new ImpactoOptions() {Text= "Muy poco" },
            new ImpactoOptions() {Text= "Poco" },
            new ImpactoOptions() {Text= "Moderado" },
            new ImpactoOptions() {Text= "Alto" },
            new ImpactoOptions() {Text= "Muy alto" },
        };
        protected List<BaseOption> OperacionData = new List<BaseOption> {
            new BaseOption() {Text= "Normal" },
            new BaseOption() {Text= "Anormal" },
            new BaseOption() {Text= "Emergencia" },
        };
        protected List<BaseOption> ControlData = new List<BaseOption> {
            new BaseOption() {Text= "Directa" },
            new BaseOption() {Text= "Indirecta" },
        };
        protected List<BaseOption> NatDelImpData = new List<BaseOption> {
            new BaseOption() {Text= "Beneficioso" },
            new BaseOption() {Text= "Adverso" },
        };
        protected List<BaseOption> GestionData = new List<BaseOption> {
            new BaseOption() {Text= "Optima" },
            new BaseOption() {Text= "Parcial" },
            new BaseOption() {Text= "Sin gestion" },
        };

        protected async override Task OnInitializedAsync()
        {
            var response = await AspAmbService.Get();
            if (!response.Error)
            {
                aspAmbientales = response.Response;
            }
            var response2 = await TipoAreaService.Get();
            if (!response2.Error)
            {
                roles = response2.Response;
            }
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
            bool guardado = false;
            if (isos.ESNUEVO)
            {
                guardado = await Agregar(isos);
            }
            else
            {
                guardado = await Actualizar(isos);
            }

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
            await this.ToastObj.Show(new ToastModel
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

        protected void OnChange(Syncfusion.Blazor.DropDowns.ChangeEventArgs<string, BaseOption> args)
        {
            if (args.Value == "RIESGO" || args.Value == "OPORTUNIDAD")
                edit = false;
            else
                edit = true;
        }
    }
}
