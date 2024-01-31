using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SupplyChain.Client.HelperService;
using SupplyChain.Client.RepositoryHttp;
using SupplyChain.Client.Shared;
using SupplyChain.Shared.Models;
using Syncfusion.Blazor.Lists;
using Syncfusion.Blazor.Notifications;
using Syncfusion.Blazor.Spinner;
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
using System.Threading.Tasks;
using SupplyChain.Shared.CDM;
using SupplyChain.Shared.Context;
using Syncfusion.Blazor.Schedule;


namespace SupplyChain.Client.Pages.PCP.Carga_de_Maquina
{
    public class CargaMaquinaCopiaBase : ComponentBase
    {
        [Inject] IJSRuntime JSRuntime { get; set; }
        [Inject] protected IJSRuntime JS { get; set; }
        [Inject] public PdfService PdfService { get; set; }
        [Inject] public HttpClient Http { get; set; }
        [Inject] public IRepositoryHttp Http2 { get; set; }
        [Inject] public NavigationManager NavigationManager { get; set; }
        protected bool verHojaRuta = false;
        protected bool verMP = true;
        protected bool verSE = true;
        protected ConfirmacionDialog confirmacionDescargarPlanoDialog;
        public DateTime ProjectStart = new DateTime(2019, 3, 25);
        public DateTime ProjectEnd = new DateTime(2019, 7, 28);
        protected int extensionDias = 365;
        protected int totalDiasNoLaborables = 0;
        protected int CantidadColumnasPorPeriodo = 8;
        protected int xHora = 1;
        protected int xColumna;
        protected string xCg_celda;
        protected int xColumnaInicialBarra;
        protected bool isEventoDialogVisible = false;
        protected bool isOrdenDialogVisible = false;
        protected bool isScrapDialogVisible = false;
        protected bool sabadosLaborables = false;
        protected bool domingosLaborables = false;
        protected SfToast ToastObj;
        protected string ordenNumero = "0";
        protected string ordenTitulo = "";
        protected ModeloOrdenFabricacion ordenFabricacion;
        protected ModeloOrdenFabricacion ordenFabricacionOriginal;
        protected ModeloOrdenFabricacionEncabezado ordenFabricacionEncabezado;
        protected List<ModeloOrdenFabricacionMP> ordenFabricacionMP;
        protected List<ModeloOrdenFabricacionSE> ordenFabricacionSE;
        protected List<ModeloOrdenFabricacionHojaRuta> ordenFabricacionHojaRuta;
        protected List<ModeloGenericoIntString> dbOrdenesDependientes;
        protected List<ModeloGenericoIntString> dbEstadoCarga;
        protected List<ModeloGenericoStringString> dbCeldas;
        protected List<ModeloGenericoStringString> dbProcesos;
        protected List<ModeloGenericoStringString> dbDiasFestivos;
        protected List<ModeloGenericoIntString> dbScrap;
        protected int? scrapSeleccionado;
        protected string Usuario = "USER";
        protected int OrdenDeFabAlta;
        public SfListView<ModeloGenericoIntString> listViewScrap;
        protected string scrapSeleccionadoMensaje { get; set; } = "";
        protected IEnumerable<Operario> operariosBE3;
        protected List<Solution> rutas;

        protected List<Operario> operariosList = new List<Operario>();
        protected List<PedCli> PedCliList = new List<PedCli>();
        protected Producto prodList = new();
        protected SfSpinner SpinnerCDM;
        protected bool Visible = false;
        protected string MensajeCargando = "Cargando...";

        protected List<Operario> operarios = new List<Operario>();
        protected List<PLANNER> ordenesIA = new List<PLANNER>();
        private DateTime? projectStart = null;
        private DateTime? projectEnd = null;
        protected int CurrentYear;
        protected List<ResourceData> ResourceDatasource = new List<ResourceData>();
        protected List<EventData> ordenesData = new List<EventData>();
        protected List<String> celdasList = new List<string>();
        protected string[] groupData = { "Resources" };
        protected DateTime CurrentDate { get; set; }
        protected View CurrentView { get; set; } = View.TimelineDay;
        protected SfSchedule<EventData> scheduleObj;


        public void OpenExternalLink()
        {
            string url = "http://192.168.0.247:8080/aerre/index.html";
            if (!string.IsNullOrEmpty(url))
                JSRuntime.InvokeAsync<object>("open", url, "_blank");
        }

        protected override async Task OnInitializedAsync()
        {
            Visible = true;

            dbEstadoCarga = new List<ModeloGenericoIntString>();
            dbEstadoCarga.Add(new ModeloGenericoIntString() { ID = 2, TEXTO = "FIRME" });
            dbEstadoCarga.Add(new ModeloGenericoIntString() { ID = 3, TEXTO = "EN CURSO" });
            dbEstadoCarga.Add(new ModeloGenericoIntString() { ID = 4, TEXTO = "CERRADA" });
            dbEstadoCarga.Add(new ModeloGenericoIntString() { ID = 5, TEXTO = "ANULADA" });

            operariosList = await Http.GetFromJsonAsync<List<Operario>>("api/Operario");

            operariosBE3 = from operariosBE3 in (IEnumerable<Operario>)operariosList
                where operariosBE3.CG_OPER == 51 || operariosBE3.CG_OPER == 131 || operariosBE3.CG_OPER == 135 ||
                      operariosBE3.CG_OPER == 139 || operariosBE3.CG_OPER == 144
                select operariosBE3;

            celdasList = await Http.GetFromJsonAsync<List<String>>("api/CargasIA/GetCeldas");

            await Refrescar();

            Visible = false;
        }

        protected async Task Refrescar()
        {
            try
            {
                Visible = true;

                ResourceDatasource = await GenerateResourceData();

                ordenesData = await GenerateEvents();

                // turno
                List<ModeloGenericoIntString> xTurno = await Http.GetFromJsonAsync<List<ModeloGenericoIntString>>(
                    "api/ModelosGenericosIntString/Select Top 1 CONVERT(INT, ValorN) ID, '' TEXTO From Solution Where Campo = 'HORASDIA'");
                CantidadColumnasPorPeriodo = xTurno.FirstOrDefault().ID;
                // Dias de calendario
                List<ModeloGenericoIntString> xDias = await Http.GetFromJsonAsync<List<ModeloGenericoIntString>>(
                    "api/ModelosGenericosIntString/Select Top 1 CONVERT(INT, ValorN) ID, '' TEXTO From Solution Where Campo = 'DIASCARGA'");
                extensionDias = xDias.FirstOrDefault().ID;
                // Sabados laborables
                List<ModeloGenericoIntString> xSabadosLaborables =
                    await Http.GetFromJsonAsync<List<ModeloGenericoIntString>>(
                        "api/ModelosGenericosIntString/Select Top 1 CONVERT(INT, ValorN) ID, '' TEXTO From Solution Where Campo = 'SABADOSLABORABLES'");
                sabadosLaborables = (xSabadosLaborables.FirstOrDefault().ID == 1) ? true : false;
                // Domingos laborables
                List<ModeloGenericoIntString> xDomingosLaborables =
                    await Http.GetFromJsonAsync<List<ModeloGenericoIntString>>(
                        "api/ModelosGenericosIntString/Select Top 1 CONVERT(INT, ValorN) ID, '' TEXTO From Solution Where Campo = 'DOMINGOSLABORABLES'");
                domingosLaborables = (xDomingosLaborables.FirstOrDefault().ID == 1) ? true : false;
                // Dias laborables
                dbDiasFestivos = await Http.GetFromJsonAsync<List<ModeloGenericoStringString>>(
                    "api/ModelosGenericosStringString/select distinct convert(char(8), Fecha, 112) ID, '' TEXTO from CalendarioFestivos");
                //Busca Scrap
                dbScrap = await Http.GetFromJsonAsync<List<ModeloGenericoIntString>>(
                    "api/ModelosGenericosIntString/SELECT convert(int, cg_scrap) ID, des_scrap TEXTO FROM scrap ORDER BY Cg_scrap");

                Visible = false;
            }
            catch
            {
                throw;
            }
        }

        protected async Task OrdenFabricacionOpen(EventClickArgs<EventData> args)
        {
            string xOrdenFabricacion = args.Event.CG_ORDF;
            bool xExigeOA = false;
            int xPedido = 0;
            string xCgProd = args.Event.CG_ORDF;
            string xCantidad = args.Event.cantidad;
            Visible = true;
            try
            {
                // Titulo
                if (xExigeOA)
                    ordenTitulo = "ORDEN DE ARMADO Nº " + xOrdenFabricacion;
                else
                    ordenTitulo = "ORDEN DE FABRICACIÓN Nº " + xOrdenFabricacion;

                if (xPedido > 0)
                    ordenTitulo += " - SERIE / PEDIDO Nº " + xPedido.ToString();

                // Datos de la orden
                ordenNumero = xOrdenFabricacion;
                ordenFabricacion =
                    await Http.GetFromJsonAsync<ModeloOrdenFabricacion>("api/OrdenesFabricacion/" + ordenNumero);
                ordenFabricacionOriginal =
                    Newtonsoft.Json.JsonConvert.DeserializeObject<ModeloOrdenFabricacion>(
                        Newtonsoft.Json.JsonConvert.SerializeObject(ordenFabricacion));
                // Ordenes dependientes
                string xSQLcommand = String.Format("SELECT 0 ID, CONVERT(varchar, 0) TEXTO " +
                                                   "UNION " +
                                                   "SELECT DISTINCT CG_ORDF ID, CONVERT(varchar, CG_ORDF) TEXTO FROM PROGRAMA WHERE CG_ORDFASOC = {0} AND CG_ORDF != {1}",
                    ordenFabricacion.CG_ORDFASOC,
                    ordenFabricacion.CG_ORDF);
                dbOrdenesDependientes =
                    await Http.GetFromJsonAsync<List<ModeloGenericoIntString>>("api/ModelosGenericosIntString/" +
                                                                               xSQLcommand);
                // Celdas
                xSQLcommand =
                    String.Format("SELECT ltrim(rtrim(CG_CELDA)) ID, DES_CELDA TEXTO FROM CELDAS ORDER BY CG_CELDA");
                dbCeldas = await Http.GetFromJsonAsync<List<ModeloGenericoStringString>>(
                    "api/ModelosGenericosStringString/" + xSQLcommand);
                // Procesos
                xSQLcommand = String.Format("SELECT PROCESO ID, DESCRIP TEXTO FROM PROTAB ORDER BY PROCESO");
                dbProcesos =
                    await Http.GetFromJsonAsync<List<ModeloGenericoStringString>>("api/ModelosGenericosStringString/" +
                        xSQLcommand);

                var cg_ordfAsoc = ordenesData.Where(c => c.CG_ORDF.ToString().Trim() == ordenNumero)
                    .MinBy(c => c.CG_ORDF).OFinicial;
                //Get Datos de la cantidad 
                //xSQLcommand = String.Format("select cantidad from programa where cg_ordfasoc={0}", ordenFabricacion.CG_ORDFASOC);
                //ordenFabxCantidad = await Http.GetFromJsonAsync<List<ModeloGenericoStringString>>("api/ModelosGenericosStringString/" + xSQLcommand);
                // Datos del encabezado del detalle
                ordenFabricacionEncabezado =
                    await Http.GetFromJsonAsync<ModeloOrdenFabricacionEncabezado>("api/OrdenesFabricacionEncabezado/" +
                        ordenNumero);
                // Materias primas
                ordenFabricacionMP =
                    await Http.GetFromJsonAsync<List<ModeloOrdenFabricacionMP>>("api/OrdenesFabricacionMP/" +
                        cg_ordfAsoc.ToString());
                // Semi elaborados
                ordenFabricacionSE =
                    await Http.GetFromJsonAsync<List<ModeloOrdenFabricacionSE>>("api/OrdenesFabricacionSE/" +
                        cg_ordfAsoc.ToString());
                // Semi elaborados
                ordenFabricacionHojaRuta = await Http
                    .GetFromJsonAsync<List<ModeloOrdenFabricacionHojaRuta>>(
                        $"api/OrdenesFabricacionHojaRuta/GetByFilter?CodigoProd={xCgProd}&Cantidad={xCantidad}");
                Visible = false;
                isOrdenDialogVisible = true;
            }
            catch
            {
                throw;
            }
        }

        protected void OrdenFabricacionClose(Object args)
        {
            try
            {
                isOrdenDialogVisible = false;
                ordenFabricacion = null;
            }
            catch
            {
                throw;
            }
        }

        protected async Task AbreEvento()
        {
            isOrdenDialogVisible = false;
            isEventoDialogVisible = true;
        }

        protected async Task DialogEventoClose(Object args)
        {
            isOrdenDialogVisible = true;
            isEventoDialogVisible = false;
        }

        protected async Task OrdenFabricacionOk(Object args)
        {
            Visible = true;

            isOrdenDialogVisible = false;
            var respuesta =
                await Http.PutAsJsonAsync("api/OrdenesFabricacion/" + ordenFabricacion.CG_ORDF, ordenFabricacion);
            if (respuesta.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                Console.WriteLine("Error en api/OrdenesFabricacion");

                var mensajeServidor = await respuesta.Content.ReadAsStringAsync();
                Console.WriteLine(mensajeServidor);
                await this.ToastObj.ShowAsync(new ToastModel
                {
                    Title = "ERROR!",
                    Content = $"Ocurrrio un error.Error al intentar Guardar OF: {ordenFabricacion.CG_ORDF} ",
                    CssClass = "e-toast-danger",
                    Icon = "e-error toast-icons",
                    ShowCloseButton = true,
                    ShowProgressBar = true
                });
            }

            if (ordenFabricacion.CG_ESTADOCARGA == 2 || ordenFabricacion.CG_ESTADOCARGA == 3
                && (ordenFabricacion.CG_ESTADOCARGA != ordenFabricacionOriginal.CG_ESTADOCARGA))
            {
                //actualizada en estado Firme o Curso: si es la primera actualiza el grupo completo.
                string sqlCommandString = string.Format("UPDATE Programa SET CG_ESTADOCARGA = {0}," +
                                                        "Fe_curso = GETDATE(), CG_ESTADO = {1} WHERE (Cg_ordf = {2} OR Cg_ordfAsoc = {2})",
                    ordenFabricacion.CG_ESTADOCARGA,
                    ordenFabricacionOriginal.CG_ESTADOCARGA,
                    ordenFabricacion.CG_ORDF);
                await Http.PutAsJsonAsync("api/SQLgenericCommandString/" + sqlCommandString, ordenFabricacion);

                ordenFabricacion = null;
                //await Refrescar();
            }
            else if (ordenFabricacion.CG_ESTADOCARGA == 3 && ordenFabricacion.CANTFAB > 0
                                                          && (ordenFabricacion.CG_ESTADOCARGA ==
                                                              ordenFabricacionOriginal.CG_ESTADOCARGA))
            {
                //var cantidadcero= await
                await Altaparcial();
                ordenFabricacion = null;
                await Refrescar();
            }
            else if (ordenFabricacion.CG_ESTADOCARGA == 4)
            {
                if (ordenFabricacion.CANTFAB <= 0)
                {
                    await this.ToastObj.ShowAsync(new ToastModel
                    {
                        Title = "AVISO!",
                        Content = "Debe ingresar la cantidad fabricada.",
                        CssClass = "e-toast-warning",
                        Icon = "e-warning toast-icons"
                    });
                    return;
                }


                var ordenesGrupo = await this.Http.GetFromJsonAsync<List<Programa>>(
                    $"api/Programa/GetOrdenesAbiertas/{ordenFabricacion.CG_ORDFASOC}/{ordenFabricacion.CG_ORDF}");
                //var ordenesGrupo = await this.Http.GetFromJsonAsync<List<Pedidos>>($"api/Programa/GetOrdenesAbiertas/{cg_ordgasoc}/{cg_ordf}");

                if (ordenesGrupo.Count >= 1 && ordenFabricacion != null)
                {
                    await this.ToastObj.ShowAsync(new ToastModel
                    {
                        Title = "AVISO!",
                        Content = "No se puede cerrar esta Orden. Existen ordenes anteriores abiertas.",
                        CssClass = "e-toast-waring",
                        Icon = "e-error toast-icons"
                    });

                    return;
                }

                //VERIFIFCAR QUE SE LA ULTIMA: LA QUE DA DE ALTA AL PRODUCTO
                var lOfAsocs = ordenesData.Where(c => c.OFinicial == ordenFabricacion.CG_ORDFASOC.ToString().Trim())
                    .OrderByDescending(o => o.CG_ORDF).ToList();
                var ultimaOF = lOfAsocs.Max(m => m.CG_ORDF);
                if (dbScrap != null && ordenFabricacion.CG_ORDF.ToString().Trim() == ultimaOF)
                {
                    isScrapDialogVisible = true;
                    StateHasChanged();
                }
                else
                {
                    await CerrarOrdenFabricacion();
                    await Refrescar();
                }
            }
            else if (ordenFabricacion.CG_ESTADOCARGA == 5)
            {
                string sqlCommandString = "EXEC NET_PCP_Anular_OrdenFabricacion " +
                                          ordenFabricacion.CG_ORDF.ToString() + ", '" + Usuario + "'";
                var respuesta2 = await Http.PutAsJsonAsync("api/SQLgenericCommandString/" + sqlCommandString,
                    ordenFabricacion);
                if (respuesta2.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    await this.ToastObj.ShowAsync(new ToastModel
                    {
                        Title = "ERROR!",
                        Content = $"Ocurrio un error. Error al intentar anular OF: {ordenFabricacion.CG_ORDF}",
                        CssClass = "e-toast-danger",
                        Icon = "e-error toast-icons",
                        ShowCloseButton = true,
                        ShowProgressBar = true
                    });
                }

                StateHasChanged();
                await this.ToastObj.ShowAsync(new ToastModel
                {
                    Title = "AVISO!",
                    Content = "Órden anulada.",
                    CssClass = "e-toast-warning",
                    Icon = "e-warning toast-icons"
                });
                ordenFabricacion = null;
                await Refrescar();
            }

            //await this.ToastObj.ShowAsync(new ToastModel
            //{
            //    Title = "Exito!",
            //    Content = $"Guardado Correctamente!  Se actualizo OF: {ordenFabricacion.CG_PROD.Trim()}",
            //    CssClass = "e-toast-success",
            //    Icon = "e-success toast-icons",
            //    ShowCloseButton = true,
            //    ShowProgressBar = true
            //});


            Visible = false;
        }

        protected void Scrap_Selection(ModeloGenericoIntString args)
        {
            scrapSeleccionadoMensaje = "";
            scrapSeleccionado = args.ID;
        }

        protected async Task DialogScrapClose(Object args)
        {
            try
            {
                isScrapDialogVisible = false;
                await CerrarOrdenFabricacion();
            }
            catch
            {
                throw;
            }
        }

        protected async Task DialogScrapOk(Object args)
        {
            try
            {
                if (scrapSeleccionado == null)
                {
                    scrapSeleccionadoMensaje = "No seleccionó ningún Item";
                }
                else
                {
                    isScrapDialogVisible = false;
                    await CerrarOrdenFabricacion();
                }
            }
            catch
            {
                throw;
            }
        }

        protected async Task Altaparcial()
        {
            Visible = true;

            string sqlCommandString = "EXEC NET_PCP_Altaparcial_OrdenFabricacion " +
                                      ordenFabricacion.CG_ORDF.ToString() + ", '" + Usuario + "'";
            var respuesta2 =
                await Http.PutAsJsonAsync("api/SQLgenericCommandString/" + sqlCommandString, ordenFabricacion);
            if (respuesta2.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                await this.ToastObj.ShowAsync(new ToastModel
                {
                    Title = "ERROR!",
                    Content = $"Ocurrio un error. Error al registrar el alta",
                    CssClass = "e-toast-danger",
                    Icon = "e-error toast-icons",
                    ShowCloseButton = true,
                    ShowProgressBar = true
                });
            }

            Visible = false;
        }

        protected async Task CerrarOrdenFabricacion()
        {
            Visible = true;
            scrapSeleccionado = scrapSeleccionado == null ? 0 : scrapSeleccionado;
            string sqlCommandString = "EXEC NET_PCP_Cerrar_OrdenFabricacion " + ordenFabricacion.CG_ORDF.ToString() +
                                      ", '" + Usuario + "', " + scrapSeleccionado.ToString();
            await Http.PutAsJsonAsync("api/SQLgenericCommandString/" + sqlCommandString, ordenFabricacion);
            if (ordenFabricacion.CG_ORDF == ordenFabricacion.ULTIMAORDENASOCIADA)
            {
                await this.ToastObj.ShowAsync(new ToastModel
                {
                    Title = "Exito!",
                    Content = $"Guardado Correctamente! Alta {ordenFabricacion.CG_PROD.Trim()}",
                    CssClass = "e-toast-success",
                    Icon = "e-success toast-icons",
                    ShowCloseButton = true,
                    ShowProgressBar = true
                });
            }
            else
            {
                await this.ToastObj.ShowAsync(new ToastModel
                {
                    Title = "Exito!",
                    Content = "Guardado Correctamente!\n" +
                              $"OF Cerrada {ordenFabricacion.CG_ORDF}",
                    CssClass = "e-toast-success",
                    Icon = "e-success toast-icons",
                    ShowCloseButton = true,
                    ShowProgressBar = true
                });
            }

            ordenFabricacion = null;
            await Refrescar();
            Visible = false;
            if (scrapSeleccionado > 0)
            {
                isEventoDialogVisible = true;
            }

            scrapSeleccionado = null;
        }

        protected async Task EstadoCarga_Change()
        {
            if (ordenFabricacionOriginal.CG_ESTADOCARGA == 0 && ordenFabricacion.CG_ESTADOCARGA == 2)
            {
                await this.ToastObj.ShowAsync(new ToastModel
                {
                    Title = "ERROR!",
                    Content = "No puede pasar una órden de fabricación EMITIDA a estado EN FIRME.",
                    CssClass = "e-toast-danger",
                    Icon = "e-error toast-icons"
                });
            }
            else if (ordenFabricacionOriginal.CG_ESTADOCARGA == 0 && ordenFabricacion.CG_ESTADOCARGA == 3)
            {
                await this.ToastObj.ShowAsync(new ToastModel
                {
                    Title = "ERROR!", Content = "No puede pasar una órden de fabricación EMITIDA a estado EN CURSO.",
                    CssClass = "e-toast-danger", Icon = "e-error toast-icons"
                });
            }
            else if (ordenFabricacionOriginal.CG_ESTADOCARGA == 1 && ordenFabricacion.CG_ESTADOCARGA == 3)
            {
                await this.ToastObj.ShowAsync(new ToastModel
                {
                    Title = "ERROR!", Content = "No puede pasar una órden de fabricación PLANEADA a estado EN CURSO.",
                    CssClass = "e-toast-danger", Icon = "e-error toast-icons"
                });
            }
        }

        protected async Task BuscarOrden_Click()
        {
            await Refrescar();
        }

        protected async Task DescrgarPlano()
        {
            var file = ordenFabricacion.CG_PROD.Substring(0, 7) + ".pdf";
            await confirmacionDescargarPlanoDialog.HideAsync();
            var fileArray = await Http.GetByteArrayAsync($"api/AdministracionArchivos/GetPlano/{file}/Load");

            await Task.Run(() => { JS.SaveAs(ordenFabricacion.CG_PROD.Substring(0, 7) + ".pdf", fileArray); });
        }

        protected async Task ConfirmaDescargarPlano()
        {
            var file = ordenFabricacion.CG_PROD.Substring(0, 7) + ".pdf";
            Console.WriteLine("Verificando existencia de archivo " + file);
            if (await ExistePlano(file))
            {
                await confirmacionDescargarPlanoDialog.ShowAsync();
            }
            else
            {
                await confirmacionDescargarPlanoDialog.HideAsync();
                await this.ToastObj.ShowAsync(new ToastModel
                {
                    Title = "ERROR!",
                    Content = "No existe plano",
                    CssClass = "e-toast-danger",
                    Icon = "e-success toast-icons",
                    ShowCloseButton = true,
                    ShowProgressBar = true
                });
            }
        }

        protected async Task<bool> ExistePlano(string file)
        {
            var response = await Http2.GetFromJsonAsync<bool>($"api/AdministracionArchivos/ExistePlano/{file}");
            if (response.Error)
            {
                Console.WriteLine(response.HttpResponseMessage.ReasonPhrase);
                return false;
            }

            return response.Response;
        }

        protected async Task VerPlano()
        {
            var response = await Http2.GetFromJsonAsync<Producto>($"api/Prod/{ordenFabricacion.CG_PROD.Trim()}");

            if (response.Error)
            {
            }
            else
            {
                var cg_prod = response.Response.Id;
                if (response.Response.CG_ORDEN == 1)
                {
                    cg_prod = response.Response.Id;
                }

                if (response.Response.CG_ORDEN == 3)
                {
                    cg_prod = response.Response.Id.Substring(0, 7);
                }

                var file = cg_prod + ".pdf";
                if (await ExistePlano(file))
                {
                    await JS.InvokeVoidAsync("open", $"Pdf/{cg_prod.Trim()}/RUTAOF", "_blank");
                }
                else
                {
                    await this.ToastObj.ShowAsync(new ToastModel
                    {
                        Title = "ERROR!",
                        Content = "No existe plano",
                        CssClass = "e-toast-danger",
                        Icon = "e-success toast-icons",
                        ShowCloseButton = true,
                        ShowProgressBar = true
                    });
                }
            }
        }

        protected async Task IrAServicio(string pedido)
        {
            //NavigationManager.NavigateTo($"sc/servicio/list/{pedido}");
            await JS.InvokeVoidAsync("open", new object[2] { $"servicio/list/{pedido}", $"servicio/list/{pedido}" });
        }

        protected async Task IrAPrograma(string cg_prod)
        {
            //Pdf/@(ordenFabricacion.CG_PROD)/RUTACNC

            //await JS.InvokeVoidAsync("open", new object[2] { $"Pdf/{cg_prod}/RUTACNC", $"Pdf/{cg_prod}/RUTACNC" });
            await JS.InvokeVoidAsync("open", new object[1] { "http://192.168.0.131:8080/autentio/dnc.html" });
        }

        protected async Task Ensayos(string pedido)
        {
            //NavigationManager.NavigateTo($"Pdf/{pedido}/ENSAYOS");

            await JS.InvokeVoidAsync("open", new object[2] { $"/pdf-ensayos/{pedido}/RUTAENSAYO", $"_blank" });
        }

        protected async Task DownloadText()
        {
            string presion = ordenFabricacionEncabezado.CAMPOCOM4.Trim();

            if (String.IsNullOrEmpty(presion))
            {
                presion = ordenFabricacionEncabezado.CAMPOCOM1.Trim();
            }

            presion = presion.Replace(',', '.');
            // Generate a text file
            //byte[] file;

            ordenFabricacion.Des_Cli = ordenFabricacionEncabezado.DES_CLI;
            ordenFabricacion.Presion = presion;
            if (String.IsNullOrEmpty(ordenFabricacion.DES_OPER))
            {
                await this.ToastObj.ShowAsync(new ToastModel
                {
                    Title = "AVISO!",
                    Content = "Antes de generar el archivo debe asignar un operario",
                    CssClass = "e-toast-danger",
                    Icon = "e-success toast-icons"
                });
            }
            else
            {
                //TODO: HACER DESDE CONTROLER ADMINISTRADOR DE ARCHIVOS
                var respuesta =
                    await Http.PostAsJsonAsync<ModeloOrdenFabricacion>("api/AdministracionArchivos/DownloadText",
                        ordenFabricacion);
                if (respuesta.StatusCode == System.Net.HttpStatusCode.BadRequest
                    || respuesta.StatusCode == System.Net.HttpStatusCode.NotFound
                    || respuesta.StatusCode == System.Net.HttpStatusCode.UnsupportedMediaType)
                {
                    await this.ToastObj.ShowAsync(new ToastModel
                    {
                        Title = "ERROR!",
                        Content = "Error al descargar archivo",
                        CssClass = "e-toast-danger",
                        Icon = "e-success toast-icons",
                        ShowCloseButton = true,
                        ShowProgressBar = true
                    });
                }
                else
                {
                    await this.ToastObj.ShowAsync(new ToastModel
                    {
                        Title = "EXITO!",
                        Content = "Archivo generado con éxito",
                        CssClass = "e-toast-success",
                        Icon = "e-success toast-icons",
                        ShowCloseButton = true,
                        ShowProgressBar = true
                    });
                }
            }
        }

        protected async Task Etiqueta()
        {
            PedCliList = await Http.GetFromJsonAsync<List<PedCli>>($"api/PedCli/ByPedido/{ordenFabricacion.PEDIDO}");

            prodList = await Http.GetFromJsonAsync<Producto>(
                $"api/Prod/GetByFilter?Codigo={ordenFabricacion.CG_PROD.Trim()}" +
                $"&Descripcion={string.Empty}");


            if (ordenFabricacion.CG_CELDA == "BE3" || ordenFabricacion.CG_CELDA == "GC1")
            {
                if (ordenFabricacion.CG_PROD.Substring(0, 1) == "2")
                {
                    await DescargarTxtParaImpresoraQR(PedCliList[0].PEDIDO, "Bridada");
                    //string espaciosPedido = "";
                    //string CLIENTE = PedCliList.Where(t => t.PEDIDO == ordenFabricacion.PEDIDO)
                    //    .OrderByDescending(t => t.PEDIDO).FirstOrDefault().DES_CLI.Trim();
                    //if (CLIENTE.Contains("YPF") || CLIENTE.Contains("Y.P.F"))
                    //{
                    //    espaciosPedido = await PdfService.EtiquetaYPF(espaciosPedido, PedCliList, ordenFabricacion, prodList);
                    //}
                    //else
                    //{
                    //    //TODO:HACER CSV Y GUARDAR CON EL NOMBRE  ID + PEDIDO

                    //    await DescargarTxtParaImpresoraQR(PedCliList[0].PEDIDO, "Bridada");
                    //    //await EtiquetaClientesNOypf();
                    //}
                }

                if (ordenFabricacion.CG_PROD.Substring(0, 1) == "1")
                {
                    await DescargarTxtParaImpresoraQR(ordenFabricacion.PEDIDO, "Roscada");
                    //await EtiquetaInicio1();
                }

                if (ordenFabricacion.CG_PROD.Substring(0, 4) == "0012" ||
                    ordenFabricacion.CG_PROD.Substring(0, 5) == "00130" ||
                    ordenFabricacion.CG_PROD.Substring(0, 5) == "00131")
                {
                    await DescargarTxtParaImpresoraQR(PedCliList[0].PEDIDO, "Reparacion");
                    //await EtiquetaReparaciones();
                }
            }
            else
            {
                //await JS.InvokeVoidAsync("open", new object[2] { $"/api/ReportRDLC/GetReportEtiquetaOF?cg_ordf={ordenFabricacion.CG_ORDF}", "_blank" });

                OrdenDeFabAlta = Convert.ToInt32(ordenesData
                    .Where(t => t.OFinicial == ordenFabricacion.CG_ORDFASOC.ToString().Trim()).MaxBy(t => t.CG_ORDF)
                    .CG_ORDF);
                await PdfService.EtiquetaOF(OrdenDeFabAlta, ordenFabricacion);
            }
        }

        private async Task EtiquetaReparaciones()
        {
            string espaciosbar = "";
            for (int i = 0;
                 i < (16 - PedCliList.Where(t => t.PEDIDO == ordenFabricacion.PEDIDO).OrderByDescending(t => t.PEDIDO)
                     .FirstOrDefault().LOTE.Trim().Length);
                 i++)
            {
                espaciosbar = espaciosbar + " ";
            }

            //Chapa de 31 x 78
            PdfDocument document1 = new PdfDocument();
            document1.PageSettings.Size = new Syncfusion.Drawing.SizeF(117, 295);
            document1.PageSettings.Orientation = PdfPageOrientation.Landscape;
            document1.PageSettings.Margins.All = 0;
            PdfGrid pdfGrid1 = new PdfGrid();
            PdfPage page = document1.Pages.Add();
            PdfGraphics graphics = page.Graphics;
            PdfFont font = new PdfStandardFont(PdfFontFamily.Courier, 16);
            PdfLightTable pdfTable = new PdfLightTable();

            string presionMostrar = PedCliList.Where(t => t.PEDIDO == ordenFabricacion.PEDIDO)
                .OrderByDescending(t => t.PEDIDO).FirstOrDefault().CAMPOCOM1.Trim();
            int found = presionMostrar.ToUpper().IndexOf("B");
            if (found == -1)
            {
                presionMostrar = PedCliList.Where(t => t.PEDIDO == ordenFabricacion.PEDIDO)
                    .OrderByDescending(t => t.PEDIDO).FirstOrDefault().CAMPOCOM1.Trim();
            }
            else
            {
                presionMostrar = presionMostrar.Substring(0, found);
            }

            graphics.DrawString(
                $"\"\r\n\r\n\r\n        {ordenFabricacion.PEDIDO}           {DateTime.Now.Month}/{DateTime.Now.Year} " +
                $"\r\n        {PedCliList.Where(t => t.PEDIDO == ordenFabricacion.PEDIDO).OrderByDescending(t => t.PEDIDO).FirstOrDefault().LOTE.Trim()}{espaciosbar}{presionMostrar}" +
                $"\r\n\r\n                              .", font, PdfBrushes.Black,
                new Syncfusion.Drawing.PointF(0, 0));

            MemoryStream xx = new MemoryStream();
            document1.Save(xx);
            document1.Close(true);
            await JS.SaveAs("ETOF" + ordenFabricacion.CG_PROD.Trim() + ".pdf", xx.ToArray());
        }

        private async Task EtiquetaInicio1()
        {
            //Chapa de 25 x 95
            PdfDocument document1 = new PdfDocument();
            document1.PageSettings.Size = new Syncfusion.Drawing.SizeF(359, 94);
            document1.PageSettings.Margins.All = 0;
            document1.PageSettings.Rotate = PdfPageRotateAngle.RotateAngle180;
            PdfGrid pdfGrid1 = new PdfGrid();
            PdfPage page = document1.Pages.Add();
            PdfGraphics graphics = page.Graphics;
            PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 12);
            PdfLightTable pdfTable = new PdfLightTable();
            page.Graphics.RotateTransform(-90);
            string espaciosTag = "";
            for (int i = 0; i < (20 - ordenFabricacion.PEDIDO.ToString().Length); i++)
            {
                espaciosTag = espaciosTag + " ";
            }

            string espaciosAnio = "";
            for (int i = 0;
                 i < (32 - PedCliList.Where(t => t.PEDIDO == ordenFabricacion.PEDIDO).OrderByDescending(t => t.PEDIDO)
                     .FirstOrDefault().LOTE.Trim().Length);
                 i++)
            {
                espaciosAnio = espaciosAnio + " ";
            }

            string espaciosOrif = "";
            for (int i = 0; i < (32 - prodList.CAMPOCOM2.Trim().Length); i++)
            {
                espaciosOrif = espaciosOrif + " ";
            }

            string espaciosClase = "";
            for (int i = 0;
                 i < (20 - PedCliList.Where(t => t.PEDIDO == ordenFabricacion.PEDIDO).OrderByDescending(t => t.PEDIDO)
                     .FirstOrDefault().CAMPOCOM4.Trim().Length);
                 i++)
            {
                espaciosClase = espaciosClase + " ";
            }

            string espaciosSinLote = "";
            for (int i = 0; i < 43; i++)
            {
                espaciosSinLote = espaciosSinLote + " ";
            }

            //graphics.DrawString($"\r\n\r\n\r\n                  {ordenFabricacion.PEDIDO}{espaciosTag}{PedCliList.Where(t => t.PEDIDO == ordenFabricacion.PEDIDO).OrderByDescending(t => t.PEDIDO).FirstOrDefault().LOTE.Trim()}{espaciosAnio}{DateTime.Now.Year} " +
            //    $"\r\n\r\n                        {ordenFabricacion.CG_PROD.Trim()}{espaciosMed}{prodList.Where(t => t.CG_PROD == ordenFabricacion.CG_PROD).OrderByDescending(t => t.CG_PROD).FirstOrDefault().CAMPOCOM2.Trim()}{espaciosOrif}{prodList.Where(t => t.CG_PROD.Trim() == ordenFabricacion.CG_PROD.Trim()).OrderByDescending(t => t.CG_PROD.Trim()).FirstOrDefault().CAMPOCOM3.Trim()} " +
            //    $"\r\n\r\n                                                                        {PedCliList.Where(t => t.PEDIDO == ordenFabricacion.PEDIDO).OrderByDescending(t => t.PEDIDO).FirstOrDefault().CAMPOCOM4.Trim()}{espaciosClase}{prodList.Where(t => t.CG_PROD.Trim() == ordenFabricacion.CG_PROD.Trim()).OrderByDescending(t => t.CG_PROD.Trim()).FirstOrDefault().CAMPOCOM5.Trim()} ", font, PdfBrushes.Black, new Syncfusion.Drawing.PointF(-359, 0));
            if (!String.IsNullOrEmpty(PedCliList.Where(t => t.PEDIDO == ordenFabricacion.PEDIDO)
                    .OrderByDescending(t => t.PEDIDO).FirstOrDefault().LOTE.Trim()))
            {
                graphics.DrawString(
                    $"\r\n\r\n                                     {ordenFabricacion.PEDIDO}{espaciosSinLote}{DateTime.Now.Year}     .  .  .  .  ." +
                    $"\r\n               {ordenFabricacion.CG_PROD.Trim()}          {prodList.CAMPOCOM2.Trim()}{espaciosOrif}{prodList.CAMPOCOM3.Trim()} " +
                    $"\r\n.                                            {PedCliList.Where(t => t.PEDIDO == ordenFabricacion.PEDIDO).OrderByDescending(t => t.PEDIDO).FirstOrDefault().CAMPOCOM4.Trim()}{espaciosClase}{prodList.CAMPOCOM5.Trim()}     .  .  .  .  .",
                    font, PdfBrushes.Black, new Syncfusion.Drawing.PointF(-359, 0));
            }
            else
            {
                graphics.DrawString(
                    $"                                                                                                           \"\r\n\r\n                                     {ordenFabricacion.PEDIDO}{espaciosTag}{PedCliList.Where(t => t.PEDIDO == ordenFabricacion.PEDIDO).OrderByDescending(t => t.PEDIDO).FirstOrDefault().LOTE.Trim()}{espaciosAnio}{DateTime.Now.Year}" +
                    $"\r\n               {ordenFabricacion.CG_PROD.Trim()}          {prodList.CAMPOCOM2.Trim()}{espaciosOrif}{prodList.CAMPOCOM3.Trim()} " +
                    $"\r\n                                             {PedCliList.Where(t => t.PEDIDO == ordenFabricacion.PEDIDO).OrderByDescending(t => t.PEDIDO).FirstOrDefault().CAMPOCOM4.Trim()}{espaciosClase}{prodList.CAMPOCOM5.Trim()}" +
                    $"\r\n\r\n._", font, PdfBrushes.Black, new Syncfusion.Drawing.PointF(-359, 0));
            }

            MemoryStream xx = new MemoryStream();
            document1.Save(xx);
            document1.Close(true);
            await JS.SaveAs("ETOF" + ordenFabricacion.CG_PROD.Trim() + ".pdf", xx.ToArray());
        }

        private async Task EtiquetaClientesNOypf()
        {
            string espaciosPedidox = "";
            string espaciosAnio = "";
            string espaciosSegundoCampo3bis = "";
            string espaciosSegundoCampo4bis = "";
            string espaciosSegundoCampo5bis = "";

            //Chapa de 101 mm x 78 mm
            PdfDocument document1 = new PdfDocument();
            document1.PageSettings.Size = new Syncfusion.Drawing.SizeF(382, 295);
            document1.PageSettings.Margins.All = 0;
            PdfGrid pdfGrid1 = new PdfGrid();
            PdfPage page = document1.Pages.Add();
            PdfGraphics graphics = page.Graphics;
            PdfFont font = new PdfStandardFont(PdfFontFamily.Courier, 16);
            PdfLightTable pdfTable = new PdfLightTable();
            page.Graphics.RotateTransform(-360);

            for (int i = 0;
                 i < (25 - PedCliList.Where(t => t.PEDIDO == ordenFabricacion.PEDIDO).OrderByDescending(t => t.PEDIDO)
                     .FirstOrDefault().LOTE.Length);
                 i++)
            {
                espaciosPedidox = espaciosPedidox + " ";
            }

            for (int i = 0; i < (16 - ordenFabricacion.CG_PROD.Length); i++)
            {
                espaciosAnio = espaciosAnio + " ";
            }

            for (int i = 0; i < (25 - prodList.CAMPOCOM5.Trim().Length); i++)
            {
                espaciosSegundoCampo3bis = espaciosSegundoCampo3bis + " ";
            }

            for (int i = 0;
                 i < (25 - PedCliList.Where(t => t.PEDIDO == ordenFabricacion.PEDIDO).OrderByDescending(t => t.PEDIDO)
                     .FirstOrDefault().CAMPOCOM3.Trim().Length);
                 i++)
            {
                espaciosSegundoCampo4bis = espaciosSegundoCampo4bis + " ";
            }

            for (int i = 0; i < (25 - prodList.CAMPOCOM5.Trim().Length); i++)
            {
                espaciosSegundoCampo5bis = espaciosSegundoCampo5bis + " ";
            }

            string xd1 = prodList.CAMPOCOM2.Trim();
            string xd2 = prodList.CAMPOCOM5.Trim();
            int UbicacionXMedida = xd1.ToLower().IndexOf('x');
            int UbicacionXMedida2 = xd2.ToLower().IndexOf('x');

            string primeramedida1 = xd1.Substring(0, UbicacionXMedida);
            string segundamedida1 = xd1.Substring(UbicacionXMedida + 1);
            string primeramedida2 = xd2.Substring(0, UbicacionXMedida2);
            string segundamedida2 = xd2.Substring(UbicacionXMedida2 + 1);

            graphics.DrawString(
                $"\r\n\r\n\r\n\r\n\r\n\r\n    {PedCliList.Where(t => t.PEDIDO == ordenFabricacion.PEDIDO).OrderByDescending(t => t.PEDIDO).FirstOrDefault().LOTE}{espaciosPedidox}{ordenFabricacion.PEDIDO} " +
                $"\r\n\r\n         {ordenFabricacion.CG_PROD} {espaciosAnio}     {DateTime.Now.Year} " +
                $"\r\n             {primeramedida1}                {segundamedida1}" +
                $"\r\n\r\n    {primeramedida2}   {segundamedida2}        {prodList.CAMPOCOM3.Trim()}" +
                $"\r\n                     {PedCliList.Where(t => t.PEDIDO == ordenFabricacion.PEDIDO).OrderByDescending(t => t.PEDIDO).FirstOrDefault().CAMPOCOM1.Trim()}    " +
                $"\r\n\r\n       {PedCliList.Where(t => t.PEDIDO == ordenFabricacion.PEDIDO).OrderByDescending(t => t.PEDIDO).FirstOrDefault().CAMPOCOM3.Trim()}{espaciosSegundoCampo4bis} " +
                $"\r\n         {PedCliList.Where(t => t.PEDIDO == ordenFabricacion.PEDIDO).OrderByDescending(t => t.PEDIDO).FirstOrDefault().CAMPOCOM5.Trim()}                        " +
                $"\r\n\r\n" +
                $"\r\n" +
                $"\r\n\r\n                         {PedCliList.Where(t => t.PEDIDO == ordenFabricacion.PEDIDO).OrderByDescending(t => t.PEDIDO).FirstOrDefault().CAMPOCOM4.Trim()}   " +
                $"\r\n                     {PedCliList.Where(t => t.PEDIDO == ordenFabricacion.PEDIDO).OrderByDescending(t => t.PEDIDO).FirstOrDefault().CAMPOCOM2.Trim()}" +
                $"\r\n\r\n" +
                $"\r\n    " +
                $"\r\n    ", font, PdfBrushes.Black, new Syncfusion.Drawing.PointF(0, 0));

            MemoryStream xx = new MemoryStream();
            document1.Save(xx);
            document1.Close(true);


            await JS.SaveAs("ETOF" + ordenFabricacion.CG_PROD.Trim() + ".pdf", xx.ToArray());
        }

        protected async Task EnviarCSVDataCore()
        {
            var response = await Http.GetAsync("api/Programa/EnviarCsvDataCore");
            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest ||
                response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                await this.ToastObj.ShowAsync(new ToastModel
                {
                    Title = "ERROR!",
                    Content = "No se pudo enviar Archivo.",
                    CssClass = "e-toast-danger",
                    Icon = "e-error toast-icons",
                    ShowCloseButton = true,
                    ShowProgressBar = true
                });
            }
            else
            {
                await this.ToastObj.ShowAsync(new ToastModel
                {
                    Title = "EXITO!",
                    Content = "Archivo enviado con éxito",
                    CssClass = "e-toast-success",
                    Icon = "e-success toast-icons",
                    ShowCloseButton = true,
                    ShowProgressBar = true
                });
            }
        }

        protected async Task DescargarTxtParaImpresoraQR(int pedido, string filename)
        {
            var fileName = filename + ".txt";
            if (await GeneraCsv(pedido))
            {
                var fileArray = await Http.GetByteArrayAsync($"api/AdministracionArchivos/GetTxt/{fileName}");

                await JS.SaveAs(fileName, fileArray);
            }
        }

        protected async Task<bool> GeneraCsv(int pedido)
        {
            var creado = true;
            var response = await Http.GetAsync($"api/Programa/GeneraCsvImpresoraQR/{pedido}");
            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest ||
                response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                creado = false;
                await this.ToastObj.ShowAsync(new ToastModel
                {
                    Title = "ERROR!",
                    Content = "No se pudo generar Archivo.",
                    CssClass = "e-toast-danger",
                    Icon = "e-error toast-icons",
                    ShowCloseButton = true,
                    ShowProgressBar = true
                });
            }
            else
            {
                creado = true;
                await this.ToastObj.ShowAsync(new ToastModel
                {
                    Title = "EXITO!",
                    Content = "Archivo generado con éxito",
                    CssClass = "e-toast-success",
                    Icon = "e-success toast-icons",
                    ShowCloseButton = true,
                    ShowProgressBar = true
                });
            }

            return creado;
        }

        protected class EventData
        {
            public int Id { get; set; }
            public string Subject { get; set; }
            public string CG_ORDF { get; set; }
            public string Location { get; set; }
            public string Description { get; set; }
            public DateTime StartTime { get; set; }
            public DateTime EndTime { get; set; }
            public bool? IsAllDay { get; set; }
            public string CategoryColor { get; set; }
            public string RecurrenceRule { get; set; }
            public int? RecurrenceID { get; set; }
            public string RecurrenceException { get; set; }
            public string StartTimezone { get; set; }
            public string EndTimezone { get; set; }
            public string cantidad { get; set; }
            public string proceso { get; set; }
            public string OFinicial { get; set; }
            public string OFalta { get; set; }
            public string titulo { get; set; }
            public int ResourceId { get; set; }
            public int cambiarPrioridad { get; set; } = 0;
        }

        protected class ResourceData
        {
            public int Id { get; set; }
            public string Text { get; set; }
            public string Name { get; set; }
            public string Designation { get; set; }
            public string Color { get; set; }
        }

        protected async Task<List<ResourceData>> GenerateResourceData()
        {
            CurrentYear = DateTime.Today.Year;
            CurrentDate = new DateTime(CurrentYear, 1, 1);
            int cantCeldas = celdasList.Count;
            List<ResourceData> resources = new List<ResourceData>(cantCeldas);
            var colors = new string[]
            {
                "#ff8787", "#9775fa", "#748ffc", "#3bc9db", "#69db7c",
                "#fdd835", "#748ffc", "#9775fa", "#df5286", "#7fa900",
                "#fec200", "#5978ee", "#00bdae", "#ea80fc"
            };
            for (int a = 0; a < cantCeldas; a++)
            {
                resources.Add(new ResourceData()
                {
                    Id = a + 1,
                    Text = celdasList[a],
                    Color = colors[a % colors.Length],
                    Name = celdasList[a],
                    Designation = celdasList[a]
                });
            }

            return resources;
        }


        protected async Task OnDragged(DragEventArgs<EventData> args)
        {
            if (args.Data.StartTime <= DateTime.Now && args.Data.EndTime >= DateTime.Now)
            {
                await this.ToastObj.ShowAsync(new ToastModel
                {
                    Title = "ERROR!",
                    Content = "No puede mover una orden que esta en curso.",
                    CssClass = "e-toast-danger",
                    Icon = "e-error toast-icons",
                    ShowCloseButton = true,
                    ShowProgressBar = true
                });
            }

            DateTime fechaOrig = ordenesData.FirstOrDefault(t => t.CG_ORDF == args.Data.CG_ORDF)!.StartTime;
            DateTime fechaDest = args.Data.StartTime;

            if (fechaOrig < fechaDest)
            {
                var ordenes = ordenesData
                    .Where(t => t.ResourceId == args.Data.ResourceId &&
                                ((t.StartTime >= fechaOrig && t.EndTime <= fechaDest) ||
                                 (fechaDest >= t.StartTime && fechaDest <= t.EndTime))).OrderBy(t => t.StartTime)
                    .ToList();

                if (ordenes.Count <= 1)
                    return;

                if (ordenes.Count(t => t.OFalta == ordenes[0].OFalta) > 1)
                {
                    await this.ToastObj.ShowAsync(new ToastModel
                    {
                        Title = "ERROR!",
                        Content = "No puede intercambiar el orden del proceso.",
                        CssClass = "e-toast-danger",
                        Icon = "e-error toast-icons",
                        ShowCloseButton = true,
                        ShowProgressBar = true
                    });
                    return;
                }

                ordenes.ForEach(t => t.cambiarPrioridad = 0);
                ordenes[0].cambiarPrioridad = 3;

                var firstStartDate = ordenes[0].StartTime;
                var lastEndDate = ordenes[^1].EndTime;

                var auxStart = ordenesData.FirstOrDefault(t => t.CG_ORDF == ordenes[0].CG_ORDF)!.StartTime;

                for (int i = 1; i < ordenes.Count; i++)
                {
                    var orden = ordenesData.FirstOrDefault(t => t.CG_ORDF == ordenes[i].CG_ORDF);
                    orden!.EndTime = auxStart.AddMinutes(orden.EndTime.Subtract(orden.StartTime).TotalMinutes);
                    orden.StartTime = auxStart;
                    auxStart = orden.EndTime;
                }

                var last = ordenesData.FirstOrDefault(t => t.CG_ORDF == ordenes[^1].CG_ORDF);
                var orig = ordenesData.FirstOrDefault(t => t.CG_ORDF == args.Data.CG_ORDF);
                orig!.EndTime = last!.EndTime.AddMinutes(args.Data.EndTime.Subtract(args.Data.StartTime).TotalMinutes);
                orig.StartTime = last.EndTime;

                foreach (var orden in ordenes)
                {
                    // quiero chequear si alguna de las ordenes por algun error quedo fuera del rango firstStartDate/lastEndDate
                    if (orden.StartTime < firstStartDate || orden.EndTime > lastEndDate)
                    {
                        await this.ToastObj.ShowAsync(new ToastModel
                        {
                            Title = "ERROR!",
                            Content = "Hubo un error al mover la orden, por favor intente nuevamente.",
                            CssClass = "e-toast-danger",
                            Icon = "e-error toast-icons",
                            ShowCloseButton = true,
                            ShowProgressBar = true
                        });
                        ordenesData = await GenerateEvents();
                        return;
                    }
                }

                foreach (var orden in ordenes)
                {
                    var ordenFabricacion = ordenesIA.FirstOrDefault(t => t.CG_ORDF.ToString().Trim() == orden.CG_ORDF);
                    ordenFabricacion!.INICIO = orden.StartTime;
                    ordenFabricacion.FIN = orden.EndTime;
                    ordenFabricacion.cambiarPrioridad = orden.cambiarPrioridad;
                    var result =
                        await Http.PutAsJsonAsync($"api/CargasIA/UpdateOrden/{orden.CG_ORDF}", ordenFabricacion);
                }
            }
            else
            {
                (fechaOrig, fechaDest) = (fechaDest, fechaOrig);

                var ordenes = ordenesData
                    .Where(t => t.ResourceId == args.Data.ResourceId &&
                                ((t.StartTime >= fechaOrig && t.EndTime <= fechaDest) ||
                                 (fechaOrig >= t.StartTime && fechaOrig <= t.EndTime) || fechaDest == t.StartTime))
                    .OrderBy(t => t.StartTime).ToList();

                if (ordenes.Count <= 1)
                    return;

                if (ordenes.Count(t => t.OFalta == ordenes[^1].OFalta) > 1)
                {
                    await this.ToastObj.ShowAsync(new ToastModel
                    {
                        Title = "ERROR!",
                        Content = "No puede intercambiar el orden del proceso.",
                        CssClass = "e-toast-danger",
                        Icon = "e-error toast-icons",
                        ShowCloseButton = true,
                        ShowProgressBar = true
                    });
                    return;
                }
                
                ordenes.ForEach(t => t.cambiarPrioridad = 0);
                ordenes[^1].cambiarPrioridad = -3;

                var firstStartDate = ordenes[0].StartTime;
                var lastEndDate = ordenes[^1].EndTime;

                var last = ordenesData.FirstOrDefault(t => t.CG_ORDF == ordenes[^1].CG_ORDF);
                last!.StartTime = ordenesData.FirstOrDefault(t => t.CG_ORDF == ordenes[0].CG_ORDF)!.StartTime;
                last.EndTime = last.StartTime.AddMinutes(args.Data.EndTime.Subtract(args.Data.StartTime).TotalMinutes);

                var auxStart = last.EndTime;

                for (int i = 0; i < ordenes.Count - 1; i++)
                {
                    var orden = ordenesData.FirstOrDefault(t => t.CG_ORDF == ordenes[i].CG_ORDF);
                    orden!.EndTime = auxStart.AddMinutes(orden.EndTime.Subtract(orden.StartTime).TotalMinutes);
                    orden.StartTime = auxStart;
                    auxStart = orden.EndTime;
                }

                foreach (var orden in ordenes)
                {
                    // quiero chequear si alguna de las ordenes por algun error quedo fuera del rango firstStartDate/lastEndDate
                    if (orden.StartTime < firstStartDate || orden.EndTime > lastEndDate)
                    {
                        await this.ToastObj.ShowAsync(new ToastModel
                        {
                            Title = "ERROR!",
                            Content = "Hubo un error al mover la orden, por favor intente nuevamente.",
                            CssClass = "e-toast-danger",
                            Icon = "e-error toast-icons",
                            ShowCloseButton = true,
                            ShowProgressBar = true
                        });
                        ordenesData = await GenerateEvents();
                        return;
                    }
                }


                foreach (var orden in ordenes)
                {
                    var ordenFabricacion = ordenesIA.FirstOrDefault(t => t.CG_ORDF.ToString().Trim() == orden.CG_ORDF);
                    ordenFabricacion!.INICIO = orden.StartTime;
                    ordenFabricacion.FIN = orden.EndTime;
                    ordenFabricacion.cambiarPrioridad = orden.cambiarPrioridad;
                    var result =
                        await Http.PutAsJsonAsync($"api/CargasIA/UpdateOrden/{orden.CG_ORDF}", ordenFabricacion);
                }
            }

            await scheduleObj.RefreshEventsAsync();
        }

        protected async Task<List<EventData>> GenerateEvents()
        {
            CurrentYear = DateTime.Today.Year;
            var colors = new string[]
            {
                "#ff8787", "#9775fa", "#748ffc", "#3bc9db", "#69db7c",
                "#fdd835", "#df5286", "#7fa900",
                "#fec200", "#5978ee", "#00bdae", "#ea80fc",
                "#81c784", "#ffb74d", "#ba68c8", "#4db6ac", "#ff8a65",
                "#64b5f6", "#a1887f", "#ffcc80", "#90a4ae", "#9ccc65"
            };
            ordenesIA = await Http.GetFromJsonAsync<List<PLANNER>>("api/CargasIA");
            int cantOrdenes = ordenesIA.Count;
            List<EventData> data = new List<EventData>(cantOrdenes);
            var ult_asoc = ordenesIA[0].ULT_ASOC;
            var ult_asoc_ant = ult_asoc;
            var color_index = 0;
            for (int a = 0; a < cantOrdenes; a++)
            {
                var orden = ordenesIA[a];
                int resourceId = celdasList.IndexOf(orden.CG_CELDA.Trim());
                ult_asoc = orden.ULT_ASOC;
                if (ult_asoc != ult_asoc_ant)
                    color_index++;
                data.Add(new EventData
                {
                    Id = a,
                    CG_ORDF = orden.CG_ORDF.ToString().Trim(),
                    Subject = orden.CG_ORDF.ToString().Trim() + " - " + orden.DES_PROD.Trim() + " - " +
                              orden.PROCESO.Trim(),
                    StartTime = orden.INICIO,
                    EndTime = orden.FIN,
                    ResourceId = resourceId + 1,
                    cantidad = orden.CANT.ToString(),
                    Description = orden.DES_PROD.Trim(),
                    proceso = orden.PROCESO.Trim(),
                    OFinicial = orden.CG_ORDFASOC.ToString(),
                    CategoryColor = colors[color_index % colors.Length],
                    OFalta = ordenesIA.Where(t => t.CG_ORDFASOC == orden.CG_ORDFASOC).MaxBy(t => t.CG_ORDF)!.CG_ORDF
                        .ToString(),
                });
                if (orden.INICIO < projectStart || projectStart == null)
                    projectStart = orden.INICIO;
                if (orden.FIN > projectEnd || projectEnd == null)
                    projectEnd = orden.FIN;
                ult_asoc_ant = orden.ULT_ASOC;
            }

            CurrentDate = projectStart ?? DateTime.Today;
            return data;
        }
    }
}