using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using SupplyChain.Client.HelperService;
using SupplyChain.Client.RepositoryHttp;
using SupplyChain.Client.Shared;
using SupplyChain.Shared.Models;
using Syncfusion.Blazor.Lists;
using Syncfusion.Blazor.Notifications;
using Syncfusion.Blazor.Spinner;
using Syncfusion.Drawing;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Grid;
using Syncfusion.Pdf.Tables;

namespace SupplyChain.Client.Pages.PCP.Carga_de_Maquina;

public class CargaMaquinaBase : ComponentBase
{
    protected int CantidadColumnasPorPeriodo = 8;
    protected ConfirmacionDialog confirmacionDescargarPlanoDialog;
    protected List<ModeloCarga> dbCarga;
    protected List<ModeloGenericoStringString> dbCeldas;
    protected List<ModeloGenericoStringString> dbDiasFestivos;
    protected List<ModeloGenericoIntString> dbEstadoCarga;
    protected List<ModeloGenericoIntString> dbOrdenesDependientes;
    protected List<ModeloGenericoStringString> dbProcesos;
    protected List<ModeloGenericoIntString> dbScrap;
    protected bool domingosLaborables;
    protected int extensionDias = 365;
    protected DateTime fechaInicial;
    protected bool isEventoDialogVisible;
    protected bool isOrdenDialogVisible;
    protected bool isScrapDialogVisible;
    public SfListView<ModeloGenericoIntString> listViewScrap;
    protected string MensajeCargando = "Cargando...";
    protected IEnumerable<Operario> operariosBE3;

    protected List<Operario> operariosList = new();
    protected int ordenAbuscar = 0;
    protected int OrdenDeFabAlta;
    protected ModeloOrdenFabricacion ordenFabricacion;
    protected ModeloOrdenFabricacionEncabezado ordenFabricacionEncabezado;
    protected List<ModeloOrdenFabricacionHojaRuta> ordenFabricacionHojaRuta;
    protected List<ModeloOrdenFabricacionMP> ordenFabricacionMP;
    protected ModeloOrdenFabricacion ordenFabricacionOriginal;
    protected List<ModeloOrdenFabricacionSE> ordenFabricacionSE;
    protected int ordenNumero;
    protected string ordenTitulo = "";
    protected List<PedCli> PedCliList = new();
    protected Producto prodList = new();
    public DateTime ProjectEnd = new(2019, 7, 28);
    public DateTime ProjectStart = new(2019, 3, 25);
    protected List<Solution> rutas;
    protected bool sabadosLaborables;
    protected int? scrapSeleccionado;
    protected SfSpinner SpinnerCDM;
    protected SfToast ToastObj;
    protected int totalDiasNoLaborables = 0;
    protected string Usuario = "USER";
    protected bool verHojaRuta = false;
    protected bool verMP = true;
    protected bool verSE = true;
    protected bool Visible;
    protected string xCg_celda;
    protected int xColumna;
    protected int xColumnaInicialBarra;
    protected int xHora = 1;
    [Inject] private IJSRuntime JSRuntime { get; set; }
    [Inject] protected IJSRuntime JS { get; set; }
    [Inject] public PdfService PdfService { get; set; }
    [Inject] public HttpClient Http { get; set; }
    [Inject] public IRepositoryHttp Http2 { get; set; }
    [Inject] public NavigationManager NavigationManager { get; set; }
    protected string scrapSeleccionadoMensaje { get; set; } = "";

    public void OpenExternalLink()
    {
        //string url = "https://aerre.grafana.net/public-dashboards/42c12fc6b1ad4c57b9ad51817fa6d364";
        var url = "http://192.168.0.247:8080/aerre/index.html";
        if (!string.IsNullOrEmpty(url))
            // Open the URL in a new tab or window
            JSRuntime.InvokeAsync<object>("open", url, "_blank");
    }

    protected override async Task OnInitializedAsync()
    {
        //await SpinnerCDM.ShowAsync();
        Visible = true;

        dbEstadoCarga = new List<ModeloGenericoIntString>();
        dbEstadoCarga.Add(new ModeloGenericoIntString { ID = 2, TEXTO = "FIRME" });
        dbEstadoCarga.Add(new ModeloGenericoIntString { ID = 3, TEXTO = "EN CURSO" });
        dbEstadoCarga.Add(new ModeloGenericoIntString { ID = 4, TEXTO = "CERRADA" });
        dbEstadoCarga.Add(new ModeloGenericoIntString { ID = 5, TEXTO = "ANULADA" });


        operariosList = await Http.GetFromJsonAsync<List<Operario>>("api/Operario");

        operariosBE3 = from operariosBE3 in operariosList
            where operariosBE3.CG_OPER == 51 || operariosBE3.CG_OPER == 131 || operariosBE3.CG_OPER == 135 ||
                  operariosBE3.CG_OPER == 139 || operariosBE3.CG_OPER == 144
            select operariosBE3;


        await Refrescar();

        //await SpinnerCDM.HideAsync();
        Visible = false;
    }

    protected async Task Refrescar()
    {
        Visible = true;
        dbCarga = await Http.GetFromJsonAsync<List<ModeloCarga>>("api/Cargas");
        // turno
        var xTurno = await Http.GetFromJsonAsync<List<ModeloGenericoIntString>>(
            "api/ModelosGenericosIntString/Select Top 1 CONVERT(INT, ValorN) ID, '' TEXTO From Solution Where Campo = 'HORASDIA'");
        CantidadColumnasPorPeriodo = xTurno.FirstOrDefault().ID;
        // Dias de calendario
        var xDias = await Http.GetFromJsonAsync<List<ModeloGenericoIntString>>(
            "api/ModelosGenericosIntString/Select Top 1 CONVERT(INT, ValorN) ID, '' TEXTO From Solution Where Campo = 'DIASCARGA'");
        extensionDias = xDias.FirstOrDefault().ID;
        // Sabados laborables
        var xSabadosLaborables = await Http.GetFromJsonAsync<List<ModeloGenericoIntString>>(
            "api/ModelosGenericosIntString/Select Top 1 CONVERT(INT, ValorN) ID, '' TEXTO From Solution Where Campo = 'SABADOSLABORABLES'");
        sabadosLaborables = xSabadosLaborables.FirstOrDefault().ID == 1 ? true : false;
        // Domingos laborables
        var xDomingosLaborables = await Http.GetFromJsonAsync<List<ModeloGenericoIntString>>(
            "api/ModelosGenericosIntString/Select Top 1 CONVERT(INT, ValorN) ID, '' TEXTO From Solution Where Campo = 'DOMINGOSLABORABLES'");
        domingosLaborables = xDomingosLaborables.FirstOrDefault().ID == 1 ? true : false;
        // Dias laborables
        dbDiasFestivos = await Http.GetFromJsonAsync<List<ModeloGenericoStringString>>(
            "api/ModelosGenericosStringString/select distinct convert(char(8), Fecha, 112) ID, '' TEXTO from CalendarioFestivos");
        //Busca Scrap
        dbScrap = await Http.GetFromJsonAsync<List<ModeloGenericoIntString>>(
            "api/ModelosGenericosIntString/SELECT convert(int, cg_scrap) ID, des_scrap TEXTO FROM scrap ORDER BY Cg_scrap");
        // fecha inicial
        if (dbCarga.Where(x => x.FE_CURSO != null && x.FE_CURSO.Year != 1900 && x.CG_ESTADOCARGA == 3).ToList().Count >
            0)
            fechaInicial = dbCarga.Where(x => x.FE_CURSO != null && x.FE_CURSO.Year != 1900 && x.CG_ESTADOCARGA == 3)
                .Min(x => x.FE_CURSO);
        else
            fechaInicial = DateTime.Now;


        Visible = false;
    }

    protected async Task OrdenFabricacionOpen(int xOrdenFabricacion, bool xExigeOA, int xPedido, string xCgProd,
        decimal xCantidad)
    {
        Visible = true;
        // Titulo
        if (xExigeOA)
            ordenTitulo = "ORDEN DE ARMADO Nº " + xOrdenFabricacion;
        else
            ordenTitulo = "ORDEN DE FABRICACIÓN Nº " + xOrdenFabricacion;
        if (xPedido > 0) ordenTitulo += " - SERIE / PEDIDO Nº " + xPedido;
        // Datos de la orden
        ordenNumero = xOrdenFabricacion;
        ordenFabricacion = await Http.GetFromJsonAsync<ModeloOrdenFabricacion>("api/OrdenesFabricacion/" + ordenNumero);
        ordenFabricacionOriginal =
            JsonConvert.DeserializeObject<ModeloOrdenFabricacion>(JsonConvert.SerializeObject(ordenFabricacion));
        // Ordenes dependientes
        var xSQLcommand = string.Format("SELECT 0 ID, CONVERT(varchar, 0) TEXTO " +
                                        "UNION " +
                                        "SELECT DISTINCT CG_ORDF ID, CONVERT(varchar, CG_ORDF) TEXTO FROM PROGRAMA WHERE CG_ORDFASOC = {0} AND CG_ORDF != {1}",
            ordenFabricacion.CG_ORDFASOC,
            ordenFabricacion.CG_ORDF);
        dbOrdenesDependientes =
            await Http.GetFromJsonAsync<List<ModeloGenericoIntString>>("api/ModelosGenericosIntString/" + xSQLcommand);
        // Celdas
        xSQLcommand = "SELECT ltrim(rtrim(CG_CELDA)) ID, DES_CELDA TEXTO FROM CELDAS ORDER BY CG_CELDA";
        dbCeldas = await Http.GetFromJsonAsync<List<ModeloGenericoStringString>>("api/ModelosGenericosStringString/" +
            xSQLcommand);
        // Procesos
        xSQLcommand = "SELECT PROCESO ID, DESCRIP TEXTO FROM PROTAB ORDER BY PROCESO";
        dbProcesos =
            await Http.GetFromJsonAsync<List<ModeloGenericoStringString>>("api/ModelosGenericosStringString/" +
                                                                          xSQLcommand);

        var cg_ordfAsoc = dbCarga.Where(c => c.CG_ORDF == ordenNumero).OrderBy(c => c.CG_ORDF).FirstOrDefault()
            .CG_ORDFASOC;
        // Datos del encabezado del detalle
        ordenFabricacionEncabezado =
            await Http.GetFromJsonAsync<ModeloOrdenFabricacionEncabezado>("api/OrdenesFabricacionEncabezado/" +
                                                                          ordenNumero);
        // Materias primas
        ordenFabricacionMP =
            await Http.GetFromJsonAsync<List<ModeloOrdenFabricacionMP>>("api/OrdenesFabricacionMP/" + cg_ordfAsoc);
        // Semi elaborados
        ordenFabricacionSE =
            await Http.GetFromJsonAsync<List<ModeloOrdenFabricacionSE>>("api/OrdenesFabricacionSE/" + cg_ordfAsoc);
        // Semi elaborados
        ordenFabricacionHojaRuta = await Http
            .GetFromJsonAsync<List<ModeloOrdenFabricacionHojaRuta>>(
                $"api/OrdenesFabricacionHojaRuta/GetByFilter?CodigoProd={xCgProd}&Cantidad={xCantidad.ToString()}");
        Visible = false;
        isOrdenDialogVisible = true;
    }

    protected void OrdenFabricacionClose(object args)
    {
        isOrdenDialogVisible = false;
        ordenFabricacion = null;
    }

    protected async Task AbreEvento()
    {
        isOrdenDialogVisible = false;
        isEventoDialogVisible = true;
    }

    protected async Task DialogEventoClose(object args)
    {
        isOrdenDialogVisible = true;
        isEventoDialogVisible = false;
    }

    protected async Task OrdenFabricacionOk(object args)
    {
        Visible = true;

        isOrdenDialogVisible = false;
        var respuesta =
            await Http.PutAsJsonAsync("api/OrdenesFabricacion/" + ordenFabricacion.CG_ORDF, ordenFabricacion);
        if (respuesta.StatusCode == HttpStatusCode.BadRequest)
        {
            Console.WriteLine("Error en api/OrdenesFabricacion");

            var mensajeServidor = await respuesta.Content.ReadAsStringAsync();
            Console.WriteLine(mensajeServidor);
            await ToastObj.ShowAsync(new ToastModel
            {
                Title = "ERROR!",
                Content = $"Ocurrrio un error.Error al intentar Guardar OF: {ordenFabricacion.CG_ORDF} ",
                CssClass = "e-toast-danger",
                Icon = "e-error toast-icons",
                ShowCloseButton = true,
                ShowProgressBar = true
            });
        }

        if (ordenFabricacion.CG_ESTADOCARGA == 2 || (ordenFabricacion.CG_ESTADOCARGA == 3
                                                     && ordenFabricacion.CG_ESTADOCARGA !=
                                                     ordenFabricacionOriginal.CG_ESTADOCARGA))
        {
            //actualizada en estado Firme o Curso: si es la primera actualiza el grupo completo.
            var sqlCommandString = string.Format("UPDATE Programa SET CG_ESTADOCARGA = {0}," +
                                                 "Fe_curso = GETDATE(), CG_ESTADO = {1} WHERE (Cg_ordf = {2} OR Cg_ordfAsoc = {2})",
                ordenFabricacion.CG_ESTADOCARGA,
                ordenFabricacionOriginal.CG_ESTADOCARGA,
                ordenFabricacion.CG_ORDF);
            await Http.PutAsJsonAsync("api/SQLgenericCommandString/" + sqlCommandString, ordenFabricacion);


            var ofsList = dbCarga
                .Where(c => c.CG_ORDF == ordenFabricacion.CG_ORDF || c.CG_ORDFASOC == ordenFabricacion.CG_ORDF)
                .ToList();
            foreach (var item in ofsList)
            {
                item.CG_ESTADOCARGA = ordenFabricacion.CG_ESTADOCARGA;
                item.FE_CURSO = DateTime.Now;
            }

            ordenFabricacion = null;
            //await Refrescar();
        }
        else if (ordenFabricacion.CG_ESTADOCARGA == 3 && ordenFabricacion.CANTFAB > 0
                                                      && ordenFabricacion.CG_ESTADOCARGA ==
                                                      ordenFabricacionOriginal.CG_ESTADOCARGA)
        {
            await Altaparcial();
            ordenFabricacion = null;
            await Refrescar();
        }
        else if (ordenFabricacion.CG_ESTADOCARGA == 4)
        {
            if (ordenFabricacion.CANTFAB == 0)
                await ToastObj.Show(new ToastModel
                {
                    Title = "AVISO!",
                    Content = "Órden sin indicar cantidad fabricada. Se continuará igualmente.",
                    CssClass = "e-toast-warning",
                    Icon = "e-warning toast-icons"
                });
            //VERIFIFCAR QUE SE LA ULTIMA: LA QUE DA DE ALTA AL PRODUCTO
            var lOfAsocs = dbCarga.Where(c => c.CG_ORDFASOC == ordenFabricacion.CG_ORDFASOC)
                .OrderByDescending(o => o.CG_ORDF).ToList();
            var ultimaOF = lOfAsocs.Max(m => m.CG_ORDF);
            if (dbScrap != null && ordenFabricacion.CG_ORDF == ultimaOF)
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
            var sqlCommandString = "EXEC NET_PCP_Anular_OrdenFabricacion " + ordenFabricacion.CG_ORDF + ", '" +
                                   Usuario + "'";
            var respuesta2 =
                await Http.PutAsJsonAsync("api/SQLgenericCommandString/" + sqlCommandString, ordenFabricacion);
            if (respuesta2.StatusCode == HttpStatusCode.BadRequest)
                await ToastObj.Show(new ToastModel
                {
                    Title = "ERROR!",
                    Content = $"Ocurrio un error. Error al intentar anular OF: {ordenFabricacion.CG_ORDF}",
                    CssClass = "e-toast-danger",
                    Icon = "e-error toast-icons",
                    ShowCloseButton = true,
                    ShowProgressBar = true
                });
            StateHasChanged();
            await ToastObj.Show(new ToastModel
            {
                Title = "AVISO!",
                Content = "Órden anulada.",
                CssClass = "e-toast-warning",
                Icon = "e-warning toast-icons"
            });
            ordenFabricacion = null;
            await Refrescar();
        }

        //await this.ToastObj.Show(new ToastModel
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

    protected async Task DialogScrapClose(object args)
    {
        isScrapDialogVisible = false;
        await CerrarOrdenFabricacion();
    }

    protected async Task DialogScrapOk(object args)
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

    protected async Task Altaparcial()
    {
        Visible = true;

        var sqlCommandString = "EXEC NET_PCP_Altaparcial_OrdenFabricacion " + ordenFabricacion.CG_ORDF + ", '" +
                               Usuario + "'";
        var respuesta2 = await Http.PutAsJsonAsync("api/SQLgenericCommandString/" + sqlCommandString, ordenFabricacion);
        if (respuesta2.StatusCode == HttpStatusCode.BadRequest)
            await ToastObj.Show(new ToastModel
            {
                Title = "ERROR!",
                Content = "Ocurrio un error. Error al registrar el alta",
                CssClass = "e-toast-danger",
                Icon = "e-error toast-icons",
                ShowCloseButton = true,
                ShowProgressBar = true
            });

        Visible = false;
    }

    protected async Task CerrarOrdenFabricacion()
    {
        Visible = true;
        scrapSeleccionado = scrapSeleccionado == null ? 0 : scrapSeleccionado;
        var sqlCommandString = "EXEC NET_PCP_Cerrar_OrdenFabricacion " + ordenFabricacion.CG_ORDF + ", '" + Usuario +
                               "', " + scrapSeleccionado;
        await Http.PutAsJsonAsync("api/SQLgenericCommandString/" + sqlCommandString, ordenFabricacion);
        if (ordenFabricacion.CG_ORDF == ordenFabricacion.ULTIMAORDENASOCIADA)
            await ToastObj.Show(new ToastModel
            {
                Title = "Exito!",
                Content = $"Guardado Correctamente! Alta {ordenFabricacion.CG_PROD.Trim()}",
                CssClass = "e-toast-success",
                Icon = "e-success toast-icons",
                ShowCloseButton = true,
                ShowProgressBar = true
            });
        else
            await ToastObj.Show(new ToastModel
            {
                Title = "Exito!",
                Content = "Guardado Correctamente!\n" +
                          $"OF Cerrada {ordenFabricacion.CG_ORDF}",
                CssClass = "e-toast-success",
                Icon = "e-success toast-icons",
                ShowCloseButton = true,
                ShowProgressBar = true
            });
        //dbCarga.Where(w => w.CG_ORDF != ordenFabricacion.CG_ORDF);
        ordenFabricacion = null;
        await Refrescar();
        Visible = false;
        if (scrapSeleccionado > 0) isEventoDialogVisible = true;
        scrapSeleccionado = null;
    }

    protected async Task EstadoCarga_Change()
    {
        if (ordenFabricacionOriginal.CG_ESTADOCARGA == 0 && ordenFabricacion.CG_ESTADOCARGA == 2)
            await ToastObj.Show(new ToastModel
            {
                Title = "ERROR!",
                Content = "No puede pasar una órden de fabricación EMITIDA a estado EN FIRME.",
                CssClass = "e-toast-danger",
                Icon = "e-error toast-icons"
            });
        else if (ordenFabricacionOriginal.CG_ESTADOCARGA == 0 && ordenFabricacion.CG_ESTADOCARGA == 3)
            await ToastObj.Show(new ToastModel
            {
                Title = "ERROR!", Content = "No puede pasar una órden de fabricación EMITIDA a estado EN CURSO.",
                CssClass = "e-toast-danger", Icon = "e-error toast-icons"
            });
        else if (ordenFabricacionOriginal.CG_ESTADOCARGA == 1 && ordenFabricacion.CG_ESTADOCARGA == 3)
            await ToastObj.Show(new ToastModel
            {
                Title = "ERROR!", Content = "No puede pasar una órden de fabricación PLANEADA a estado EN CURSO.",
                CssClass = "e-toast-danger", Icon = "e-error toast-icons"
            });
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
            await ToastObj.ShowAsync(new ToastModel
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
            if (response.Response.CG_ORDEN == 1) cg_prod = response.Response.Id;

            if (response.Response.CG_ORDEN == 3) cg_prod = response.Response.Id.Substring(0, 7);

            var file = cg_prod + ".pdf";
            if (await ExistePlano(file))
                await JS.InvokeVoidAsync("open", $"Pdf/{cg_prod.Trim()}/RUTAOF", "_blank");
            else
                await ToastObj.ShowAsync(new ToastModel
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

    protected async Task IrAServicio(string pedido)
    {
        //NavigationManager.NavigateTo($"sc/servicio/list/{pedido}");
        await JS.InvokeVoidAsync("open", $"servicio/list/{pedido}", $"servicio/list/{pedido}");
    }

    protected async Task IrAPrograma(string cg_prod)
    {
        //Pdf/@(ordenFabricacion.CG_PROD)/RUTACNC
        await JS.InvokeVoidAsync("open", $"Pdf/{cg_prod}/RUTACNC", $"Pdf/{cg_prod}/RUTACNC");
    }

    protected async Task Ensayos(string pedido)
    {
        //NavigationManager.NavigateTo($"Pdf/{pedido}/ENSAYOS");

        await JS.InvokeVoidAsync("open", $"/pdf-ensayos/{pedido}/RUTAENSAYO", "_blank");
    }

    protected async Task DownloadText()
    {
        var presion = ordenFabricacionEncabezado.CAMPOCOM4.Trim();

        if (string.IsNullOrEmpty(presion)) presion = ordenFabricacionEncabezado.CAMPOCOM1.Trim();
        presion = presion.Replace(',', '.');
        // Generate a text file
        //byte[] file;

        ordenFabricacion.Des_Cli = ordenFabricacionEncabezado.DES_CLI;
        ordenFabricacion.Presion = presion;
        if (string.IsNullOrEmpty(ordenFabricacion.DES_OPER))
        {
            await ToastObj.ShowAsync(new ToastModel
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
            var respuesta = await Http.PostAsJsonAsync("api/AdministracionArchivos/DownloadText", ordenFabricacion);
            if (respuesta.StatusCode == HttpStatusCode.BadRequest
                || respuesta.StatusCode == HttpStatusCode.NotFound
                || respuesta.StatusCode == HttpStatusCode.UnsupportedMediaType)
                await ToastObj.Show(new ToastModel
                {
                    Title = "ERROR!",
                    Content = "Error al descargar archivo",
                    CssClass = "e-toast-danger",
                    Icon = "e-success toast-icons",
                    ShowCloseButton = true,
                    ShowProgressBar = true
                });
            else
                await ToastObj.Show(new ToastModel
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

    protected async Task Etiqueta()
    {
        PedCliList = await Http.GetFromJsonAsync<List<PedCli>>($"api/PedCli/ByPedido/{ordenFabricacion.PEDIDO}");

        prodList = await Http.GetFromJsonAsync<Producto>(
            $"api/Prod/GetByFilter?Codigo={ordenFabricacion.CG_PROD.Trim()}" +
            $"&Descripcion={string.Empty}");


        if (ordenFabricacion.CG_CELDA == "BE3" || ordenFabricacion.CG_CELDA == "GC1")
        {
            if (ordenFabricacion.CG_PROD.Substring(0, 1) == "2")
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
            if (ordenFabricacion.CG_PROD.Substring(0, 1) == "1")
                await DescargarTxtParaImpresoraQR(ordenFabricacion.PEDIDO, "Roscada");
            //await EtiquetaInicio1();
            if (ordenFabricacion.CG_PROD.Substring(0, 4) == "0012" ||
                ordenFabricacion.CG_PROD.Substring(0, 5) == "00130" ||
                ordenFabricacion.CG_PROD.Substring(0, 5) == "00131")
                await DescargarTxtParaImpresoraQR(PedCliList[0].PEDIDO, "Reparacion");
            //await EtiquetaReparaciones();
        }
        else
        {
            //await JS.InvokeVoidAsync("open", new object[2] { $"/api/ReportRDLC/GetReportEtiquetaOF?cg_ordf={ordenFabricacion.CG_ORDF}", "_blank" });

            OrdenDeFabAlta = dbCarga.Where(t => t.CG_ORDFASOC == ordenFabricacion.CG_ORDFASOC)
                .OrderByDescending(t => t.CG_ORDF).FirstOrDefault().CG_ORDF;
            await PdfService.EtiquetaOF(OrdenDeFabAlta, ordenFabricacion);
        }
    }

    private async Task EtiquetaReparaciones()
    {
        var espaciosbar = "";
        for (var i = 0;
             i < 16 - PedCliList.Where(t => t.PEDIDO == ordenFabricacion.PEDIDO).OrderByDescending(t => t.PEDIDO)
                 .FirstOrDefault().LOTE.Trim().Length;
             i++) espaciosbar = espaciosbar + " ";
        //Chapa de 31 x 78
        var document1 = new PdfDocument();
        document1.PageSettings.Size = new SizeF(117, 295);
        document1.PageSettings.Orientation = PdfPageOrientation.Landscape;
        document1.PageSettings.Margins.All = 0;
        var pdfGrid1 = new PdfGrid();
        var page = document1.Pages.Add();
        var graphics = page.Graphics;
        PdfFont font = new PdfStandardFont(PdfFontFamily.Courier, 16);
        var pdfTable = new PdfLightTable();

        var presionMostrar = PedCliList.Where(t => t.PEDIDO == ordenFabricacion.PEDIDO).OrderByDescending(t => t.PEDIDO)
            .FirstOrDefault().CAMPOCOM1.Trim();
        var found = presionMostrar.ToUpper().IndexOf("B");
        if (found == -1)
            presionMostrar = PedCliList.Where(t => t.PEDIDO == ordenFabricacion.PEDIDO).OrderByDescending(t => t.PEDIDO)
                .FirstOrDefault().CAMPOCOM1.Trim();
        else
            presionMostrar = presionMostrar.Substring(0, found);

        graphics.DrawString(
            $"\"\r\n\r\n\r\n        {ordenFabricacion.PEDIDO}           {DateTime.Now.Month}/{DateTime.Now.Year} " +
            $"\r\n        {PedCliList.Where(t => t.PEDIDO == ordenFabricacion.PEDIDO).OrderByDescending(t => t.PEDIDO).FirstOrDefault().LOTE.Trim()}{espaciosbar}{presionMostrar}" +
            $"\r\n\r\n                              .", font, PdfBrushes.Black, new PointF(0, 0));

        var xx = new MemoryStream();
        document1.Save(xx);
        document1.Close(true);
        await JS.SaveAs("ETOF" + ordenFabricacion.CG_PROD.Trim() + ".pdf", xx.ToArray());
    }

    private async Task EtiquetaInicio1()
    {
        //Chapa de 25 x 95
        var document1 = new PdfDocument();
        document1.PageSettings.Size = new SizeF(359, 94);
        document1.PageSettings.Margins.All = 0;
        document1.PageSettings.Rotate = PdfPageRotateAngle.RotateAngle180;
        var pdfGrid1 = new PdfGrid();
        var page = document1.Pages.Add();
        var graphics = page.Graphics;
        PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 12);
        var pdfTable = new PdfLightTable();
        page.Graphics.RotateTransform(-90);
        var espaciosTag = "";
        for (var i = 0; i < 20 - ordenFabricacion.PEDIDO.ToString().Length; i++) espaciosTag = espaciosTag + " ";
        var espaciosAnio = "";
        for (var i = 0;
             i < 32 - PedCliList.Where(t => t.PEDIDO == ordenFabricacion.PEDIDO).OrderByDescending(t => t.PEDIDO)
                 .FirstOrDefault().LOTE.Trim().Length;
             i++) espaciosAnio = espaciosAnio + " ";
        var espaciosOrif = "";
        for (var i = 0; i < 32 - prodList.CAMPOCOM2.Trim().Length; i++) espaciosOrif = espaciosOrif + " ";
        var espaciosClase = "";
        for (var i = 0;
             i < 20 - PedCliList.Where(t => t.PEDIDO == ordenFabricacion.PEDIDO).OrderByDescending(t => t.PEDIDO)
                 .FirstOrDefault().CAMPOCOM4.Trim().Length;
             i++) espaciosClase = espaciosClase + " ";
        var espaciosSinLote = "";
        for (var i = 0; i < 43; i++) espaciosSinLote = espaciosSinLote + " ";

        //graphics.DrawString($"\r\n\r\n\r\n                  {ordenFabricacion.PEDIDO}{espaciosTag}{PedCliList.Where(t => t.PEDIDO == ordenFabricacion.PEDIDO).OrderByDescending(t => t.PEDIDO).FirstOrDefault().LOTE.Trim()}{espaciosAnio}{DateTime.Now.Year} " +
        //    $"\r\n\r\n                        {ordenFabricacion.CG_PROD.Trim()}{espaciosMed}{prodList.Where(t => t.CG_PROD == ordenFabricacion.CG_PROD).OrderByDescending(t => t.CG_PROD).FirstOrDefault().CAMPOCOM2.Trim()}{espaciosOrif}{prodList.Where(t => t.CG_PROD.Trim() == ordenFabricacion.CG_PROD.Trim()).OrderByDescending(t => t.CG_PROD.Trim()).FirstOrDefault().CAMPOCOM3.Trim()} " +
        //    $"\r\n\r\n                                                                        {PedCliList.Where(t => t.PEDIDO == ordenFabricacion.PEDIDO).OrderByDescending(t => t.PEDIDO).FirstOrDefault().CAMPOCOM4.Trim()}{espaciosClase}{prodList.Where(t => t.CG_PROD.Trim() == ordenFabricacion.CG_PROD.Trim()).OrderByDescending(t => t.CG_PROD.Trim()).FirstOrDefault().CAMPOCOM5.Trim()} ", font, PdfBrushes.Black, new Syncfusion.Drawing.PointF(-359, 0));
        if (!string.IsNullOrEmpty(PedCliList.Where(t => t.PEDIDO == ordenFabricacion.PEDIDO)
                .OrderByDescending(t => t.PEDIDO).FirstOrDefault().LOTE.Trim()))
            graphics.DrawString(
                $"\r\n\r\n                                     {ordenFabricacion.PEDIDO}{espaciosSinLote}{DateTime.Now.Year}     .  .  .  .  ." +
                $"\r\n               {ordenFabricacion.CG_PROD.Trim()}          {prodList.CAMPOCOM2.Trim()}{espaciosOrif}{prodList.CAMPOCOM3.Trim()} " +
                $"\r\n.                                            {PedCliList.Where(t => t.PEDIDO == ordenFabricacion.PEDIDO).OrderByDescending(t => t.PEDIDO).FirstOrDefault().CAMPOCOM4.Trim()}{espaciosClase}{prodList.CAMPOCOM5.Trim()}     .  .  .  .  .",
                font, PdfBrushes.Black, new PointF(-359, 0));
        else
            graphics.DrawString(
                $"                                                                                                           \"\r\n\r\n                                     {ordenFabricacion.PEDIDO}{espaciosTag}{PedCliList.Where(t => t.PEDIDO == ordenFabricacion.PEDIDO).OrderByDescending(t => t.PEDIDO).FirstOrDefault().LOTE.Trim()}{espaciosAnio}{DateTime.Now.Year}" +
                $"\r\n               {ordenFabricacion.CG_PROD.Trim()}          {prodList.CAMPOCOM2.Trim()}{espaciosOrif}{prodList.CAMPOCOM3.Trim()} " +
                $"\r\n                                             {PedCliList.Where(t => t.PEDIDO == ordenFabricacion.PEDIDO).OrderByDescending(t => t.PEDIDO).FirstOrDefault().CAMPOCOM4.Trim()}{espaciosClase}{prodList.CAMPOCOM5.Trim()}" +
                $"\r\n\r\n._", font, PdfBrushes.Black, new PointF(-359, 0));

        var xx = new MemoryStream();
        document1.Save(xx);
        document1.Close(true);
        await JS.SaveAs("ETOF" + ordenFabricacion.CG_PROD.Trim() + ".pdf", xx.ToArray());
    }


    private async Task EtiquetaClientesNOypf()
    {
        var espaciosPedidox = "";
        var espaciosAnio = "";
        var espaciosSegundoCampo3bis = "";
        var espaciosSegundoCampo4bis = "";
        var espaciosSegundoCampo5bis = "";

        //Chapa de 101 mm x 78 mm
        var document1 = new PdfDocument();
        document1.PageSettings.Size = new SizeF(382, 295);
        document1.PageSettings.Margins.All = 0;
        var pdfGrid1 = new PdfGrid();
        var page = document1.Pages.Add();
        var graphics = page.Graphics;
        PdfFont font = new PdfStandardFont(PdfFontFamily.Courier, 16);
        var pdfTable = new PdfLightTable();
        page.Graphics.RotateTransform(-360);

        for (var i = 0;
             i < 25 - PedCliList.Where(t => t.PEDIDO == ordenFabricacion.PEDIDO).OrderByDescending(t => t.PEDIDO)
                 .FirstOrDefault().LOTE.Length;
             i++) espaciosPedidox = espaciosPedidox + " ";
        for (var i = 0; i < 16 - ordenFabricacion.CG_PROD.Length; i++) espaciosAnio = espaciosAnio + " ";
        for (var i = 0; i < 25 - prodList.CAMPOCOM5.Trim().Length; i++)
            espaciosSegundoCampo3bis = espaciosSegundoCampo3bis + " ";
        for (var i = 0;
             i < 25 - PedCliList.Where(t => t.PEDIDO == ordenFabricacion.PEDIDO).OrderByDescending(t => t.PEDIDO)
                 .FirstOrDefault().CAMPOCOM3.Trim().Length;
             i++) espaciosSegundoCampo4bis = espaciosSegundoCampo4bis + " ";
        for (var i = 0; i < 25 - prodList.CAMPOCOM5.Trim().Length; i++)
            espaciosSegundoCampo5bis = espaciosSegundoCampo5bis + " ";

        var xd1 = prodList.CAMPOCOM2.Trim();
        var xd2 = prodList.CAMPOCOM5.Trim();
        var UbicacionXMedida = xd1.ToLower().IndexOf('x');
        var UbicacionXMedida2 = xd2.ToLower().IndexOf('x');

        var primeramedida1 = xd1.Substring(0, UbicacionXMedida);
        var segundamedida1 = xd1.Substring(UbicacionXMedida + 1);
        var primeramedida2 = xd2.Substring(0, UbicacionXMedida2);
        var segundamedida2 = xd2.Substring(UbicacionXMedida2 + 1);

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
            $"\r\n    ", font, PdfBrushes.Black, new PointF(0, 0));

        var xx = new MemoryStream();
        document1.Save(xx);
        document1.Close(true);


        await JS.SaveAs("ETOF" + ordenFabricacion.CG_PROD.Trim() + ".pdf", xx.ToArray());
    }

    protected async Task EnviarCSVDataCore()
    {
        var response = await Http.GetAsync("api/Programa/EnviarCsvDataCore");
        if (response.StatusCode == HttpStatusCode.BadRequest ||
            response.StatusCode == HttpStatusCode.InternalServerError)
            await ToastObj.Show(new ToastModel
            {
                Title = "ERROR!",
                Content = "No se pudo enviar Archivo.",
                CssClass = "e-toast-danger",
                Icon = "e-error toast-icons",
                ShowCloseButton = true,
                ShowProgressBar = true
            });
        else
            await ToastObj.Show(new ToastModel
            {
                Title = "EXITO!",
                Content = "Archivo enviado con éxito",
                CssClass = "e-toast-success",
                Icon = "e-success toast-icons",
                ShowCloseButton = true,
                ShowProgressBar = true
            });
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
        if (response.StatusCode == HttpStatusCode.BadRequest ||
            response.StatusCode == HttpStatusCode.InternalServerError)
        {
            creado = false;
            await ToastObj.Show(new ToastModel
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
            await ToastObj.Show(new ToastModel
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
}