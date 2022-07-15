using Microsoft.AspNetCore.Components;
using SupplyChain.Client.HelperService;
using SupplyChain.Client.Shared.BuscadorCliente;
using SupplyChain.Client.Shared.BuscadorPrecios;
using SupplyChain.Client.Shared.BuscadorProducto;
using SupplyChain.Shared;
using SupplyChain.Shared.Models;
using Syncfusion.Blazor.Inputs;
using Syncfusion.Blazor.Notifications;
using Syncfusion.Blazor.Spinner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SupplyChain.Client.Pages.Ventas._4_Solicitudes
{
    public class FormSolicitudBase : ComponentBase
    {
        [Inject] public SolicitudService SolicitudService { get; set; }
        [Inject] public ClienteService ClienteService { get; set; }
        [Inject] public PrecioArticuloService PrecioArticuloService { get; set; }
        /// <summary>
        /// objecto modificado el cual tambien obtiene la id nueva en caso de agregar un nuevo
        /// </summary>
        [Parameter] public Solicitud Solicitud { get; set; } = new Solicitud();
        [Parameter] public bool Show { get; set; } = false;
        [Parameter] public EventCallback<Solicitud> OnGuardar { get; set; }
        [Parameter] public EventCallback OnCerrar { get; set; }

        [Parameter] public string HeightDialog { get; set; } = "280px";
        protected ClientesDialog refClienteDialog;
        protected SfSpinner refSpinnerCli;
        protected bool popupBuscadorVisibleCliente { get; set; } = false;

        protected PreciosDialog refPreciosDialog;
        protected bool popupBuscadorVisiblePrecio { get; set; } = false;
        

        protected bool SpinnerVisible = false;
        protected SfToast ToastObj;

        protected Dictionary<string, object> HtmlAttribute = new()
        {
           {"type", "button" }
        };

        protected Dictionary<string, object> HtmlAttributeSubmit = new()
        {
            { "type", "submit" }
        };

        protected async Task BuscarClientes()
        {
            SpinnerVisible = true;
            await refClienteDialog.Show();
            SpinnerVisible = false;
            popupBuscadorVisibleCliente = true;
        }

        protected async Task BuscarPreciosArticulos()
        {
            SpinnerVisible = true;
            await refPreciosDialog.Show();
            SpinnerVisible = false;
            popupBuscadorVisiblePrecio = true;
        }

        protected async Task ClienteExternoSelected(ClienteExterno clienteSelected)
        {
            await refSpinnerCli.ShowAsync();
            popupBuscadorVisibleCliente = false;
            Solicitud.CG_CLI = Convert.ToInt32(clienteSelected.CG_CLI);
            Solicitud.Des_Cli = clienteSelected.DESCRIPCION.Trim();
            Solicitud.Cuit = clienteSelected.CUIT;
            await refSpinnerCli.HideAsync();
        }

        protected async Task PrecioSelected(PreciosArticulos precioSelected)
        {
            await refSpinnerCli.ShowAsync();
            popupBuscadorVisiblePrecio = false;
            Solicitud.Producto = precioSelected.Id;
            Solicitud.Des_Prod = precioSelected.Descripcion;
            //if (Solicitud.Producto.StartsWith("00"))
            //{
            //    HeightDialog = "450px";
            //}
            //else
            //{
            //    HeightDialog = "500px";
            //}
            await refSpinnerCli.HideAsync();
        }


        protected async Task<bool> Agregar(Solicitud solicitud)
        {
            var response = await SolicitudService.Agregar(solicitud);
            if (response.Error)
            {
                return false;
            }
            Solicitud = response.Response;
            return true;
        }

        protected async Task<bool> Actualizar(Solicitud solicitud)
        {
            var response = await SolicitudService.Actualizar(solicitud.Id, solicitud);
            if (response.Error)
            {
                return false;
            }

            Solicitud = solicitud;
            return true;
        }

        protected async Task Guardar()
        {
            bool guardado;
            if (Solicitud.Id == 0)
            {
                guardado = await Agregar(Solicitud);
                Solicitud.EsNuevo = true;
            }
            else
            {
                guardado = await Actualizar(Solicitud);
            }

            Show = false;
            Solicitud.Guardado = guardado;
            await OnGuardar.InvokeAsync(Solicitud);
        }

        public async Task Hide()
        {
            Show = false;
        }

        protected async Task Des_cli_Changed(InputEventArgs args)
        {
            string des_cli = args.Value.ToString();

            Solicitud.Des_Cli = des_cli;
            var response = await ClienteService.Search(Solicitud.CG_CLI, Solicitud.Des_Cli);
            if (response.Error)
            {

                await ToastMensajeError("Al obtener cliente");
            }
            else
            {
                if (response.Response != null)
                {
                    if (response.Response.Count == 1)
                    {
                        Solicitud.CG_CLI = int.Parse(response.Response[0].CG_CLI);
                        Solicitud.Des_Cli = response.Response[0].DESCRIPCION;
                        Solicitud.Cuit = response.Response[0].CUIT;
                    }
                }
                else
                {
                    Solicitud.CG_CLI = 0;
                    Solicitud.Des_Cli = string.Empty;
                }

            }
        }


        protected async Task Cg_cli_Changed(ChangeEventArgs args)
        {
            string cg_cli = args.Value.ToString();
            if (!string.IsNullOrEmpty(cg_cli))
            {
                //Solicitud.CG_CLI = int.Parse(cg_cli);
                if (Solicitud.CG_CLI > 0)
                {
                    var response = await ClienteService.Search(Solicitud.CG_CLI, Solicitud.Des_Cli);
                    if (response.Error)
                    {
                        await ToastMensajeError("Al obtener cliente");
                    }
                    else
                    {
                        if (response.Response != null)
                        {
                            if (response.Response.Count == 1)
                            {
                                Solicitud.CG_CLI = int.Parse(response.Response[0].CG_CLI);
                                Solicitud.Des_Cli = response.Response[0].DESCRIPCION;
                                Solicitud.Cuit = response.Response[0].CUIT;
                            }
                            else
                            {
                                Solicitud.CG_CLI = 0;
                                Solicitud.Des_Cli = string.Empty;
                            }
                        }
                        else
                        {
                            Solicitud.CG_CLI = 0;
                            Solicitud.Des_Cli = string.Empty;
                        }

                    }
                }
            }
            else
            {
                Solicitud.CG_CLI = 0;
                Solicitud.Des_Cli = string.Empty;
            }


        }

        protected async Task Cg_Prod_Changed(InputEventArgs args)
        {
            string cg_prod = args.Value;

            Solicitud.Producto = cg_prod;
            var response = await PrecioArticuloService.Search(cg_prod, Solicitud.Des_Prod);
            if (response.Error)
            {

                await ToastMensajeError("Al obtener Precio de articulo");
            }
            else
            {
                if (response.Response != null)
                {
                    if (response.Response.Count == 1)
                    {
                        Solicitud.Producto = response.Response[0].Id;
                        Solicitud.Des_Prod = response.Response[0].Descripcion;
                    }
                    else
                    {
                        Solicitud.Des_Prod = string.Empty;
                    }
                }

            }
        }

        protected async Task Des_Prod_Changed(InputEventArgs args)
        {
            string des_prod = args.Value;

            Solicitud.Des_Prod = des_prod;
            var response = await PrecioArticuloService.Search(Solicitud.Producto, Solicitud.Des_Prod);
            if (response.Error)
            {

                await ToastMensajeError("Al obtener Precio de articulo");
            }
            else
            {
                if (response.Response != null)
                {
                    if (response.Response.Count == 1)
                    {
                        Solicitud.Producto = response.Response[0].Id;
                        Solicitud.Des_Prod = response.Response[0].Descripcion;
                    }
                    else
                    {
                        Solicitud.Producto = string.Empty;
                    }
                }

            }
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
        protected async Task CerrarDialogCliente()
        {
            popupBuscadorVisibleCliente = false;
        }
        protected async Task CerrarDialogPrecio()
        {
            popupBuscadorVisiblePrecio = false;
        }

        protected async Task OnCerrarDialog()
        {
            await OnCerrar.InvokeAsync();
        }
        
    }
}
