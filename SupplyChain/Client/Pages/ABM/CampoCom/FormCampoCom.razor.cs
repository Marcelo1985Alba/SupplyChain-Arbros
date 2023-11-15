using Microsoft.AspNetCore.Components;
using SupplyChain.Client.HelperService;
using SupplyChain.Shared;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Notifications;
using Syncfusion.Blazor.Spinner;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace SupplyChain.Client.Pages.ABM.CampoCom
{
    public class FormCampoComBase : CampoComBase
    {
        [Inject] protected HttpClient Http {  get; set; }
        [Inject] public CampoComService CampoComService { get; set; }
        [Parameter] public CampoComodin campos { get; set; } = new();
        [Parameter] public bool Show { get; set; }
        [Parameter] public EventCallback<CampoComodin> OnGuardar { get; set; }
        [Parameter] public EventCallback<CampoComodin> OnEliminar { get; set; }
        [Parameter] public EventCallback OnCerrar { get; set; }

        protected SfGrid<CampoComodin> refGridItems;
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

        protected async Task<bool> Agregar(CampoComodin campo)
        {
            var response = await CampoComService.Existe(campo.Id);
            if (!response)
            {
                var response_2 = await CampoComService.Agregar(campo);
                if (response_2.Error)
                {
                    Console.WriteLine(await response_2.HttpResponseMessage.Content.ReadAsStringAsync());
                    await ToastMensajeError("Error al intentar Guardar el comodin.");
                    return false;
                }
                campos = response_2.Response;
                return true;
            }
            await ToastMensajeError($"El comodin con codigo {campo.Id} ya existe.\n\rO el comodin no es permitida.");
            return false;
        }

        protected async Task<bool> Actualizar(CampoComodin campo)
        {
            var response = await CampoComService.Actualizar(campo.Id, campo);
            if (response.Error)
            {
                await ToastMensajeError("Error al intentar Guardar el comodin.");
                return false;
            }
            campos = campo;
            return true;
        }

        protected async Task GuardarCampo()
        {
            bool guardado = false;
            if (campos.ESNUEVO)
            {
                guardado = await Agregar(campos);
            }
            else
            {
                guardado = await Actualizar(campos);
            }

            if (guardado)
            {
                Show = false;
                campos.GUARDADO = guardado;
                await OnGuardar.InvokeAsync(campos);
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
            await this.ToastObj.ShowAsync(new ToastModel
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
            await ToastObj.ShowAsync(new ToastModel
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
