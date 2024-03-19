using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SupplyChain.Client.HelperService;
using SupplyChain.Shared;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Notifications;
using Syncfusion.Blazor.Spinner;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace SupplyChain.Client.Pages.ABM.ProcunProcesos
{
    public class FormProcunProcesosBase : ComponentBase
    {
        [Inject] protected HttpClient Http { get; set; }
        [Inject] protected IJSRuntime JSRuntime { get; set; }
        [Inject] protected ProcunProcesoService ProcunProcesoService { get; set; }
        [Parameter] public ProcunProceso ProcunProceso { get; set; }
        [Parameter] public bool Show {  get; set; }
        [Parameter] public EventCallback<ProcunProceso> OnGuardar { get; set; }
        [Parameter] public EventCallback<ProcunProceso> OnEliminar{ get; set; }
        [Parameter] public EventCallback OnCerrar{ get; set; }
        protected SfGrid<ProcunProceso> refGridItems;
        protected SfSpinner refSpinnerCli;
        protected bool SpinnerVisible=false;
        protected SfToast ToastObj;
        protected Dictionary<string, object> HtmlAttributeSubmit = new()
        {
            { "type", "submit" }
        };
        protected bool IsAdd { get; set; }

        protected async override Task OnInitializedAsync()
        {
        }

        protected async Task<bool> Agregar(ProcunProceso procunProceso)
        {
            var response = await ProcunProcesoService.Existe(procunProceso.Id);
            if (!response)
            {
                var response2= await ProcunProcesoService.Agregar(procunProceso);
                if(response2.Error)
                {
                    Console.WriteLine(await response2.HttpResponseMessage.Content.ReadAsStringAsync());
                    await ToastMensajeError("Error al intentar Guardar el procedimiento.");
                    return false;
                }
                procunProceso = response2.Response;
                return true;
            }
            await ToastMensajeError($"El procedimiento con codigo {procunProceso.Id} ya existe.\n\rO el procedimiento no es permitido.");
            return false;
        }

        protected async Task<bool> Actualizar(ProcunProceso procunProceso)
        {
            var response = await ProcunProcesoService.Actualizar(procunProceso.Id, procunProceso);
            if(response.Error)
            {
                await ToastMensajeError("Error al intentar Guardar el procedimiento.");
                return false;
            }
            ProcunProceso = procunProceso;
            return true;
        }

        protected async Task GurdarProcedimiento()
        {
            bool guardado=false;
            if (ProcunProceso.ESNUEVO)
            {
                guardado = await Agregar(ProcunProceso);
            }
            else
            {
                guardado = await Actualizar(ProcunProceso);
            }

            if (guardado)
            {
                Show = false;
                ProcunProceso.GUARDADO=guardado;
                await OnGuardar.InvokeAsync(ProcunProceso);
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
                Icon = "e-su    ccess toast-icons",
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
