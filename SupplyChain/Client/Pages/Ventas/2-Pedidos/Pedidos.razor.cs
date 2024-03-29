﻿using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using SupplyChain.Client.HelperService;
using SupplyChain.Client.RepositoryHttp;
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
using System.Net.Http;
using System.Threading.Tasks;

namespace SupplyChain.Client.Pages.Ventas._2_Pedidos
{
    public class PedidosBase : ComponentBase
    {
        [Inject] public PedCliService PedCliService { get; set; }
        [Inject] public StockService StockService { get; set; }
        [Inject] public CondicionPagoService CondicionPagoService { get; set; }
        [Inject] public CondicionEntregaService CondicionEntregaService { get; set; }
        [Inject] public DireccionEntregaService DireccionEntregaService { get; set; }
        [Inject] public TransporteService TransporteService { get; set; }
        [CascadingParameter] public MainLayout MainLayout { get; set; }
        protected SfGrid<PedCli> refGrid;
        protected SfSpinner refSpinner;
        protected SfToast ToastObj;
        protected ConfirmacionDialog refConfirmacionDialog;
        protected FormPedido refFormPedido;
        protected List<PedCli> pedidos = new();
        protected PedCliEncabezado PedidoSeleccionado = new();
        protected bool SpinnerVisible = false;
        protected bool popupFormVisible = false;
        protected bool abrePorPedido = false;
        protected bool eliminaPorPedido = false;

        #region "Vista Grilla"
        protected const string APPNAME = "grdPedidos";
        protected string state;
        #endregion

        protected List<Object> Toolbaritems = new()
        {
            "Search",
            //new ItemModel { Text = "Add", TooltipText = "Agregar un nuevo Presupuesto", PrefixIcon = "e-add", Id = "Add" },
            "Add",
            "Edit",
            "Delete",
            new ItemModel { Text = "Imprimir", TooltipText = "Imprimir por Pedido o OCI", 
                PrefixIcon = "e-print e-icons e-btn-icon e-icon-left", Id = "imprimir" },
            //new ItemModel { Text = "Copy", TooltipText = "Copy", PrefixIcon = "e-copy", Id = "copy" },
            new ItemModel { Text= "ExcelExport", Id="ExcelExport", TooltipText="ExcelExport" },
            new ItemModel { Text = "", TooltipText = "Actualizar Grilla", PrefixIcon = "e-refresh", Id = "refresh" },
            new ItemModel { Text = "Ver Todos", Id = "VerTodos" },
            new ItemModel { Text = "Ver Pendientes", Id = "VerPendientes" },
        };
        protected List<string> direccionesEntregas = new();
        protected List<vCondicionesPago> condicionesPagos = new();
        protected List<vCondicionesEntrega> condicionesEntrega = new();
        protected List<vTransporte> transportes = new();
        protected ConfirmacionDialog ConfirmacionEliminarDialog;
        protected ConfirmacionDialog PreguntaEliminarDialog;
        protected string textbotonPorPedido = "Por Pedido";
        protected string textbotonPorOCI = "Por OCI";
        protected async override Task OnInitializedAsync()
        {
            MainLayout.Titulo = "Pedidos";
            SpinnerVisible = true;
            await GetPedidos(TipoFiltro.Pendientes);
            //await refGrid.AutoFitColumnsAsync();
            SpinnerVisible = false;
        }


        #region "Eventos Vista Grilla"
        protected async Task OnVistaSeleccionada(VistasGrillas vistasGrillas)
        {
            await refGrid.SetPersistData(vistasGrillas.Layout);
        }
        protected async Task OnReiniciarGrilla()
        {
            await refGrid.ResetPersistData();
        }
        #endregion

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
                pedidos = response.Response.OrderByDescending(p=> p.PEDIDO).ToList();
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

        protected async Task Imprimir()
        {
            var seleccion = await refGrid.GetSelectedRecordsAsync();
            if (seleccion != null && seleccion.Count > 0)
            {
                await PedCliService.ImprimirNumOci(seleccion[0].NUMOCI);
            }
            
        }


        protected async Task OnActionBeginHandler(ActionEventArgs<PedCli> args)
        {
            if (args.RequestType == Syncfusion.Blazor.Grids.Action.Add ||
                args.RequestType == Syncfusion.Blazor.Grids.Action.BeginEdit ||
                args.RequestType == Syncfusion.Blazor.Grids.Action.Delete )
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
                
                PedidoSeleccionado.CG_CLI = args.Data.CG_CLI;
                PedidoSeleccionado.DES_CLI= args.Data.DES_CLI;
                PedidoSeleccionado.TC = Convert.ToDouble(args.Data.VA_INDIC);
                PedidoSeleccionado.DIRENT = args.Data.DIRENT;
                PedidoSeleccionado.MONEDA = args.Data.MONEDA;
                PedidoSeleccionado.PEDIDO = args.Data.PEDIDO;
                PedidoSeleccionado.NUMOCI = args.Data.NUMOCI;
                PedidoSeleccionado.BONIFIC = args.Data.BONIFIC;
                PedidoSeleccionado.CG_TRANS = args.Data.CG_TRANS;
                PedidoSeleccionado.CG_COND_ENTREGA = args.Data.CG_COND_ENTREGA;
                PedidoSeleccionado.CONDVEN = args.Data.CONDVEN;
                PedidoSeleccionado.Items.Add(args.Data);
                //TODO preguntar si abre por pedido o numoci
                await refConfirmacionDialog.ShowAsync();
            }

            if (args.RequestType == Syncfusion.Blazor.Grids.Action.Delete &&
                (await refGrid.GetSelectedRecordsAsync()).Count > 0)
            {
                var pedido = (await refGrid.GetSelectedRecordsAsync()).FirstOrDefault();
                
                var response = await StockService.TieneRemitoAsociado(pedido.PEDIDO);

                if (response.Error)
                {
                    await ToastMensajeError("No se pudo consultar si el pedido\n\r" +
                        "tiene remito asociado.");
                }
                else
                {
                    bool tieneRemito = response.Response;
                    if (tieneRemito)
                    {
                        await ToastMensajeError("El Pedido tiene Alta de produccion o Orden de armado o" +
                            "Remito asociado.\r\nNose puede eliminar.");
                    }
                    else
                    {
                        PedidoSeleccionado = new PedCliEncabezado();
                        PedidoSeleccionado.PEDIDO = args.RowData.PEDIDO;
                        PedidoSeleccionado.NUMOCI = args.RowData.NUMOCI;
                        textbotonPorOCI = $"Por OCI {PedidoSeleccionado.NUMOCI}";
                        textbotonPorPedido = $"Por Pedido {PedidoSeleccionado.PEDIDO}";
                        await PreguntaEliminarDialog.ShowAsync();
                    }
                }

                

            }
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

        protected async Task PreguntaEliminarPorNumOCIPedido(bool porPedido)
        {
            await PreguntaEliminarDialog.HideAsync();

            eliminaPorPedido = porPedido;


            await ConfirmacionEliminarDialog.ShowAsync();

        }

        protected async Task Eliminar()
        {
            await ConfirmacionEliminarDialog.HideAsync();
            SpinnerVisible = true;
            HttpResponseMessage responseMessage= null;
            if (eliminaPorPedido)
            {
                responseMessage = await PedCliService.EliminarPedido(PedidoSeleccionado.PEDIDO);
            }
            else
            {
                responseMessage = await PedCliService.EliminarOCI(PedidoSeleccionado.NUMOCI);
            }

            if (responseMessage.IsSuccessStatusCode)
            {
                if (eliminaPorPedido)
                {
                    pedidos = pedidos.Where(p=> p.PEDIDO != PedidoSeleccionado.PEDIDO).ToList();
                }
                else
                {
                    pedidos = pedidos.Where(p => p.PEDIDO != PedidoSeleccionado.NUMOCI).ToList();

                    
                }
            }
            else
            {
                if (responseMessage.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    await ToastMensajeError("Error al buscar pedido/oci.");
                }
                else if (responseMessage.StatusCode == System.Net.HttpStatusCode.Conflict)
                {
                    await ToastMensajeError(responseMessage.ReasonPhrase);
                }
                else
                {
                    await ToastMensajeError();
                }
                
            }

            SpinnerVisible = false;
        }

        protected async Task AbrirNumOCIPedido(bool abrePedido)
        {
            await refConfirmacionDialog.HideAsync();
            SpinnerVisible = true;
            
            abrePorPedido = abrePedido;
            popupFormVisible = true;
            
            SpinnerVisible = false;
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
            else if (args.Item.Id == "imprimir")
            {
                SpinnerVisible = true;
                await Imprimir();
                SpinnerVisible = false;
            }
            else if (args.Item.Id == "ExcelExport")
            {
                await refGrid.ExportToExcelAsync();
              
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
