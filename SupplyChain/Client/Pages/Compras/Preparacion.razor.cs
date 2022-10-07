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


namespace SupplyChain.Client.Pages.Preparacion
{
    public class PreparacionPageBase : ComponentBase
    {
        [Inject] protected HttpClient Http { get; set; }
        [Inject] protected IJSRuntime JsRuntime { get; set; }
        [Inject] protected Microsoft.JSInterop.IJSRuntime JS { get; set; }

        [CascadingParameter] public MainLayout MainLayout { get; set; }

        // variables generales
        public bool IsVisible { get; set; } = false;
        public bool IsVisibleMatprove { get; set; } = false;
        public bool IsVisibleProveedores { get; set; } = false;
        public bool IsVisiblecant1 { get; set; } = true;

        
        public SfToast ToastObj;
        protected NotificacionToast NotificacionObj;
        protected bool ToastVisible { get; set; } = false;

        
        protected string CgMat = "";
        protected DateTime? xFeprev;
        protected string DesMat = "";
        protected int xCgProve = 0;
        protected int xRegistrocompras = 0;
        protected string xDesProve = "";
        protected string xtituloboton = "Agregar";
        protected decimal xPrecio = 0;
        protected decimal xCant = 0;
        protected decimal xCant1 = 0;
        protected decimal xCgden = 0;
        protected string xUnid = "";
        protected string xUnid1 = "";
        protected string xUnidMP = "";
        protected string xMoneda = "";
        protected bool enablepreciocant = false;
        protected bool buttondisableprove = true;
        protected bool buttondisablepreciocant = true;
        protected List<Producto> CG_PRODlist = new();
        protected List<Producto> DES_PRODlist = new();
        protected List<Producto> Busquedalist = new();
        protected List<Matprove_busquedaprove> Busquedamatprove = new();
        protected List<Compra> listapreparacion = new();
        protected List<Compra> datapreparacion = new();
        protected int CantidadMostrar = 100;
        protected SfGrid<Producto> Grid2;
        protected SfGrid<Matprove_busquedaprove> Grid3;
        protected SfGrid<Compra> Gridprep;

        protected List<Object> Toolbaritems = new List<Object>(){
        new ItemModel { Text = "Abrir Preparacion", TooltipText = "Abrir Preparacion", PrefixIcon = "e-annotation-edit", Id = "Preparacion" },
        new ItemModel { Type = ItemType.Separator },
        new ItemModel { Text = "Abrir Sugerencia", TooltipText = "Abrir Sugerencia", PrefixIcon = "e-iconsets", Id = "Sugerencia" },
        new ItemModel { Type = ItemType.Separator },
        new ItemModel { Text = "Editar", TooltipText = "Editar", PrefixIcon = "e-edit", Id = "Editar" },
        new ItemModel { Type = ItemType.Separator },
        new ItemModel { Text = "Eliminar", TooltipText = "Eliminar", PrefixIcon = "e-delete", Id = "Eliminar" },
        new ItemModel { Type = ItemType.Separator },
        "Search",
//        new ItemModel { Type = ItemType.Separator },
//        "Print",
        new ItemModel { Type = ItemType.Separator },
        "ExcelExport"
        };

        //        protected SfSpinner SpinnerObj;
        public MainLayout Layout { get; set; }
        protected override async Task OnInitializedAsync()
        {
            MainLayout.Titulo = "Preparación de Compras";

        }

        protected async Task OnInputCG_PROD(InputEventArgs args)
        {
            if (args.Value != "")
            {
                CG_PRODlist = await Http.GetFromJsonAsync<List<Producto>>($"api/Prod/BuscarPorCG_PROD/{args.Value}");
                if (CG_PRODlist.Count > 0)
                {
                    DesMat = CG_PRODlist.FirstOrDefault().DES_PROD;
                    xUnidMP = CG_PRODlist.FirstOrDefault().UNID;
                    cambiapaso(2);
                }
                else
                {
                    DesMat = "";
                    xUnidMP = "";
                    cambiapaso(1);
                }
            }
        }

        protected async Task OnInputDES_PROD(InputEventArgs args)
        {
            if (args.Value != "")
            {
                CG_PRODlist = await Http.GetFromJsonAsync<List<Producto>>($"api/Prod/BuscarPorDES_PROD/{args.Value}");
                if (CG_PRODlist.Count > 0)
                {
                    CgMat = CG_PRODlist.FirstOrDefault().Id;
                    xUnidMP = CG_PRODlist.FirstOrDefault().UNID;
                    cambiapaso(2);
                }
                else
                {
                    CgMat = "";
                    xUnidMP = "";
                    cambiapaso(1);
                }
            }
        }

        protected async Task BuscarProductoPreparacion()
        {
            if (string.IsNullOrEmpty(DesMat) && string.IsNullOrEmpty(CgMat))
            {
                Busquedalist = await Http.GetFromJsonAsync<List<Producto>>($"api/Prod/BuscarProducto/Vacio" +
                        $"/Vacio/{CantidadMostrar}");
                IsVisible = true;
            }
            if (!string.IsNullOrEmpty(DesMat) || !string.IsNullOrEmpty(CgMat))
            {
                if (string.IsNullOrEmpty(DesMat))
                {
                    Busquedalist = await Http.GetFromJsonAsync<List<Producto>>($"api/Prod/BuscarProducto/{CgMat}" +
                        $"/Vacio/{CantidadMostrar}");
                }
                else if (string.IsNullOrEmpty(CgMat))
                {
                    Busquedalist = await Http.GetFromJsonAsync<List<Producto>>($"api/ProdBuscarProducto/Vacio/{DesMat}/{CantidadMostrar}");
                }
                else
                {
                    Busquedalist = await Http.GetFromJsonAsync<List<Producto>>($"api/Prod/BuscarProducto/{CgMat}/{DesMat}/{CantidadMostrar}");
                }
                IsVisible = true;
            }
        }

        public void OnSelected()
        {
            CgMat = this.Grid2.GetSelectedRecords().Result.FirstOrDefault().Id; // return the details of selected record
            DesMat = this.Grid2.GetSelectedRecords().Result.FirstOrDefault().DES_PROD; 
            xUnidMP = this.Grid2.GetSelectedRecords().Result.FirstOrDefault().UNID; 
            IsVisible = false;
            cambiapaso(2);
        }

        protected async Task AgregarValores()
        {
            CantidadMostrar = CantidadMostrar + 100;

            await BuscarProductoPreparacion();
        }


        protected async Task BuscarProveedorMatprovePreparacion()
        {
            if ( ! string.IsNullOrEmpty(CgMat))
            {
//                Busquedamatprove = await Http.GetFromJsonAsync<List<Matprove_busquedaprove>>($"api/Matprove/BuscarProve/{CgMat}");
                IsVisibleMatprove = true;
            }
        }
        public void OnSelectedMatproveProve(Matprove_busquedaprove matp)
        {

//            xCgProve = this.Grid3.GetSelectedRecords().Result.FirstOrDefault().CG_PROVE; // return the details of selected record
            xCgProve = matp.NROCLTE;
            xDesProve = matp.DES_PROVE;
            xPrecio = matp.PRECIO;
            xUnid = matp.UNID;
            xUnid1 = matp.UNID1;
            xMoneda = matp.MONEDA;
            xCgden = matp.CG_DEN;
            IsVisibleMatprove = false;
            cambiapaso(3);
        }

        protected async Task BuscarProveedores()
        {
            IsVisibleProveedores = true;
        }

        public void OnSelectedProve(Proveedor prove)
        {

            //            xCgProve = this.Grid3.GetSelectedRecords().Result.FirstOrDefault().CG_PROVE; // return the details of selected record
            xCgProve = prove.Id;
            xDesProve = prove.DES_PROVE;
            xUnid = xUnidMP;
            xUnid1 = "";
            xCgden = 0;
            xMoneda = "PESOS";
            IsVisibleProveedores = false;
            cambiapaso(3);
        }

        protected async Task cambiapaso(int flag)
        {
            if (flag == 0)
            {
                enablepreciocant = false;
                buttondisableprove = true;
                buttondisablepreciocant = true;
                CgMat = "";
                DesMat = "";
                xCgProve = 0;
                xDesProve = "";
                xCgden = 0;
                xUnid = "";
                xUnid1 = "";
                xUnidMP = "";
                xMoneda = "PESOS";
                xCant = 0;
                xCant1 = 0;
                xPrecio = 0;
            }
            else if (flag == 1)
            {
                enablepreciocant = false;
                buttondisableprove = true;
                buttondisablepreciocant = true;
                xCgProve = 0;
                xDesProve = "";
                xCgden = 0;
                xUnid = "";
                xUnid1 = "";
                xMoneda = "PESOS";
                xCant = 0;
                xCant1 = 0;
                xPrecio = 0;
            }
            else if (flag == 2)
            {
                enablepreciocant = false;
                buttondisableprove = false;
                buttondisablepreciocant = true;
            }
            else if (flag == 3)
            {
                enablepreciocant = true;
                buttondisableprove = false;
                buttondisablepreciocant = false;
            }
            calculacant1();
        }
        protected async Task Agregaritem()
        {
            if ( CgMat == "")
            {
                await this.ToastObj.Show(new ToastModel
                {
                    Title = "ERROR!",
                    Content = "Debe Indicar un Insumo",
                    CssClass = "e-toast-danger",
                    Icon = "e-error toast-icons",
                    ShowCloseButton = true,
                    ShowProgressBar = true
                });
                return;
            }
            if (xCant == 0)
            {
                await this.ToastObj.Show(new ToastModel
                {
                    Title = "ERROR!",
                    Content = "Debe Indicar la cantidad",
                    CssClass = "e-toast-danger",
                    Icon = "e-error toast-icons",
                    ShowCloseButton = true,
                    ShowProgressBar = true
                });
                return;
            }
            /*
            if (xCgProve == 0)
            {
                await this.ToastObj.Show(new ToastModel
                {
                    Title = "ERROR!",
                    Content = "Debe Indicar un Proveedor",
                    CssClass = "e-toast-danger",
                    Icon = "e-error toast-icons",
                    ShowCloseButton = true,
                    ShowProgressBar = true
                });
                return;
            }
            if (xPrecio == 0)
            {
                await this.ToastObj.Show(new ToastModel
                {
                    Title = "ERROR!",
                    Content = "Debe Indicar el precio",
                    CssClass = "e-toast-danger",
                    Icon = "e-error toast-icons",
                    ShowCloseButton = true,
                    ShowProgressBar = true
                });
                return;
            }
            */
            //Console.WriteLine(itemagregarcompras);
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
            itemagregarcompras.CG_DEN = xCgden;
            itemagregarcompras.PRECIO = xPrecio;
            itemagregarcompras.PRECIONETO = xPrecio;
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
            itemagregarcompras.FE_VENC = DateTime.Now.Date;
            itemagregarcompras.CONDVEN = "";
            itemagregarcompras.ESPECIFICA = "";
            itemagregarcompras.CG_CIA = 1;
            itemagregarcompras.USUARIO = "USER";
            itemagregarcompras.FE_REG = DateTime.Now.Date;
            itemagregarcompras.Id = xRegistrocompras;

            HttpResponseMessage response = null;
            if (xRegistrocompras > 0) {
                response = await Http.PutAsJsonAsync("api/compras/actualizaitem/" + xRegistrocompras, itemagregarcompras);

            }
            else
            {
                response = await Http.PostAsJsonAsync("api/compras/agregaitem", itemagregarcompras);
            }
            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest
                || response.StatusCode == System.Net.HttpStatusCode.NotFound
                || response.StatusCode == System.Net.HttpStatusCode.Conflict)
            {
                var mensServidor = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"Error: {mensServidor}");
                //toastService.ShowToast($"{mensServidor}", TipoAlerta.Error);
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
                    await this.ToastObj.Show(new ToastModel
                    {
                        Title = "EXITO!",
                        Content = "Item Agregado a preparación Correctamente.",
                        CssClass = "e-toast-success",
                        Icon = "e-success toast-icons",
                        ShowCloseButton = false,
                        ShowProgressBar = false
                    });
                    cambiapaso(0);
                    xtituloboton = "Agregar";
                    await cargapreparacion();
                    xRegistrocompras = 0;
                }
            }
        }

        public async Task ClickHandler(ClickEventArgs args)
        {
            if (args.Item.Text == "ExcelExport")
            {
                await this.Gridprep.ExcelExport();
            }

            if (args.Item.Text == "Editar")
            {
                if (this.Gridprep.SelectedRecords.Count > 0)
                {
                    foreach (Compra selectedRecord in this.Gridprep.SelectedRecords)
                    {
                        xRegistrocompras = selectedRecord.Id;
                        CgMat = selectedRecord.CG_MAT;
                        xFeprev = selectedRecord.FE_PREV;
                        DesMat = selectedRecord.DES_MAT;
                        xCgProve = selectedRecord.NROCLTE;
                        xDesProve = selectedRecord.DES_PROVE;
                        xUnid = selectedRecord.UNID;
                        xUnid1 = selectedRecord.UNID1;
                        xUnidMP = selectedRecord.UNID;
                        xMoneda = selectedRecord.MONEDA;
                        xCgden = selectedRecord.CG_DEN.GetValueOrDefault();
                        xCant = selectedRecord.SOLICITADO.GetValueOrDefault();
                        if ( xCant == 0)
                        {
                            xCant = selectedRecord.NECESARIO.GetValueOrDefault();
                        }
                        xPrecio = selectedRecord.PRECIO.GetValueOrDefault();
                    }

                    cambiapaso(3);
                    xtituloboton = "Actualizar";
                }
            }
            if (args.Item.Id == "Preparacion")
            {
                await cargapreparacion();
            }
            if (args.Item.Id == "Sugerencia")
            {
                await cargasugerencia();
            }
            if (args.Item.Id == "Eliminar")
            {
                xRegistrocompras = 0;
                if (this.Gridprep.SelectedRecords.Count > 0)
                {
                    foreach (Compra selectedRecord in this.Gridprep.SelectedRecords)
                    {
                        xRegistrocompras = selectedRecord.Id;
                    }

                }
                if (xRegistrocompras > 0)
                {

                    try
                    {
                        bool isConfirmed = await JsRuntime.InvokeAsync<bool>("confirm", "Seguro de que desea eliminar el registro?");
                        if (isConfirmed)
                        {
                            //servicios.Remove(servicios.Find(m => m.PEDIDO == args.Data.PEDIDO));
                            await Http.DeleteAsync($"api/Compras/{xRegistrocompras}");
                            await cargapreparacion();
                            xRegistrocompras = 0;

                        }
                    }
                    catch (Exception ex)
                    {

                    }


                }


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
        public async Task cargasugerencia()
        {
            try
            {
                listapreparacion = await Http.GetFromJsonAsync<List<Compra>>("api/Compras/GetSugerencia/");
            }
            catch (Exception e)
            {
                //return BadRequest(e);
            }

//            datapreparacion = await Http.GetFromJsonAsync<List<Compra>>("api/Compras/GetSugerencia/");
  //          RefrescaLista();
        }

        public void RefrescaLista()
        {
            listapreparacion = datapreparacion;
            Gridprep.Refresh();
        }

        protected void calculacant1()
        {
            IsVisiblecant1 = true;
            xCant1 = 0;
            if ( xUnid != xUnid1 && xCgden > 0)
            {
                IsVisiblecant1 = false;
                if (xCant > 0 )
                {
                    xCant1 = xCant / xCgden;
                }
            }
        }

    }
}
