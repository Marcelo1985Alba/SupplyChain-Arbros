using Microsoft.AspNetCore.Components;
using SupplyChain.Client.HelperService;
using SupplyChain.Client.Shared.BuscadorCliente;
using SupplyChain.Client.Shared.BuscadorProducto;
using SupplyChain.Shared;
using SupplyChain.Shared.Models;
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
        /// <summary>
        /// objecto modificado el cual tambien obtiene la id nueva en caso de agregar un nuevo
        /// </summary>
        [Parameter] public Solicitud Solicitud { get; set; } = new Solicitud();
        [Parameter] public bool Show { get; set; } = false;
        [Parameter] public EventCallback<Solicitud> OnGuardar { get; set; }
        [Parameter] public EventCallback OnCerrar { get; set; }

        [Parameter] public string HeightDialog { get; set; } = "380px";
        protected ClientesDialog refClienteDialog;
        protected SfSpinner refSpinnerCli;
        protected bool popupBuscadorVisibleCliente { get; set; } = false;

        protected ProductoDialog refProductoDialog;
        protected bool popupBuscadorVisibleProducto { get; set; } = false;


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

        protected async Task BuscarProductos()
        {
            SpinnerVisible = true;
            await refProductoDialog.Show();
            SpinnerVisible = false;
            popupBuscadorVisibleProducto = true;
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

        protected async Task ProductoSelected(Producto productoSelected)
        {
            await refSpinnerCli.ShowAsync();
            popupBuscadorVisibleProducto = false;
            Solicitud.Producto = productoSelected.Id;
            Solicitud.Des_Prod = productoSelected.DES_PROD;
            if (Solicitud.Producto.StartsWith("00"))
            {
                HeightDialog = "450px";
            }
            else
            {
                HeightDialog = "600px";
            }
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

        protected async Task CerrarDialogCliente()
        {
            popupBuscadorVisibleCliente = false;
        }
        protected async Task CerrarDialogProducto()
        {
            popupBuscadorVisibleProducto = false;
        }

        protected async Task OnCerrarDialog()
        {
            await OnCerrar.InvokeAsync();
        }
        
    }
}
