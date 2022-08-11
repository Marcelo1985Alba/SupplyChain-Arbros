﻿using Microsoft.AspNetCore.Components;
using SupplyChain.Client.HelperService;
using SupplyChain.Client.Shared;
using SupplyChain.Shared;
using SupplyChain.Shared.Enum;
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
        [Inject] public TransporteService TransporteService { get; set; }
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
            new ItemModel { Text = "Ver Todos", Id = "VerTodos" },
            new ItemModel { Text = "Ver Pendientes", Id = "VerPendientes" },
        };
        protected List<string> direccionesEntregas = new();
        protected List<vCondicionesPago> condicionesPagos = new();
        protected List<vCondicionesEntrega> condicionesEntrega = new();
        protected List<vTransporte> transportes = new();

        protected async override Task OnInitializedAsync()
        {
            MainLayout.Titulo = "Pedidos";
            SpinnerVisible = true;
            await GetPedidos(TipoFiltro.Pendientes);
            //await refGrid.AutoFitColumnsAsync();
            SpinnerVisible = false;
        }

        protected async Task GetPedidos(TipoFiltro tipoFiltro = TipoFiltro.Todos)
        {
            var response = await PedCliService.GetByFilter(tipoFiltro);
            if (response.Error)
            {

            }
            else
            {
                await GetCondicionesPago();
                await GetCondicionesEntrega();
                await GetTransportes();
                pedidos = response.Response.OrderByDescending(p=> p.FE_PED).ToList();
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

        protected async Task GetTransportes()
        {
            var response = await TransporteService.Get();
            if (response.Error)
            {
                //await ToastMensajeError("Error al obtener Transportes.");
            }
            else
            {
                transportes = response.Response;
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
                    foreach (var item in PedidoSeleccionado.Items)
                    {
                        item.ESTADO = SupplyChain.Shared.Enum.EstadoItem.Modificado;
                    }
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

        protected async Task OnPedidoGuardado(PedCliEncabezado pedidoEncabezado )
        {
            foreach (var item in pedidoEncabezado.Items)
            {
                if (item.ESTADO == SupplyChain.Shared.Enum.EstadoItem.Agregado)
                {
                    pedidos.Add(item);
                }

                if (item.ESTADO == SupplyChain.Shared.Enum.EstadoItem.Modificado)
                {
                    var pedido = pedidos.Where(p => p.PEDIDO == item.PEDIDO).First();
                    pedido.ORCO = item.ORCO;
                    pedido.CANTPED = item.CANTPED;
                    pedido.OBSERITEM = item.OBSERITEM;
                    pedido.TOTAL = item.TOTAL;
                }
            }

            popupFormVisible = false;
            pedidos = pedidos.OrderByDescending(c => c.PEDIDO).ToList();
            refGrid.Refresh();
        }

        protected async Task OnToolbarHandler(ClickEventArgs args)
        {
            if (args.Item.Id == "refresh" || args.Item.Id == "VerTodos")
            {
                SpinnerVisible = true;
                await GetPedidos();
                SpinnerVisible = false;
            }
            else if(args.Item.Id == "VerPendientes")
            {
                SpinnerVisible = true;
                await GetPedidos(TipoFiltro.Pendientes);
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
