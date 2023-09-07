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
using SupplyChain.Shared;
using Syncfusion.Blazor.Kanban.Internal;
using System.Drawing;
using Syncfusion.Blazor.SplitButtons;
using System.Text.Json;

namespace SupplyChain.Client.Pages.Emision
{
    public class EmisionPageBase : ComponentBase
    {
        [Inject] protected HttpClient Http { get; set; }
        [Inject] protected IJSRuntime JsRuntime { get; set; }
        [Inject] protected Microsoft.JSInterop.IJSRuntime JS { get; set; }
        [Inject] protected IRepositoryHttp repositoryHttp2 { get; set; }


        [CascadingParameter] public MainLayout MainLayout { get; set; }

        // variables generales
        public bool Anularoc { get; set; } = false;
        public bool Anularpp { get; set; } = false;

        public bool IsVisibleguarda { get; set; } = false;
        public bool IsVisibleimprime { get; set; } = true;
        public bool IsVisibleAnular { get; set; } = true;
        public bool ocagenerar { get; set; } = true;
        public bool ocabierta { get; set; } = false;
        public string proveocabierta { get; set; } = "";
        public int onumero { get; set; } = 0;
        public int numeroorden { get; set; } = 0;
        public Compra item { get; set; } = new Compra();
        public Compra item2 { get; set; } = new Compra();
        public decimal? bonif { get; set; } = 0;
        public string listaordenescompra { get; set; } = "";

        public SfToast ToastObj;
        protected NotificacionToast NotificacionObj;
        protected bool ToastVisible { get; set; } = false;

        protected List<vCondicionesPago> condiconespago = new List<vCondicionesPago>();
        protected List<Proveedores_compras> proveedorescompras = new List<Proveedores_compras>();
        protected List<Compra> insumosproveedor = new();
        protected List<Compra> ordenSeleccionada = new();
        protected SfGrid<Compra> GridProve;
        protected List<Compra> listapreparacion = new();



        public string xespecif = "";
        public string xespecif2 = "";

        public string DropVal = "";
        public string xcondven = "";
        public string Obs = "";

        protected string CgMat = "";
        protected DateTime? xFeprev;
        protected string DesMat = "";
        // string xespecif = "";
        protected int xCgProve = 0;
        protected int xRegistrocompras = 0;
        protected string xDesProve = "";
        protected string xtituloboton = "Agregar";
        protected decimal xPrecio = 0;
        protected decimal xDescuento = 0;
        protected decimal xCant = 0;
        protected decimal? xCant1 = 0;
        protected decimal? xCgden = 0;
        protected int xDiasvige = 0;
        protected string xUnid = "";
        protected string xUnid1 = "";
        protected string xUnidMP = "";
        protected string xMoneda = "";
        protected bool enableprecio = false;


        protected BuscadorEmergente<Compra> Buscador;
        protected Compra[] ItemsABuscar = null;
        protected string[] ColumnasBuscador = new string[] { "NUMERO", "FE_EMIT", "CG_MAT", "SOLICITADO", "UNID", "DES_PROVE" };
        protected string TituloBuscador = "Seleccion de Orden de Compra para apertura";
        protected bool PopupBuscadorVisible = false;


        protected Dictionary<string, object> HtmlAttribute = new Dictionary<string, object>()
        {
                {"type", "button" }
        };

        public MainLayout Layout { get; set; }
        protected override async Task OnInitializedAsync()
        {
            MainLayout.Titulo = "Emisión de Ordenes de Compras";

            condiconespago = await Http.GetFromJsonAsync<List<vCondicionesPago>>("api/Condven/Itris");


            proveedorescompras = await Http.GetFromJsonAsync<List<Proveedores_compras>>("api/compras/GetProveedorescompras/");

        }

        //        public void SeleccionProveedor(Syncfusion.Blazor.DropDowns.ChangeEventArgs<int, Proveedores_compras> args)
        public async Task SeleccionProveedor(Syncfusion.Blazor.DropDowns.ChangeEventArgs<int, Proveedores_compras> args)
        {
            if (args.Value > 0)
            {
                limpia();
                insumosproveedor = await Http.GetFromJsonAsync<List<Compra>>("api/Compras/GetPreparacion/" + args.Value);
                IsVisibleguarda = false;
                IsVisibleimprime = true;
                IsVisibleAnular = true;
                if (insumosproveedor.Count > 0)
                {
                    DropVal = insumosproveedor[0].CONDVEN;
                }

            }
        }
        public async Task limpia()
        {
            ocabierta = false;
            ocagenerar = true;
            insumosproveedor = new();
            item = new();
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
            item = compra;
            PopupBuscadorVisible = false;
            await Buscador.HideAsync();
            BuscarOC();
        }

        protected async Task BuscarOC()
        {
            if (item is null || item.NUMERO == 0)
            {
                await this.ToastObj.ShowAsync(new ToastModel
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
                insumosproveedor = await Http.GetFromJsonAsync<List<Compra>>("api/compras/GetCompraByNumero/" + item.NUMERO);
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



        public async Task anularpp()
        {
            if (item is null || item.NUMERO == 0)
            {
                await this.ToastObj.ShowAsync(new ToastModel
                {
                    Title = "ERROR!",
                    Content = "Debe seleccionar la Orden de Compra",
                    CssClass = "e-toast-danger",
                    Icon = "e-errror toast-icons",
                    ShowCloseButton = true,
                    ShowProgressBar = true
                });
            }
            else
            {
                var orden = await Http.PutAsJsonAsync($"api/compras/anulacionpp/{item.NUMERO}", item);
                Anularpp = true;
                await this.ToastObj.ShowAsync(new ToastModel
                {
                    Title = "EXITO!",
                    Content = "Orden de Compra Anulada",
                    Icon = "e-succes toast-icons",
                    ShowCloseButton = true,
                    ShowProgressBar = true
                });
                Compra itemagregarcompras = new Compra();

                itemagregarcompras.NUMERO = 0;
                itemagregarcompras.FE_EMIT = DateTime.Now.Date;
                itemagregarcompras.CG_ORDEN = 4;
                itemagregarcompras.CG_MAT = CgMat;
                itemagregarcompras.DES_MAT = DesMat;
                itemagregarcompras.SOLICITADO = xCant;
                itemagregarcompras.UNID = xUnid;
                itemagregarcompras.UNID1 = xUnid1;
                itemagregarcompras.MONEDA = xMoneda;
                itemagregarcompras.DIASVIGE = xDiasvige;
                itemagregarcompras.CG_DEN = xCgden;
                itemagregarcompras.PRECIO = xPrecio;
                itemagregarcompras.DESCUENTO = xDescuento;

                if (xDescuento > 0)
                {
                    itemagregarcompras.PRECIONETO = Math.Round(xPrecio * (1 - (xDescuento / 100)), 2);
                }
                else
                {
                    itemagregarcompras.PRECIONETO = xPrecio;
                }
                itemagregarcompras.ESPECIFICA = xespecif;
                if (xUnid != xUnid1 && xCgden > 0)
                {
                    itemagregarcompras.AUTORIZADO = xCant / xCgden;
                }
                else
                {
                    itemagregarcompras.AUTORIZADO = xCant;
                }
                itemagregarcompras.PRECIOTOT = xPrecio * itemagregarcompras.AUTORIZADO;
                itemagregarcompras.NROCLTE = xCgProve;
                itemagregarcompras.DES_PROVE = xDesProve;
                itemagregarcompras.FE_PREV = DateTime.Now.Date;
                itemagregarcompras.CONDVEN = "";
                itemagregarcompras.CG_CIA = 1;
                itemagregarcompras.USUARIO = "USER";
                itemagregarcompras.FE_REG = DateTime.Now.Date;
                itemagregarcompras.Id = xRegistrocompras;

                HttpResponseMessage response = null;
                if (xRegistrocompras > 0)
                {
                    response = await Http.PutAsJsonAsync("api/compras/actualizaitem/" + xRegistrocompras, itemagregarcompras);
                }
                else
                {
                    response = await Http.PostAsJsonAsync("api/compras/agregaritem", itemagregarcompras);
                }
                if (response.StatusCode == System.Net.HttpStatusCode.BadRequest
                    || response.StatusCode == System.Net.HttpStatusCode.NotFound
                    || response.StatusCode == System.Net.HttpStatusCode.Conflict)
                {
                    var mensServidor = await response.Content.ReadAsStringAsync();

                    Console.WriteLine($"Error:{mensServidor}");
                    await this.ToastObj.Show(new ToastModel
                    {
                        Title = "ERROR!",
                        Content = "Error al agregar item",
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
                            Content = "Item Agregado a preparación Correctamente.",
                            CssClass = "e-toast-success",
                            Icon = "e-success toast-icons",
                            ShowCloseButton = false,
                            ShowProgressBar = false
                        });
                        xtituloboton = "Agregar";
                        await cargapreparacion();
                        xRegistrocompras = 0;

                    }
                }
                if (xRegistrocompras > 0)
                {
                    response = await Http.PostAsJsonAsync("api/compras/agregaritem", itemagregarcompras);
                }
                limpia();

            }
        }

        public async Task itemseleccionado(MenuEventArgs args)
        {
            if (args.Item.Text == "Anular OC")
            {
                await anularoc();
            }
            else if (args.Item.Text == "Anular PP")
            {
                await anularpp();
            }
        }

        public async Task anularoc()
        {
            if (item is null || item.NUMERO == 0)
            {
                await this.ToastObj.ShowAsync(new ToastModel
                {
                    Title = "ERROR!",
                    Content = "Debe seleccionar la Orden de Compra",
                    CssClass = "e-toast-danger",
                    Icon = "e-errror toast-icons",
                    ShowCloseButton = true,
                    ShowProgressBar = true
                });
                //CAMVBIAR ORDEN DEL IF
                //VERIFICA ANTES
            }

            else
            {

                var orden = await Http.PutAsJsonAsync($"api/compras/anulacionoc/{item.NUMERO}", item);
                Anularoc = true;


                await this.ToastObj.ShowAsync(new ToastModel
                {

                    Title = "EXITO!",
                    Content = "Orden de Compra Anulada",
                    Icon = "e-succes toast-icons",
                    ShowCloseButton = true,
                    ShowProgressBar = true
                });
                Compra itemagregarcompras = new Compra();

                itemagregarcompras.NUMERO = 0;
                itemagregarcompras.FE_EMIT = DateTime.Now.Date;
                itemagregarcompras.CG_ORDEN = 4;
                itemagregarcompras.CG_MAT = CgMat;
                itemagregarcompras.DES_MAT = DesMat;
                itemagregarcompras.SOLICITADO = xCant;
                itemagregarcompras.UNID = xUnid;
                itemagregarcompras.UNID1 = xUnid1;
                itemagregarcompras.MONEDA = xMoneda;
                itemagregarcompras.DIASVIGE = xDiasvige;
                itemagregarcompras.CG_DEN = xCgden;
                itemagregarcompras.PRECIO = xPrecio;
                itemagregarcompras.DESCUENTO = xDescuento;

                if (xDescuento > 0)
                {
                    itemagregarcompras.PRECIONETO = Math.Round(xPrecio * (1 - (xDescuento / 100)), 2);
                }
                else
                {
                    itemagregarcompras.PRECIONETO = xPrecio;
                }
                itemagregarcompras.ESPECIFICA = xespecif;
                if (xUnid != xUnid1 && xCgden > 0)
                {
                    itemagregarcompras.AUTORIZADO = xCant / xCgden;
                }
                else
                {
                    itemagregarcompras.AUTORIZADO = xCant;
                }
                itemagregarcompras.PRECIOTOT = xPrecio * itemagregarcompras.AUTORIZADO;
                itemagregarcompras.NROCLTE = xCgProve;
                itemagregarcompras.DES_PROVE = xDesProve;
                itemagregarcompras.FE_PREV = DateTime.Now.Date;
                itemagregarcompras.CONDVEN = "";
                itemagregarcompras.CG_CIA = 1;
                itemagregarcompras.USUARIO = "USER";
                itemagregarcompras.FE_REG = DateTime.Now.Date;
                itemagregarcompras.Id = xRegistrocompras;

                HttpResponseMessage response = null;
                if (xRegistrocompras > 0)
                {
                    response = await Http.PutAsJsonAsync("api/compras/actualizaitem/" + xRegistrocompras, itemagregarcompras);
                }
                else
                {
                    response = await Http.PostAsJsonAsync("api/compras/agregaritem", itemagregarcompras);
                }
                if (response.StatusCode == System.Net.HttpStatusCode.BadRequest
                    || response.StatusCode == System.Net.HttpStatusCode.NotFound
                    || response.StatusCode == System.Net.HttpStatusCode.Conflict)
                {
                    var mensServidor = await response.Content.ReadAsStringAsync();

                    Console.WriteLine($"Error:{mensServidor}");
                    await this.ToastObj.Show(new ToastModel
                    {
                        Title = "ERROR!",
                        Content = "Error al agregar item",
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
                            Content = "Item Agregado a preparación Correctamente.",
                            CssClass = "e-toast-success",
                            Icon = "e-success toast-icons",
                            ShowCloseButton = false,
                            ShowProgressBar = false
                        });
                        xtituloboton = "Agregar";
                        await cargapreparacion();
                        xRegistrocompras = 0;

                    }
                }
                if (xRegistrocompras > 0)
                {
                    response = await Http.PostAsJsonAsync("api/compras/agregaritem", itemagregarcompras);
                }
                limpia();

            }
        }

        public async Task cargapreparacion()
        {
            try
            {
                listapreparacion = await Http.GetFromJsonAsync<List<Compra>>("api/Compras/GetPreparacion/0");
            }
            catch (Exception e)
            {
                //return BadRequest(e);
            }

            //datapreparacion = await Http.GetFromJsonAsync<List<Compra>>("api/Compras/GetPreparacion/");
            //RefrescaLista();
        }


        public async Task imprimiroc()
        {
            if (item is null || item.NUMERO == 0)
            {
                await this.ToastObj.ShowAsync(new ToastModel
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
                graphics.DrawString("Orden de Compra: " + item.NUMERO.ToString(), font, PdfBrushes.Black, new Syncfusion.Drawing.PointF(0, 0));

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

                await JS.InvokeVoidAsync("open", new object[2] { $"/api/ReportRDLC/GetReportOC?numero={item.NUMERO}", "_blank" });

            }
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
            response = await Http.PutAsJsonAsync("api/compras/actualizaoc/" + listaordenescompra + '/' + xespecif2 + '/' + xcondven + '/' + bonif, listaordenescompra);

            if (!response.IsSuccessStatusCode)
            {
                var mensServidor = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"Error: {mensServidor}");
                await this.ToastObj.ShowAsync(new ToastModel
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

                if (response.IsSuccessStatusCode)
                {
                    String responseString = await response.Content.ReadAsStringAsync();
                    //var comprasJSON = await responseCompras.HttpResponseMessage.Content.ReadAsStringAsync();
                    //var jsonSerializerOptions= new JsonSerializerOptions() { PropertyNameCaseInsensitive = true } ;
                    //item = System.Text.Json.JsonSerializer.Deserialize<Compra>(comprasJSON, jsonSerializerOptions);
                    item.NUMERO = Convert.ToInt32(responseString);
                    await this.ToastObj.ShowAsync(new ToastModel
                    {
                        Title = "EXITO!",
                        Content = "Orden de Compra " + item.NUMERO + " Generada",
                        CssClass = "e-toast-success",
                        Icon = "e-success toast-icons",
                        ShowCloseButton = false,
                        ShowProgressBar = false
                    });

                }
                listaordenescompra = listaordenescompra;

                //proveedorescompras = await Http.GetFromJsonAsync<List<Proveedores_compras>>("api/compras/GetProveedorescompras/");
                //PdfDocument document= new PdfDocument();
                //PdfPage page = document.Pages.Add();
                //PdfGraphics graphics= page.Graphics;
                //PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 20);
                //graphics.DrawString("Orden de Compra "+ responseCompras,  font, PdfBrushes.Black, new Syncfusion.Drawing.PointF(0, 0));
                //MemoryStream xx = new MemoryStream();
                //document.Save(xx);
                //document.Close(true);

                //await JS.InvokeVoidAsync("open", new object[2] { $"/api/ReportRDLC/GetReportOC?numero={listaordenescompra}", "_blank" });
                await imprimiroc();
                await limpia();

            }

        }
    }
}