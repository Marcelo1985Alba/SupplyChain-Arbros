using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Navigations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using SupplyChain.Shared.Models;
using System.IO;
using Syncfusion.Blazor.Inputs;
using Newtonsoft.Json;
using Syncfusion.Blazor.Notifications;
using Syncfusion.Blazor.DropDowns;
using Syncfusion.Blazor.Data;
using Syncfusion.Blazor.Buttons;
using SupplyChain.Client.Shared;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Grid;
using Syncfusion.Pdf.Tables;
using SupplyChain.Client.HelperService;


namespace SupplyChain.Client.Pages.NoConf
{
    public class NoConformidadesPageBase : ComponentBase
    {
        [Inject] protected HttpClient Http { get; set; }
        [Inject] protected IJSRuntime JsRuntime { get; set; }
        [Inject] protected Microsoft.JSInterop.IJSRuntime JS { get; set; }

        [CascadingParameter] public MainLayout MainLayout { get; set; }

        // variables generales
        public bool IsVisible { get; set; } = false;
        public SfToast ToastObj;
        public bool ocultadivlista { get; set; } = false;
        public bool ocultadivAcciones { get; set; } = true;
        public bool ocultadivform { get; set; } = true;
        public bool ocultabotvolver { get; set; } = true;
        public bool ocultafechacierre { get; set; } = true;
        public bool ocultacheckfechacierre { get; set; } = false;
        public bool deshabradio { get; set; } = false;
        public bool desfechacierre { get; set; } = false;




        protected NotificacionToast NotificacionObj;
        protected bool ToastVisible { get; set; } = false;
//        protected SfSpinner SpinnerObj;

        // funciones base
        public async Task nuevoevento()
        {

            await Limpiadatos();

            ValorCg_noconf = 0;
            ValorOF = 0;
            ValorPedido = 0;
            ValorOC = 0;
            ocultadivAcciones = true;
            ocultadivlista = true;
            ocultadivform = false;
            ocultabotvolver = false;
            deshabradio = false;
            valorradio = "sinvinculos";
            cantidadnoconf = 0;
            DropVal = 0;
            ocultadivof = true;
            ocultadivocompra = true;
            ocultadivpedido = true;

            ocultadivtextodatos = true;
            ocultadivdatosgenerales = false;

            OFSoloLectura = false;
            DeshabilitaBotonOF = false;
            OCSoloLectura = false;
            DeshabilitaBotonOC = false;
            PedidoSoloLectura = false;
            DeshabilitaBotonPedido = false;
            desfechacierre = false;
            ocultafechacierre = true;
        }

        [Parameter] public int vieneOF { get; set; } = 0;

        public async Task volver()
        {
            ocultadivlista = false;
            ocultadivform = true;
            ocultabotvolver = true;
        }

        // variables form
        public string valorradio { get; set; } = "sinvinculos";

        NoConformidades NoConf = new NoConformidades();

        public int DropVal;
        public DateTime fechahoy;
        public DateTime fechaocurr;
        public DateTime fechaimplemen;
        public DateTime fechacierre;
        public DateTime fechaaccion;
        protected bool checkcierra { get; set; } = false;

        public bool MostrarSpin { get; set; } = false;
        public int ValorOF { get; set; } = 0;
        public int ValorOC { get; set; } = 0;
        public int ValorPedido { get; set; } = 0;
        public int ValorCg_noconf { get; set; } = 0;


        public int cg_cli { get; set; } = 0;
        public string des_cli { get; set; } = "";
        public int cg_prove { get; set; } = 0;
        public string des_prove { get; set; } = "";

        public DateTime fecha { get; set; }


        public decimal cantidadnoconf { get; set; } = 0;

        public string cg_prod { get; set; } = "";
        public string des_prod { get; set; } = "";
        public decimal cantidad { get; set; } = 0;
        public string lote { get; set; } = "";
        public string serie { get; set; } = "";
        public string despacho { get; set; } = "";

        public string textodatos { get; set; } = "";
        public string datosnoconfor1 { get; set; } = "";
        public string comentarios { get; set; } = "";

        public bool ocultadivof { get; set; } = true;
        public bool ocultadivpedido { get; set; } = true;
        public bool ocultadivocompra { get; set; } = true;
        public bool ocultadivtextodatos { get; set; } = true;
        public bool ocultadivdatosgenerales { get; set; } = false;

        public class ListaDespachos
        {
            public string despachocombo { get; set; }
        }
        protected List<ListaDespachos> datosdespacho;

        public class ListaLotes
        {
            public string lotecombo { get; set; }
        }
        public List<ListaLotes> datoslote;
        #region "PARA BUSCAR ORDENES DE FABRICACION"
        public bool OFSoloLectura = false;
        public bool DeshabilitaBotonOF = false;
        public bool OCSoloLectura = false;
        public bool DeshabilitaBotonOC = false;
        public bool PedidoSoloLectura = false;
        public bool DeshabilitaBotonPedido = false;

        public bool MostrarBotorOF = true;
        #endregion
        public int OrdenFabricacion { get; set; } = 0;

        public Programa[] ItemsABuscar = null;

        public PedCli[] PedidosABuscar = null;

        public Pedidos[] DespachosABuscar = null;

        public MainLayout Layout { get; set; }


        public async Task refrescagrid()
        {
            try
            {
                listanoconf = await Http.GetFromJsonAsync<List<NoConformidadesQuery>>("api/NoConformidades");
            }
            catch (Exception e)
            {
                //return BadRequest(e);
            }
        }

        // funciones form
        public async Task Limpiadatos()
        {
            cg_cli = 0;
            des_cli = "";
            cg_prove = 0;
            des_prove = "";
            cg_prod = "";
            des_prod = "";
            cantidad = 0;
            lote = "";
            serie = "";
            despacho = "";

            datosnoconfor1 = "";
            comentarios = "";

            datosdespacho = new List<ListaDespachos>();
            datoslote = new List<ListaLotes>();

            textodatos = "";
            ocultadivtextodatos = true;
            ocultadivdatosgenerales = true;

            fechaocurr = fechahoy;
            ocultacheckfechacierre = false;

        }

        public async Task HaceCheckCierra()
        {
            if (checkcierra == true)
            {
                ocultafechacierre = false;
                fechacierre = fechahoy;
            }
            else
            {
                ocultafechacierre = true;
            }
        }

        public async Task guardanoconfor()
        {

            
            NoConf.Cg_NoConf = ValorCg_noconf;

            NoConf.Cg_TipoNc = @DropVal;
            NoConf.Orden = 1;

            NoConf.Observaciones = @datosnoconfor1;
            NoConf.Comentarios = comentarios;

            if (despacho == null)
            {
                despacho = "";
            }
            if (lote == null)
            {
                lote = "";
            }

            NoConf.Aprob = false;
            NoConf.Cg_Cli = cg_cli;
            NoConf.Cg_Prod = cg_prod;
            // iria el cg_orden de prod que no estoy trayendo. Ver si vale la pena completar
            NoConf.Cg_Orden = 0;
            NoConf.Lote = lote;
            NoConf.Serie = serie;
            NoConf.Despacho = despacho;
            NoConf.Cg_Ordf = @ValorOF;
            NoConf.Pedido = @ValorPedido;
            NoConf.Cg_Cia = 1;
            NoConf.Usuario = "USER";
            NoConf.CG_PROVE = cg_prove;
            NoConf.OCOMPRA = @ValorOC;
            NoConf.CANT = cantidadnoconf;
            NoConf.NOCONF = false;
            NoConf.DES_CLI = des_cli;
            NoConf.DES_PROVE = des_prove;

            NoConf.FE_EMIT = fechahoy;
            NoConf.FE_PREV = null;
            NoConf.FE_SOLUC = null;

            NoConf.Fe_Ocurrencia = fechaocurr;
            if (fechaimplemen.Year == 1)
            {
                NoConf.fe_implemen = null;
            }
            else
            {
                NoConf.fe_implemen = fechaimplemen;
            }

            if (fechacierre.Year == 1)
            {
                NoConf.fe_cierre = null;
            }
            else
            {
                NoConf.fe_cierre = fechacierre;
            }

            NoConf.Fe_Aprobacion = null;


            HttpResponseMessage response = null;
            if (NoConf.Cg_NoConf == 0)
            {
                response = await Http.PostAsJsonAsync("api/NoConformidades", NoConf);
            }
            else
            {
                var registro = NoConf.Cg_NoConf;
                response = await Http.PutAsJsonAsync("api/NoConformidades/"+registro, NoConf);

            }
            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest
                || response.StatusCode == System.Net.HttpStatusCode.NotFound
                || response.StatusCode == System.Net.HttpStatusCode.Conflict)
            {
                var mensServidor = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"Error: {mensServidor}");
                //toastService.ShowToast($"{mensServidor}", TipoAlerta.Error);
                await this.ToastObj.ShowAsync(new ToastModel
                {
                    Title = "ERROR!",
                    Content = "Error al guardar",
                    CssClass = "e-toast-danger",
                    Icon = "e-error toast-icons",
                    ShowCloseButton = true,
                    ShowProgressBar = true
                });
            }
            else
            {
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {

                    await this.ToastObj.ShowAsync(new ToastModel
                    {
                        Title = "EXITO!",
                        Content = "Evento Guardado Correctamente.",
                        CssClass = "e-toast-success",
                        Icon = "e-success toast-icons",
                        ShowCloseButton = false,
                        ShowProgressBar = false
                    });
                    if ( vieneOF == 0)
                    {
                        await refrescagrid();

                        ocultadivlista = false;
                        ocultadivform = true;
                        ocultabotvolver = true;
                    }

                }
            }
        }


        public async Task BuscarxOF()
        {

            if (ValorOF == 0)
            {
                await this.ToastObj.ShowAsync(new ToastModel { Title = "AVISO!", Content = "Debe indicar una Orden de Fabricación ", CssClass = "e-toast-warning", Icon = "e-warning toast-icons" });
            }
            else
            {
                Limpiadatos();

                var ItemsABuscar = await Http.GetFromJsonAsync<Programa[]>($"api/Programa/GetProgramaByOF/" + ValorOF);

                if (ItemsABuscar is null || ItemsABuscar.Count() < 1)
                {
                    await this.ToastObj.ShowAsync(new ToastModel { Title = "AVISO!", Content = "No se encuentra la Orden de Fabricación ", CssClass = "e-toast-warning", Icon = "e-warning toast-icons" });

                }
                else
                {
                    cg_prod = ItemsABuscar[0].CG_PROD;
                    des_prod = ItemsABuscar[0].DES_PROD;
                    cantidad = ItemsABuscar[0].CANT;
                    //fecha = ItemsABuscar[0].FE_EMIT;

                    textodatos = "Producto/SemiElaborado: " + cg_prod + " " + des_prod;

                    ocultadivtextodatos = false;
                    ocultadivdatosgenerales = false;

                    var DespachosABuscar = await Http.GetFromJsonAsync<ModeloPedidosDespacho[]>($"api/NoConformidades/GetDespachosByOF/" + ValorOF);

                    datosdespacho = new List<ListaDespachos>();
                    datoslote = new List<ListaLotes>();

                    foreach (var itemDespacho in DespachosABuscar)
                    {
                        datosdespacho.Add(new ListaDespachos() { despachocombo = itemDespacho.DESPACHO });
                    }

                    var LotesABuscar = await Http.GetFromJsonAsync<ModeloPedidosLote[]>($"api/NoConformidades/GetLotesByOF/" + ValorOF);
                    foreach (var itemLote in LotesABuscar)
                    {
                        datoslote.Add(new ListaLotes() { lotecombo = itemLote.LOTE });
                    }
                }

                /*
                await Buscador.ShowAsync();
                //ItemsABuscar = await Http.GetFromJsonAsync<Programa[]>("api/Programas");
                ItemsABuscar = await Http.GetFromJsonAsync<Programa[]>("api/Programa/GetPlaneadas");
                await InvokeAsync(StateHasChanged);
                */
            }
        }

        public async Task BuscarxOC()
        {

            if (@ValorOC == 0)
            {
                await this.ToastObj.ShowAsync(new ToastModel { Title = "AVISO!", Content = "Debe indicar una Orden de Compra ", CssClass = "e-toast-warning", Icon = "e-warning toast-icons" });
            }
            else
            {
                Limpiadatos();
                var ItemsABuscar = await Http.GetFromJsonAsync<Compra[]>($"api/NoConformidades/GetOrdenCompra/" + ValorOC);

                if (ItemsABuscar is null || ItemsABuscar.Count() < 1)
                {
                    await this.ToastObj.ShowAsync(new ToastModel { Title = "AVISO!", Content = "No se encuentra la Orden de Compra ", CssClass = "e-toast-warning", Icon = "e-warning toast-icons" });
                }
                else
                {
                    cg_prod = ItemsABuscar[0].CG_MAT;
                    des_prod = ItemsABuscar[0].DES_MAT;
                    cg_prove = ItemsABuscar[0].NROCLTE;
                    des_prove = ItemsABuscar[0].DES_PROVE;

                    textodatos = "Materia Prima: " + cg_prod + " " + des_prod;

                    ocultadivtextodatos = false;
                    ocultadivdatosgenerales = false;

                    var DespachosABuscar = await Http.GetFromJsonAsync<ModeloPedidosDespacho[]>($"api/NoConformidades/GetDespachosByOC/" + ValorOC);

                    datosdespacho = new List<ListaDespachos>();
                    datoslote = new List<ListaLotes>();

                    foreach (var itemDespacho in DespachosABuscar)
                    {
                        datosdespacho.Add(new ListaDespachos() { despachocombo = itemDespacho.DESPACHO });
                    }

                    var LotesABuscar = await Http.GetFromJsonAsync<ModeloPedidosLote[]>($"api/NoConformidades/GetLotesByOC/" + ValorOC);
                    foreach (var itemLote in LotesABuscar)
                    {
                        datoslote.Add(new ListaLotes() { lotecombo = itemLote.LOTE });
                    }


                }


            }
        }

        public async Task BuscarxPedido()
        {

            if (@ValorPedido == 0)
            {
                await this.ToastObj.ShowAsync(new ToastModel { Title = "AVISO!", Content = "Debe indicar un Pedido", CssClass = "e-toast-warning", Icon = "e-warning toast-icons" });
            }
            else
            {
                Limpiadatos();
                var PedidosABuscar = await Http.GetFromJsonAsync<PedCli[]>($"api/Pedcli/" + @ValorPedido);

                if (PedidosABuscar is null || PedidosABuscar.Count() < 1)
                {
                    await this.ToastObj.ShowAsync(new ToastModel { Title = "AVISO!", Content = "No se encuentra el Pedido", CssClass = "e-toast-warning", Icon = "e-warning toast-icons" });

                }
                else
                {
                    cg_prod = PedidosABuscar[0].CG_ART;
                    des_prod = PedidosABuscar[0].DES_ART;
                    cg_cli = PedidosABuscar[0].CG_CLI;
                    des_cli = PedidosABuscar[0].DES_CLI;

                    textodatos = "Cliente: " + PedidosABuscar[0].DES_CLI;

                    ocultadivtextodatos = false;
                    ocultadivdatosgenerales = false;

                    var DespachosABuscar = await Http.GetFromJsonAsync<ModeloPedidosDespacho[]>($"api/NoConformidades/GetDespachosByPedido/" + ValorPedido);

                    datosdespacho = new List<ListaDespachos>();
                    datoslote = new List<ListaLotes>();

                    foreach (var itemDespacho in DespachosABuscar)
                    {
                        datosdespacho.Add(new ListaDespachos() { despachocombo = itemDespacho.DESPACHO });
                    }

                    var LotesABuscar = await Http.GetFromJsonAsync<ModeloPedidosLote[]>($"api/NoConformidades/GetLotesByPedido/" + ValorPedido);
                    foreach (var itemLote in LotesABuscar)
                    {
                        datoslote.Add(new ListaLotes() { lotecombo = itemLote.LOTE });
                    }



                }

            }
        }


        //    SfRadioButton radiobtn;
        public void OnRadioChange(ChangeArgs<string> args)
        {
            Limpiadatos();

            ValorOF = 0;
            ValorOC = 0;
            ValorPedido = 0;

            if (args.Value == "pedido")
            {
                ocultadivof = true;
                ocultadivpedido = false;
                ocultadivocompra = true;
            }
            else if (args.Value == "orden")
            {
                ocultadivof = false;
                ocultadivpedido = true;
                ocultadivocompra = true;
            }
            else if (args.Value == "ocompra")
            {
                ocultadivof = true;
                ocultadivpedido = true;
                ocultadivocompra = false;
            }
            else if (args.Value == "sinvinculos")
            {
                ocultadivof = true;
                ocultadivpedido = true;
                ocultadivocompra = true;

                ocultadivdatosgenerales = false;

            }
        }
        TiposNoConf[] TiposNc = null;

        /*
        public override async Task OnInitializedAsync()
        {
            //Layout.Titulo = "No Conformidades";
            await base.OnInitializedAsync();
            TiposNc = await Http.GetFromJsonAsync<TiposNoConf[]>("api/TiposNoConf");
            //selectedTiposNcSalida = TiposNc.FirstOrDefault(d => d.Cg_TipoNc == 0);
        }

        */

        // variables y funciones lista
        public SfGrid<NoConformidadesQuery> Grid;
        public SfGrid<NoConformidadesAcciones> GridAcciones;
        public bool VisibleProperty { get; set; } = false;

        public bool Enabled = true;
        public bool Disabled = false;
        public bool Showgrid = true;

        public string textoitemgrid { get; set; } = "";
        public string observacionesmodal { get; set; } = "";
        public int tipoaccion { get; set; } = 0;

        public List<NoConformidadesQuery> listanoconf = new List<NoConformidadesQuery>();
        NoConformidadesAcciones NoConfAcciones = new NoConformidadesAcciones();
        NoConformidadesQuery seleccionconf = new NoConformidadesQuery();

        public List<NoConformidadesAcciones> listaaccionesgrilla = new List<NoConformidadesAcciones>();
        public List<NoConformidadesAcciones> AccionCgNoconfyOrden = new List<NoConformidadesAcciones>();

        public class ListaAcciones
        {
            public int Value { get; set; }
            public string Texto { get; set; }
        }
/*
        public List<ListaAcciones> ListaAccionesData = new List<ListaAcciones> {
        new ListaAcciones() {Texto= "Acción de Contensión", Value = 2},
        new ListaAcciones() {Texto= "Causa Raiz", Value = 3},
        new ListaAcciones() {Texto= "Acción Correctiva", Value = 4},
        new ListaAcciones() {Texto= "Responsable", Value = 5},
//        new ListaAcciones() {Texto= "Fecha de implementacion de la acción", Value = 6},
        new ListaAcciones() {Texto= "Verificación de la Acción Correctiva", Value = 7},
        new ListaAcciones() {Texto= "Efectividad", Value = 8}};
*/
        public List<NoConformidadesListaAcciones> ListaAccionesData = new List<NoConformidadesListaAcciones>();

        protected string CgString = "";
        protected string DesString = "";
        protected int CantidadMostrar = 100;

        protected bool IsVisibledialogacciones { get; set; } = false;

        protected DialogSettings DialogParams = new DialogSettings { MinHeight = "400px", Width = "500px" };

        protected List<Object> Toolbaritems = new List<Object>(){
        new ItemModel { Text = "Ver", TooltipText = "Ver", PrefixIcon = "e-edit", Id = "Ver" },
        new ItemModel { Type = ItemType.Separator },
        new ItemModel { Text = "Acciones", TooltipText = "Acciones", PrefixIcon = "e-copy", Id = "Acciones" },
        new ItemModel { Type = ItemType.Separator },
        new ItemModel { Text = "Imprimir Evento", TooltipText = "Imprimir Evento", PrefixIcon = "e-print", Id = "ImprimirConf" },
        new ItemModel { Type = ItemType.Separator },
        "Search",
        new ItemModel { Type = ItemType.Separator },
        "Print",
        new ItemModel { Type = ItemType.Separator },
        "ExcelExport"
        };

        protected override async Task OnInitializedAsync()
        {
            MainLayout.Titulo = "Eventos y No Conformidades";
            fechahoy = DateTime.Now.Date;

            ListaAccionesData = await Http.GetFromJsonAsync<List<NoConformidadesListaAcciones>>("api/NoConformidadesAcciones/GetListaAcciones/");

            if ( vieneOF == 0)
            {

                listanoconf = await Http.GetFromJsonAsync<List<NoConformidadesQuery>>("api/NoConformidades");
                TiposNc = await Http.GetFromJsonAsync<TiposNoConf[]>("api/TiposNoConf");

                //await Grid.AutoFitColumns();
            }
            else
            {
               deshabradio = true;
               ocultadivAcciones = true;
               ocultadivlista = true;
               ocultadivform = false;
               ocultabotvolver = true;
               OFSoloLectura = true;
               DeshabilitaBotonOF = true;

               valorradio = "orden";
               ocultadivof = false;
               ocultadivpedido = true;
               ocultadivocompra = true;

               ocultadivtextodatos = false;
               ocultadivdatosgenerales = false;
               ValorCg_noconf = 0;
               ValorOF = vieneOF;
               ValorOC = 0;
               ValorPedido = 0;

               await BuscarxOF();
               textodatos = "Edicion Producto/SemiElaborado: " + cg_prod + " " + des_prod;

               DropVal = 0;
               despacho = "";
               lote = "";
               cantidadnoconf = 0;
               datosnoconfor1 = "";
            }
            await base.OnInitializedAsync();

        }
        public async Task OnLoad()                        //Show the spinner using initial Grid load 
        {
         //   await SpinnerObj.ShowAsync();
        }
        public async Task DataBound()                      //Hide the spinner after the data is bound to Grid(during data operations also this will be triggered) 
        {
            //await this.Grid.AutoFitColumns();
        }

        public async Task ClickHandler(ClickEventArgs args)
        {
            if (args.Item.Text == "ExcelExport")
            {
                await this.Grid.ExcelExport();
            }
            /*
            if (args.Item.Text == "Print")
            {
                await this.Grid.Print();
            }*/
            if (args.Item.Text == "Imprimir Evento")
            {
                if (this.Grid.SelectedRecords.Count > 0)
                {
                    foreach (NoConformidadesQuery selectedRecord in this.Grid.SelectedRecords)
                    {
                        seleccionconf = selectedRecord;
                    }

                    await this.ImprimeConf(seleccionconf);
                }
            }
            if (args.Item.Text == "Acciones")
            {
                if (this.Grid.SelectedRecords.Count > 0)
                {
                    foreach (NoConformidadesQuery selectedRecord in this.Grid.SelectedRecords)
                    {
                        seleccionconf = selectedRecord;
                        textoitemgrid = "Acciones Para Evento Nro. " + selectedRecord.Cg_NoConf;

                        tipoaccion = 0;
                        observacionesmodal = "";
                        fechaaccion = fechahoy;

                    }

                    IsVisibledialogacciones = true;
                }

            }

            if (args.Item.Text == "Ver")
            {
                if (this.Grid.SelectedRecords.Count > 0)
                {
                    foreach (NoConformidadesQuery selectedRecord in this.Grid.SelectedRecords)
                    {
                        seleccionconf = selectedRecord;
                        valorradio = "sinvinculos";
                        ocultadivof = true;
                        ocultadivpedido = true;
                        ocultadivocompra = true;
                        ocultadivtextodatos = true;
                        ocultadivdatosgenerales = false;
                        ValorCg_noconf = selectedRecord.Cg_NoConf;
                        ValorOF = 0;
                        ValorOC = 0;
                        ValorPedido = 0;
                        if (selectedRecord.fe_implemen == null)
                        {
                            fechaimplemen = Convert.ToDateTime("01-01-0001");
                        }
                        else
                        {
                            fechaimplemen = Convert.ToDateTime(selectedRecord.fe_implemen);
                        }
                        if (selectedRecord.fe_cierre == null)
                        {
                            fechacierre = Convert.ToDateTime("01-01-0001");
                        }
                        else
                        {
                            fechacierre = Convert.ToDateTime(selectedRecord.fe_cierre);
                            ocultacheckfechacierre = true;
                            ocultafechacierre = false;
                            desfechacierre = true;
                        }

                        if ( selectedRecord.Cg_Ordf > 0)
                        {
                            ValorOF = selectedRecord.Cg_Ordf;
                            await BuscarxOF();
                            textodatos = "Edicion Producto/SemiElaborado: " + cg_prod + " " + des_prod;
                            valorradio = "orden";
                            ocultadivof = false;
                            ocultadivtextodatos = false;

                            OFSoloLectura = true;
                            DeshabilitaBotonOF = true;

                        }
                        if (selectedRecord.Pedido > 0)
                        {
                            ValorPedido = selectedRecord.Pedido;
                            await BuscarxPedido();
                            //textodatos = "Edicion Producto/SemiElaborado: " + cg_prod + " " + des_prod;
                            valorradio = "pedido";
                            ocultadivpedido = false;
                            ocultadivtextodatos = false;

                            PedidoSoloLectura = true;
                            DeshabilitaBotonPedido = true;
                        }
                        if (selectedRecord.OCOMPRA > 0)
                        {
                            ValorOC = selectedRecord.OCOMPRA;
                            await BuscarxOC();
                            valorradio = "ocompra";
                            ocultadivocompra = false;
                            ocultadivtextodatos = false;

                            OCSoloLectura = true;
                            DeshabilitaBotonOC = true;
                        }

                        DropVal = selectedRecord.Cg_TipoNc;
                        despacho = selectedRecord.Despacho;
                        fechaocurr = (DateTime)selectedRecord.Fe_Ocurrencia;
                        lote = selectedRecord.Lote;
                        cantidadnoconf = selectedRecord.CANT;
                        datosnoconfor1 = selectedRecord.Observaciones;
                        comentarios = selectedRecord.Comentarios;
                    }
                    // busca acciones
                    listaaccionesgrilla = await Http.GetFromJsonAsync<List<NoConformidadesAcciones>>($"api/NoConformidades/GetAccionesByEvento/" + ValorCg_noconf);

                    deshabradio = true;
                    ocultadivAcciones = false;
                    ocultadivlista = true;
                    ocultadivform = false;
                    ocultabotvolver = false;

                }
            }

        }
        /*
        public void RefrescaLista()
        {
           this.Grid.Refresh();
        }
        */
        //        public async Task guardaaccion()
        protected async Task ImprimeConf(NoConformidadesQuery registro)
        {
            Console.WriteLine(registro);
//            document1.PageSettings.Size = new Syncfusion.Drawing.SizeF(359, 94);
//            document1.PageSettings.Margins.All = 0;

            //Create a new PDF document.
            PdfDocument document = new PdfDocument();
            //Add a page to the document.
            PdfPage page = document.Pages.Add();
            //Create PDF graphics for the page.
            PdfGraphics graphics = page.Graphics;
            //Set the standard font.
            PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 20);
            //Draw the text.
            //            graphics.DrawString("Evento: " + registro.Cg_NoConf.ToString(), font, PdfBrushes.Black, new Syncfusion.Drawing.PointF(0, 0));
            //          graphics.DrawString($"evento1", font, PdfBrushes.Black, new Syncfusion.Drawing.PointF(0, 0));

            var fechapdf = registro.Fe_Ocurrencia.ToString();
            fechapdf = fechapdf.Substring(0, 10);

            var fechaimplemenpdf = "";
            var fechacierre = "";
            if (registro.fe_implemen.HasValue)
            {
                fechaimplemenpdf = registro.fe_implemen.ToString();
                fechaimplemenpdf = fechaimplemenpdf.Substring(0, 10);
            }
            if (registro.fe_cierre.HasValue)
            {
                fechacierre = registro.fe_cierre.ToString();
                fechacierre = fechacierre.Substring(0, 10);
            }

            graphics.DrawString($"EVENTO: {registro.Cg_NoConf.ToString()}                   Fecha: {fechapdf} \r\n \r\n \r\n " +
                $"Fecha Implementación: {fechaimplemenpdf} \r\n \r\n " +
                $"Fecha Cierre: {fechacierre} \r\n " +
                $"Descripcion: {registro.Observaciones} \r\n \r\n " +
                $"{registro.Comentarios}" , font, PdfBrushes.Black, new Syncfusion.Drawing.PointF(0, 0));

            PdfGrid pdfGrid = new PdfGrid();
            listaaccionesgrilla = await Http.GetFromJsonAsync<List<NoConformidadesAcciones>>($"api/NoConformidades/GetAccionesByEvento/" + registro.Cg_NoConf);

            pdfGrid.DataSource = listaaccionesgrilla;
            //Create string format for PdfGrid
            PdfStringFormat format = new PdfStringFormat();
            format.Alignment = PdfTextAlignment.Center;
            format.LineAlignment = PdfVerticalAlignment.Bottom;
            //Assign string format for each column in PdfGrid
            foreach (PdfGridColumn column in pdfGrid.Columns)
                column.Format = format;
            //Apply a built-in style
            pdfGrid.ApplyBuiltinStyle(PdfGridBuiltinStyle.GridTable4Accent6);
            //Set properties to paginate the grid
            PdfGridLayoutFormat layoutFormat = new PdfGridLayoutFormat();
            layoutFormat.Break = PdfLayoutBreakType.FitPage;
            layoutFormat.Layout = PdfLayoutType.Paginate;
            //Draw grid to the page of PDF document
            pdfGrid.Draw(page, new Syncfusion.Drawing.PointF(0, 200), layoutFormat);

            /*
            graphics.DrawString($"\r\n\r\n\r\n\r\n\r\n\r\n    {PedCliList.Where(t => t.PEDIDO == ordenFabricacion.PEDIDO).OrderByDescending(t => t.PEDIDO).FirstOrDefault().LOTE}{espaciosPedidox}{ordenFabricacion.PEDIDO} " +
                        $"\r\n\r\n         {ordenFabricacion.CG_PROD} {espaciosAnio}     {DateTime.Now.Year} " +
            */
            /*
            //Save the document.
            document.Save("Output.pdf");
            //Close the document.
            document.Close(true);*/
            MemoryStream xx = new MemoryStream();
            document.Save(xx);
            document.Close(true);
            //await JS.SaveAs("Evento" + registro.Cg_NoConf.ToString() + ".pdf", xx.ToArray());

            await JS.InvokeVoidAsync("open", new object[2] { $"/api/ReportRDLC/GetReportEvento?noConf={registro.Cg_NoConf}", "_blank" });
        }

        public async Task guardaaccion()
        {

            NoConfAcciones.Cg_NoConf = seleccionconf.Cg_NoConf;
            NoConfAcciones.Orden = tipoaccion;
            NoConfAcciones.Observaciones = observacionesmodal;
            //            NoConfAcciones.Fe_Ocurrencia = DateTime.Now.Date;
            NoConfAcciones.Fe_Ocurrencia = fechaaccion;
            NoConfAcciones.Usuario = "USER";


            // se fija si existe una accion para esta no conformidad y tipoaccion
            AccionCgNoconfyOrden = await Http.GetFromJsonAsync<List<NoConformidadesAcciones>>($"api/NoConformidadesAcciones/GetAccionxCgNoConf/" + seleccionconf.Cg_NoConf +"/" + tipoaccion);
            if (AccionCgNoconfyOrden.Count > 0)
            {
                foreach (var item in AccionCgNoconfyOrden)
                {
                    await Http.DeleteAsync($"api/NoConformidadesAcciones/"+item.Cg_NoConfAcc);
                    
                }


            }
            ValidationContext valContext = new ValidationContext(NoConfAcciones, null, null);
            var validationsResults = new List<ValidationResult>();
            bool correct = Validator.TryValidateObject(NoConfAcciones, valContext, validationsResults, true);

            if (!correct)
            {
                await this.ToastObj.ShowAsync(new ToastModel
                {
                    Title = "Errores de Validacion",
                    Content = validationsResults[0].ErrorMessage,
                    CssClass = "e-toast-danger",
                    Icon = "e-error toast-icons",
                    ShowCloseButton = true,
                    ShowProgressBar = true
                });
            }
            else
            {

                HttpResponseMessage response = null;

                response = await Http.PostAsJsonAsync("api/NoConformidadesAcciones", NoConfAcciones);

                if (response.StatusCode == System.Net.HttpStatusCode.BadRequest
                    || response.StatusCode == System.Net.HttpStatusCode.NotFound
                    || response.StatusCode == System.Net.HttpStatusCode.Conflict)
                {
                    var mensServidor = await response.Content.ReadAsStringAsync();
                    IsVisibledialogacciones = false;

                    Console.WriteLine($"Error: {mensServidor}");
                    //toastService.ShowToast($"{mensServidor}", TipoAlerta.Error);
                    await this.ToastObj.ShowAsync(new ToastModel
                    {
                        Title = "ERROR!",
                        Content = "Error al guardar",
                        CssClass = "e-toast-danger",
                        Icon = "e-error toast-icons",
                        ShowCloseButton = true,
                        ShowProgressBar = true
                    });
                }
                else
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        IsVisibledialogacciones = false;
                        await this.ToastObj.ShowAsync(new ToastModel
                        {
                            Title = "EXITO!",
                            Content = "Accion Guardada Correctamente.",
                            CssClass = "e-toast-success",
                            Icon = "e-success toast-icons",
                            ShowCloseButton = false,
                            ShowProgressBar = false
                        });
                    }
                }
            }

        }


    }
}
