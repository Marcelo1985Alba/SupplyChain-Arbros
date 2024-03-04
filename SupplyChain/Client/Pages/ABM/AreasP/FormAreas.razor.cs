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

namespace SupplyChain.Client.Pages.ABM.AreasP
{
    public class FormAreasBase : ComponentBase
    {
        [Inject] protected HttpClient Http { get; set; }
        [Inject] public AreasService AreasService { get; set; }
        [Parameter] public Areas areas { get; set; } = new();
        [Parameter] public bool Show { get; set; } = false;
        [Parameter] public EventCallback<Areas> OnGuardar { get; set; }
        [Parameter] public EventCallback<Areas> OnEliminar { get; set; }
        [Parameter] public EventCallback OnCerrar { get; set; }

        protected SfGrid<Areas> refGridItems;
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

        protected async Task<bool> Agregar(Areas area)
        {
            var response = await AreasService.Existe(area.Id);
            if (!response)
            {
                var response_2 = await AreasService.Agregar(area);
                if (response_2.Error)
                {
                    Console.WriteLine(await response_2.HttpResponseMessage.Content.ReadAsStringAsync());
                    await ToastMensajeError("Error al intentar Guardar el area.");
                    return false;
                }
                areas = response_2.Response;
                return true;
            }
            await ToastMensajeError($"El area con codigo {area.Id} ya existe.\n\rO el area no es permitida.");
            return false;
        }

        protected async Task<bool> Actualizar(Areas area)
        {
            var response = await AreasService.Actualizar(area.Id, area);
            if (response.Error)
            {
                await ToastMensajeError("Error al intentar Guardar el area.");
                return false;
            }
            areas = area;
            return true;
        }

        protected async Task GuardarArea()
        {
            bool guardado = false;
            if (areas.ESNUEVO)
            {
                guardado = await Agregar(areas);
            }
            else
            {
                guardado = await Actualizar(areas);
            }

            if (guardado)
            {
                Show = false;
                areas.GUARDADO = guardado;
                await OnGuardar.InvokeAsync(areas);
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
