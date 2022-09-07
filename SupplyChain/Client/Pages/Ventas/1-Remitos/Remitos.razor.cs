using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SupplyChain.Client.HelperService;
using SupplyChain.Client.RepositoryHttp;
using SupplyChain.Client.Shared;
using SupplyChain.Shared;
using SupplyChain.Shared.Enum;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Navigations;
using Syncfusion.Blazor.Notifications;
using Syncfusion.Blazor.Spinner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace SupplyChain.Client.Pages.Ventas._1_Remitos
{
    public class RemitosBase : ComponentBase
    {
        [Inject] public IRepositoryHttp Http { get; set; }
        [Inject] public PedCliService PedCliService { get; set; }
        [Inject] public EstadoPedidoService EstadoPedidoService { get; set; }
        [Inject] public StockService StockService { get; set; }
        [Inject] public CondicionPagoService CondicionPagoService { get; set; }
        [Inject] public CondicionEntregaService CondicionEntregaService { get; set; }
        [Inject] public DireccionEntregaService DireccionEntregaService { get; set; }
        [Inject] public TransporteService TransporteService { get; set; }
        [Inject] public IJSRuntime JS { get; set; }
        [CascadingParameter] public MainLayout MainLayout { get; set; }
        protected SfGrid<vEstadoPedido> refGrid;
        protected SfSpinner refSpinner;
        protected SfToast ToastObj;
        //protected FormPedido refFormPedido;
        protected List<vEstadoPedido> pedidosPendientesRemitir = new();
        protected PedidoEncabezado PedidoSeleccionado = new();
        protected bool SpinnerVisible = false;
        protected bool popupFormVisible = false;

        protected List<Object> Toolbaritems = new()
        {
            "Search",
            //new ItemModel { Text = "Add", TooltipText = "Agregar un nuevo Presupuesto", PrefixIcon = "e-add", Id = "Add" },
            new ItemModel { Text = "Generar Remito", TooltipText = "Generar un nuevo remito a partir de los items seleccionandos", 
                PrefixIcon = "e-add", Id = "GenerarRemito", Type = ItemType.Button },
            //"Add",
            //"Edit",
            "Delete",
            "Print",
            //new ItemModel { Text = "Copy", TooltipText = "Copy", PrefixIcon = "e-copy", Id = "copy" },
            "ExcelExport",
            new ItemModel { Text = "", TooltipText = "Actualizar Grilla", PrefixIcon = "e-refresh", Id = "refresh" },
            new ItemModel { Text = "Ver Remitidos", Id = "VerRemitidos" },
            new ItemModel { Text = "Ver Pendientes", Id = "VerPendientes" },
        };
        protected List<string> direccionesEntregas = new();
        protected List<vCondicionesPago> condicionesPagos = new();
        protected List<vCondicionesEntrega> condicionesEntrega = new();
        protected List<vTransporte> transportes = new();

        protected async override Task OnInitializedAsync()
        {
            MainLayout.Titulo = "Remitos";
            SpinnerVisible = true;
            await GetPedidoPendientesRemitir();
            //await refGrid.AutoFitColumnsAsync();
            SpinnerVisible = false;
        }

        protected async Task GetPedidoPendientesRemitir()
        {
            var response = await EstadoPedidoService.ByEstado(EstadoPedido.PendienteRemitir);
            if (response.Error)
            {
                await ToastMensajeError("Ocurrio un error al obtener Remitos");
            }
            else
            {
                await GetCondicionesPago();
                await GetCondicionesEntrega();
                await GetTransportes();
                pedidosPendientesRemitir = response.Response.OrderByDescending(p => p.PEDIDO).ToList();
            }
        }

        protected async Task GetCondicionesPago()
        {
            var response = await CondicionPagoService.Get();
            if (response.Error)
            {
                await ToastMensajeError("Error al obtener Condiciones de Pago.");
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
                await ToastMensajeError("Error al obtener Condiciones de Entrega.");
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
                await ToastMensajeError("Error al obtener Transportes.");
            }
            else
            {
                transportes = response.Response;
            }
        }

        protected async Task OnActionBeginHandler(ActionEventArgs<vEstadoPedido> args)
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
                var response = await StockService.GetPedidoEncabezadoById((int)args.Data.Id);
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
                    //direccionesEntregas = PedidoSeleccionado.DireccionesEntregas.Select(d => d.DESCRIPCION).ToList();
                    popupFormVisible = true;
                }
                SpinnerVisible = false;
            }
        }
        protected async Task OnActionCompleteHandler(ActionEventArgs<vEstadoPedido> args)
        {
            if (args.RequestType == Syncfusion.Blazor.Grids.Action.Add ||
                args.RequestType == Syncfusion.Blazor.Grids.Action.BeginEdit)
            {
                args.Cancel = true;
                args.PreventRender = false;
            }


        }

        public void RowBound(RowDataBoundEventArgs<vEstadoPedido> Args)
        {
            if (string.IsNullOrEmpty(Args.Data.REMITO))
            {
                Args.Row.AddClass(new string[] { "e-removeEditcommand" });
            }

            if (string.IsNullOrEmpty(Args.Data.FACTURA))
            {
                Args.Row.AddClass(new string[] { "e-removeDeletecommand" });
            }
        }

        public async Task CommandClickHandler(CommandClickEventArgs<vEstadoPedido> args)
        {
            if (args.CommandColumn.ID == "btnDescargarRemito")
            {
                await DescargarRemito(args.RowData);
            }

            if (args.CommandColumn.ID == "btnDescargarFactura")
            {
                await DescargarFactura(args.RowData);
            }

            if (args.CommandColumn.ID == "btnDescargarCertificado")
            {
                await DescargarCertificado(args.RowData.PEDIDO);
                /*
                //OBTENGO TRAZABILIDAD PARA PODER OBTENER LOS CERTIFICADO DE MP
                var listTrazab = new List<vTrazabilidad>();
                var listArchivosDescargar = new List<Archivo>();
                var responseTrazab = await Http.GetFromJsonAsync<List<vTrazabilidad>>($"api/Trazabilidads/MostrarTrazabilidad/{pedido}");
                if (responseTrazab.Error)
                {
                    Console.WriteLine("ERROR AL OBTENER TRAZABILIDAD");
                    Console.WriteLine(await responseTrazab.HttpResponseMessage.Content.ReadAsStringAsync());
                }
                else
                {
                    listTrazab = responseTrazab.Response;
                }

                if (listTrazab.Count > 0 )
                {
                    var lineasRoscada = new List<int>(new int[] { 8, 18, 52, 23 });
                    var lineasBridada = new List<int>(new int[] { 8, 18, 23, 19 });
                    List<vTrazabilidad> lineasCertif = new();
                    var producto = listTrazab.FirstOrDefault(t => t.TIPOO == 1).CG_ART;
                    if (producto.StartsWith("00"))//reparacion
                    {

                    }
                    else if (producto.StartsWith("1")) //roscada
                    {
                        lineasCertif = listTrazab.Where(t => lineasRoscada.Contains((int)t.CG_LINEA)).ToList();
                    }
                    else if (producto.StartsWith("2")) //bridada
                    {
                        lineasCertif = listTrazab.Where(t=> lineasBridada.Contains((int)t.CG_LINEA)).ToList();
                    }

                    if (lineasCertif.Count > 0)
                    {
                        foreach (var item in lineasCertif)
                        {
                            var responseMp = await Http
                                .GetFromJsonAsync<List<Archivo>>($"api/AdministracionArchivos/ByParamRuta/RUTATRAZABILIDAD/{item.DESPACHO}.pdf");
                            if (responseMp.Error)
                            {
                                Console.WriteLine("ERROR AL OBTENER CERTIFICADOS DE MATERIA PRIMA");
                                Console.WriteLine(await responseMp.HttpResponseMessage.Content.ReadAsStringAsync());
                            }
                            else
                            {
                                if (responseMp.Response.Count > 0)
                                {
                                    listArchivosDescargar.Add(responseMp.Response[0]);
                                }
                                
                            }
                        }
                    }
                }

                var response = await Http
                    .GetFromJsonAsync<List<Archivo>>($"api/AdministracionArchivos/ByParamRuta/RUTACERTIFICADOS/{pedido}");

                if (response.Error)
                {
                    Console.WriteLine("ERROR AL OBTENER CERTIFICADOS DE PRODUCTO");
                    Console.WriteLine(await response.HttpResponseMessage.Content.ReadAsStringAsync());
                }
                else
                {
                    foreach (var item in response.Response)
                    {
                        listArchivosDescargar.Add(item);
                    }
                    foreach (Archivo item in listArchivosDescargar)
                    {
                        await JS.SaveAs(item.Nombre, item.ContenidoByte);
                    }

                }
                */
            }
        }

        private async Task DescargarCertificado(int pedido)
        {
            await JS.InvokeVoidAsync("descargarCertificado", pedido);
        }

        private async Task DescargarRemito(vEstadoPedido pedido)
        {
            var responseFactura = await Http
                                .GetFromJsonAsync<List<Archivo>>($"api/AdministracionArchivos/ByParamRuta/RUTAREMITO/{pedido.REMITO}");
            if (responseFactura.Error)
            {
                Console.WriteLine("ERROR AL OBTENER REMITO");
                Console.WriteLine(await responseFactura.HttpResponseMessage.Content.ReadAsStringAsync());
            }
            else
            {
                if (responseFactura.Response.Count > 0)
                {
                    await JS.SaveAs(responseFactura.Response[0].Nombre, responseFactura.Response[0].ContenidoByte);
                }
                else
                {

                }

            }
        }
        private async Task DescargarFactura(vEstadoPedido pedido)
        {
            var formatoFacturaBuscar = pedido.LETRA_FACTURA + pedido.FACTURA;
            var responseFactura = await Http.GetFromJsonAsync<List<Archivo>>($"api/AdministracionArchivos/ByParamRuta/RUTAFACTURA/{formatoFacturaBuscar}");
            if (responseFactura.Error)
            {
                Console.WriteLine("ERROR AL OBTENER FACTURA");
                Console.WriteLine(await responseFactura.HttpResponseMessage.Content.ReadAsStringAsync());
            }
            else
            {
                if (responseFactura.Response.Count > 0)
                {
                    await JS.SaveAs(responseFactura.Response[0].Nombre, responseFactura.Response[0].ContenidoByte);
                }
                else
                {

                }

            }
        }

        public async Task ClickHandler(Syncfusion.Blazor.Navigations.ClickEventArgs args)
        {
            if (args.Item.Text == "Seleccionar Columnas")
            {
                await refGrid.OpenColumnChooser(200, 50);
            }
            else if (args.Item.Text == "Exportar grilla en Excel")
            {
                await this.refGrid.ExcelExport();
            }
            else if (args.Item.Id == "VerRemitidos")
            {
                await GetPedidoPendientesRemitir();
            }
            else if (args.Item.Id == "GenerarRemito")
            {
                SpinnerVisible = true;
                await GenerarRemito();
                
                SpinnerVisible = false;
            }
            else
            {
                await GetPedidoPendientesRemitir();
            }
        }

        protected async Task GenerarRemito()
        {
            var selecciones = await refGrid.GetSelectedRecordsAsync();
            if (selecciones == null || selecciones.Count == 0)
            {
                await ToastMensajeError("Seleccione Item/s");
            }
            else if (selecciones.GroupBy(g=> g.CG_CLI).Count() > 1)
            {
                await ToastMensajeError("Seleccione items del mismo Cliente");
            }
            else if (selecciones.GroupBy(g => g.NUM_OCI).Count() > 1)
            {
                await ToastMensajeError("Seleccione items de la misma Orden de Compra");
            }
            else if (selecciones.Any(p=> p.ESTADO_PEDIDO != 6))
            {
                await ToastMensajeError("Hay items seleccionados que no tienen alta de produccion");
            }
            else
            {
                Console.WriteLine("Pedidos Seleccionados " + selecciones.Select(s => s.PEDIDO.ToString()).ToArray());
                var response = await StockService.GetPedidoEncabezadoByLista(selecciones.Select(s=> s.PEDIDO).ToList());
                if (response.Error)
                {
                    Console.WriteLine(response.HttpResponseMessage.ReasonPhrase);
                    await ToastMensajeError("Ocurrio un Error al cargar pedidos.");
                }
                else
                {
                    PedidoSeleccionado = response.Response;
                    popupFormVisible = true;
                }
                
            }

        }


        protected async Task OnRemitoGuardado(List<Pedidos> pedidosGuardados)
        {
            SpinnerVisible = true;
            popupFormVisible = false;
            await GetPedidoPendientesRemitir();
            SpinnerVisible = false;
        }

        public void QueryCellInfoHandler(QueryCellInfoEventArgs<vEstadoPedido> args)
        {
            if (args.Data.ESTADO_PEDIDO == 1) /*"PEDIDO A CONFIRMAR"*/
            {
                args.Cell.AddClass(new string[] { "color-estado-pedido-a-confirmar" });
            }
            else if (args.Data.ESTADO_PEDIDO == 2) /*"PEDIDO CONFIRMADO"*/
            {
                args.Cell.AddClass(new string[] { "color-estado-pedido-confirmado" });
            }
            else if (args.Data.ESTADO_PEDIDO == 3) /*"EN PRODUCCION"*/
            {
                args.Cell.AddClass(new string[] { "color-estado-pedido-produccion" });
            }
            else if (args.Data.ESTADO_PEDIDO == 4) /*"CON TOTALIDAD DE COMPONENTES"*/
            {
                args.Cell.AddClass(new string[] { "color-estado-pedido-componentes" });
            }
            else if (args.Data.ESTADO_PEDIDO == 5) /*"ARMADO Y CALIBRACION"*/
            {
                args.Cell.AddClass(new string[] { "color-estado-pedido-armado" });
            }
            else if (args.Data.ESTADO_PEDIDO == 6) /*"PENDIENTE DE REMITIR"*/
            {
                args.Cell.AddClass(new string[] { "color-estado-pedido-pendiente-remitir" });
            }
            else if (args.Data.ESTADO_PEDIDO == 7) /*"A ENTREGAR"*/
            {
                args.Cell.AddClass(new string[] { "color-estado-pedido-entregar" });
            }
            else if (args.Data.ESTADO_PEDIDO == 8) /*"ENTREGADO"*/
            {
                args.Cell.AddClass(new string[] { "color-estado-pedido-entregado" });
            }
            else if (args.Data.ESTADO_PEDIDO == 9) /*"FACTURADO"*/
            {
                args.Cell.AddClass(new string[] { "color-estado-pedido-facturado" });
            }
            else if (args.Data.ESTADO_PEDIDO == 10) /*ANULADO*/
            {
                args.Cell.AddClass(new string[] { "color-estado-pedido-anulado" });
            }
        }

        protected async Task OnRowSelected(RowSelectEventArgs<vEstadoPedido> arg)
        {
            arg.PreventRender = true;
        }

        protected async Task OnRowSelectedDouble_Click(RecordDoubleClickEventArgs<vEstadoPedido> arg)
        {
            arg.PreventRender = true;
            //PedidoSeleccionado = arg.RowData;
            //VisibleDialog = true;
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
