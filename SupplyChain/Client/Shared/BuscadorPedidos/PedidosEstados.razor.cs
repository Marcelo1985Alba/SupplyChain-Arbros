using Microsoft.AspNetCore.Components;
using SupplyChain.Client.HelperService;
using SupplyChain.Shared;
using Syncfusion.Blazor.Notifications;
using Syncfusion.Blazor.Spinner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SupplyChain.Client.Shared.BuscadorPedidos
{
    public class PedidosEstadosBase : ComponentBase
    {
        [Inject] public EstadoPedidoService EstadoPedidoService { get; set; }
        [Inject] public PedCliService PedCliService { get; set; }
        [Parameter] public bool PopupBuscadorVisible { get; set; } = false;
        [Parameter] public EventCallback<List<vEstadoPedido>> OnObjectSelected { get; set; }
        [Parameter] public EventCallback OnCerrarDialog { get; set; }
        /// <summary>
        /// Verifica la existencia del precio al seleccionar solicitud
        /// </summary>
        [Parameter] public bool ConPrecio { get; set; } = false;
        [Parameter] public bool CompararCliente { get; set; } = false;
        [Parameter] public int Cg_Cli_Comparar { get; set; } = 0;

        protected List<vEstadoPedido> pedidosEstados = new();
        protected SfSpinner refSpinner;
        protected SfToast ToastObj;
        public async Task Show()
        {
            refSpinner?.ShowAsync();
            var response = await EstadoPedidoService.ByEstado(SupplyChain.Shared.Enum.EstadoPedido.PendienteRemitir);
            if (response.Error)
            {
                refSpinner?.HideAsync();
                Console.WriteLine(await response.HttpResponseMessage.Content.ReadAsStringAsync());
            }
            else
            {
                pedidosEstados = response.Response.OrderBy(s => s.Id).ToList();
                if (Cg_Cli_Comparar > 0)
                {
                    pedidosEstados = pedidosEstados.Where(c => c.CG_CLI == Cg_Cli_Comparar).OrderByDescending(p => p.Id).ToList();
                }
                refSpinner?.HideAsync();
                PopupBuscadorVisible = true;
            }
        }
        public async Task Hide()
        {
            PopupBuscadorVisible = false;
        }
        protected async Task SendObjectSelected(vEstadoPedido obj)
        {
            if (obj != null)
            {
                var pedidos = pedidosEstados.Where(e => e.NUM_OCI == obj.NUM_OCI).ToList();
                if (pedidos.Count == 0)
                {
                    await ToastMensajeError("Error al obtener pedidos");
                }
                else
                {
                    //var presupuesto = response.Response;

                    if (CompararCliente)
                    {
                        if (pedidos.Any(p => p.CG_CLI != Cg_Cli_Comparar))
                        {
                            await ToastMensajeError("No se puede agrega Presupuesto.\nEl Cliente es distinto");
                        }
                        else
                        {
                            await OnObjectSelected.InvokeAsync(pedidos);
                            await Hide();
                        }
                    }
                    else
                    {
                        if (pedidos != null)
                        {
                            await OnObjectSelected.InvokeAsync(pedidos);
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
