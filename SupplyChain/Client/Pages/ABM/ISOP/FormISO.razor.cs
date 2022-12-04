using Microsoft.AspNetCore.Components;
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
        [Parameter] public ISO isos { get; set; } = new();
        [Parameter] public bool Show { get; set; } = false;
        [Parameter] public EventCallback<ISO> OnGuardar { get; set; }
        [Parameter] public EventCallback<ISO> OnEliminar { get; set; }
        [Parameter] public EventCallback OnCerrar { get; set; }

        protected SfGrid<ISO> refGridItems;
        protected SfSpinner refSpinnerCli;
        protected bool SpinnerVisible = false;
        protected SfToast ToastObj;
        protected Dictionary<string, object> HtmlAttributeSubmit = new()
        {
            { "type", "submit" }
        };
        protected bool IsAdd { get; set; }
        protected async override Task OnInitializedAsync()
        {
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
    }
}
