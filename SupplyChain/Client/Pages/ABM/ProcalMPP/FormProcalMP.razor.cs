using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using SupplyChain.Client.HelperService;
using Syncfusion.Blazor.Grids;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using SupplyChain;
using Syncfusion.Blazor.Spinner;
using Syncfusion.Blazor.Notifications;

namespace SupplyChain.Client.Pages.ABM.ProcalMP
{
    public class FormProcalMPBase : ComponentBase
    {
        [Inject] protected HttpClient Http { get; set; }
        [Inject] public ProcalMPService ProcalMPService { get; set; }
        [Parameter] public SupplyChain.ProcalsMP procalMP { get; set; } = new();
        [Parameter] public bool Show { get; set; } = false;
        [Parameter] public EventCallback<SupplyChain.ProcalsMP> onGuardar { get; set; }
        [Parameter] public EventCallback<SupplyChain.ProcalsMP> onEliminar { get; set; }
        [Parameter] public EventCallback OnCerrar { get; set; }

        protected SfGrid<SupplyChain.ProcalsMP> refGridItems;
        protected SfSpinner refSpinnerCli;
        protected bool SpinnerVisible = false;
        protected SfToast ToastObj;
        protected Dictionary<string, object> HtmlAttributeSubmint = new()
           {
            {"type","submit" }
           };
          protected bool IsAdd { get; set; }
        

        protected async Task<bool> Agregar(SupplyChain.ProcalsMP procalMP)
        {
            var response = await ProcalMPService.Existe(procalMP.Id);
            if (!response)
            {
                var response2 = await ProcalMPService.Agregar(procalMP);
                if (response2.Error)
                {
                    Console.WriteLine(await response2.HttpResponseMessage.Content.ReadAsStringAsync());
                    await ToastMensajeError("Error al intentar Guardar la Materia.");
                    return false;
                }
                procalMP = response2.Response;
                return true;
            }
            await ToastMensajeError($"La linea con codigo{procalMP.Id} ya existe.\n\rO la materia no es permitida.");
            return false;
        }

        protected async Task<bool> Actualizar(SupplyChain.ProcalsMP procalMP)
        {
            var response = await ProcalMPService.Actualizar(procalMP.Id, procalMP);
            if (response.Error)
            {
                await ToastMensajeError("Error al intentar Guardar la materia.");
                return false;
            }
            //procalMP = response.Response;
            return true;
        }

        protected async Task GuardarProducto()
        {
            bool guardado = false;
            if (procalMP.ESNUEVO)
            {
                guardado = await Agregar(procalMP);
            }
            else
            {
                guardado = await Actualizar(procalMP);
            }
            if (guardado)
            {
                Show = false;
                procalMP.GUARDADO = guardado;
                await onGuardar.InvokeAsync(procalMP);
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
        private async Task ToastMensajeExito(string content = "Guardado correctamente.")
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

