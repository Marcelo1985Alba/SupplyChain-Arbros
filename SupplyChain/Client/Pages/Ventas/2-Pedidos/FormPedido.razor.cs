﻿using Microsoft.AspNetCore.Components;
using SupplyChain.Client.HelperService;
using SupplyChain.Client.Shared.BuscadorCliente;
using SupplyChain.Client.Shared.BuscadorPresupuesto;
using SupplyChain.Shared;
using SupplyChain.Shared.Logística;
using Syncfusion.Blazor.DropDowns;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Inputs;
using Syncfusion.Blazor.Notifications;
using Syncfusion.Blazor.Spinner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SupplyChain.Client.Pages.Ventas._2_Pedidos
{
    public class FormPedidoBase: ComponentBase
    {
        [Inject] public PedCliService PedCliService { get; set; }
        [Inject] public ClienteService ClienteService { get; set; }
        [Inject] public PresupuestoService PresupuestoService { get; set; }
        [Inject] public PrecioArticuloService PrecioArticuloService { get; set; }
        [Inject] public CondicionPagoService CondicionPagoService { get; set; }
        [Inject] public CondicionEntregaService CondicionEntregaService { get; set; }
        [Inject] public DireccionEntregaService DireccionEntregaService { get; set; }
        [Inject] public TipoCambioService TipoCambioService { get; set; }
        [Parameter] public PedCliEncabezado Pedido { get; set; } = new();
        [Parameter] public bool Show { get; set; } = false;
        [Parameter] public List<string> DireccionesEntregas { get; set; } = new();
        [Parameter] public List<vCondicionesPago> CondicionesPagos { get; set; } = new();
        [Parameter] public List<vCondicionesEntrega> CondicionesEntrega { get; set; } = new();
        [Parameter] public EventCallback<PedCliEncabezado> OnGuardar { get; set; }
        [Parameter] public EventCallback OnCerrar { get; set; }

        protected SfGrid<PedCli> refGridItems;
        protected ClientesDialog refClienteDialog;
        protected PresupuestosDialog refPresupuestosDialog;
        protected SfSpinner refSpinnerCli;
        protected bool popupBuscadorVisibleCliente = false;
        protected bool popupBuscadorVisiblePresupuestos = false;
        protected bool BotonGuardarDisabled = false;
        
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
            { "form", "formPresupuesto" }

        };

        protected bool IsAdd { get; set; }
        protected bool ReadOnlyMoneda = true;
        protected decimal IMP_BONFIC = 0;
        protected bool agregadoPorPresupuesto = false;
        protected async override Task OnInitializedAsync()
        {
            if (Pedido.PEDIDO == 0)
            {
                ReadOnlyMoneda = false;
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
                    Pedido.TC = (double)tc;
                }
            }
            
        }
        protected async Task GetPresupuesto(int id)
        {
            var response = await PresupuestoService.GetById(id);
            if (response.Error)
            {
                await ToastMensajeError("Error al obtener Condiciones de Pago.");
            }
            else
            {

                Pedido.PRESUP = response.Response.Id;
                Pedido.BONIFIC = response.Response.BONIFIC;
                Pedido.CG_CLI = response.Response.CG_CLI;
                Pedido.DES_CLI = response.Response.DES_CLI;
                Pedido.CG_COND_ENTREGA = response.Response.CG_COND_ENTREGA;
                Pedido.CG_TRANS = response.Response.CG_TRANS;
                Pedido.CONDICION_PAGO = response.Response.CONDICION_PAGO;
                Pedido.DIRENT = response.Response.DIRENT;
                Pedido.MONEDA = response.Response.MONEDA;
                Pedido.TC = response.Response.TC;
                foreach (var itemPresup in response.Response.Items)
                {

                    itemPresup.Estado = SupplyChain.Shared.Enum.EstadoItem.Agregado;

                    var itemPedido = new PedCli()
                    {
                        OBSERITEM = itemPresup.OBSERITEM,
                        CANTPED = itemPresup.CANTIDAD,
                        CG_ART = itemPresup.CG_ART,
                        DES_ART = itemPresup.DES_ART
                    };

                    Pedido.Items.Add(itemPedido);
                }
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
            Pedido.CONDICION_PAGO = clienteSelected.ID_CON_VEN == null ? 0 : (int)clienteSelected.ID_CON_VEN;//hay cliente que no tienen asignado una condicion de pago
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

        protected async Task BuscarPresupuestos()
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

        protected async Task PresupuestoSelected(Presupuesto presupuestoSelected)
        {
            if (Pedido.CG_CLI != presupuestoSelected.CG_CLI)
            {
                await ToastMensajeError("No se puede agregar item.\nCliente no corresponde al presupuesto.");
                return;
            }

            agregadoPorPresupuesto = true;//para que no se ajecute el evento de cambio de moneda
            var pedcli = new PedCli();
            foreach (var item in presupuestoSelected.Items)
            {
                if (string.IsNullOrEmpty(Pedido.MONEDA))
                {
                    Pedido.MONEDA = presupuestoSelected.MONEDA;
                }

                Pedido.TC = presupuestoSelected.TC;
                Pedido.BONIFIC = presupuestoSelected.BONIFIC;
                pedcli = new PedCli()
                {
                    Id = Pedido.Items.Count == 0 ? -1 : Pedido.Items.Count * -1 - 1,
                    CG_CLI = Pedido.CG_CLI,
                    DES_CLI = Pedido.DES_CLI,
                    DPP = Pedido.CONDICION_PAGO,
                    BONIFIC = Pedido.BONIFIC,
                    DIRENT = Pedido.DIRENT,
                    PEDIDO = Pedido.PEDIDO,
                    CG_ART = item.CG_ART,
                    DES_ART = item.DES_ART,
                    CANTPED = item.CANTIDAD,
                    MONEDA = presupuestoSelected.MONEDA,
                    PREC_UNIT = item.PREC_UNIT,
                    PREC_UNIT_X_CANTIDAD = item.PREC_UNIT_X_CANTIDAD,
                    DESCUENTO = item.DESCUENTO,
                    IMP_DESCUENTO = item.IMP_DESCUENTO,
                    SUBTOTAL = item.TOTAL,
                    OBSERITEM = item.OBSERITEM,
                    ESTADO = SupplyChain.Shared.Enum.EstadoItem.Agregado,

                };
                Pedido.Items.Add(pedcli);
            }
            await refGridItems.RefreshColumnsAsync();
            refGridItems.Refresh();
            await refGridItems.RefreshHeaderAsync();
        }
        protected async Task Guardar()
        {
            //BotonGuardarDisabled = true;
            //bool guardado;
            //if (Pedido.PEDIDO == 0)
            //{
            //    guardado = await Agregar(Pedido.Items);
            //    Pedido.ESNUEVO = true;
            //}
            //else
            //{
            //    guardado = await Actualizar(Pedido.Items);
            //}

            //Show = false;
            
            //Pedido.GUARDADO = guardado;
            //await OnGuardar.InvokeAsync(Pedido);
            //BotonGuardarDisabled = false;

        }

        protected void CambioMoneda(ChangeEventArgs<string, string> args)
        {
            if (Pedido.Items.Count > 0 && Pedido.Items[0].MONEDA != args.Value)
            {
                var tipoCambio = Pedido.TC;
                foreach (var item in Pedido.Items)
                {
                    if (args.Value == "DOLARES")
                    {
                        item.PREC_UNIT /= (decimal)tipoCambio;
                    }
                    else
                    {
                        item.PREC_UNIT *= (decimal)tipoCambio;
                    }

                    item.PREC_UNIT_X_CANTIDAD = item.PREC_UNIT * item.CANTPED;
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
                                Pedido.CONDICION_PAGO = response.Response[0].ID_CON_VEN == null ? 0 : (int)response.Response[0].ID_CON_VEN;
                            }
                            else
                            {
                                Pedido.DES_CLI = string.Empty;
                                DireccionesEntregas = new();
                                Pedido.DIRENT = string.Empty;
                                Pedido.CG_COND_ENTREGA = 0;
                                Pedido.CONDICION_PAGO = 0;
                            }
                        }
                        else
                        {
                            Pedido.DES_CLI = string.Empty;
                            DireccionesEntregas = new();
                            Pedido.DIRENT = string.Empty;
                            Pedido.CG_COND_ENTREGA = 0;
                            Pedido.CONDICION_PAGO = 0;
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
                Pedido.CONDICION_PAGO = 0;
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
                        Pedido.CONDICION_PAGO = response.Response[0].ID_CON_VEN == null ? 0 : (int)response.Response[0].ID_CON_VEN;
                    }
                }
                else
                {
                    DireccionesEntregas = new();
                    Pedido.DIRENT = string.Empty;
                    Pedido.CG_COND_ENTREGA = 0;
                    Pedido.CONDICION_PAGO = 0;
                }

            }
        }

        protected async Task BatchDeleteHandler(BeforeBatchDeleteArgs<PedCli> args)
        {
            var item = Pedido.Items.Find(i => i.Id == args.RowData.Id);
            item.ESTADO = SupplyChain.Shared.Enum.EstadoItem.Eliminado;
        }

        public async Task CellSavedHandler(CellSaveArgs<PedCli> args)
        {
            var index = await refGridItems.GetRowIndexByPrimaryKey(args.RowData.Id);
            if (args.ColumnName == "Cantidad")
            {
                args.RowData.CANTPED = (decimal)args.Value;
                if (IsAdd)
                {

                    await refGridItems.UpdateCell(index, "PREC_UNIT", Convert.ToInt32(args.Value) * args.RowData.PREC_UNIT);
                    //await refGridItems.UpdateCell(index, "Sum", Convert.ToInt32(args.Value) + 0);
                }
                await refGridItems.UpdateCell(index, "PREC_UNIT", Convert.ToInt32(args.Value) * args.RowData.PREC_UNIT);

            }
            else if (args.ColumnName == "PREC_UNIT")
            {
                args.RowData.PREC_UNIT = (decimal)args.Value;
                if (IsAdd)
                {

                    await refGridItems.UpdateCell(index, "PREC_UNIT_X_CANTIDAD",
                        Convert.ToDouble(args.Value) * (double)args.RowData.CANTPED);
                    //await refGridItems.UpdateCell(index, "Sum", Convert.ToDouble(args.Value) + 0);
                }
                await refGridItems.UpdateCell(index, "PREC_UNIT_X_CANTIDAD", Convert.ToDouble(args.Value) * (double)args.RowData.CANTPED);

            }
            else if (args.ColumnName == "DESCUENTO")
            {
                args.RowData.DESCUENTO = (decimal)args.Value;
                if (IsAdd)
                {

                    if (args.RowData.CANTPED == 0 || args.RowData.PREC_UNIT_X_CANTIDAD == 0 || (decimal)args.Value == 0)
                    {
                        await refGridItems.UpdateCell(index, "IMP_DESCUENTO", (double)0);
                    }
                    else
                    {
                        await refGridItems.UpdateCell(index, "IMP_DESCUENTO",
                            args.RowData.PREC_UNIT_X_CANTIDAD / (1 + (decimal)args.Value / 100));
                        //await refGridItems.UpdateCell(index, "Sum", Convert.ToDouble(args.Value) + 0);
                    }



                }
                var descuento = Math.Round(args.RowData.PREC_UNIT_X_CANTIDAD * args.RowData.DESCUENTO / 100, 2);
                await refGridItems.UpdateCell(index, "IMP_DESCUENTO", descuento);
            }

            //var item = Presupuesto.Items.Find(i=> i.Id == args.RowData.Id);
            //item.TOTAL = args.RowData.PREC_UNIT_X_CANTIDAD - args.RowData.IMP_DESCUENTO;


            await refGridItems.UpdateCell(index, "TOTAL", args.RowData.PREC_UNIT_X_CANTIDAD - args.RowData.IMP_DESCUENTO);// total del item

            //Presupuesto.TOTAL = Math.Round(Presupuesto.Items.Sum(i => i.TOTAL)); //total del presupuesto - la 
            await refGridItems.EndEditAsync();
            //refGridItems.Refresh();
        }

        public void BatchAddHandler(BeforeBatchAddArgs<PedCli> args)
        {
            IsAdd = true;
        }
        public void BatchSaveHandler(BeforeBatchSaveArgs<PedCli> args)
        {
            IsAdd = false;
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