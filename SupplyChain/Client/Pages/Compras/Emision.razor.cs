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
using SupplyChain.Client.HelperService;
using SupplyChain.Client.RepositoryHttp;
using SupplyChain.Client.Shared;
using SupplyChain.Shared;
using SupplyChain.Shared.Models;
using Syncfusion.Blazor.DropDowns;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Notifications;
using Syncfusion.Drawing;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;

namespace SupplyChain.Client.Pages.Emision;

public class EmisionPageBase : ComponentBase
{
    protected BuscadorEmergente<Compra> Buscador;
    protected string[] ColumnasBuscador = { "NUMERO", "FE_EMIT", "CG_MAT", "SOLICITADO", "UNID", "DES_PROVE" };

    protected List<vCondicionesPago> condiconespago = new();

    public string DropVal = "";
    protected SfGrid<Compra> GridProve;


    protected Dictionary<string, object> HtmlAttribute = new()
    {
        { "type", "button" }
    };

    protected List<Compra> insumosproveedor = new();
    protected Compra[] ItemsABuscar;
    protected NotificacionToast NotificacionObj;
    public string Obs = "";
    protected List<Compra> ordenSeleccionada = new();
    protected bool PopupBuscadorVisible;
    protected List<Proveedores_compras> proveedorescompras = new();
    protected string TituloBuscador = "Seleccion de Orden de Compra para apertura";

    public SfToast ToastObj;
    public string xcondven = "";

    public string xespecif = "";
    public string xespecif2 = "";
    [Inject] protected HttpClient Http { get; set; }
    [Inject] protected IJSRuntime JsRuntime { get; set; }
    [Inject] protected IJSRuntime JS { get; set; }
    [Inject] protected IRepositoryHttp repositoryHttp2 { get; set; }


    [CascadingParameter] public MainLayout MainLayout { get; set; }

    // variables generales
    public bool IsVisibleguarda { get; set; }
    public bool IsVisibleimprime { get; set; } = true;
    public bool IsVisibleAnular { get; set; } = true;
    public bool ocagenerar { get; set; } = true;
    public bool ocabierta { get; set; }
    public string proveocabierta { get; set; } = "";

    public int onumero { get; set; }
    public Compra ocompraseleccionada { get; set; } = new();
    public decimal? bonif { get; set; } = 0;
    public string listaordenescompra { get; set; } = "";
    protected bool ToastVisible { get; set; } = false;

    public MainLayout Layout { get; set; }

    protected override async Task OnInitializedAsync()
    {
        MainLayout.Titulo = "Emisión de Ordenes de Compras";

        condiconespago = await Http.GetFromJsonAsync<List<vCondicionesPago>>("api/Condven/Itris");


        proveedorescompras =
            await Http.GetFromJsonAsync<List<Proveedores_compras>>("api/compras/GetProveedorescompras/");
    }

//        public void SeleccionProveedor(Syncfusion.Blazor.DropDowns.ChangeEventArgs<int, Proveedores_compras> args)
    public async Task SeleccionProveedor(ChangeEventArgs<int, Proveedores_compras> args)
    {
        if (args.Value > 0)
        {
            limpia();
            insumosproveedor = await Http.GetFromJsonAsync<List<Compra>>("api/Compras/GetPreparacion/" + args.Value);
            IsVisibleguarda = false;
            IsVisibleimprime = true;
            IsVisibleAnular = true;
            if (insumosproveedor.Count > 0) DropVal = insumosproveedor[0].CONDVEN;
        }
    }

    public async Task limpia()
    {
        ocabierta = false;
        ocagenerar = true;
        insumosproveedor = new List<Compra>();
        ocompraseleccionada = new Compra();
        bonif = 0;
        xespecif = "";
        DropVal = "";
    }

    public async Task BuscarOCompras()
    {
        // Busca todas las OC sin filtros para la apertura en emision
        await Buscador.ShowAsync();
        PopupBuscadorVisible = true;
        ItemsABuscar = await Http.GetFromJsonAsync<Compra[]>("api/Compras/todas");
        await InvokeAsync(StateHasChanged);
    }

    public async Task Cerrar(bool visible)
    {
        PopupBuscadorVisible = visible;
    }

    public async Task EnviarObjetoSeleccionado(Compra compra)
    {
        ocompraseleccionada = compra;
        PopupBuscadorVisible = false;
        await Buscador.HideAsync();
        BuscarOC();
    }

    protected async Task BuscarOC()
    {
        if (ocompraseleccionada is null || ocompraseleccionada.NUMERO == 0)
        {
            await ToastObj.ShowAsync(new ToastModel
            {
                Title = "ERROR!",
                Content = "Debe Indicar la Orden de compra para abrir",
                CssClass = "e-toast-danger",
                Icon = "e-error toast-icons",
                ShowCloseButton = true,
                ShowProgressBar = true
            });
        }
        else
        {
            insumosproveedor =
                await Http.GetFromJsonAsync<List<Compra>>("api/compras/GetCompraByNumero/" +
                                                          ocompraseleccionada.NUMERO);
            IsVisibleguarda = true;
            IsVisibleimprime = false;

            ocabierta = true;
            ocagenerar = false;
            var primerreg = insumosproveedor.FirstOrDefault();
            proveocabierta = primerreg.DES_PROVE;
            DropVal = primerreg.CONDVEN;
            bonif = primerreg.BON;
            xespecif = primerreg.ESPEGEN;
        }
    }

    public async Task anularOc()
    {
        if (ocompraseleccionada is null || ocompraseleccionada.NUMERO == 0)
        {
            await ToastObj.ShowAsync(new ToastModel
            {
                Title = "ERROR!",
                Content = "Debe seleccionar la Orden de Compra",
                CssClass = "e-toast-danger",
                Icon = "e-error toast-icons",
                ShowCloseButton = true,
                ShowProgressBar = true
            });
        }
        else
        {
            var responseMessage =
                await repositoryHttp2.PostAsJsonAsync("api/compras/PostAnularOc", ocompraseleccionada);
            if (responseMessage.Error)
            {
                await ToastObj.ShowAsync(new ToastModel
                {
                    Title = "ERROR!",
                    Content = "Debe seleccionar la Orden de Compra",
                    CssClass = "e-toast-danger",
                    Icon = "e-error toast-icons",
                    ShowCloseButton = true,
                    ShowProgressBar = true
                });
            }
            else
            {
                ocabierta = true;
                var num = responseMessage.Response;
                onumero = num.CG_ORDEN;
                Obs = num.ESPECIFICA;
            }
        }
    }

    //public async Task anularOc()
    //{
    //    try
    //    {
    //        if (ocompra == 0)
    //        {
    //            await this.ToastObj.Show(new ToastModel
    //            {
    //                Title = "ERROR!",
    //                Content = "Debe seleccionar la Orden de Compra",
    //                CssClass = "e-toast-danger",
    //                Icon = "e-error toast-icons",
    //                ShowCloseButton = true,
    //                ShowProgressBar = true
    //            });
    //        }
    //        else
    //        {
    //            ordenSeleccionada = await Http.GetFromJsonAsync<List<Compra>>("api/compras/PostAnularOc" + ocompra);

    //            ocabierta = true;
    //            Compra num = ordenSeleccionada.FirstOrDefault();
    //            onumero = num.CG_ORDEN;
    //            Obs = num.ESPECIFICA;
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        Console.WriteLine("Error al anular la Orden de Compra: " + ex.Message);
    //        await this.ToastObj.Show(new ToastModel
    //        {
    //            Title = "ERROR!",
    //            Content = "Error al anular la Orden de Compra",
    //            CssClass = "e-toast-danger",
    //            Icon = "e-error toast-icons",
    //            ShowCloseButton = true,
    //            ShowProgressBar = true
    //        });
    //    }
    //}


    public async Task imprimiroc()
    {
        if (ocompraseleccionada is null || ocompraseleccionada.NUMERO == 0)
        {
            await ToastObj.ShowAsync(new ToastModel
            {
                Title = "ERROR!",
                Content = "Debe Indicar la Orden de compra para abrir",
                CssClass = "e-toast-danger",
                Icon = "e-error toast-icons",
                ShowCloseButton = true,
                ShowProgressBar = true
            });
        }
        else
        {
            var document = new PdfDocument();
            var page = document.Pages.Add();
            var graphics = page.Graphics;
            PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 20);
            graphics.DrawString("Orden de Compra: " + ocompraseleccionada.NUMERO, font, PdfBrushes.Black,
                new PointF(0, 0));

            /*
            PdfGrid pdfGrid = new PdfGrid();
            pdfGrid.DataSource = insumosproveedor;
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
            pdfGrid.Draw(page, new Syncfusion.Drawing.PointF(0, 200), layoutFormat);
            //Save the document.
           // document.Save("Output.pdf");
            */
            var xx = new MemoryStream();
            document.Save(xx);
            document.Close(true);

            await JS.InvokeVoidAsync("open", $"/api/ReportRDLC/GetReportOC?numero={ocompraseleccionada.NUMERO}",
                "_blank");
        }
    }


    public async Task guardaoc()
    {
        var SelectedRecords = await GridProve.GetSelectedRecords();

        listaordenescompra = "";
        await SelectedRecords.ForEachAsync(async s => { listaordenescompra = listaordenescompra + s.Id + ","; });
        HttpResponseMessage response = null;

        if (string.IsNullOrEmpty(xespecif) && string.IsNullOrEmpty(xespecif))
            xespecif2 = "vacio";
        else
            xespecif2 = xespecif;
        if (string.IsNullOrEmpty(DropVal) && string.IsNullOrEmpty(DropVal))
            xcondven = "vacio";
        else
            xcondven = DropVal;
        //                  string sqlCommandString = string.Format("UPDATE COMPRAS SET NUMERO = 9999 WHERE REGISTRO IN ("+ listaordenescompra + ")");
        response = await Http.PutAsJsonAsync(
            "api/compras/actualizaoc/" + listaordenescompra + '/' + xespecif2 + '/' + xcondven + '/' + bonif,
            listaordenescompra);

        if (response.StatusCode == HttpStatusCode.BadRequest
            || response.StatusCode == HttpStatusCode.NotFound
            || response.StatusCode == HttpStatusCode.Conflict)
        {
            var mensServidor = await response.Content.ReadAsStringAsync();

            Console.WriteLine($"Error: {mensServidor}");
            await ToastObj.Show(new ToastModel
            {
                Title = "ERROR!",
                Content = "Error al actualizar OC",
                CssClass = "e-toast-danger",
                Icon = "e-error toast-icons",
                ShowCloseButton = true,
                ShowProgressBar = true
            });
        }
        else
        {
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                await ToastObj.Show(new ToastModel
                {
                    Title = "EXITO!",
                    Content = "Orden de Compra " + responseString + " Generada",
                    CssClass = "e-toast-success",
                    Icon = "e-success toast-icons",
                    ShowCloseButton = false,
                    ShowProgressBar = false
                });
            }

            proveedorescompras =
                await Http.GetFromJsonAsync<List<Proveedores_compras>>("api/compras/GetProveedorescompras/");

            limpia();
        }
    }
}