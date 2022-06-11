using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SupplyChain.Client.HelperService;
using SupplyChain.Client.Shared.BuscadorCliente;
using SupplyChain.Client.Shared.BuscadorProducto;
using SupplyChain.Client.Shared.BuscarSolicitud;
using SupplyChain.Shared;
using SupplyChain.Shared.Models;
using Syncfusion.Blazor.DropDowns;
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
        [Inject] public IJSRuntime Js { get; set; }
        [Inject] public PresupuestoService PresupuestoService { get; set; }
        [Inject] public PrecioArticuloService PrecioArticuloService { get; set; }
        [Inject] public CondicionPagoService CondicionPagoService { get; set; }
        [Inject] public CondicionEntregaService CondicionEntregaService { get; set; }
        [Inject] public DireccionEntregaService DireccionEntregaService { get; set; }
        [Inject] public TipoCambioService TipoCambioService { get; set; }
        [Inject] public SolicitudService SolicitudService { get; set; }
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

        protected SolicitudesDialog refSolicitudDialog;
        protected bool popupBuscadorVisibleSolicitud { get; set; } = false;

        protected List<string> direccionesEntregas = new();
        protected List<vCondicionesPago> condicionesPagos = new();
        protected List<vCondicionesEntrega> condicionesEntrega = new();
        protected List<string> Monedas = new() {"PESOS","DOLARES" };
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
        public async Task ShowAsync(int id)
        {
            if (id > 0)
            {
                await GetPresupuesto(id);
            }
            
            await GetCondicionesPago();
            await GetCondicionesEntrega();
            if (Presupuesto.CG_CLI > 0)
            {
                ReadOnlyMoneda = true;
                await GetDireccionesEntregaCliente(Presupuesto.CG_CLI);
            }
            else
            {
                ReadOnlyMoneda = false;
                await GetTipoCambioDolarHoy();
            }

            
        }

        protected async Task GetTipoCambioDolarHoy()
        {
            var tc = await TipoCambioService.GetValorDolarHoy();
            if (tc == 0)
            {
                await ToastMensajeError("Error al obtener Tipo de Cambio.");
            }
            else
            {
                Presupuesto.TC = (double)tc;
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
                Presupuesto = response.Response;
                foreach (var item in Presupuesto.Items)
                {
                    item.Estado = SupplyChain.Shared.Enum.EstadoItem.Modificado;
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

        protected async Task BuscarSolicitudes()
        {
            if (Presupuesto.CG_CLI == 0)
            {
                await ToastMensajeError("Seleccione Cliente.");
                return;
            }

            SpinnerVisible = true;
            await refSolicitudDialog.Show();
            SpinnerVisible = false;
            popupBuscadorVisibleSolicitud = true;
        }

        protected async Task ClienteExternoSelected(ClienteExterno clienteSelected)
        {
            await refSpinnerCli.ShowAsync();
            popupBuscadorVisibleCliente = false;
            Presupuesto.CG_CLI = Convert.ToInt32(clienteSelected.CG_CLI);
            Presupuesto.DES_CLI = clienteSelected.DESCRIPCION.Trim();
            Presupuesto.CONDICION_PAGO = clienteSelected.ID_CON_VEN == null ?  0 : (int)clienteSelected.ID_CON_VEN;//hay cliente que no tienen asignado una condicion de pago
            Presupuesto.BONIFIC = clienteSelected.DESC_COMERCIAL == null ? 0 : (decimal)clienteSelected.DESC_COMERCIAL;
            Presupuesto.CG_COND_ENTREGA = clienteSelected.ID_CON_ENT == null ? 0 : (int)clienteSelected.ID_CON_ENT;
            await GetDireccionesEntregaCliente(Presupuesto.CG_CLI);
            await refSpinnerCli.HideAsync();
        }

        private async Task GetDireccionesEntregaCliente(int id)
        {
            var response = await DireccionEntregaService.GetByNumeroCliente(id);
            if (response.Error)
            {
                await ToastMensajeError("Ocurrio un Error.\nError al intentar obtener direcciones de entrega");
            }
            else
            {
                direccionesEntregas = response.Response.Select(de=> de.DESCRIPCION).ToList();
            }
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

        protected async Task SolicitudSelected(Solicitud solicitudSelected)
        {
            if (Presupuesto.CG_CLI != solicitudSelected.CG_CLI)
            {
                await ToastMensajeError("No se puede agregar item.\nCliente no corresponde al presupuesto.");
                return;
            }
            //Debe venir siempre con precio
            if (solicitudSelected.PrecioArticulo != null)
            {
                await AgregarSolicitud(solicitudSelected);
            }

            
        }

        private async Task AgregarSolicitud(Solicitud solicitudSelected)
        {
            if (PermiteAgregItemSolicitud(solicitudSelected.Id))
            {
                await refSpinnerCli.ShowAsync();
                popupBuscadorVisibleProducto = false;

                var item = new PresupuestoDetalle()
                {
                    Id = Presupuesto.Items.Count == 0 ? -1 : Presupuesto.Items.Count * -1, //en negativos
                    SOLICITUDID = solicitudSelected.Id,
                    CANTIDAD = solicitudSelected.Cantidad,
                    CG_ART = solicitudSelected.Producto,
                    DES_ART = solicitudSelected.Des_Prod,
                    //ACCESORIOS = solicitudSelected.Accesorios,
                    //ASIENTO = solicitudSelected.Asiento,
                    //BONETE = solicitudSelected.Bonete,
                    //CUERPO = solicitudSelected.Cuerpo,
                    //DISCO = solicitudSelected.Disco,
                    //MEDIDAS = solicitudSelected.Medidas,
                    //ORIFICIO = solicitudSelected.Orificio,
                    //RESORTE = solicitudSelected.Resorte,
                    //SERIEENTRADA = solicitudSelected.SerieEntrada,
                    //SERIESALIDA = solicitudSelected.SerieSalida,
                    //TIPOENTRADA = solicitudSelected.TipoEntrada,
                    //TIPOSALIDA = solicitudSelected.TipoSalida,
                    //TOBERA = solicitudSelected.Tobera,
                    PREC_UNIT = (decimal)solicitudSelected.PrecioArticulo?.Precio,
                    Estado = SupplyChain.Shared.Enum.EstadoItem.Agregado
                };

                Presupuesto.Items.Add(item);
                await refGridItems.RefreshHeaderAsync();
                refGridItems.Refresh();
                await refGridItems.RefreshColumnsAsync();
                await refSpinnerCli.HideAsync();
            }
            else
            {
                await ToastMensajeError($"La Solicitud Nro: {solicitudSelected.Id} ya ha sido agregado");
            }
            
        }

        private bool PermiteAgregItemSolicitud(int solicitudId)
        {
            return !Presupuesto.Items.Any(i => i.SOLICITUDID == solicitudId);
        }


        protected async Task<bool> Agregar(Presupuesto presupueto)
        {
            var response = await PresupuestoService.Agregar(presupueto);
            if (response.Error)
            {
                Console.WriteLine(await response.HttpResponseMessage.Content.ReadAsStringAsync());
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
            await ImprimirDataSheet();
            await OnGuardar.InvokeAsync(Presupuesto);
            
            
        }

        protected async Task ImprimirDataSheet()
        {
            foreach (var item in Presupuesto.Items.Where(i => i.Estado != SupplyChain.Shared.Enum.EstadoItem.Eliminado))
            {
                if (item.SOLICITUDID > 0)
                {
                    var response = await SolicitudService.GetById(item.SOLICITUDID);
                    if (response.Error)
                    {
                        Console.WriteLine(await response.HttpResponseMessage.Content.ReadAsStringAsync());
                    }
                    else
                    {
                        await Js.InvokeVoidAsync("open", 
                            new object[2] { $"/api/ReportRDLC/GetReportDataSheet?id={response.Response.CalcId}", "_blank" });
                    }

                }
            }
        }

        protected async Task BatchDeleteHandler(BeforeBatchDeleteArgs<PresupuestoDetalle> args)
        {
            var item = Presupuesto.Items.Find(i => i.Id == args.RowData.Id);
            item.Estado = SupplyChain.Shared.Enum.EstadoItem.Eliminado;
        }

        public async Task CellSavedHandler(CellSaveArgs<PresupuestoDetalle> args)
        {
            var index = await refGridItems.GetRowIndexByPrimaryKey(args.RowData.Id);
            if (args.ColumnName == "Cantidad")
            {
                args.RowData.CANTIDAD = (decimal)args.Value;
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
                        Convert.ToDouble(args.Value) * (double)args.RowData.CANTIDAD);
                    //await refGridItems.UpdateCell(index, "Sum", Convert.ToDouble(args.Value) + 0);
                }
                await refGridItems.UpdateCell(index, "PREC_UNIT_X_CANTIDAD", Convert.ToDouble(args.Value) * (double)args.RowData.CANTIDAD);
                
            }
            else if (args.ColumnName == "DESCUENTO")
            {
                args.RowData.DESCUENTO = (decimal)args.Value;
                if (IsAdd)
                {

                    if (args.RowData.CANTIDAD == 0 || args.RowData.PREC_UNIT_X_CANTIDAD == 0 || (decimal)args.Value == 0)
                    {
                        await refGridItems.UpdateCell(index, "IMP_DESCUENTO", (double)0);
                    }
                    else
                    {
                        await refGridItems.UpdateCell(index, "IMP_DESCUENTO", 
                            args.RowData.PREC_UNIT_X_CANTIDAD / (1 + (decimal)args.Value / 100 ));
                        //await refGridItems.UpdateCell(index, "Sum", Convert.ToDouble(args.Value) + 0);
                    }



                }
                var descuento = Math.Round( args.RowData.PREC_UNIT_X_CANTIDAD * args.RowData.DESCUENTO / 100, 2);
                await refGridItems.UpdateCell(index, "IMP_DESCUENTO", descuento);
            }

            //var item = Presupuesto.Items.Find(i=> i.Id == args.RowData.Id);
            //item.TOTAL = args.RowData.PREC_UNIT_X_CANTIDAD - args.RowData.IMP_DESCUENTO;


            await refGridItems.UpdateCell(index, "TOTAL", args.RowData.PREC_UNIT_X_CANTIDAD - args.RowData.IMP_DESCUENTO);// total del item

            //Presupuesto.TOTAL = Math.Round(Presupuesto.Items.Sum(i => i.TOTAL)); //total del presupuesto - la 
            await refGridItems.EndEditAsync();
            //refGridItems.Refresh();
        }

        public void BatchAddHandler(BeforeBatchAddArgs<PresupuestoDetalle> args)
        {
            IsAdd = true;
        }
        public void BatchSaveHandler(BeforeBatchSaveArgs<PresupuestoDetalle> args)
        {
            IsAdd = false;
        }

        protected void CambioMoneda(ChangeEventArgs<string, string> args)
        {
            var tipoCambio = Presupuesto.TC; 
            foreach (var item in Presupuesto.Items)
            {
                if (args.Value == "DOLARES")
                {
                    item.PREC_UNIT /= (decimal)tipoCambio;
                }
                else
                {
                    item.PREC_UNIT *= (decimal)tipoCambio;
                }

                item.PREC_UNIT_X_CANTIDAD = item.PREC_UNIT * item.CANTIDAD;
            }

            refGridItems.Refresh();
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

        protected async Task CerrarDialogSolicitud()
        {
            popupBuscadorVisibleSolicitud = false;
        }

        protected async Task OnCerrarDialog()
        {
            await OnCerrar.InvokeAsync();
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
