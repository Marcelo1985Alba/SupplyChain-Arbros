using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using SupplyChain.Client.HelperService;
using SupplyChain.Client.RepositoryHttp;
using SupplyChain.Client.Shared;
using SupplyChain.Shared;
using SupplyChain.Shared.Models;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Navigations;
using Syncfusion.Blazor.Notifications;
using Syncfusion.Blazor.Popups;
using Syncfusion.Blazor.Spinner;

namespace SupplyChain.Client.Pages.Ingenieria;

public class BaseConsultaFormulas : ComponentBase
{
    protected SfGrid<vIngenieriaProductosFormulas> DetailedGridProdForm;

    protected SfDialog DialogDespieceRef;
    protected bool dialogVisibleCopiar;
    protected bool dialogVisibleEditar;
    protected SfGrid<DespiecePlanificacion> GridDespiece;
    protected SfGrid<vIngenieriaProductosFormulas> GridProdForm;

    protected List<Producto> Insumos = new();
    protected Producto insumoSeleccionado = new();
    protected List<DespiecePlanificacion> listaDespiece = new();
    protected bool mostrarCerrarTab;

    protected bool PopupBuscadorProdVisible;
    protected vIngenieriaProductosFormulas ProdSelected = new();
    protected SfTab refSfTab;
    protected SfSpinner refSpinner;
    protected List<TabData> TabItems = new();

    protected string tituloTabFormulas = string.Empty;

    protected SfToast ToastObj;

    protected List<object> Toolbaritems = new()
    {
        "ExcelExport",
        new ItemModel { Type = ItemType.Separator },
        new ItemModel
        {
            Text = "Guardar Excel", Type = ItemType.Button, CssClass = "", ShowTextOn = DisplayMode.Both,
            TooltipText = "Guardar el despiece en formato excel", PrefixIcon = "e-update e-icons", Id = "GuardarExcel"
        }
    };

    protected List<object> ToolbaritemsEditarFormula = new()
    {
        new ItemModel
        {
            Text = "Agregar Insumo", Type = ItemType.Button, CssClass = "", ShowTextOn = DisplayMode.Both,
            TooltipText = "Buscar insumos", PrefixIcon = "e-search e-icons", Id = "BuscarInsumo"
        },
        new ItemModel
        {
            Text = "Guardar", Type = ItemType.Button, CssClass = "", ShowTextOn = DisplayMode.Both,
            TooltipText = "Guardar el despiece en la base de datos", PrefixIcon = "e-update e-icons", Id = "GuardarDB"
        }
    };

    protected decimal valorDolar = 0;
    protected bool VisibleSpinner;
    [Inject] public HttpClient Http { get; set; }
    [Inject] public ExcelService ExcelService { get; set; }
    [CascadingParameter] public MainLayout MainLayout { get; set; }
    [Parameter] public string extern_codigo { get; set; }

    protected SfDialog EditFormulas { get; set; }
    protected vIngenieriaProductosFormulas Selected { get; set; }
    protected List<vIngenieriaProductosFormulas> DataOrdeProductosFormulas { get; set; } = new();
    protected List<vIngenieriaProductosFormulas> DataDetailedProductosFormulas { get; set; } = new();
    protected List<Formula> DataEditarProductosFormulas { get; set; } = new();

    protected override async Task OnInitializedAsync()
    {
        MainLayout.Titulo = "Consulta de Fórmulas";
        VisibleSpinner = true;
        DataOrdeProductosFormulas =
            await Http.GetFromJsonAsync<List<vIngenieriaProductosFormulas>>("api/Ingenieria/GetProductoFormulas");
        if (extern_codigo != null)
        {
            ProdSelected = DataOrdeProductosFormulas.FirstOrDefault(s => s.CG_PROD.Trim() == extern_codigo.Trim());
            listaDespiece =
                await Http.GetFromJsonAsync<List<DespiecePlanificacion>>(
                    $"api/Planificacion/Despiece/{extern_codigo.Trim()}/1/1");
            await DialogDespieceRef.Show();
        }

        VisibleSpinner = false;
    }

    protected async Task CommandClickHandler(CommandClickEventArgs<vIngenieriaProductosFormulas> args)
    {
        if (args.CommandColumn.Title == "Editar")
        {
            Selected = args.RowData;
            DataEditarProductosFormulas = await Http.GetFromJsonAsync<List<Formula>>("api/Formulas/BuscarPorCgProd/" +
                $"{args.RowData.CG_PROD.Trim()}");
            dialogVisibleEditar = true;
        }
        else if (args.CommandColumn.Title == "Copiar")
        {
            Selected = args.RowData;
            DataDetailedProductosFormulas =
                (await Http.GetFromJsonAsync<List<vIngenieriaProductosFormulas>>("api/Ingenieria/GetProductoFormulas"))
                .Where(s => s.TIENE_FORM && s.FORM_ACTIVA).ToList();
            dialogVisibleCopiar = true;
        }
        else if (args.CommandColumn.Title == "Despiece")
        {
            VisibleSpinner = true;
            ProdSelected = args.RowData;
            listaDespiece = await Http.GetFromJsonAsync<List<DespiecePlanificacion>>("api/Planificacion/Despiece/" +
                $"{args.RowData.CG_PROD.Trim()}/1/1");


            VisibleSpinner = false;
            await DialogDespieceRef.Show();
        }
        else if (args.CommandColumn.Title == "Ver Plano")
        {
            VisibleSpinner = true;
            tituloTabFormulas = "Formulas";
            mostrarCerrarTab = true;
            TabItems.Add(new TabData
            {
                Header = $"Plano ({args.RowData.CG_PROD.Trim()})",
                //Verificar si es un semi o un prod, los semis obtener los primeros 7 digitos
                CodigoInsumo = new string(args.RowData.CG_PROD.Take(2).ToArray()) == "C-"
                               || new string(args.RowData.CG_PROD.Take(2).ToArray()) == "D-"
                    ? args.RowData.CG_PROD.Substring(0, 7)
                    : args.RowData.CG_PROD
            });


            //El primer tab no se encuentra dentro de la lista
            //le resto el que agregue para que no lo desactive
            //for (int i = 0; i < TabItems.Count + 1; i++)
            //{
            //    await refSfTab.EnableTab(i, false);
            //}


            //await refSfTab.EnableTab(TabItems.Count, true);
            //await refSfTab.Select(TabItems.Count);
            VisibleSpinner = false;
        }
        else if (args.CommandColumn.Title == "Ver Programas")
        {
            VisibleSpinner = true;
            tituloTabFormulas = "Formulas";
            mostrarCerrarTab = true;
            TabItems.Add(new TabData
            {
                Header = $"Programas ({args.RowData.CG_PROD.Trim()})",
                CodigoInsumo = args.RowData.CG_PROD.Trim()
            });


            //El primer tab no se encuentra dentro de la lista
            //le resto el que agregue para que no lo desactive
            //for (int i = 0; i < TabItems.Count + 1; i++)
            //{
            //    await refSfTab.EnableTab(i, false);
            //}


            //await refSfTab.EnableTab(TabItems.Count, true);
            //await refSfTab.Select(TabItems.Count);
            VisibleSpinner = false;
        }
    }


    protected async Task ToolbarProdFormClickHandler(ClickEventArgs args)
    {
        if (args.Item.Id == "ProdForm_excelexport") //Id is combination of Grid's ID and itemname
        {
            VisibleSpinner = true;
            await GridProdForm.ExcelExport();
            VisibleSpinner = false;
        }
    }


    protected async Task ToolbarEditFormClickHandler(ClickEventArgs args)
    {
        if (args.Item.Id == "BuscarInsumo")
        {
            VisibleSpinner = true;
            await BuscarInsumo();
            VisibleSpinner = false;
        }
    }


    protected async Task BuscarInsumo()
    {
        PopupBuscadorProdVisible = true;
        Insumos = await Http.GetFromJsonAsync<List<Producto>>($"api/Prod/ByTipo/{true}/{true}/{false}");
    }

    protected void OnInsumoSeleccionado(Producto insumoSeleccionado)
    {
        PopupBuscadorProdVisible = false;
        if (insumoSeleccionado != null)
        {
            var nuevoInsumoFormula = new Formula
            {
                ACTIVO = "S",
                CG_ORDEN = insumoSeleccionado.CG_ORDEN,
                Cg_Mat = insumoSeleccionado.CG_ORDEN == 4 ? insumoSeleccionado.Id : string.Empty,
                Cg_Se = insumoSeleccionado.CG_ORDEN == 2 ? insumoSeleccionado.Id : string.Empty,
                CANTIDAD = 0M,
                Cg_Prod = DataEditarProductosFormulas[0].Cg_Prod
            };

            DataEditarProductosFormulas.Add(nuevoInsumoFormula);
        }
    }

    protected async Task ToolbarClickHandler(ClickEventArgs args)
    {
        if (args.Item.Id == "Despiece_excelexport") //Id is combination of Grid's ID and itemname
        {
            VisibleSpinner = true;
            await GridDespiece.ExportToExcelAsync();
            VisibleSpinner = false;
        }

        if (args.Item.Text == "Guardar Excel")
        {
            VisibleSpinner = true;
            var dataEspecial = listaDespiece.Select(d => new
            {
                d.CG_PROD,
                d.CG_SE,
                d.CG_MAT,
                d.DES_PROD,
                d.CG_FORM,
                CANT = d.CANT_MAT
            }).ToDataTable();
            await ExcelService.CreateExcel(dataEspecial, ProdSelected.CG_PROD.Trim());
            VisibleSpinner = false;

            await MostrarMensajeToastSuccess();
        }
    }

    private async Task MostrarMensajeToastSuccess()
    {
        await ToastObj.Show(new ToastModel
        {
            Title = "EXITO!",
            Content = "Guardado Correctamente.",
            CssClass = "e-toast-success",
            Icon = "e-success toast-icons",
            ShowCloseButton = true,
            ShowProgressBar = true
        });
    }


    protected async Task QueryCellInfoHandler(QueryCellInfoEventArgs<vIngenieriaProductosFormulas> args)
    {
        if (!args.Data.TIENE_FORM)
            args.Cell.AddClass(new[] { "rojas" });
        if (args.Data.TIENE_FORM && !args.Data.FORM_ACTIVA)
            args.Cell.AddClass(new[] { "amarillas" });
    }

    protected async Task TabEliminando(RemoveEventArgs removeEventArgs)
    {
        var tab = refSfTab.Items[removeEventArgs.RemovedIndex];
        if (removeEventArgs.RemovedIndex == 0)
            removeEventArgs.Cancel = true;
    }

    protected async Task TabEliminado(RemoveEventArgs removeEventArgs)
    {
        if (refSfTab.Items.Count == 1)
        {
            mostrarCerrarTab = false;
            tituloTabFormulas = string.Empty;
        }
    }

    protected async Task CopiarFormula(MouseEventArgs args)
    {
        if (DetailedGridProdForm.SelectedRecords.Count != 0)
            if (Selected != null)
            {
                var SelectedFormula2 =
                    DetailedGridProdForm.SelectedRecords.FirstOrDefault();
                if (SelectedFormula2 != null)
                {
                    //List<Formula> formulas = await Http.GetFromJsonAsync<List<Formula>>("api/Formulas");
                    var httpResponse =
                        await Http.GetAsync($"api/Formulas/BuscarPorCgProd/{SelectedFormula2.CG_PROD}");
                    var responseContent = await httpResponse.Content.ReadFromJsonAsync<List<Formula>>();
                    var isError = !httpResponse.IsSuccessStatusCode;
                    var responseFormulas =
                        new HttpResponseWrapper<List<Formula>>(responseContent, httpResponse, isError);
                    if (responseFormulas.Error)
                    {
                        var errorReason = responseFormulas.HttpResponseMessage?.ReasonPhrase;
                        Console.WriteLine(errorReason);
                    }
                    else
                    {
                        var formulas = responseFormulas.Response;
                        var aGuardar = new List<Formula>();
                        foreach (var form in formulas)
                        {
                            var aux = form;
                            aux.Id = 0;
                            aux.Cg_Prod = Selected.CG_PROD;
                            aux.CG_FORM = 1;
                            aux.USUARIO = "USER";
                            aGuardar.Add(aux);
                        }

                        await Http.PostAsJsonAsync("api/Formulas/PostList", aGuardar);
                    }
                }
            }

        dialogVisibleCopiar = false;
    }

    private async Task ToastMensajeError(string content = "Ocurrio un Error.")
    {
        await ToastObj.Show(new ToastModel
        {
            Title = "Error!",
            Content = content,
            CssClass = "e-toast-warning",
            Icon = "e-warning toast-icons",
            ShowCloseButton = true,
            ShowProgressBar = true
        });
    }

    public class TabData
    {
        public string Header { get; set; }
        public string CodigoInsumo { get; set; }
        public RenderFragment Content { get; set; }
    }
}