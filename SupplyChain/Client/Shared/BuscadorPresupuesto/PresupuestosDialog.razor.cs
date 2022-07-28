using Microsoft.AspNetCore.Components;
using SupplyChain.Client.HelperService;
using SupplyChain.Shared;
using Syncfusion.Blazor.Notifications;
using Syncfusion.Blazor.Spinner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SupplyChain.Client.Shared.BuscadorPresupuesto
{
    public class PresupuestosDialogBase : ComponentBase
    {
        [Inject] public PresupuestoService PresupuestoService { get; set; }
        [Parameter] public bool PopupBuscadorVisible { get; set; } = false;
        [Parameter] public EventCallback<Presupuesto> OnObjectSelected { get; set; }
        [Parameter] public EventCallback OnCerrarDialog { get; set; }
        /// <summary>
        /// Verifica la existencia del precio al seleccionar solicitud
        /// </summary>
        [Parameter] public bool ConPrecio { get; set; } = false;
        [Parameter] public bool CompararCliente { get; set; } = false;
        [Parameter] public int Cg_Cli_Comparar { get; set; } = 0;

        protected List<vPresupuestos> presupuestos = new();
        protected SfSpinner refSpinner;
        protected SfToast ToastObj;
        public async Task Show()
        {
            refSpinner?.ShowAsync();
            var response = await PresupuestoService.GetVistaParaGrilla();
            if (response.Error)
            {
                refSpinner?.HideAsync();
                Console.WriteLine(await response.HttpResponseMessage.Content.ReadAsStringAsync());
            }
            else
            {
                presupuestos = response.Response.OrderBy(s => s.Id).ToList();
                if (Cg_Cli_Comparar > 0)
                {
                    presupuestos = presupuestos.Where(c => c.CG_CLI == Cg_Cli_Comparar).ToList();
                }
                refSpinner?.HideAsync();
                PopupBuscadorVisible = true;
            }
        }
        public async Task Hide()
        {
            PopupBuscadorVisible = false;
        }
        protected async Task SendObjectSelected(vPresupuestos obj)
        {
            if (obj != null)
            {
                var response = await PresupuestoService.GetById(obj.Id);
                if (response.Error)
                {
                    await ToastMensajeError("Error al obtener datos del Presupuesto");
                }
                else
                {
                    var presupuesto = response.Response;

                    if (CompararCliente)
                    {
                        if (presupuesto.CG_CLI != Cg_Cli_Comparar)
                        {
                            await ToastMensajeError("No se puede agrega Presupuesto.\nEl Cliente es distinto");
                        }
                        else
                        {
                            await OnObjectSelected.InvokeAsync(presupuesto);
                            await Hide();
                        }
                    }
                    else
                    {
                        if (presupuesto != null)
                        {
                            await OnObjectSelected.InvokeAsync(presupuesto);
                            await Hide();
                        }

                    }

                }
            }


        }

        protected async Task CerrarDialog()
        {
            await Hide();
            await OnCerrarDialog.InvokeAsync();
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
