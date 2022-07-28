using Microsoft.AspNetCore.Components;
using SupplyChain.Client.HelperService;
using SupplyChain.Client.Shared;
using SupplyChain.Shared;
using SupplyChain.Shared.Logística;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Navigations;
using Syncfusion.Blazor.Notifications;
using Syncfusion.Blazor.Spinner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SupplyChain.Client.Pages.Ventas._2_Pedidos
{
    public class PedidosBase : ComponentBase
    {
        [Inject] public PedCliService PedCliService { get; set; }
        [Inject] public CondicionPagoService CondicionPagoService { get; set; }
        [Inject] public CondicionEntregaService CondicionEntregaService { get; set; }
        [Inject] public DireccionEntregaService DireccionEntregaService { get; set; }
        [CascadingParameter] public MainLayout MainLayout { get; set; }
        protected SfGrid<PedCli> refGrid;
        protected SfSpinner refSpinner;
        protected SfToast ToastObj;
        protected FormPedido refFormPedido;
        protected List<PedCli> pedidos = new();
        protected PedCliEncabezado PedidoSeleccionado = new();
        protected bool SpinnerVisible = false;
        protected bool popupFormVisible = false;

        protected List<Object> Toolbaritems = new()
        {
            "Search",
            //new ItemModel { Text = "Add", TooltipText = "Agregar un nuevo Presupuesto", PrefixIcon = "e-add", Id = "Add" },
            "Add",
            "Edit",
            "Delete",
            "Print",
            //new ItemModel { Text = "Copy", TooltipText = "Copy", PrefixIcon = "e-copy", Id = "copy" },
            "ExcelExport",
            new ItemModel { Text = "", TooltipText = "Actualizar Grilla", PrefixIcon = "e-refresh", Id = "refresh" },
        };
        protected List<string> direccionesEntregas = new();
        protected List<vCondicionesPago> condicionesPagos = new();
        protected List<vCondicionesEntrega> condicionesEntrega = new();

        protected async override Task OnInitializedAsync()
        {
            SpinnerVisible = true;
            await GetPedidos();
            //await refGrid.AutoFitColumnsAsync();
            SpinnerVisible = false;
        }

        protected async Task GetPedidos()
        {
            var response = await PedCliService.Get();
            if (response.Error)
            {

            }
            else
            {
                await GetCondicionesPago();
                await GetCondicionesEntrega();
                pedidos = response.Response;
            }
        }

        protected async Task GetCondicionesPago()
        {
            var response = await CondicionPagoService.Get();
            if (response.Error)
            {
                //await ToastMensajeError("Error al obtener Condiciones de Pago.");
            }
            else
            {
                condicionesPagos = response.Response;
            }
        }
        protected async Task GetCondicionesEntrega()
        {
            var response = await CondicionEntregaService.Get();
            if (response.Error)
            {
                //await ToastMensajeError("Error al obtener Condiciones de Entrega.");
            }
            else
            {
                condicionesEntrega = response.Response;
            }
        }


        protected async Task OnActionBeginHandler(ActionEventArgs<PedCli> args)
        {
            if (args.RequestType == Syncfusion.Blazor.Grids.Action.Add ||
                args.RequestType == Syncfusion.Blazor.Grids.Action.BeginEdit)
            {
                args.Cancel = true;
                args.PreventRender = false;
            }

            if (args.RequestType == Syncfusion.Blazor.Grids.Action.Add)
            {
                SpinnerVisible = true;
                PedidoSeleccionado = new();
                popupFormVisible = true;
                SpinnerVisible = false;
            }

            if (args.RequestType == Syncfusion.Blazor.Grids.Action.BeginEdit)
            {
                SpinnerVisible = true;
                var response = await PedCliService.GetPedidoEncabezadoById(args.Data.Id);
                if (response.Error)
                {
                    await ToastMensajeError();
                }
                else
                {

                    PedidoSeleccionado = response.Response;
                    direccionesEntregas = PedidoSeleccionado.DireccionesEntregas.Select(d=> d.DESCRIPCION).ToList();
                    popupFormVisible = true;
                }
                SpinnerVisible = false;
            }
        }

        protected async Task OnActionCompleteHandler(ActionEventArgs<PedCli> args)
        {
            if (args.RequestType == Syncfusion.Blazor.Grids.Action.Add ||
                args.RequestType == Syncfusion.Blazor.Grids.Action.BeginEdit)
            {
                args.Cancel = true;
                args.PreventRender = false;
            }


        }

        protected async Task OnToolbarHandler(ClickEventArgs args)
        {
            if (args.Item.Id == "refresh")
            {
                SpinnerVisible = true;
                await GetPedidos();
                SpinnerVisible = false;
            }
        }


        private async Task ToastMensajeError()
        {
            await ToastObj.Show(new ToastModel
            {
                Title = "Error!",
                Content = "Ocurrio un Error.",
                CssClass = "e-toast-warning",
                Icon = "e-warning toast-icons",
                ShowCloseButton = true,
                ShowProgressBar = true
            });
        }
    }
}
