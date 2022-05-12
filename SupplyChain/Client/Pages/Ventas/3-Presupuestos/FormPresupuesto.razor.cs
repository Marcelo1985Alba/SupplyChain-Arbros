using Microsoft.AspNetCore.Components;
using SupplyChain.Client.HelperService;
using SupplyChain.Client.Shared.BuscadorCliente;
using SupplyChain.Client.Shared.BuscadorProducto;
using SupplyChain.Shared;
using SupplyChain.Shared.Models;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Notifications;
using Syncfusion.Blazor.Spinner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SupplyChain.Client.Pages.Ventas._3_Presupuestos
{
    public class FormPresupuestoBase : ComponentBase
    {
        [Inject] public PresupuestoService PresupuestoService { get; set; }
        /// <summary>
        /// objecto modificado el cual tambien obtiene la id nueva en caso de agregar un nuevo
        /// </summary>
        [Parameter] public Presupuesto Presupuesto { get; set; } = new ();
        [Parameter] public bool Show { get; set; } = false;
        [Parameter] public EventCallback<Presupuesto> OnGuardar { get; set; }
        [Parameter] public EventCallback OnCerrar { get; set; }

        protected SfGrid<PresupuestoDetalle> refGridItems;
        protected ClientesDialog refClienteDialog;
        protected SfSpinner refSpinnerCli;
        protected bool popupBuscadorVisibleCliente { get; set; } = false;

        protected ProductoDialog refProductoDialog;
        protected bool popupBuscadorVisibleProducto { get; set; } = false;

        protected List<string> Monedas = new() {"PESOS","DOLARES" };
        protected bool SpinnerVisible = false;
        protected SfToast ToastObj;

        protected Dictionary<string, object> HtmlAttribute = new()
        {
            { "type", "button" }
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
            Presupuesto.CG_CLI = Convert.ToInt32(clienteSelected.CG_CLI);
            Presupuesto.DES_CLI = clienteSelected.DESCRIPCION.Trim();
            //Presupuesto.Cuit = clienteSelected.CUIT;
            await refSpinnerCli.HideAsync();
        }

        protected async Task ProductoSelected(Producto productoSelected)
        {
            await refSpinnerCli.ShowAsync();
            popupBuscadorVisibleProducto = false;

            var item = new PresupuestoDetalle()
            {
                CANTIDAD = 1,
                CG_ART = productoSelected.Id,
                DES_ART = productoSelected.DES_PROD
            };

            Presupuesto.Items.Add(item);
            await refGridItems.RefreshHeaderAsync();
            refGridItems.Refresh();
            await refGridItems.RefreshColumnsAsync();
            await refSpinnerCli.HideAsync();
        }


        protected async Task<bool> Agregar(Presupuesto presupueto)
        {
            var response = await PresupuestoService.Agregar(presupueto);
            if (response.Error)
            {
                return false;
            }
            Presupuesto = response.Response;
            return true;
        }

        protected async Task<bool> Actualizar(Presupuesto presupuesto)
        {
            var response = await PresupuestoService.Actualizar(presupuesto.Id, presupuesto);
            if (response.Error)
            {
                return false;
            }

            Presupuesto = presupuesto;
            return true;
        }

        protected async Task Guardar()
        {
            bool guardado;
            if (Presupuesto.Id == 0)
            {
                guardado = await Agregar(Presupuesto);
                Presupuesto.ESNUEVO = true;
            }
            else
            {
                guardado = await Actualizar(Presupuesto);
            }

            Show = false;
            Presupuesto.GUARDADO = guardado;
            await OnGuardar.InvokeAsync(Presupuesto);
        }

        public async Task Hide()
        {
            Show = false;
        }

        protected async Task OnCerrarDialog()
        {
            await OnCerrar.InvokeAsync();
        }
    }
}
