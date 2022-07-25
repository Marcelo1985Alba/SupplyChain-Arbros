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

namespace SupplyChain.Client.Pages.ABM.LineasP
{
    public class FormLineasBase : ComponentBase
    {
        [Inject] protected HttpClient Http { get; set; }
        [Inject] public LineasService LineasService { get; set; }
        [Parameter] public Lineas lineas { get; set; } = new();
        [Parameter] public bool Show { get; set; } = false;
        [Parameter] public EventCallback<Lineas> OnGuardar { get; set; }
        [Parameter] public EventCallback<Lineas> OnEliminar { get; set; }
        [Parameter] public EventCallback OnCerrar { get; set; }

        protected SfGrid<Lineas> refGridItems;
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

        protected async Task<bool> Agregar(Lineas linea)
        {
            var response = await LineasService.Existe(linea.Id);
            if (!response)
            {
                var response_2 = await LineasService.Agregar(linea);
                if (response_2.Error)
                {
                    Console.WriteLine(await response_2.HttpResponseMessage.Content.ReadAsStringAsync());
                    await ToastMensajeError("Error al intentar Guardar la linea.");
                    return false;
                }
                linea = response_2.Response;
                return true;
            }
            await ToastMensajeError($"La linea con codigo {linea.Id} ya existe.\n\rO la linea no es permitida.");
            return false;
        }

        protected async Task<bool> Actualizar(Lineas linea)
        {
            var response = await LineasService.Actualizar(linea.Id, linea);
            if (response.Error)
            {
                await ToastMensajeError("Error al intentar Guardar la linea.");
                return false;
            }
            lineas = linea;
            return true;
        }

        protected async Task GuardarLinea()
        {
            bool guardado = false;
            if (lineas.ESNUEVO)
            {
                guardado = await Agregar(lineas);
            }
            else
            {
                guardado = await Actualizar(lineas);
            }

            if (guardado)
            {
                Show = false;
                lineas.GUARDADO = guardado;
                await OnGuardar.InvokeAsync(lineas);
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
