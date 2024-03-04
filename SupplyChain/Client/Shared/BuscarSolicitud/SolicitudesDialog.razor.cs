using Microsoft.AspNetCore.Components;
using SupplyChain.Client.HelperService;
using SupplyChain.Client.RepositoryHttp;
using SupplyChain.Shared;
using SupplyChain.Shared.Enum;
using SupplyChain.Shared.Models;
using Syncfusion.Blazor.Notifications;
using Syncfusion.Blazor.Spinner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SupplyChain.Client.Shared.BuscarSolicitud
{
    public class SolicitudesDialogBase : ComponentBase
    {
        [Inject] public SolicitudService SolicitudService { get; set; }
        [Inject] public PrecioArticuloService PrecioArticuloService { get; set; }
        [Parameter] public bool PopupBuscadorVisible { get; set; } = false;
        [Parameter] public EventCallback<Solicitud> OnObjectSelected { get; set; }
        [Parameter] public EventCallback OnCerrarDialog { get; set; }
        /// <summary>
        /// Verifica la existencia del precio al seleccionar solicitud
        /// </summary>
        [Parameter] public TipoFiltro TipoFiltro { get; set; } = TipoFiltro.Todos;
        [Parameter] public bool ConPrecio { get; set; } = false;
        [Parameter] public bool CompararCliente { get; set; } = false;
        //[Parameter] public int Cg_Cli_Comparar { get; set; } = 0;

        protected List<vSolicitudes> solicitudes = new();
        protected SfSpinner refSpinner;
        protected SfToast ToastObj;
        public async Task Show()
        {
            refSpinner?.ShowAsync();
            var response = await SolicitudService.GetVistaParaGrilla(TipoFiltro);
            if (response.Error)
            {
                refSpinner?.HideAsync();
                Console.WriteLine(await response.HttpResponseMessage.Content.ReadAsStringAsync());
            }
            else
            {
                solicitudes = response.Response.OrderByDescending(s=> s.Id).ToList();
                //if (Cg_Cli_Comparar > 0)
                //{
                //    solicitudes = solicitudes.Where(c => c.CG_CLI == Cg_Cli_Comparar).ToList();
                //}
                refSpinner?.HideAsync();
                PopupBuscadorVisible = true;
            }
        }
        public async Task Hide()
        {
            PopupBuscadorVisible = false;
        }
        protected async Task SendObjectSelected(vSolicitudes obj)
        {
            if (obj != null)
            {
                var response = await SolicitudService.GetById(obj.Id);
                if (response.Error)
                {
                    await ToastMensajeError("Error al obtener datos de la Solicitud");
                }
                else
                {
                    var solicitud = response.Response;
                    solicitud.Des_Prod = obj.Descripcion;
                    solicitud.Des_Cli = obj.DES_CLI;
                    if (ConPrecio)
                    {
                        await SetPrecio(solicitud);
                    }

                    if (CompararCliente)
                    {
                        if (ConPrecio && solicitud.PrecioArticulo != null)
                        {
                            if (solicitud != null)
                            {
                                await OnObjectSelected.InvokeAsync(solicitud);
                                await Hide();
                            }
                        }
                    }
                    else
                    {
                        if (solicitud != null)
                        {
                            await OnObjectSelected.InvokeAsync(solicitud);
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

        private async Task SetPrecio(Solicitud solicitud)
        {
            var precio = await GetPrecio(solicitud.Producto);

            if (precio != null)
            {
                solicitud.PrecioArticulo = precio;
            }
            else
            {
                await ToastMensajeError("No se puede agregar Solicitud.\nProducto sin precio");
            }
        }

        protected async Task<PreciosArticulos> GetPrecio(string precioArt)
        {
            if (await PrecioArticuloService.Existe(precioArt))
            {
                var response = await PrecioArticuloService.GetById(precioArt);
                if (response.Error)
                {
                    await ToastMensajeError();
                    return null;
                }

                return response.Response;
            }

            return null;
            
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
