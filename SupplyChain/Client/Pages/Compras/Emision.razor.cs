using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SupplyChain.Client.RepositoryHttp;
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
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;


namespace SupplyChain.Client.Pages.Emision
{
    public class EmisionPageBase : ComponentBase
    {
        [Inject] protected HttpClient Http { get; set; }
        [Inject] protected IJSRuntime JsRuntime { get; set; }
        [Inject] protected Microsoft.JSInterop.IJSRuntime JS { get; set; }
        [CascadingParameter] public MainLayout MainLayout { get; set; }


        // variables generales
        public bool IsVisibleguarda { get; set; } = false;
        public bool IsVisibleimprime { get; set; } = true;
        public bool ocagenerar { get; set; } = true;
        public bool ocabierta { get; set; } = false;
        public string proveocabierta { get; set; } = "";
                
        public int ocompra { get; set; } = 0;
        public decimal? bonif { get; set; } = 0;
        public string listaordenescompra { get; set; } = "";

        public SfToast ToastObj;
        protected NotificacionToast NotificacionObj;
        protected bool ToastVisible { get; set; } = false;

        protected List<Proveedores_compras> proveedorescompras = new List<Proveedores_compras>();
        protected List<Compra> insumosproveedor = new();
        protected SfGrid<Compra> GridProve;

        public string xespecif = "";
        public string xespecif2 = "";

        public string DropVal = "";
        public string xcondven = "";

        protected BuscadorEmergente<Compra> Buscador;
        protected Compra[] ItemsABuscar = null;
        protected string[] ColumnasBuscador = new string[] { "NUMERO", "FE_EMIT", "CG_MAT", "SOLICITADO", "UNID", "DES_PROVE" };
        protected string TituloBuscador = "Seleccion de Orden de Compra para apertura";
        protected bool PopupBuscadorVisible = false;

        protected readonly HttpClient httpClient;

        protected Dictionary<string, object> HtmlAttribute = new Dictionary<string, object>()
        {
                {"type", "button" }
        };

        public MainLayout Layout { get; set; }
        protected override async Task OnInitializedAsync()
        {
            MainLayout.Titulo = "Emisión de Ordenes de Compras";

            proveedorescompras = await Http.GetFromJsonAsync<List<Proveedores_compras>>("api/compras/GetProveedorescompras/");

        }

//        public void SeleccionProveedor(Syncfusion.Blazor.DropDowns.ChangeEventArgs<int, Proveedores_compras> args)
        public async Task SeleccionProveedor(Syncfusion.Blazor.DropDowns.ChangeEventArgs<int, Proveedores_compras> args)
        {
            if (args.Value > 0)
            {
                limpia();
                insumosproveedor = await Http.GetFromJsonAsync<List<Compra>>("api/Compras/GetPreparacion/"+ args.Value);
                IsVisibleguarda = false;
                IsVisibleimprime = true;
            }
        }
        public async Task limpia()
        {
            ocabierta = false;
            ocagenerar = true;
            insumosproveedor = new();
            ocompra = 0;
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
            ocompra = compra.NUMERO;
            PopupBuscadorVisible = false;
            await Buscador.HideAsync();
            BuscarOC();
        }

        protected async Task BuscarOC()
        {
            if ( ocompra == 0)
            {
                await this.ToastObj.Show(new ToastModel
                {
                    Title = "ERROR!",
                    Content = "Debe Indicar la Orden de compra para abrir",
                    CssClass = "e-toast-danger",
                    Icon = "e-error toast-icons",
                    ShowCloseButton = true,
                    ShowProgressBar = true
                });
            }else {
                insumosproveedor = await Http.GetFromJsonAsync<List<Compra>>("api/compras/GetCompraByNumero/" + ocompra);
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
        public async Task imprimiroc()
        {
            if (ocompra == 0)
            {
                await this.ToastObj.Show(new ToastModel
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

                PdfDocument document = new PdfDocument();
                PdfPage page = document.Pages.Add();
                PdfGraphics graphics = page.Graphics;
                PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 20);
                graphics.DrawString("Orden de Compra: " + ocompra.ToString(), font, PdfBrushes.Black, new Syncfusion.Drawing.PointF(0, 0));

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
                MemoryStream xx = new MemoryStream();
                document.Save(xx);
                document.Close(true);

                await JS.InvokeVoidAsync("open", new object[2] { $"/api/ReportRDLC/GetReportOC?numero={ocompra}", "_blank" });

            }
        }



        public async Task<ActionResult<Compra>> AnularOrdenCompra()
        {
            var numeroOrden = ocompra;
            var response = await httpClient.GetAsync($"api/compras/anularoc?numero={numeroOrden}");

            if (response.IsSuccessStatusCode)
            {
                var numeroOc = await response.Content.ReadAsStringAsync();
                await this.ToastObj.Show(new ToastModel 
                {
                    Title = "EXITO!",
                    Content = "Orden de Compra " + numeroOc + " Anulado",
                    CssClass = "e-toast-success",
                    Icon = "e-success toast-icons",
                    ShowCloseButton = false,
                    ShowProgressBar = false
                });
            }
            else
            {
                await this.ToastObj.Show(new ToastModel
                {
                    Title = "Error",
                    Content = "No se pudo anular la Orden de Compra",
                    CssClass = "e-toast-x|error",
                    Icon = "e-error toast-icons",
                    ShowCloseButton = false,
                    ShowProgressBar = false 
                });

            }
            return null;
        }

        public async Task guardaoc()
        {

            var SelectedRecords = await GridProve.GetSelectedRecords();

            listaordenescompra = "";
            await SelectedRecords.ForEachAsync(async s =>
            {
                listaordenescompra = listaordenescompra + s.Id + ",";
            });
            HttpResponseMessage response = null;

            if (string.IsNullOrEmpty(xespecif) && string.IsNullOrEmpty(xespecif))
            {
                xespecif2 = "vacio";
            }
            else
            {
                xespecif2 = xespecif;
            }
            if (string.IsNullOrEmpty(@DropVal) && string.IsNullOrEmpty(@DropVal))
            {
                xcondven = "vacio";
            }
            else
            {
                xcondven = @DropVal;
            }
            //                  string sqlCommandString = string.Format("UPDATE COMPRAS SET NUMERO = 9999 WHERE REGISTRO IN ("+ listaordenescompra + ")");
            response = await Http.PutAsJsonAsync("api/compras/actualizaoc/" + listaordenescompra+ '/'+xespecif2 + '/' + xcondven + '/' + bonif, listaordenescompra);

            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest
                || response.StatusCode == System.Net.HttpStatusCode.NotFound
                || response.StatusCode == System.Net.HttpStatusCode.Conflict)
            {
                var mensServidor = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"Error: {mensServidor}");
                await this.ToastObj.Show(new ToastModel
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

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    String responseString = await response.Content.ReadAsStringAsync();
                    await this.ToastObj.Show(new ToastModel
                    {
                        Title = "EXITO!",
                        Content = "Orden de Compra "+responseString+" Generada",
                        CssClass = "e-toast-success",
                        Icon = "e-success toast-icons",
                        ShowCloseButton = false,
                        ShowProgressBar = false
                    });
                }
                proveedorescompras = await Http.GetFromJsonAsync<List<Proveedores_compras>>("api/compras/GetProveedorescompras/");

                limpia();
            }
        }
    }
}
