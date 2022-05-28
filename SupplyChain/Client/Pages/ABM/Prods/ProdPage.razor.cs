using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SupplyChain;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Navigations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using SupplyChain.Shared.Models;
using SupplyChain.Client.Shared;
using Syncfusion.Blazor.Spinner;
using Syncfusion.Blazor.Notifications;
using SupplyChain.Shared;

namespace SupplyChain.Pages.Prods
{
    public class ProdsPageBase : ComponentBase
    {
        [Inject] protected HttpClient Http { get; set; }
        [Inject] protected IJSRuntime JsRuntime { get; set; }

        protected SfSpinner refSpinner;
        protected SfGrid<Producto> Grid;
        protected SfToast ToastObj;
        public bool SpinnerVisible = false;
        public bool isAdding = false;
        public bool habilitaCodigo = false;
        protected bool IsVisible { get; set; } = false;

        public bool Enabled = true;
        public bool Disabled = false;
        public bool camposConf = true;


        protected List<Producto> prods = new();
        public Producto prod = new Producto();
        public Producto prodAux = new Producto();

        protected List<Unidades> unidades = new List<Unidades>();
        protected List<Moneda> monedas = new List<Moneda>();
        protected List<Celdas> celda = new List<Celdas>();
        protected List<Areas> area = new List<Areas>();
        protected List<Lineas> linea = new List<Lineas>();
        protected List<TipoArea> tipoarea = new List<TipoArea>();
        protected List<Cat> cat = new List<Cat>();

        protected const string APPNAME = "grdProdABM";
        protected string state;

        protected List<Object> Toolbaritems = new List<Object>(){
        "Search",
        "Add",
        "Edit",
        "Delete",
        "Print",
        new ItemModel { Text = "Copy", TooltipText = "Copy", PrefixIcon = "e-copy", Id = "copy" },
        "ExcelExport"
    };
        [CascadingParameter] MainLayout MainLayout { get; set; }
        protected override async Task OnInitializedAsync()
        {
            MainLayout.Titulo = "Productos";

            SpinnerVisible = true;
            prods = await Http.GetFromJsonAsync<List<Producto>>("api/Prod");
            unidades = await Http.GetFromJsonAsync<List<Unidades>>("api/unidades");
            monedas = await Http.GetFromJsonAsync<List<Moneda>>("api/Monedas");
            celda = await Http.GetFromJsonAsync<List<Celdas>>("api/Celdas");
            area = await Http.GetFromJsonAsync<List<Areas>>("api/Areas");
            linea = await Http.GetFromJsonAsync<List<Lineas>>("api/Lineas");
            tipoarea = await Http.GetFromJsonAsync<List<TipoArea>>("api/TipoArea");
            cat = await Http.GetFromJsonAsync<List<Cat>>("api/Cat");

            SpinnerVisible = false;
            await base.OnInitializedAsync();
        }

        public async Task Begin(ActionEventArgs<Producto> args)
        {
            if (args.RequestType == Syncfusion.Blazor.Grids.Action.Grouping
                || args.RequestType == Syncfusion.Blazor.Grids.Action.UnGrouping
                || args.RequestType == Syncfusion.Blazor.Grids.Action.ClearFiltering
                || args.RequestType == Syncfusion.Blazor.Grids.Action.CollapseAllComplete
                || args.RequestType == Syncfusion.Blazor.Grids.Action.ColumnState
                || args.RequestType == Syncfusion.Blazor.Grids.Action.ClearFiltering
                || args.RequestType == Syncfusion.Blazor.Grids.Action.Reorder
                || args.RequestType == Syncfusion.Blazor.Grids.Action.Sorting
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

            if (args.RequestType == Syncfusion.Blazor.Grids.Action.RowDragAndDrop)
            {

            }
        }

        public async Task Cerrar()
        {
            prod = new();
        }

        public async Task guardarProd()
        {
            if (isAdding == true)
            {
                var existe = await Http.GetFromJsonAsync<bool>($"api/Prod/ExisteProducto/{prodAux.CG_PROD}");

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

                    if (response.StatusCode == System.Net.HttpStatusCode.Created)
                    {
                        var prodNuevo = await response.Content.ReadFromJsonAsync<Producto>();
                        prods.Add(prodNuevo);
                        prods.OrderByDescending(p => p.CG_PROD);
                        await Grid.RefreshColumns();
                        Grid.Refresh();
                        await Grid.RefreshHeader();

                        await this.ToastObj.Show(new ToastModel
                        {
                            Title = "EXITO!",
                            Content = $"PRODUCTO {prodAux.CG_PROD} Guardado Correctamente.",
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
                        await this.ToastObj.Show(new ToastModel
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
                    await this.ToastObj.Show(new ToastModel
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
                var response = await Http.PutAsJsonAsync($"api/Prod/{prodAux.CG_PROD}", prodAux);

                if (response.IsSuccessStatusCode)
                {
                    var prodNuevo = await response.Content.ReadFromJsonAsync<Producto>();
                    prodNuevo.CG_PROD = prodAux.CG_PROD;
                    var prodSinModificar = prods.Where(p => p.CG_PROD == prodAux.CG_PROD).FirstOrDefault();
                    prodSinModificar.CG_PROD = prodNuevo.CG_PROD;
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
                    prods.OrderByDescending(p => p.CG_PROD);
                    await this.ToastObj.Show(new ToastModel
                    {
                        Title = "EXITO!",
                        Content = $"PRODUCTO {prodAux.CG_PROD} editado Correctamente.",
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
        public async Task ClickHandler(Syncfusion.Blazor.Navigations.ClickEventArgs args)
        {
            if (args.Item.Text == "Edit")
            {
                if (this.Grid.SelectedRecords.Count < 2)
                {
                    foreach (Producto selectedRecord in this.Grid.SelectedRecords)
                    {
                        prod.CG_PROD = selectedRecord.CG_PROD;
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
                    await this.ToastObj.Show(new ToastModel
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
                if (this.Grid.SelectedRecords.Count < 2)
                {
                    prod = new();
                    foreach (Producto selectedRecord in this.Grid.SelectedRecords)
                    {
                        bool isConfirmed = await JsRuntime.InvokeAsync<bool>("confirm", "Seguro de que desea copiar el producto?");
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
                    await this.ToastObj.Show(new ToastModel
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

            if (args.Item.Text == "Exportar Excel")
            {
                await this.Grid.ExcelExport();
            }

            if (args.Item.Text == "Eliminar")
            {
                //Verificar si tienen formula o esta dentro
                if ((await Grid.GetSelectedRecordsAsync()).Count > 0)
                {
                    var productos = await Grid.GetSelectedRecordsAsync();
                    var productosSeleccionados = productos.Select(p=> p.CG_PROD.Trim()).ToList();
                    var query = string.Empty;
                    var i = 0;
                    foreach (var item in productosSeleccionados)
                    {
                        if (i == 0)
                        {
                            query += $"insumos={item}";
                        }
                        else
                        {
                            query += $"&&insumos={item}";
                        }
                        i++;
                    }

                    bool HayInsumoEnFormula = await Http.GetFromJsonAsync<bool>($"api/Formulas/VerificaFormula?{query}");
                    if (!HayInsumoEnFormula)
                    {
                        bool isConfirmed = await JsRuntime.InvokeAsync<bool>("confirm", "Seguro de que desea eliminar los insumos seleccionados?");
                        if (isConfirmed)
                        {
                            var response = await Http.PostAsJsonAsync("api/Prod/PostList", productos);
                            if (response.IsSuccessStatusCode)
                            {
                                await this.ToastObj.Show(new ToastModel
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
                                await this.ToastObj.Show(new ToastModel
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
                        await this.ToastObj.Show(new ToastModel
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
}
