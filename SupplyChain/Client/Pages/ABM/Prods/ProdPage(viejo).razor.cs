using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SupplyChain.Client.Shared;
using SupplyChain.Shared;
using SupplyChain.Shared.Models;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Navigations;
using Syncfusion.Blazor.Notifications;
using Syncfusion.Blazor.Spinner;

namespace SupplyChain.Pages.Prods;

public class ProdsPageBase : ComponentBase
{
    protected const string APPNAME = "grdProdABM";
    protected List<Areas> area = new();
    public bool camposConf = true;
    protected List<Cat> cat = new();
    protected List<Celdas> celda = new();
    public bool Disabled = false;

    public bool Enabled = true;
    protected SfGrid<Producto> Grid;
    public bool habilitaCodigo;
    public bool isAdding;
    protected List<Lineas> linea = new();
    protected List<Moneda> monedas = new();
    public Producto prod = new();
    public Producto prodAux = new();


    protected List<Producto> prods = new();

    protected SfSpinner refSpinner;
    public bool SpinnerVisible;
    protected string state;
    protected List<TipoArea> tipoarea = new();
    protected List<TipoMat> tipomat = new();
    protected SfToast ToastObj;

    protected List<object> Toolbaritems = new()
    {
        "Search",
        "Add",
        "Edit",
        "Delete",
        "Print",
        new ItemModel { Text = "Copy", TooltipText = "Copy", PrefixIcon = "e-copy", Id = "copy" },
        "ExcelExport"
    };

    protected List<Unidades> unidades = new();
    [Inject] protected HttpClient Http { get; set; }
    [Inject] protected IJSRuntime JsRuntime { get; set; }
    protected bool IsVisible { get; set; }
    [CascadingParameter] private MainLayout MainLayout { get; set; }

    protected override async Task OnInitializedAsync()
    {
        MainLayout.Titulo = "Productos";

        SpinnerVisible = true;
        prods = await Http.GetFromJsonAsync<List<Producto>>("api/Prod");
        prods = prods.OrderBy(s => s.CG_ORDEN).ToList();

        unidades = await Http.GetFromJsonAsync<List<Unidades>>("api/unidades");
        //tipomat = await Http.GetFromJsonAsync<List<TipoMat>>("api/TipoMat");
        monedas = await Http.GetFromJsonAsync<List<Moneda>>("api/Monedas");
        celda = await Http.GetFromJsonAsync<List<Celdas>>("api/Celdas");
        area = await Http.GetFromJsonAsync<List<Areas>>("api/Areas");
        linea = await Http.GetFromJsonAsync<List<Lineas>>("api/Lineas");
        tipoarea = await Http.GetFromJsonAsync<List<TipoArea>>("api/TipoArea");
        cat = await Http.GetFromJsonAsync<List<Cat>>("api/Cat");

        SpinnerVisible = false;
        await base.OnInitializedAsync();
    }

    protected async Task OnCompleteHandler(ActionEventArgs<Producto> args)
    {
        //Grid.PreventRender();
    }

    protected async Task OnActionBeginHandler(ActionEventArgs<Producto> args)
    {
        if (args.RequestType == Action.BeginEdit)
            //SolicitudSeleccionada = args.Data;
            args.PreventRender = false;

        if (args.RequestType == Action.Grouping
            || args.RequestType == Action.UnGrouping
            || args.RequestType == Action.ClearFiltering
            || args.RequestType == Action.CollapseAllComplete
            || args.RequestType == Action.ColumnState
            || args.RequestType == Action.ClearFiltering
            || args.RequestType == Action.Reorder
            || args.RequestType == Action.Sorting
           )
        {
            //VisibleProperty = true;
            Grid.PreventRender();
            Grid.Refresh();

            state = await Grid.GetPersistData();
            await Grid.AutoFitColumnsAsync();
            await Grid.RefreshColumns();
            await Grid.RefreshHeader();
        }
    }

    protected async Task OnActionCompleteHandler(ActionEventArgs<Producto> args)
    {
        if (args.RequestType == Action.BeginEdit)
            //SolicitudSeleccionada = args.Data;
            args.PreventRender = false;
    }

    public async Task Cerrar()
    {
        prod = new Producto();
    }

    public async Task guardarProd()
    {
        if (isAdding)
        {
            var existe = await Http.GetFromJsonAsync<bool>($"api/Prod/ExisteProducto/{prodAux.Id}");

            if (!existe && prodAux.CG_ORDEN != 1 && prodAux.CG_ORDEN != 3)
            {
                switch (prodAux.CG_ORDEN)
                {
                    case 1:
                        prodAux.EXIGESERIE = true;
                        prodAux.EXIGEOA = true;
                        break;
                    case 3:
                        prodAux.EXIGELOTE = true;
                        break;
                    case 4:
                        prodAux.EXIGEDESPACHO = true;
                        break;
                }

                var response = await Http.PostAsJsonAsync("api/Prod", prodAux);

                if (response.StatusCode == HttpStatusCode.Created)
                {
                    var prodNuevo = await response.Content.ReadFromJsonAsync<Producto>();
                    prods.Add(prodNuevo);
                    prods.OrderByDescending(p => p.Id);
                    await Grid.RefreshColumns();
                    Grid.Refresh();
                    await Grid.RefreshHeader();

                    await ToastObj.Show(new ToastModel
                    {
                        Title = "EXITO!",
                        Content = $"PRODUCTO {prodAux.Id} Guardado Correctamente.",
                        CssClass = "e-toast-success",
                        Icon = "e-success toast-icons",
                        ShowCloseButton = true,
                        ShowProgressBar = true
                    }); //DESPUES DE AGREGAR
                    isAdding = false; //DESPUES DE AGREGAR
                    habilitaCodigo = false; //DESPUES DE AGREGAR
                    IsVisible = false;
                    await Cerrar();
                }
                else
                {
                    await ToastObj.Show(new ToastModel
                    {
                        Title = "ERROR!",
                        Content = "Error al verificar datos",
                        CssClass = "e-toast-danger",
                        Icon = "e-error toast-icons",
                        ShowCloseButton = true,
                        ShowProgressBar = true
                    });
                }
            }
            else
            {
                await ToastObj.Show(new ToastModel
                {
                    Title = "ERROR!",
                    Content = "Error al verificar datos",
                    CssClass = "e-toast-danger",
                    Icon = "e-error toast-icons",
                    ShowCloseButton = true,
                    ShowProgressBar = true
                });
            }
        }
        else
        {
            var response = await Http.PutAsJsonAsync($"api/Prod/{prodAux.Id}", prodAux);

            if (response.IsSuccessStatusCode)
            {
                var prodNuevo = await response.Content.ReadFromJsonAsync<Producto>();
                prodNuevo.Id = prodAux.Id;
                var prodSinModificar = prods.Where(p => p.Id == prodAux.Id).FirstOrDefault();
                prodSinModificar.Id = prodNuevo.Id;
                prodSinModificar.DES_PROD = prodNuevo.DES_PROD;
                prodSinModificar.CAMPOCOM1 = prodNuevo.CAMPOCOM1;
                prodSinModificar.CAMPOCOM2 = prodNuevo.CAMPOCOM2;
                prodSinModificar.CAMPOCOM3 = prodNuevo.CAMPOCOM3;
                prodSinModificar.CAMPOCOM4 = prodNuevo.CAMPOCOM4;
                prodSinModificar.CAMPOCOM5 = prodNuevo.CAMPOCOM5;
                prodSinModificar.CAMPOCOM6 = prodNuevo.CAMPOCOM6;
                prodSinModificar.CAMPOCOM7 = prodNuevo.CAMPOCOM7;
                prodSinModificar.CAMPOCOM8 = prodNuevo.CAMPOCOM8;
                prodSinModificar.CAMPOCOM9 = prodNuevo.CAMPOCOM9;
                prodSinModificar.CAMPOCOM10 = prodNuevo.CAMPOCOM10;
                prodSinModificar.CG_ORDEN = prodNuevo.CG_ORDEN;
                prodSinModificar.TIPO = prodNuevo.TIPO;
                prodSinModificar.CG_CLAS = prodNuevo.CG_CLAS;
                prodSinModificar.UNID = prodNuevo.UNID;
                prodSinModificar.CG_DENSEG = prodNuevo.CG_DENSEG;
                prodSinModificar.UNIDSEG = prodNuevo.UNIDSEG;
                prodSinModificar.PESO = prodNuevo.PESO;
                prodSinModificar.UNIDPESO = prodNuevo.UNIDPESO;
                prodSinModificar.NORMA = prodNuevo.NORMA;
                prodSinModificar.EXIGEDESPACHO = prodNuevo.EXIGEDESPACHO;
                prodSinModificar.EXIGELOTE = prodNuevo.EXIGELOTE;
                prodSinModificar.EXIGESERIE = prodNuevo.EXIGESERIE;
                prodSinModificar.EXIGEOA = prodNuevo.EXIGEOA;
                prodSinModificar.STOCKMIN = prodNuevo.STOCKMIN;
                prodSinModificar.ESPECIF = prodNuevo.ESPECIF;
                prodSinModificar.LOPTIMO = prodNuevo.LOPTIMO;
                prodSinModificar.TIEMPOFAB = prodNuevo.TIEMPOFAB;
                prodSinModificar.COSTO = prodNuevo.COSTO;
                prodSinModificar.COSTOTER = prodNuevo.COSTOTER;
                prodSinModificar.MONEDA = prodNuevo.MONEDA;
                prodSinModificar.FE_UC = prodNuevo.FE_UC;
                prodSinModificar.CG_CELDA = prodNuevo.CG_CELDA;
                prodSinModificar.CG_AREA = prodNuevo.CG_AREA;
                prodSinModificar.CG_LINEA = prodNuevo.CG_LINEA;
                prodSinModificar.CG_TIPOAREA = prodNuevo.CG_TIPOAREA;
                prodSinModificar.FE_UC = prodNuevo.FE_UC;
                prods.OrderByDescending(p => p.Id);
                await ToastObj.Show(new ToastModel
                {
                    Title = "EXITO!",
                    Content = $"PRODUCTO {prodAux.Id} editado Correctamente.",
                    CssClass = "e-toast-success",
                    Icon = "e-success toast-icons",
                    ShowCloseButton = true,
                    ShowProgressBar = true
                }); //DESPUES DE EDITAR
                Grid.Refresh();
                IsVisible = false;
                await Cerrar();
            }
        }
    }

    public async Task ClickHandler(ClickEventArgs args)
    {
        if (args.Item.Text == "Edit")
        {
            if (Grid.SelectedRecords.Count < 2)
            {
                foreach (var selectedRecord in Grid.SelectedRecords)
                {
                    prod.Id = selectedRecord.Id;
                    prod.DES_PROD = selectedRecord.DES_PROD;
                    prod.CAMPOCOM1 = selectedRecord.CAMPOCOM1;
                    prod.CAMPOCOM2 = selectedRecord.CAMPOCOM2;
                    prod.CAMPOCOM3 = selectedRecord.CAMPOCOM3;
                    prod.CAMPOCOM4 = selectedRecord.CAMPOCOM4;
                    prod.CAMPOCOM5 = selectedRecord.CAMPOCOM5;
                    prod.CAMPOCOM6 = selectedRecord.CAMPOCOM6;
                    prod.CAMPOCOM7 = selectedRecord.CAMPOCOM7;
                    prod.CAMPOCOM8 = selectedRecord.CAMPOCOM8;
                    prod.CAMPOCOM9 = selectedRecord.CAMPOCOM9;
                    prod.CAMPOCOM10 = selectedRecord.CAMPOCOM10;
                    prod.CG_ORDEN = selectedRecord.CG_ORDEN;
                    prod.TIPO = selectedRecord.TIPO;
                    prod.CG_CLAS = selectedRecord.CG_CLAS;
                    prod.UNID = selectedRecord.UNID;
                    prod.CG_DENSEG = selectedRecord.CG_DENSEG;
                    prod.UNIDSEG = selectedRecord.UNIDSEG;
                    prod.PESO = selectedRecord.PESO;
                    prod.UNIDPESO = selectedRecord.UNIDPESO;
                    prod.NORMA = selectedRecord.NORMA;
                    prod.EXIGEDESPACHO = selectedRecord.EXIGEDESPACHO;
                    prod.EXIGELOTE = selectedRecord.EXIGELOTE;
                    prod.EXIGESERIE = selectedRecord.EXIGESERIE;
                    prod.EXIGEOA = selectedRecord.EXIGEOA;
                    prod.STOCKMIN = selectedRecord.STOCKMIN;
                    prod.ESPECIF = selectedRecord.ESPECIF;
                    prod.LOPTIMO = selectedRecord.LOPTIMO;
                    prod.TIEMPOFAB = selectedRecord.TIEMPOFAB;
                    prod.COSTO = selectedRecord.COSTO;
                    prod.COSTOTER = selectedRecord.COSTOTER;
                    prod.MONEDA = selectedRecord.MONEDA;
                    prod.FE_UC = selectedRecord.FE_UC;
                    prod.CG_CELDA = selectedRecord.CG_CELDA;
                    prod.CG_AREA = selectedRecord.CG_AREA;
                    prod.CG_LINEA = selectedRecord.CG_LINEA;
                    prod.CG_TIPOAREA = selectedRecord.CG_TIPOAREA;
                    if (prod.CG_ORDEN == 1)
                        camposConf = true;
                    else
                        camposConf = false;
                }

                prodAux = prod;
                isAdding = false;
                habilitaCodigo = false;
                IsVisible = true;
            }
            else
            {
                await ToastObj.Show(new ToastModel
                {
                    Title = "ERROR!",
                    Content = "Solo se puede editar un item",
                    CssClass = "e-toast-danger",
                    Icon = "e-error toast-icons",
                    ShowCloseButton = true,
                    ShowProgressBar = true
                });
            }
        }

        if (args.Item.Text == "Copy" || args.Item.Text == "Add")
        {
            if (Grid.SelectedRecords.Count < 2)
            {
                prod = new Producto();
                foreach (var selectedRecord in Grid.SelectedRecords)
                {
                    var isConfirmed =
                        await JsRuntime.InvokeAsync<bool>("confirm", "Seguro de que desea copiar el producto?");
                    if (isConfirmed)
                    {
                        prod.DES_PROD = selectedRecord.DES_PROD;
                        prod.CAMPOCOM1 = selectedRecord.CAMPOCOM1;
                        prod.CAMPOCOM2 = selectedRecord.CAMPOCOM2;
                        prod.CAMPOCOM3 = selectedRecord.CAMPOCOM3;
                        prod.CAMPOCOM4 = selectedRecord.CAMPOCOM4;
                        prod.CAMPOCOM5 = selectedRecord.CAMPOCOM5;
                        prod.CAMPOCOM6 = selectedRecord.CAMPOCOM6;
                        prod.CAMPOCOM7 = selectedRecord.CAMPOCOM7;
                        prod.CAMPOCOM8 = selectedRecord.CAMPOCOM8;
                        prod.CAMPOCOM9 = selectedRecord.CAMPOCOM9;
                        prod.CAMPOCOM10 = selectedRecord.CAMPOCOM10;
                        prod.CG_ORDEN = selectedRecord.CG_ORDEN;
                        prod.TIPO = selectedRecord.TIPO;
                        prod.CG_CLAS = selectedRecord.CG_CLAS;
                        prod.UNID = selectedRecord.UNID;
                        prod.CG_DENSEG = selectedRecord.CG_DENSEG;
                        prod.UNIDSEG = selectedRecord.UNIDSEG;
                        prod.PESO = selectedRecord.PESO;
                        prod.UNIDPESO = selectedRecord.UNIDPESO;
                        prod.NORMA = selectedRecord.NORMA;
                        prod.EXIGEDESPACHO = selectedRecord.EXIGEDESPACHO;
                        prod.EXIGELOTE = selectedRecord.EXIGELOTE;
                        prod.EXIGESERIE = selectedRecord.EXIGESERIE;
                        prod.EXIGEOA = selectedRecord.EXIGEOA;
                        prod.STOCKMIN = selectedRecord.STOCKMIN;
                        prod.ESPECIF = selectedRecord.ESPECIF;
                        prod.LOPTIMO = selectedRecord.LOPTIMO;
                        prod.TIEMPOFAB = selectedRecord.TIEMPOFAB;
                        prod.COSTO = selectedRecord.COSTO;
                        prod.COSTOTER = selectedRecord.COSTOTER;
                        prod.MONEDA = selectedRecord.MONEDA;
                        prod.FE_UC = selectedRecord.FE_UC;
                        prod.CG_CELDA = selectedRecord.CG_CELDA;
                        prod.CG_AREA = selectedRecord.CG_AREA;
                        prod.CG_LINEA = selectedRecord.CG_LINEA;
                        prod.CG_TIPOAREA = selectedRecord.CG_TIPOAREA;
                        if (prod.CG_ORDEN == 1)
                            camposConf = true;
                        else
                            camposConf = false;
                    }
                }

                prodAux = prod;
                IsVisible = true;
                isAdding = true;
                habilitaCodigo = true;
            }
            else
            {
                await ToastObj.Show(new ToastModel
                {
                    Title = "ERROR!",
                    Content = "Solo se puede copiar un item",
                    CssClass = "e-toast-danger",
                    Icon = "e-error toast-icons",
                    ShowCloseButton = true,
                    ShowProgressBar = true
                });
            }
        }

        if (args.Item.Text == "Exportar Excel") await Grid.ExcelExport();

        if (args.Item.Text == "Eliminar")
            //Verificar si tienen formula o esta dentro
            if ((await Grid.GetSelectedRecordsAsync()).Count > 0)
            {
                var productos = await Grid.GetSelectedRecordsAsync();
                var productosSeleccionados = productos.Select(p => p.Id.Trim()).ToList();
                var query = string.Empty;
                var i = 0;
                foreach (var item in productosSeleccionados)
                {
                    if (i == 0)
                        query += $"insumos={item}";
                    else
                        query += $"&&insumos={item}";
                    i++;
                }

                var HayInsumoEnFormula = await Http.GetFromJsonAsync<bool>($"api/Formulas/VerificaFormula?{query}");
                if (!HayInsumoEnFormula)
                {
                    var isConfirmed = await JsRuntime.InvokeAsync<bool>("confirm",
                        "Seguro de que desea eliminar los insumos seleccionados?");
                    if (isConfirmed)
                    {
                        var response = await Http.PostAsJsonAsync("api/Prod/PostList", productos);
                        if (response.IsSuccessStatusCode)
                        {
                            await ToastObj.Show(new ToastModel
                            {
                                Title = "EXITO!",
                                Content = "Insumos Eliminados Correctamente.",
                                CssClass = "e-toast-success",
                                Icon = "e-success toast-icons",
                                ShowCloseButton = true,
                                ShowProgressBar = true
                            });
                            //await Grid.ClearSelectionAsync();
                        }
                        else
                        {
                            //cancela eliminacion en grilla por error
                            args.Cancel = true;
                            await ToastObj.Show(new ToastModel
                            {
                                Title = "ERROR!",
                                Content = "Error al Eliminar insumos: contactar al administrador",
                                CssClass = "e-toast-danger",
                                Icon = "e-error toast-icons",
                                ShowCloseButton = true,
                                ShowProgressBar = true
                            });
                        }
                    }
                    else
                    {
                        //cancela eliminacion en grilla por cancelacion
                        args.Cancel = true;
                    }
                }
                else
                {
                    //cancela eliminacion en grilla por estaar en una formula
                    args.Cancel = true;
                    await ToastObj.Show(new ToastModel
                    {
                        Title = "Atención!",
                        Content = "No se puede Eliminar insumos: insumos relacionados con formula",
                        CssClass = "e-toast-warning",
                        Icon = "e-warning toast-icons",
                        ShowCloseButton = true,
                        ShowProgressBar = true
                    });
                }
            }
    }

    public async Task OnVistaSeleccionada(VistasGrillas vistasGrillas)
    {
        await Grid.SetPersistData(vistasGrillas.Layout);
    }

    public async Task OnReiniciarGrilla()
    {
        await Grid.ResetPersistData();
    }
}