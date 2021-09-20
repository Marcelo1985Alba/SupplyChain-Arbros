﻿using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SupplyChain.Client.HelperService;
using SupplyChain.Client.Shared;
using SupplyChain.Shared;
using SupplyChain.Shared.Models;
using SupplyChain.Shared.Prod;
using Syncfusion.Blazor.Notifications;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Grid;
using Syncfusion.Pdf.Tables;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace SupplyChain.Client.Pages.Inventarios
{
    public class StockOperacionesBase : ComponentBase
    {
        [Inject] public IJSRuntime JsRuntime { get; set; }
        [Inject] public HttpClient Http { get; set; }
        [CascadingParameter] public MainLayout MainLayout { get; set; }
        [Parameter] public int OperacionId { get; set; } = 0;
        [Parameter] public bool EsEntrega { get; set; } = false;
        [Parameter] public int OrdFab { get; set; } = 0;
        [Parameter] public PedidoEncabezado StockEncabezado { get; set; } = new PedidoEncabezado();

        protected bool DisableCssClass
        {
            get => StockEncabezado.TIPOO == 0 ? true : EsEntrega ? true : false;
        }

        protected SfToast ToastObj;
        protected ConfirmacionDialog ConfirmacionEliminarDialog;
        protected ConfirmacionDialog ConfirmacionGuardarDialog;
        protected bool SpinnerVisible { get; set; } = false;
        private bool puedeBuscarStock = false;
        protected SupplyChain.Client.Shared.BuscadorEmergenteResumenStock BuscadorEmergenteResumenStock;
        protected bool DepositoSalidaSoloLectura = true;
        protected bool DepositoIngresoSoloLectura = true;
        protected bool abrioVale = false;
        protected Dictionary<string, object> HtmlAttribute = new Dictionary<string, object>()
        {
                {"type", "button" }
        };

        protected bool PermiteAgregarItem { get; set; } = false;
        protected bool PermiteEditarItem { get; set; } = false;
        protected bool PermiteEliminarItem { get; set; } = false;

        #region "PARA BUSCAR ORDENES DE COMPRAS"
        protected bool OCSoloLectura = true;
        protected bool DeshabilitaBotonOC = true;
        protected bool MostrarBotorOC = false;
        #endregion

        #region "PARA BUSCAR ORDENES DE FABRICACION"
        protected bool OFSoloLectura = true;
        protected bool DeshabilitaBotonOF = true;
        protected bool MostrarBotorOF = false;
        #endregion

        protected string labelClienteProveedor = "Cliente/Prove";

        Producto selectedProducto;
        public Producto SelectedProducto
        {
            get { return selectedProducto; }
            set
            {
                selectedProducto = value;
                InvokeAsync(StateHasChanged);
            }
        }


        #region "CABACERA VALE"
        protected int? Cg_CLI_Cg_PROVE { get; set; } = 0;
        protected string Codigoi { get; set; } = "";
        protected string DescripcionPro { get; set; } = "";



        protected void OnChanged() { InvokeAsync(StateHasChanged); }
        #endregion

        protected override async Task OnInitializedAsync()
        {
            MainLayout.Titulo = "Administracion de Stock";
            await GetVale();
            StockEncabezado.Items = new List<Pedidos>();

            if (OperacionId == 10 && EsEntrega && OrdFab > 0)
            {
                MainLayout.Titulo = "Administracion de Stock: ingreso por Planificacion";
                var tire = new Tire();
                tire.Tipoo = 10;
                await SelectedTireChanged(tire);
                var programa = await Http.GetFromJsonAsync<List<Programa>>($"api/Programa/GetProgramaByOF/{OrdFab}");
                await OnProgramaSelected(programa[0]);
            }
            await InvokeAsync(StateHasChanged);

        }

        protected async Task GetVale()
        {
            StockEncabezado.VALE = await Http.GetFromJsonAsync<int>($"api/Stock/GetMaxVale");
        }


        protected async Task NuevoVale()
        {
            abrioVale = false;
            StockEncabezado = new();
            StockEncabezado.FE_MOV = DateTime.Now;
            await GetVale();
            StockEncabezado.Items = new List<Pedidos>();
            CantRegistros = 0;
            ItemsABuscar = null;
        }

        protected async Task SelectedTireChanged(Tire tire)
        {
            SpinnerVisible = true;
            await NuevoVale();

            //Dependiendo el TIPOO Habilitar controles:
            //if (StockEncabezado.TIPOO != tire.Tipoo)
            //{

            //}
            StockEncabezado.TIPOO = tire.Tipoo;
            MostrarBotorOC = false;
            DeshabilitaBotonOC = true;
            OCSoloLectura = true;
            DeshabilitaBotonOF = true;
            OFSoloLectura = true;
            DepositoSalidaSoloLectura = true;
            DepositoIngresoSoloLectura = true;
            puedeBuscarStock = false;

            if (tire.Tipoo == 5) //recep prove
            {
                DeshabilitaBotonOC = false;
                MostrarBotorOC = true;
                DepositoIngresoSoloLectura = false;
                labelClienteProveedor = "Proveedor";
                PermiteAgregarItem = true;
                //StockEncabezado.CG_DEP = 4;
                foreach (var item in StockEncabezado.Items)
                {
                    item.CG_DEP = 4;
                }
            }

            if (StockEncabezado.TIPOO == 6)//dev prove
            {
                DeshabilitaBotonOC = false;
                MostrarBotorOC = true;
                DepositoSalidaSoloLectura = false;
                labelClienteProveedor = "Proveedor:";
            }

            if (StockEncabezado.TIPOO == 9) //Movimeinto Entre deposito
            {
                puedeBuscarStock = true;
                DepositoIngresoSoloLectura = false;

            }

            if (StockEncabezado.TIPOO == 10) //Entrega de insumos a una orden de fabricación
            {
                PermiteEditarItem = true;
                OFSoloLectura = true;
                DeshabilitaBotonOF = false;
                MostrarBotorOF = true;
                puedeBuscarStock = true;
            }

            if (StockEncabezado.TIPOO == 21 || StockEncabezado.TIPOO == 27) //Ajuste de Invetario y Entrega a Planta Sin OF
            {
                PermiteAgregarItem = true;
                PermiteEditarItem = true;
                PermiteEliminarItem = true;
                puedeBuscarStock = true;
            }

            SpinnerVisible = false;
            await InvokeAsync(StateHasChanged);
        }

        protected void SelectedDepositoSalidaChanged(Deposito deposito)
        {
            //selectedDepositoSalida = deposito;

            if (StockEncabezado.TIPOO == 6) //DEVOL prove
            {
                //StockEncabezado.CG_DEP = deposito.CG_DEP;
                StockEncabezado.Items.ForEach(s =>
                {
                    s.CG_DEP = deposito.CG_DEP;
                });
            }
        }
        protected void SelectedDepositoIngresoChanged(Deposito deposito)
        {
            //selectedDepositoIngreso = deposito;
            if (StockEncabezado.TIPOO == 5) //RECEP prove
            {
                //StockEncabezado.CG_DEP = deposito.CG_DEP;
                StockEncabezado.Items.ForEach(s =>
                {
                    s.CG_DEP = deposito.CG_DEP;
                });
            }

            if (StockEncabezado.TIPOO == 9) //MOVIM ENTRE DEP
            {
                PermiteAgregarItem = true;
                StockEncabezado.Items = new List<Pedidos>();
            }
        }

        protected async Task CargarVale()
        {
            //Cargando Datos de Cabecera
            SpinnerVisible = true;
            StockEncabezado.VALE = StockEncabezado.Items[0].VALE;
            StockEncabezado.FE_MOV = StockEncabezado.Items[0].FE_MOV;
            //depos: ver cual campo es ingreso y salida: tal vez depende segun el tipo de operacion
            //StockEncabezado.CG_DEP = StockEncabezado.Items[0].CG_DEP;
            //selectedDepositoIngreso = Depositos.Where(d => d.CG_DEP == ItemsVale[0].CG_DEP).FirstOrDefault();
            StockEncabezado.CG_ORDF = (int)StockEncabezado.Items[0].CG_ORDF;
            StockEncabezado.PEDIDO = (int)StockEncabezado.Items[0].PEDIDO;

            StockEncabezado.OCOMPRA = StockEncabezado.Items[0].OCOMPRA;

            if (StockEncabezado.TIPOO == 5)
            {
                Cg_CLI_Cg_PROVE = StockEncabezado.Items[0].CG_CLI == 0 ? StockEncabezado.Items[0].CG_PROVE : StockEncabezado.Items[0].CG_CLI;
                DescripcionPro = StockEncabezado.Items[0].CG_CLI == 0 ? StockEncabezado.Items[0].Proveedor.DES_PROVE.Trim() : "Cliente";
                StockEncabezado.REMITO = StockEncabezado.Items[0].REMITO;

                //Cargando datos para controlar si exige lote etc: para controlar validaciones en la edicion.
                await StockEncabezado.Items.ForEachAsync(async i =>
                {
                    var cg_prod = i.CG_ART;
                    var prod = await Http.GetFromJsonAsync<Producto>($"api/Prod/{cg_prod}");
                    i.EXIGEDESPACHO = prod.EXIGEDESPACHO;
                    i.EXIGELOTE = prod.EXIGELOTE;
                    i.EXIGESERIE = prod.EXIGESERIE;
                });
            }


            if (StockEncabezado.TIPOO == 10)
            {
                await StockEncabezado.Items.ForEachAsync(async i =>
                {
                    i.STOCK = Math.Abs((decimal)i.STOCK);

                });
            }


            PermiteAgregarItem = false;
            PermiteEditarItem = true;
            PermiteEliminarItem = true;

            abrioVale = true;
            SpinnerVisible = false;
            //await InvokeAsync(StateHasChanged);
        }

        #region "BUSCADOR EMERGENTE"
        protected string tituloBuscador { get; set; } = "";
        protected bool popupVisible = false;
        protected bool PopupVisible { get => popupVisible; set { popupVisible = value; InvokeAsync(StateHasChanged); } }


        protected async Task BuscarResumenStock()
        {
            PermiteAgregarItem = false;
            popupBuscadorVisible = true;
            await InvokeAsync(StateHasChanged);
        }

        protected async Task onResumenStockSelected(ResumenStock item)
        {
            popupBuscadorVisible = false;
            List<Pedidos> lStock = new List<Pedidos>();
            int registronegativo = 0;

            registronegativo--;
            Pedidos pedido = new Pedidos();
            pedido.CG_ART = item.CG_ART;
            pedido.CG_DEP = item.CG_DEP;
            pedido.SERIE = item.SERIE;
            pedido.LOTE = item.LOTE;
            pedido.DESPACHO = item.DESPACHO;
            pedido.STOCK = item.STOCK;

            //datos del producto: exigeserie etc.
            var prod = await Http.GetFromJsonAsync<Producto>($"api/Prod/{item.CG_ART}");
            pedido.UNID = prod.UNID;
            pedido.EXIGEDESPACHO = prod.EXIGEDESPACHO;
            pedido.EXIGESERIE = prod.EXIGESERIE;
            pedido.EXIGELOTE = prod.EXIGELOTE;
            pedido.DES_ART = prod.DES_PROD;
            pedido.REGISTRO = registronegativo;

            pedido.CG_PROVE = 0;
            Cg_CLI_Cg_PROVE = 0;

            lStock.Add(pedido);

            StockEncabezado.Items = lStock;

            PermiteAgregarItem = false;
            PermiteEditarItem = true;
            PermiteEliminarItem = true;
        }



        protected async Task OnProgramaSelected(Programa programaSel)
        {
            SpinnerVisible = true;
            List<Pedidos> lStock = new List<Pedidos>();
            int registronegativo = 0;
            StockEncabezado.CG_ORDF = programaSel.CG_ORDF;
            StockEncabezado.ModeloOrdenFabricacionEncabezado = await Http.GetFromJsonAsync<ModeloOrdenFabricacionEncabezado>("api/OrdenesFabricacionEncabezado/" + programaSel.CG_ORDF.ToString());
            var ItemsPrograma = await Http.GetFromJsonAsync<ItemAbastecimiento[]>($"api/Programa/GetAbastecimientoByOF/{programaSel.CG_ORDF}");
            //Cargar los item de sp en los items de items para guardar vale
            foreach (var item in ItemsPrograma)
            {
                registronegativo--;
                Pedidos pedido = new();
                pedido.CG_ORDF = programaSel.CG_ORDF;
                pedido.TIPOO = StockEncabezado.TIPOO;
                pedido.CG_PROVE = 0;
                Cg_CLI_Cg_PROVE = programaSel.CG_CLI;
                pedido.CG_DEP = item.CG_DEP;
                pedido.CG_ART = item.CG_ART.Trim();
                pedido.DES_ART = item.DES_ART;
                pedido.STOCK = item.CANTPED;
                pedido.UNID = item.UNID;
                pedido.LOTE = item.LOTE;
                pedido.SERIE = item.SERIE;
                pedido.DESPACHO = item.DESPACHO;
                pedido.ResumenStock = item.ResumenStock;

                pedido.REGISTRO = registronegativo;
                pedido.PENDIENTEOC = item.STOCK; //STOCK
                lStock.Add(pedido);


            }


            StockEncabezado.Items = lStock;
            PermiteAgregarItem = true;
            PermiteEditarItem = true;
            PermiteEliminarItem = true;
            SpinnerVisible = false;
            await InvokeAsync(StateHasChanged);
        }

        protected async Task OnCompraSelected(List<Compra> lcompraSel)
        {
            List<Pedidos> lStock = new List<Pedidos>();
            int registronegativo = 0;
            StockEncabezado.OCOMPRA = lcompraSel[0].NUMERO;
            Genera genera;
            if (StockEncabezado.TIPOO == 5)
            {

                foreach (var item in lcompraSel)
                {
                    registronegativo--;

                    Pedidos pedido = new();
                    pedido.TIPOO = StockEncabezado.TIPOO;
                    pedido.Proveedor = lcompraSel[0].ProveedorNavigation;
                    pedido.OCOMPRA = item.NUMERO;
                    pedido.CG_PROVE = item.NROCLTE;
                    Cg_CLI_Cg_PROVE = item.NROCLTE;
                    DescripcionPro = item.DES_PROVE;
                    pedido.CG_ART = item.CG_MAT;
                    pedido.DES_ART = item.DES_MAT;
                    pedido.MONEDA = item.MONEDA;
                    pedido.UNID = item.UNID;
                    pedido.CG_DEP = 4;
                    pedido.CG_DEN = item.CG_DEN;
                    //Calcular stockA
                    pedido.PENDIENTEOC = item.PENDIENTE;
                    pedido.UNIDA = item.UNID1;
                    pedido.STOCKA = item.AUTORIZADO;
                    pedido.STOCK = item.PENDIENTE;
                    pedido.IMPORTE1 = item.PRECIONETO;


                    //datos del producto: exigeserie etc.

                    var filterProd = new FilterProd()
                    {
                        Codigo = item.CG_MAT.Trim(),
                        Descripcion = ""

                    };

                    var prod = await Http.GetFromJsonAsync<Producto>($"api/Prod/GetByFilter?Codigo={filterProd.Codigo}&Descripcion={filterProd.Descripcion}");

                    pedido.EXIGEDESPACHO = prod.EXIGEDESPACHO;
                    pedido.EXIGESERIE = prod.EXIGESERIE;
                    pedido.EXIGELOTE = prod.EXIGELOTE;

                    if (pedido.EXIGEDESPACHO)
                    {
                        genera = await Http.GetFromJsonAsync<Genera>($"api/Genera/Reserva/CG_ORDING");
                        pedido.DESPACHO = Convert.ToInt32(genera.VALOR1).ToString();
                        await Http.GetFromJsonAsync<Genera>($"api/Genera/Libera/CG_ORDING");
                    }

                    pedido.REGISTRO = registronegativo;


                    lStock.Add(pedido);
                    StockEncabezado.Items = lStock;

                }


            }

            if (StockEncabezado.TIPOO == 6)
            {
                //Devuelve con los items con su respectivo stock
                StockEncabezado.Items = await Http.GetFromJsonAsync<List<Pedidos>>($"api/Stock/AbriValeByOCParaDevol/{lcompraSel[0].NUMERO}");
                //ItemsVale = new ObservableCollection<Stock>(data);

                registronegativo = 0;
                await StockEncabezado.Items.ForEachAsync(async stock =>
                {
                    registronegativo--;
                    stock.REGISTRO = registronegativo;
                    stock.STOCK = Math.Abs((decimal)stock.STOCK);
                    await InvokeAsync(StateHasChanged);
                });
            }

            PermiteAgregarItem = true;
            PermiteEditarItem = true;
            PermiteEliminarItem = true;
        }

        protected async Task OnResumenStockSelected(Pedidos stockSel)
        {
            //ItemsVale = null;
            PopupBuscadorVisible = false;
            //Get items del vale
            var vale = stockSel.VALE;
            StockEncabezado.Items = await Http.GetFromJsonAsync<List<Pedidos>>($"api/Stock/AbriVale/{stockSel.VALE}");
            //ItemsVale = new ObservableCollection<Stock>(data);

            if (StockEncabezado.Items != null || StockEncabezado.Items.Count > 0)
            {
                await CargarVale();
            }

            await InvokeAsync(StateHasChanged);
        }

        #endregion

        #region "BUSCAR VALES"

        protected int CantRegistros = 0;
        private bool popupBuscadorVisible = false;
        protected bool PopupBuscadorVisible { get => popupBuscadorVisible; set { popupBuscadorVisible = value; InvokeAsync(StateHasChanged); } }
        private bool popupBuscadorVisibleRS = false;
        protected bool PopupBuscadorVisibleRS { get => popupBuscadorVisibleRS; set { popupBuscadorVisibleRS = value; InvokeAsync(StateHasChanged); } }
        protected string[] ColumnasBuscador = null; /*{ "VALE", "FE_MOV", "CG_ART", "DES_ART", "DESPACHO", "LOTE", "SERIE" };*/
        protected Pedidos[] ItemsABuscar;
        protected ResumenStock[] buscarResumenStock;

        protected async Task OnObjectSelected(Pedidos stockSel)
        {
            PopupBuscadorVisible = false;
            //Get items del vale
            var vale = stockSel.VALE;
            StockEncabezado.Items = await Http.GetFromJsonAsync<List<Pedidos>>($"api/Stock/AbriVale/{stockSel.VALE}");
            //ItemsVale = new ObservableCollection<Stock>(data);

            //ItemsABuscar = ItemsABuscar.Where(i => i.VALE == stockSel.VALE).ToList().ToArray();

            if (StockEncabezado.Items != null || StockEncabezado.Items.Count > 0)
            {
                await CargarVale();
            }
        }

        protected async Task AbrirVale()
        {

            var vale = StockEncabezado.VALE;
            StockEncabezado.Items = await Http.GetFromJsonAsync<List<Pedidos>>($"api/Stock/AbriVale/{vale}");

            if (StockEncabezado.Items != null || StockEncabezado.Items.Count > 0)
            {
                await BuscarVales();
            }
            else
            {
                await CargarVale();
            }
        }

        protected async Task BuscarVales()
        {
            CantRegistros += 100;
            abrioVale = true;
            PopupBuscadorVisible = true;
            tituloBuscador = $"Listado de Vales";
            ColumnasBuscador = new string[] { "VALE", "FE_MOV", "CG_ART", "DES_ART", "STOCK" };
            var tipoo = StockEncabezado.TIPOO;
            ItemsABuscar = await Http.GetFromJsonAsync<Pedidos[]>($"api/Stock/GetValesByTipo/{tipoo}/{CantRegistros}");
            if (ItemsABuscar == null)
            {
                ItemsABuscar = new List<Pedidos>().ToArray();
            }
            await InvokeAsync(StateHasChanged);
        }

        #endregion

        protected async Task OnGuardar()
        {
            await ConfirmacionGuardarDialog.HideAsync();

            SpinnerVisible = true;

            await StockEncabezado.Items.ForEachAsync(async s =>
            {
                await SetearCamposParaGuardar(s);
            });

            await GuardarDB(StockEncabezado.Items);


            if (StockEncabezado.TIPOO == 5)
            {
                
                await StockEncabezado.Items.ForEachAsync(async s =>
                {
                    await Etiqueta(s);
                });
            }

            await MostrarMensajeToastSuccess();

            StockEncabezado = new();
            SpinnerVisible = false;
        }

        private async Task MostrarMensajeToastSuccess()
        {
            await this.ToastObj.Show(new ToastModel
            {
                Title = "EXITO!",
                Content = $"Vale {StockEncabezado.VALE} Guardado Correctamente.",
                CssClass = "e-toast-success",
                Icon = "e-success toast-icons",
                ShowCloseButton = true,
                ShowProgressBar = true
            });
        }

        protected async Task OnGetItemsGrid(List<Pedidos> pedidos)
        {
            //StockEncabezado.Items.AddRange(pedidos);
            //await InvokeAsync(StateHasChanged);
        }

        protected async Task SetearCamposParaGuardar(Pedidos stock)
        {
            stock.VALE = StockEncabezado.VALE;
            stock.FE_MOV = StockEncabezado.FE_MOV;
            stock.CG_PROVE = Cg_CLI_Cg_PROVE;
            stock.TIPOO = StockEncabezado.TIPOO;

            //TODO: controlar el TIPOO al guardar para setar cada campo
            if (stock.TIPOO == 10)
            {
                stock.CG_ORDF = StockEncabezado.CG_ORDF;
                stock.AVISO = "ENTREGA A ORDEN DE FABRICACION";
            }


            if (stock.TIPOO == 5)
            {

                stock.AVISO = "RECEPCION DE INSUMOS DE PROVEEDOR"; /*VERIFICAR TIPOO*/
                stock.OCOMPRA = StockEncabezado.OCOMPRA;
                //stock.DES_PROVE = DescripcionPro.Trim();
                stock.REMITO = StockEncabezado.REMITO;
                //stock.CG_DEP = StockEncabezado.CG_DEP;
            }

            if (stock.TIPOO == 6)
            {

                stock.OCOMPRA = StockEncabezado.OCOMPRA;
                //stock.DES_PROVE = DescripcionPro.Trim();
                //stock.CG_DEP = SelectedDepositoSalida.CG_DEP;
                stock.STOCK = -stock.STOCK;
            }

            if (stock.TIPOO == 9)//movim depos
            {
                stock.CG_DEP_ALT = StockEncabezado.CG_DEP_ALT;
            }

            if (StockEncabezado.TIPOO == 21)
            {
                stock.AVISO = "AJUSTE INVENTARIOS";
            }

            if (StockEncabezado.TIPOO == 27)
            {
                stock.AVISO = "ENTREGA A PLANTA SIN OF";
                stock.STOCK = -stock.STOCK;
            }

            stock.ENTRREAL = DateTime.UtcNow;
            

        }

        private async Task GuardarDB(List<Pedidos> lStock)
        {
            HttpResponseMessage response = null;
            response = await Http.PostAsJsonAsync("api/Pedidos/PostList", lStock);



            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest
                || response.StatusCode == System.Net.HttpStatusCode.NotFound
                || response.StatusCode == System.Net.HttpStatusCode.Conflict)
            {
                var mensServidor = await response.Content.ReadAsStringAsync();


                Console.WriteLine($"Error: {mensServidor}");
                //toastService.ShowToast($"{mensServidor}", TipoAlerta.Error);
                await MensajeToastError();
            }
            else
            {
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var lpedidos = await response.Content.ReadFromJsonAsync<List<Pedidos>>();
                    StockEncabezado.Items = lpedidos;
                    await InvokeAsync(StateHasChanged);
                    //var itemsJson = JsonSerializer.Serialize(lpedidos);

                }
            }
        }

        private async Task MensajeToastError()
        {
            await this.ToastObj.Show(new ToastModel
            {
                Title = "ERROR!",
                Content = "Error al guardar",
                CssClass = "e-toast-danger",
                Icon = "e-error toast-icons",
                ShowCloseButton = true,
                ShowProgressBar = true
            });
        }

        protected async Task AbrirConfirmacionGuardarVale()
        {
            await ConfirmacionGuardarDialog.ShowAsync();
        }

        protected async Task AbrirConfirmacionEliminarVale()
        {
            await ConfirmacionEliminarDialog.ShowAsync();
        }

        protected async Task EliminarVale()
        {
            if (abrioVale)
            {
                await ConfirmacionEliminarDialog.HideAsync();
                SpinnerVisible = true;
                var response = await Http.DeleteAsync($"api/Pedidos/{StockEncabezado.VALE}");

                if (!response.IsSuccessStatusCode)
                {
                    SpinnerVisible = false;
                    var mensServidor = await response.Content.ReadAsStringAsync();


                    Console.WriteLine($"Error: {mensServidor}");
                    //toastService.ShowToast($"{mensServidor}", TipoAlerta.Error);
                    await this.ToastObj.Show(new ToastModel
                    {
                        Title = "ERROR!",
                        Content = "Error al eliminar vale",
                        CssClass = "e-toast-danger",
                        Icon = "e-error toast-icons",
                        ShowCloseButton = true,
                        ShowProgressBar = true
                    });
                }
                else
                {
                    SpinnerVisible = false;
                    StockEncabezado = new();
                    await this.ToastObj.Show(new ToastModel
                    {
                        Title = "EXITO!",
                        Content = $"Vale anulado Correctamente.",
                        CssClass = "e-toast-success",
                        Icon = "e-success toast-icons",
                        ShowCloseButton = true,
                        ShowProgressBar = true
                    });

                }


            }
        }

        protected void OnBlurRemito()
        {
            if (!string.IsNullOrEmpty(StockEncabezado.REMITO))
            {

                var puntoVenta = StockEncabezado.REMITO.Substring(0, StockEncabezado.REMITO.IndexOf("-"));
                puntoVenta = puntoVenta.PadLeft(4, '0');
                var numero = StockEncabezado.REMITO.Substring(StockEncabezado.REMITO.IndexOf("-") + 1);
                numero = numero.PadLeft(8, '0');
                StockEncabezado.REMITO = puntoVenta + "-" + numero;
            }

        }

        protected async Task ImprimirEtiqueta()
        {
            if (abrioVale)
            {
                await StockEncabezado.Items.ForEachAsync(async s =>
                {
                    await Etiqueta(s);
                });
            }
        }

        protected async Task Etiqueta(Pedidos pedidos)
        {

            PdfDocument document1 = new();
            document1.PageSettings.Size = new Syncfusion.Drawing.SizeF(227, 70);//110
            int margin = -25;

            document1.PageSettings.Margins.Left = -2;
            document1.PageSettings.Margins.Right = -15;
            document1.PageSettings.Margins.Top = 10;
            document1.PageSettings.Margins.Bottom = -10;
            //document1.PageSettings.Margins.All = margin;
            PdfGrid pdfGrid1 = new PdfGrid();
            PdfPage page = document1.Pages.Add();
            PdfGraphics graphics = page.Graphics;
            PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 10);
            PdfLightTable pdfTable = new PdfLightTable();
            page.Graphics.RotateTransform(-90);


            graphics.DrawString($"{pedidos.CG_ART.Trim()}                  OC {pedidos.OCOMPRA}\r\n{pedidos.DES_ART.Trim()}\r\n" +
                $"Despacho {pedidos.DESPACHO} Lote {pedidos.LOTE} VALE {pedidos.VALE}\n" +
                $"{pedidos.Proveedor?.DES_PROVE.Trim()}", font, PdfBrushes.Black, new Syncfusion.Drawing.PointF(-200, 10));

            //document1.PageSettings.Margins.Left = margin;
            //document1.PageSettings.Margins.Right = margin;
            //document1.PageSettings.Margins.Top = margin;
            //document1.PageSettings.Margins.Bottom = margin;
            MemoryStream xx = new MemoryStream();
            document1.Save(xx);
            document1.Close(true);
            await JsRuntime.SaveAs("ETOC" + pedidos.CG_ART.Trim() + ".pdf", xx.ToArray());
        }
    }
}