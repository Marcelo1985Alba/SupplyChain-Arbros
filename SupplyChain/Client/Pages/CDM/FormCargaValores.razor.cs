using Microsoft.AspNetCore.Components;
using SupplyChain.Client.HelperService;
using Syncfusion.Blazor.Grids;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Syncfusion.Blazor.Spinner;
using Syncfusion.Blazor.Notifications;

namespace SupplyChain.Client.Pages.CDM
{
    public class FormCargaValoresBase : ComponentBase
    {
        [Inject] protected HttpClient Http { get; set; }
        [Inject] public CargaValoresService CargaValoresService { get; set; }
        [Parameter] public Valores valor { get; set; } = new();
        [Parameter] public bool Show { get; set; } = false;
        [Parameter] public EventCallback<Valores> OnGuardar { get; set; }
        [Parameter] public EventCallback<Valores> OnEliminar { get; set; }
        [Parameter] public EventCallback OnCerrar { get; set; }

        protected SfGrid<Valores> refGridItems;
        protected SfSpinner refSpinnerCli;
        protected bool SpinnerVisible = false;
        protected SfToast ToastObj;
        protected Dictionary<string, object> HtmlAttributeSubmint = new()
        {
            {"type", "submit" }
        };
        protected bool IsAdd { get; set; }

        protected async Task<bool>Agregar(Valores valor)
        {
            var response = await CargaValoresService.Existe(valor.Id);
            if (!response)
            {
                var response2 = await CargaValoresService.Agregar(valor);
                if (response2.Error)
                {
                    Console.WriteLine(await response2.HttpResponseMessage.Content.ReadAsStringAsync());
                    await ToastMensajeError("Error al intentar Guardar un Proceso.");
                    return false;
                }
                valor = response2.Response;
                return true;
            }
            await ToastMensajeError($"El proceso con codigo{valor.Id} ya existe.\n\rO el proceso no es permitido.");
            return false;
        }

        protected async Task<bool> Actualizar(Valores valor)
        {
            var response= await CargaValoresService.Actualizar(valor.Id, valor);
            if (response.Error)
            {
                await ToastMensajeError("Error al intenar guardar el Proceso.");
                return false;  
            }
            return true;
        }

        protected async Task GuardarProceso()
        {
            bool guardado = false;
            if (valor.ESNUEVO)
            {
                guardado = await Agregar(valor);
            }
            else
            {
                guardado = await Actualizar(valor);
            }
            if (guardado)
            {
                Show = false;
                valor.GUARDADO = guardado;
                await OnGuardar.InvokeAsync(valor);
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
        private async Task ToastMensajeExito(string content="Guardado Correctamente.")
        {
            await this.ToastObj.Show(new ToastModel
            {
                Title = "EXITO!",
                Content = content,
                CssClass = "e-toast-success",
                Icon = "e-warning toast-icons",
                ShowCloseButton = true,
                ShowProgressBar = true
            });
        }
        private async Task ToastMensajeError(string content = "Ocurrio un error.")
        {
            await ToastObj.Show(new ToastModel
            {
                Title = "EXITO!",
                Content = content,
                CssClass = "e-toast-success",
                Icon = "e-warning toast-icons",
                ShowCloseButton = true,
                ShowProgressBar = true
            });

        }
    }
}
