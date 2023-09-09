using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SupplyChain.Client.RepositoryHttp;
using SupplyChain.Client.Shared;
using SupplyChain.Shared;
using SupplyChain.Shared.Models;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Inputs;
using Syncfusion.Blazor.Navigations;
using Syncfusion.Blazor.Notifications;

namespace SupplyChain.Client.Pages.Preparacion;

public class PreparacionPageBase : ComponentBase
{
    protected List<Producto> Busquedalist = new();
    protected List<Matprove_busquedaprove> Busquedamatprove = new();
    protected bool buttondisablepreciocant = true;
    protected bool buttondisableprove = true;
    protected int CantidadMostrar = 100;
    protected List<Producto> CG_PRODlist = new();


    protected string CgMat = "";
    protected List<Compra> datapreparacion = new();
    protected List<Producto> DES_PRODlist = new();
    protected string DesMat = "";
    protected bool enableprecio;
    protected bool enablepreciocant;
    protected SfGrid<Producto> Grid2;
    protected SfGrid<Matprove_busquedaprove> Grid3;
    protected SfGrid<Compra> Gridprep;
    protected bool IsVisibleform;
    protected bool IsVisiblegrilla = true;
    protected List<Compra> listapreparacion = new();
    protected NotificacionToast NotificacionObj;


    public SfToast ToastObj;

    protected List<object> Toolbaritems = new()
    {
        new ItemModel
            { Text = "Nuevo Item", TooltipText = "Nuevo Item", PrefixIcon = "e-annotation-add", Id = "Nuevo" },
        new ItemModel { Type = ItemType.Separator },
        //new ItemModel { Text = "Abrir Preparacion", TooltipText = "Abrir Preparacion", PrefixIcon = "e-annotation-edit", Id = "Preparacion" },
        new ItemModel { Type = ItemType.Separator },
        new ItemModel
        {
            Text = "Abrir Preparacion", TooltipText = "Abrir Preparacion", PrefixIcon = "e-iconsets", Id = "Sugerencia"
        },
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

    protected string xCalle = "";
    protected decimal xCant;
    protected decimal? xCant1 = 0;
    protected decimal? xCgden = 0;
    protected int xCgProve;
    protected decimal xDescuento;
    protected string xDesProve = "";
    protected int xDiasvige;
    protected string xespecif = "";
    protected DateTime? xFeprev;
    protected string xMoneda = "";
    protected decimal xPrecio;
    protected int xRegistrocompras;
    protected string xtituloboton = "Agregar";
    protected string xUnid = "";
    protected string xUnid1 = "";
    protected string xUnidMP = "";
    [Inject] protected IRepositoryHttp HttpNew { get; set; }
    [Inject] protected HttpClient Http { get; set; }
    [Inject] protected IJSRuntime JsRuntime { get; set; }
    [Inject] protected IJSRuntime JS { get; set; }

    [CascadingParameter] public MainLayout MainLayout { get; set; }

    // variables generales
    public bool IsVisible { get; set; }
    public bool IsVisibleMatprove { get; set; }
    public bool IsVisibleProveedores { get; set; }
    public bool IsVisiblecant1 { get; set; } = true;
    protected bool ToastVisible { get; set; } = false;

    //        protected SfSpinner SpinnerObj;
    public MainLayout Layout { get; set; }

    protected override async Task OnInitializedAsync()
    {
        MainLayout.Titulo = "Preparación de Compras";
    }

    protected async Task OnInputCG_PROD(InputEventArgs args)
    {
        if (!string.IsNullOrWhiteSpace(args.Value))
        {
            DesMat = string.IsNullOrEmpty(DesMat) ? string.Empty : DesMat;
            var query = $"Codigo={args.Value.Trim()}&Descripcion={DesMat}";
            var respose = await HttpNew.GetFromJsonAsync<Producto>($"api/Prod/GetByFilter?{query}");

            if (respose.Response != null)
            {
                xUnid = respose.Response.UNID;
                xUnidMP = respose.Response.UNID;
                xespecif = respose.Response.ESPECIF;
                xUnid1 = respose.Response.UNIDSEG;
                xCgden = respose.Response.CG_DENSEG;
                DesMat = respose.Response.DES_PROD;
                cambiapaso(2);
            }
            else
            {
                xUnid = "";
                xUnidMP = "";
                xespecif = "";
                xUnid1 = "";
                xCgden = 0;
                DesMat = "";
                cambiapaso(1);
            }
        }
    }


    protected async Task OnInputDES_PROD(InputEventArgs args)
    {
        if (args.Value != "")
        {
            CG_PRODlist = await Http.GetFromJsonAsync<List<Producto>>($"api/Prod/BuscarPorDES_PROD_PREP/{args.Value}");
            if (CG_PRODlist.Count > 0)
            {
                CgMat = CG_PRODlist.FirstOrDefault().Id;
                xUnid = CG_PRODlist.FirstOrDefault().UNID;
                xUnidMP = CG_PRODlist.FirstOrDefault().UNID;
                xespecif = CG_PRODlist.FirstOrDefault().ESPECIF;
                cambiapaso(2);
            }
            else
            {
                CgMat = "";
                xUnid = "";
                xUnidMP = "";
                xespecif = "";
                cambiapaso(1);
            }
        }
    }

    protected async Task BuscarProductoPreparacion()
    {
        var url = string.Empty;
        if (string.IsNullOrEmpty(DesMat) && string.IsNullOrEmpty(CgMat))
        {
            Busquedalist = await Http.GetFromJsonAsync<List<Producto>>($"api/Prod/BuscarProducto_PREP/Vacio" +
                                                                       $"/Vacio/{CantidadMostrar}");
            IsVisible = true;
        }

        if (!string.IsNullOrEmpty(DesMat) || !string.IsNullOrEmpty(CgMat))
        {
            if (string.IsNullOrEmpty(DesMat))
            {
                url = $"api/Prod/Buscar?CG_PROD={CgMat}&DES_PROD=VACIO&Busqueda={CantidadMostrar}";
                //Busquedalist = await Http.GetFromJsonAsync<List<Producto>>($"api/Prod/BuscarProducto_PREP/{CgMat}" +
                //    $"/Vacio/{CantidadMostrar}");

                Busquedalist = await Http.GetFromJsonAsync<List<Producto>>(url);
            }
            else if (string.IsNullOrEmpty(CgMat))
            {
                url = $"api/Prod/Buscar?CG_PROD=VACIO&DES_PROD={DesMat}&Busqueda={CantidadMostrar}";
                Busquedalist = await Http.GetFromJsonAsync<List<Producto>>(url);
            }
            else
            {
                url = $"api/Prod/Buscar?CG_PROD={CgMat}&DES_PROD={DesMat}&Busqueda={CantidadMostrar}";
                Busquedalist = await Http.GetFromJsonAsync<List<Producto>>(url);
            }

            IsVisible = true;
        }
    }

    public void OnSelected()
    {
        CgMat = Grid2.GetSelectedRecords().Result.FirstOrDefault().Id; // return the details of selected record
        DesMat = Grid2.GetSelectedRecords().Result.FirstOrDefault().DES_PROD;
        xUnidMP = Grid2.GetSelectedRecords().Result.FirstOrDefault().UNID;
        xUnid = Grid2.GetSelectedRecords().Result.FirstOrDefault().UNID;
        xespecif = Grid2.GetSelectedRecords().Result.FirstOrDefault().ESPECIF;
        xUnid1 = Grid2.GetSelectedRecords().Result.FirstOrDefault().UNIDSEG;
        xCgden = Grid2.GetSelectedRecords().Result.FirstOrDefault().CG_DENSEG;
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
        if (!string.IsNullOrEmpty(CgMat))
            //                Busquedamatprove = await Http.GetFromJsonAsync<List<Matprove_busquedaprove>>($"api/Matprove/BuscarProve/{CgMat}");
            IsVisibleMatprove = true;
    }

    public void OnSelectedMatproveProve(Matprove_busquedaprove matp)
    {
//            xCgProve = this.Grid3.GetSelectedRecords().Result.FirstOrDefault().CG_PROVE; // return the details of selected record
        xCgProve = matp.NROCLTE;
        xDesProve = matp.DES_PROVE;
        xPrecio = matp.PRECIO;
        xDiasvige = matp.DIASVIGE;
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

    public void OnSelectedProve(vProveedorItris prove)
    {
        //            xCgProve = this.Grid3.GetSelectedRecords().Result.FirstOrDefault().CG_PROVE; // return the details of selected record
        xCgProve = prove.Id;
        xDesProve = prove.DESCRIPCION;
        xUnid = xUnidMP;
//            xUnid1 = "";
//            xCgden = 0;
        xMoneda = "PESOS";
        xDiasvige = 0;
        IsVisibleProveedores = false;
        xCalle = prove.CALLE;
        cambiapaso(3);
    }

    protected async Task cambiapaso(int flag)
    {
        if (flag == 0)
        {
            enableprecio = false;
            enablepreciocant = false;
            buttondisableprove = true;
            buttondisablepreciocant = true;
            CgMat = "";
            DesMat = "";
            xCgProve = 0;
            xDesProve = "";
            xCgden = 0;
            xespecif = "";
            xUnid = "";
            xUnid1 = "";
            xUnidMP = "";
            xMoneda = "PESOS";
            xDiasvige = 0;
            xCant = 0;
            xCant1 = 0;
            xPrecio = 0;
            xDescuento = 0;
            xCalle = "";
            xFeprev = DateTime.Now.Date;
        }
        else if (flag == 1)
        {
            enableprecio = false;
            enablepreciocant = false;
            buttondisableprove = true;
            buttondisablepreciocant = true;
            xCgProve = 0;
            xDesProve = "";
            xCgden = 0;
            xUnid = "";
            xUnid1 = "";
            xMoneda = "PESOS";
            xDiasvige = 0;
            xCant = 0;
            xCant1 = 0;
            xPrecio = 0;
            xDescuento = 0;
            xCalle = "";
        }
        else if (flag == 2)
        {
            enableprecio = false;
            enablepreciocant = true;
            buttondisableprove = false;
            buttondisablepreciocant = false;
        }
        else if (flag == 3)
        {
            enableprecio = true;
            enablepreciocant = true;
            buttondisableprove = false;
            buttondisablepreciocant = false;
        }

        calculacant1();
    }

    protected async Task Agregaritem()
    {
        if (CgMat == "")
        {
            await ToastObj.ShowAsync(new ToastModel
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
            await ToastObj.ShowAsync(new ToastModel
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
        var itemagregarcompras = new Compra();

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
            itemagregarcompras.PRECIONETO = Math.Round(xPrecio * (1 - xDescuento / 100), 2);
        else
            itemagregarcompras.PRECIONETO = xPrecio;

        itemagregarcompras.ESPECIFICA = xespecif;
        if (xUnid != xUnid1 && xCgden > 0)
            itemagregarcompras.AUTORIZADO = xCant / xCgden;
        else
            itemagregarcompras.AUTORIZADO = xCant;
        itemagregarcompras.PRECIOTOT = xPrecio * itemagregarcompras.AUTORIZADO;
        itemagregarcompras.NROCLTE = xCgProve;
        itemagregarcompras.DES_PROVE = xDesProve;
        itemagregarcompras.FE_PREV = xFeprev;
        itemagregarcompras.FE_VENC = DateTime.Now.Date;
        itemagregarcompras.CONDVEN = "";
        itemagregarcompras.CG_CIA = 1;
        itemagregarcompras.USUARIO = "USER";
        itemagregarcompras.FE_REG = DateTime.Now.Date;
        itemagregarcompras.Id = xRegistrocompras;

        HttpResponseMessage response = null;
        if (xRegistrocompras > 0)
            response = await Http.PutAsJsonAsync("api/compras/actualizaitem/" + xRegistrocompras, itemagregarcompras);
        else
            response = await Http.PostAsJsonAsync("api/compras/agregaitem", itemagregarcompras);
        if (response.StatusCode == HttpStatusCode.BadRequest
            || response.StatusCode == HttpStatusCode.NotFound
            || response.StatusCode == HttpStatusCode.Conflict)
        {
            var mensServidor = await response.Content.ReadAsStringAsync();

            Console.WriteLine($"Error: {mensServidor}");
            //toastService.ShowToast($"{mensServidor}", TipoAlerta.Error);
            await ToastObj.Show(new ToastModel
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
            if (response.StatusCode == HttpStatusCode.OK)
            {
                await ToastObj.ShowAsync(new ToastModel
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
                IsVisibleform = false;
                IsVisiblegrilla = true;
            }
        }
    }


    protected async Task Editar()
    {
        if (Gridprep.SelectedRecords.Count > 0)
        {
            foreach (var selectedRecord in Gridprep.SelectedRecords)
            {
                xRegistrocompras = selectedRecord.Id;
                CgMat = selectedRecord.CG_MAT.Trim();
                xFeprev = selectedRecord.FE_PREV;
                DesMat = selectedRecord.DES_MAT;
                xCgProve = selectedRecord.NROCLTE;
                xDesProve = selectedRecord.DES_PROVE;
                xUnid = selectedRecord.UNID;
                xUnid1 = selectedRecord.UNID1;
                xUnidMP = selectedRecord.UNID;
                xMoneda = selectedRecord.MONEDA;
                xDiasvige = selectedRecord.DIASVIGE;
                xespecif = selectedRecord.ESPECIFICA;
                if (string.IsNullOrEmpty(xespecif))
                {
                    await BuscarProductoPreparacion();
                    IsVisible = false;
                    if (Busquedalist is not null && Busquedalist.Count == 1) xespecif = Busquedalist[0].ESPECIF.Trim();
                }


                xCgden = selectedRecord.CG_DEN.GetValueOrDefault();
                xCant = selectedRecord.SOLICITADO.GetValueOrDefault();
                xFeprev = selectedRecord.FE_PREV.GetValueOrDefault();
                if (xCant == 0) xCant = selectedRecord.NECESARIO.GetValueOrDefault();
                xPrecio = selectedRecord.PRECIO.GetValueOrDefault();
                xDescuento = selectedRecord.DESCUENTO.GetValueOrDefault();
            }

            if (xCgProve > 0)
                cambiapaso(3);
            else
                cambiapaso(2);
            xtituloboton = "Actualizar";
            IsVisibleform = true;
            IsVisiblegrilla = false;
        }
    }

    protected async Task DoubleClickHandler(RecordDoubleClickEventArgs<Compra> args)
    {
        await Editar();
    }

    public async Task ClickHandler(ClickEventArgs args)
    {
        if (args.Item.Text == "ExcelExport") await Gridprep.ExcelExport();

        if (args.Item.Text == "Editar") await Editar();
        if (args.Item.Id == "Nuevo")
        {
            cambiapaso(0);
            IsVisibleform = true;
            IsVisiblegrilla = false;
        }

        //if (args.Item.Id == "Preparacion")
        //{
        //    await cargapreparacion();
        //}
        if (args.Item.Id == "Sugerencia") await Cargasugerencia();
        if (args.Item.Id == "Eliminar")
        {
            xRegistrocompras = 0;
            if (Gridprep.SelectedRecords.Count > 0)
                foreach (var selectedRecord in Gridprep.SelectedRecords)
                    xRegistrocompras = selectedRecord.Id;
            if (xRegistrocompras > 0)
                try
                {
                    var isConfirmed =
                        await JsRuntime.InvokeAsync<bool>("confirm", "Seguro de que desea eliminar el registro?");
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

    public async Task Cargasugerencia()
    {
        try
        {
            listapreparacion = await Http.GetFromJsonAsync<List<Compra>>("api/Compras/GetSugerencia/");

            var lista_prep = await Http.GetFromJsonAsync<List<Compra>>("api/Compras/GetPreparacion/0");

            listapreparacion.AddRange(lista_prep);
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
        if (xUnid != xUnid1 && xCgden > 0)
        {
            IsVisiblecant1 = false;
            if (xCant > 0) xCant1 = xCant / xCgden;
        }
    }

    protected void volvergrilla()
    {
        IsVisibleform = false;
        IsVisiblegrilla = true;
    }
}