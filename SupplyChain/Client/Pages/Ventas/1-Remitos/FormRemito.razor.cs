using Microsoft.AspNetCore.Components;
using SupplyChain.Client.HelperService;
using SupplyChain.Client.Shared.BuscadorCliente;
using SupplyChain.Client.Shared.BuscadorPedidos;
using SupplyChain.Client.Shared.BuscadorPresupuesto;
using SupplyChain.Shared;
using Syncfusion.Blazor.DropDowns;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Inputs;
using Syncfusion.Blazor.Notifications;
using Syncfusion.Blazor.Spinner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SupplyChain.Client.Pages.Ventas._1_Remitos
{
    public class FormRemitoBase : ComponentBase
    {
        [Inject] public PedCliService PedCliService { get; set; }
        [Inject] public StockService StockService { get; set; }
        [Inject] public ClienteService ClienteService { get; set; }
        [Inject] public PresupuestoService PresupuestoService { get; set; }
        [Inject] public PrecioArticuloService PrecioArticuloService { get; set; }
        [Inject] public CondicionPagoService CondicionPagoService { get; set; }
        [Inject] public CondicionEntregaService CondicionEntregaService { get; set; }
        [Inject] public DireccionEntregaService DireccionEntregaService { get; set; }
        [Inject] public TransporteService TransporteService { get; set; }
        [Inject] public TipoCambioService TipoCambioService { get; set; }
        [Parameter] public PedidoEncabezado Pedido { get; set; } = new();
        [Parameter] public bool Show { get; set; } = false;
        [Parameter] public List<string> DireccionesEntregas { get; set; } = new();
        [Parameter] public List<vCondicionesPago> CondicionesPagos { get; set; } = new();
        [Parameter] public List<vCondicionesEntrega> CondicionesEntrega { get; set; } = new();
        [Parameter] public List<vTransporte> Transportes { get; set; } = new();
        [Parameter] public EventCallback<List<Pedidos>> OnGuardar { get; set; }
        [Parameter] public EventCallback OnCerrar { get; set; }

        protected SfGrid<Pedidos> refGridItems;
        protected ClientesDialog refClienteDialog;
        protected PedidosEstados refPresupuestosDialog;
        protected SfSpinner refSpinnerCli;
        protected bool popupBuscadorVisibleCliente = false;
        protected bool popupBuscadorVisiblePresupuestos = false;
        protected bool BotonGuardarDisabled = false;
        protected bool spinerVisible = false;

        protected List<string> Monedas = new() { "PESOS", "DOLARES" };
        protected bool SpinnerVisible = false;
        protected SfToast ToastObj;

        protected Dictionary<string, object> HtmlAttribute = new()
        {
            { "type", "button" }
        };

        protected Dictionary<string, object> HtmlAttributeButtonTC = new()
        {
            { "type", "button" },
            { "title", "Obtener ultima Cotizacion Dolar" }
        };

        protected Dictionary<string, object> HtmlAttributeSubmit = new()
        {
            { "type", "submit" },
            { "form", "formPedido" }

        };

        protected bool IsAdd { get; set; }
        protected bool ReadOnlyMoneda = true;
        protected decimal IMP_BONFIC = 0;
        protected bool agregadoPorPresupuesto = false;
        protected async override Task OnInitializedAsync()
        {
            if (Transportes.Count == 0)
            {
                await GetTransportes();
            }
            if (CondicionesPagos.Count == 0)
            {
                await GetCondicionesPago();
            }

            if (Pedido.PEDIDO == 0)
            {
                ReadOnlyMoneda = false;
            }
            if (Pedido.CG_CLI > 0 && DireccionesEntregas.Count == 0)
            {
                await GetDireccionesEntregaCliente(Pedido.CG_CLI);
            }

            await GetTipoCambioDolarHoy();
        }

        public async Task GetTipoCambioDolarHoy()
        {
            if (Pedido.PEDIDO == 0)
            {
                var tc = await TipoCambioService.GetValorDolarHoy();
                if (tc == 0)
                {
                    await ToastMensajeError("Error al obtener Tipo de Cambio.");
                }
                else
                {
                    Pedido.VA_INDIC = tc;
                }
            }

        }
        protected async Task GetPedido(int id)
        {
            var response = await PedCliService.GetById(id);
            if (response.Error)
            {
                await ToastMensajeError("Error al obtener Condiciones de Pago.");
            }
            else
            {
                Pedido.PEDIDO = response.Response.Id;
                Pedido.CG_CLI = response.Response.CG_CLI;
                Pedido.DES_CLI = response.Response.DES_CLI;
                Pedido.CG_COND_ENTREGA = response.Response.CG_COND_ENTREGA;
                Pedido.CG_TRANS = response.Response.CG_TRANS;
                Pedido.CG_CONDICION_PAGO = response.Response.DPP;
                Pedido.DIRENT = response.Response.DIRENT;
                Pedido.MONEDA = response.Response.MONEDA;
                Pedido.VA_INDIC = response.Response.VA_INDIC;


                var itemPedido = new Pedidos()
                {
                    PEDIDO = response.Response.Id,
                    CG_COND_ENTREGA = response.Response.CG_COND_ENTREGA,
                    CG_TRANS = response.Response.CG_TRANS,
                    CG_CONDICION_PAGO = response.Response.DPP,
                    DIRENT = response.Response.DIRENT,
                    MONEDA = response.Response.MONEDA,
                    VA_INDIC = response.Response.VA_INDIC,
                    OBSERITEM = response.Response.OBSERITEM,
                    CANTPED = response.Response.CANTPED,
                    CG_ART = response.Response.CG_ART,
                    DES_ART = response.Response.DES_ART,
                    BONIFIC = response.Response.BONIFIC,
                    CG_DEP = 1,
                    CG_CLI = response.Response.CG_CLI,
                    DES_CLI = response.Response.DES_CLI,
                    COMPROB = "REMITO",
                    VOUCHER = 0,
                    CG_ORDEN = 1,
                    IMPORTE1 = response.Response.PREC_UNIT,
                    IMPORTE2 = response.Response.PREC_UNIT_X_CANTIDAD,
                    IMPORTE3 = response.Response.IMP_DESCUENTO,
                    IMPORTE4 = response.Response.TOTAL,
                    DESCUENTO = response.Response.DESCUENTO,
                    TIPOO = 1,
                    OCOMPRA = response.Response.NUMOCI,
                    //todo traer datos de alta
                };

                Pedido.Items.Add(itemPedido);
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
                CondicionesPagos = response.Response;
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
                CondicionesEntrega = response.Response;
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
                Transportes = response.Response;
            }
        }
        protected async Task BuscarClientes()
        {
            SpinnerVisible = true;
            await refClienteDialog.Show();
            SpinnerVisible = false;
            popupBuscadorVisibleCliente = true;
        }

        public async Task ClienteExternoSelected(ClienteExterno clienteSelected)
        {
            if (refSpinnerCli != null)
            {
                await refSpinnerCli.ShowAsync();
            }

            popupBuscadorVisibleCliente = false;
            Pedido.CG_CLI = Convert.ToInt32(clienteSelected.CG_CLI);
            Pedido.DES_CLI = clienteSelected.DESCRIPCION.Trim();
            Pedido.CG_CONDICION_PAGO = clienteSelected.ID_CON_VEN == null ? 0 : (int)clienteSelected.ID_CON_VEN;//hay cliente que no tienen asignado una condicion de pago
            Pedido.BONIFIC = clienteSelected.DESC_COMERCIAL == null ? 0 : (decimal)clienteSelected.DESC_COMERCIAL;
            Pedido.CG_COND_ENTREGA = clienteSelected.ID_CON_ENT == null ? 0 : (int)clienteSelected.ID_CON_ENT;
            await GetDireccionesEntregaCliente(Pedido.CG_CLI);

            if (refSpinnerCli != null)
            {
                await refSpinnerCli?.HideAsync();
            }

        }
        public async Task GetDireccionesEntregaCliente(int idCliente)
        {
            if (idCliente > 0)
            {
                var response = await DireccionEntregaService.GetByNumeroCliente(idCliente);
                if (response.Error)
                {
                    await ToastMensajeError("Ocurrio un Error.\nError al intentar obtener direcciones de entrega");
                }
                else
                {
                    DireccionesEntregas = response.Response.Select(de => de.DESCRIPCION).ToList();

                    if (DireccionesEntregas.Count > 0)
                    {
                        if (string.IsNullOrEmpty(Pedido.DIRENT))
                        {
                            Pedido.DIRENT = DireccionesEntregas[0];
                        }

                    }
                }
            }

        }

        protected async Task BuscarPedidosPendientes()
        {
            if (Pedido.CG_CLI == 0)
            {
                await ToastMensajeError("Seleccione Cliente.");
                return;
            }

            SpinnerVisible = true;
            await refPresupuestosDialog.Show();
            SpinnerVisible = false;
            popupBuscadorVisiblePresupuestos = true;
        }

        protected async Task PresupuestoSelected(List<vEstadoPedido> estadoPedidos)
        {
            if (estadoPedidos.Any(e=> e.CG_CLI != Pedido.CG_CLI))
            {
                await ToastMensajeError("No se puede agregar item.\nCliente no corresponde al presupuesto.");
                return;
            }

            agregadoPorPresupuesto = true;//para que no se ajecute el evento de cambio de moneda
            var pedcli = new PedCli();
            foreach (var item in estadoPedidos)
            {
                


            }
            popupBuscadorVisiblePresupuestos = false;
            await refGridItems.RefreshColumnsAsync();
            refGridItems.Refresh();
            await refGridItems.RefreshHeaderAsync();
        }
        protected async Task Guardar()
        {
            spinerVisible = true;
            BotonGuardarDisabled = true;

            //actualiza todos los campos de encabezado a los items
            foreach (var item in Pedido.Items)
            {
                item.CG_TRANS = Pedido.CG_TRANS;
                item.CG_COND_ENTREGA = Pedido.CG_COND_ENTREGA;
                item.CG_CONDICION_PAGO = Pedido.CG_CONDICION_PAGO;
                item.TIPOO = 1;
                item.DIRENT = Pedido.DIRENT ?? string.Empty;
                item.OBS1 = Pedido.BULTOS;
                item.OBS3 = Pedido.MONTO;
                item.ORCO = Pedido.ORCO;
                item.COMPROB = "REMITO";
            }

            var response = await StockService.GuardarLista(Pedido.Items);
            if (response.Error)
            {
                spinerVisible = false;
                BotonGuardarDisabled = false;
                await ToastMensajeError();

            }
            else
            {
                spinerVisible = false;
                Show = false;
                Pedido.Items = response.Response;
                await StockService.Imprimir(Pedido.Items[0].REMITO, true);


                if (OnGuardar.HasDelegate)
                {
                    await OnGuardar.InvokeAsync(Pedido.Items);
                }
                
            }

            BotonGuardarDisabled = false;
        }


        protected void CambioMoneda(ChangeEventArgs<string, string> args)
        {
            if (Pedido.Items.Count > 0 && Pedido.Items[0].MONEDA != args.Value)
            {
                var tipoCambio = Pedido.VA_INDIC;
                foreach (var item in Pedido.Items)
                {
                    if (args.Value == "DOLARES")
                    {
                        item.IMPORTE1 /= (decimal)tipoCambio;
                    }
                    else
                    {
                        item.IMPORTE1 *= (decimal)tipoCambio;
                    }

                    item.IMPORTE2 = item.IMPORTE1 * Math.Abs((decimal)item.STOCK);
                }

                refGridItems.Refresh();
            }
        }

        protected async Task Cg_cli_Changed(ChangeEventArgs args)
        {
            string cg_cli = args.Value.ToString();
            if (!string.IsNullOrEmpty(cg_cli))
            {
                Pedido.CG_CLI = int.Parse(cg_cli);
                if (Pedido.CG_CLI > 0)
                {
                    var response = await ClienteService.Search(Pedido.CG_CLI, Pedido.DES_CLI);
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
                                Pedido.CG_CLI = int.Parse(response.Response[0].CG_CLI);
                                Pedido.DES_CLI = response.Response[0].DESCRIPCION;
                                await GetDireccionesEntregaCliente(Pedido.CG_CLI);
                                Pedido.CG_COND_ENTREGA = response.Response[0].ID_CON_ENT == null ? 0 : (int)response.Response[0].ID_CON_ENT;
                                Pedido.CG_CONDICION_PAGO = response.Response[0].ID_CON_VEN == null ? 0 : (int)response.Response[0].ID_CON_VEN;
                            }
                            else
                            {
                                Pedido.DES_CLI = string.Empty;
                                DireccionesEntregas = new();
                                Pedido.DIRENT = string.Empty;
                                Pedido.CG_COND_ENTREGA = 0;
                                Pedido.CG_CONDICION_PAGO = 0;
                            }
                        }
                        else
                        {
                            Pedido.DES_CLI = string.Empty;
                            DireccionesEntregas = new();
                            Pedido.DIRENT = string.Empty;
                            Pedido.CG_COND_ENTREGA = 0;
                            Pedido.CG_CONDICION_PAGO = 0;
                        }

                    }
                }
            }
            else
            {
                Pedido.DES_CLI = string.Empty;
                DireccionesEntregas = new();
                Pedido.DIRENT = string.Empty;
                Pedido.CG_COND_ENTREGA = 0;
                Pedido.CG_CONDICION_PAGO = 0;
            }


        }

        protected async Task Des_cli_Changed(InputEventArgs args)
        {
            string des_cli = args.Value.ToString();
            //if (!string.IsNullOrEmpty(des_cli))
            //{

            //}

            Pedido.DES_CLI = des_cli;
            var response = await ClienteService.Search(Pedido.CG_CLI, Pedido.DES_CLI);
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
                        Pedido.CG_CLI = int.Parse(response.Response[0].CG_CLI);
                        Pedido.DES_CLI = response.Response[0].DESCRIPCION;
                        await GetDireccionesEntregaCliente(Pedido.CG_CLI);
                        Pedido.CG_COND_ENTREGA = response.Response[0].ID_CON_ENT == null ? 0 : (int)response.Response[0].ID_CON_ENT;
                        Pedido.CG_CONDICION_PAGO = response.Response[0].ID_CON_VEN == null ? 0 : (int)response.Response[0].ID_CON_VEN;
                    }
                }
                else
                {
                    DireccionesEntregas = new();
                    Pedido.DIRENT = string.Empty;
                    Pedido.CG_COND_ENTREGA = 0;
                    Pedido.CG_CONDICION_PAGO = 0;
                }

            }
        }

        protected async Task BatchDeleteHandler(BeforeBatchDeleteArgs<Pedidos> args)
        {
            var item = Pedido.Items.Find(i => i.Id == args.RowData.Id);
            item.ESTADO = SupplyChain.Shared.Enum.EstadoItem.Eliminado;
        }

        public async Task CellSavedHandler(CellSaveArgs<Pedidos> args)
        {
            var index = await refGridItems.GetRowIndexByPrimaryKey(args.RowData.Id);
            //if (args.ColumnName == "Cantidad")
            //{
            //    args.RowData.CANTPED = (decimal)args.Value;
            //    if (IsAdd)
            //    {

            //        await refGridItems.UpdateCell(index, "PREC_UNIT", Convert.ToInt32(args.Value) * args.RowData.PREC_UNIT);
            //        //await refGridItems.UpdateCell(index, "Sum", Convert.ToInt32(args.Value) + 0);
            //    }
            //    await refGridItems.UpdateCell(index, "PREC_UNIT", Convert.ToInt32(args.Value) * args.RowData.PREC_UNIT);

            //}
            //else if (args.ColumnName == "PREC_UNIT")
            //{
            //    args.RowData.PREC_UNIT = (decimal)args.Value;
            //    if (IsAdd)
            //    {

            //        await refGridItems.UpdateCell(index, "PREC_UNIT_X_CANTIDAD",
            //            Convert.ToDouble(args.Value) * (double)args.RowData.CANTPED);
            //        //await refGridItems.UpdateCell(index, "Sum", Convert.ToDouble(args.Value) + 0);
            //    }
            //    await refGridItems.UpdateCell(index, "PREC_UNIT_X_CANTIDAD", Convert.ToDouble(args.Value) * (double)args.RowData.CANTPED);

            //}
            //else if (args.ColumnName == "DESCUENTO")
            //{
            //    args.RowData.DESCUENTO = (decimal)args.Value;
            //    if (IsAdd)
            //    {

            //        if (args.RowData.CANTPED == 0 || args.RowData.PREC_UNIT_X_CANTIDAD == 0 || (decimal)args.Value == 0)
            //        {
            //            await refGridItems.UpdateCell(index, "IMP_DESCUENTO", (double)0);
            //        }
            //        else
            //        {
            //            await refGridItems.UpdateCell(index, "IMP_DESCUENTO",
            //                args.RowData.PREC_UNIT_X_CANTIDAD / (1 + (decimal)args.Value / 100));
            //            //await refGridItems.UpdateCell(index, "Sum", Convert.ToDouble(args.Value) + 0);
            //        }



            //    }
            //    //var descuento = Math.Round(args.RowData.PREC_UNIT_X_CANTIDAD * args.RowData.DESCUENTO / 100, 2);
            //    //await refGridItems.UpdateCell(index, "IMP_DESCUENTO", descuento);
            //}



            //await refGridItems.UpdateCell(index, "TOTAL", args.RowData.PREC_UNIT_X_CANTIDAD - args.RowData.IMP_DESCUENTO);// total del item

            await refGridItems.EndEditAsync();
            //refGridItems.Refresh();
        }

        public void BatchAddHandler(BeforeBatchAddArgs<Pedidos> args)
        {
            IsAdd = true;
        }
        public void BatchSaveHandler(BeforeBatchSaveArgs<Pedidos> args)
        {
            IsAdd = false;
        }

        public async Task Imprimir()
        {
            await StockService.Imprimir(Pedido.Items[0].REMITO, true);
        }
        public async Task Hide()
        {
            Show = false;
        }

        protected async Task OnCerrarDialog()
        {
            await OnCerrar.InvokeAsync();
        }

        public void HabilitarComboMoneda()
        {
            ReadOnlyMoneda = false;
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
