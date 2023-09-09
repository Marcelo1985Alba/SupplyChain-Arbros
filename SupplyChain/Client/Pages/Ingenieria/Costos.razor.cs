using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SupplyChain.Client.RepositoryHttp;
using SupplyChain.Shared;
using SupplyChain.Shared.Models;
using Syncfusion.Blazor.Calendars;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Inputs;
using Syncfusion.Blazor.Notifications;
using Syncfusion.Blazor.Popups;
using Syncfusion.Blazor.Spinner;

namespace SupplyChain.Client.Pages.Ingenieria;

public class BaseCostos : ComponentBase
{
    protected List<Producto> Busquedalist = new();
    protected int CantidadMostrar = 12;
    protected List<Producto> CG_PRODlist = new();
    protected string classValue = "fa fa-table e-btn-icon";
    protected string Codigo = "";
    protected decimal costoGen;
    protected decimal costoProd;
    protected SupplyChain.Shared.Costos costos;
    protected List<Deposito> depositos = new();
    protected string Descripcion = "";
    protected SfDialog DialogDespieceRef;

    protected SfGrid<Pedidos> Grid;
    protected SfGrid<Producto> gridCostos;
    protected SfGrid<DespiecePlanificacion> GridDespiece;
    protected SfGrid<vIngenieriaProductosFormulas> GridProdForm;
    protected List<DespiecePlanificacion> listaDespiece = new();
    protected Programa[] ordenesFabricacion = null;
    protected Programa[] ordenesPlaneadas = null;
    protected decimal precio;
    protected vIngenieriaProductosFormulas ProdSelected = new();

    protected SfSpinner refSpinner;

    protected DateTime selectedEndDate =
        new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(-1).AddDays(-1);

    protected DateTime selectedEndDateForGrid =
        new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(-1).AddDays(-1);

    protected DateTime selectedStartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(-7);
    protected DateTime selectedStartDateForGrid = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(-7);
    protected bool showCostResults;
    protected bool showResults;
    protected SfToast ToastObj;
    protected List<object> Toolbaritems = new() { "Search" };
    [Inject] public HttpClient Http { get; set; }
    [Inject] protected IRepositoryHttp HttpNew { get; set; }
    [Inject] protected IJSRuntime JsRuntime { get; set; }
    protected bool SpinnerVisible { get; set; }
    public bool IsVisible { get; set; }
    protected List<vIngenieriaProductosFormulas> DataOrdeProductosFormulas { get; set; } = new();
    public List<Pedidos> DataSource { get; set; } = null!;

    protected async Task CalculateCost()
    {
        SpinnerVisible = true;
        try
        {
            var response = await Http.GetFromJsonAsync<SupplyChain.Shared.Costos>(
                $"api/Ingenieria/getValues?startDate={selectedStartDate.ToString()}&endDate={selectedEndDate.ToString()}");

            if (response != default)
            {
                costos = response;
                showResults = true;
                StateHasChanged();
            }
            else
            {
                await ToastMensajeError("No se obtuvieron resultados.");
            }
        }
        catch (Exception ex)
        {
            await ToastMensajeError($"Error: {ex.Message}");
        }

        showResults = true;
        SpinnerVisible = false;
    }

    protected async Task SearchForGrid()
    {
        SpinnerVisible = true;
        DataOrdeProductosFormulas = await Http.GetFromJsonAsync<List<vIngenieriaProductosFormulas>>($"api/Ingenieria/" +
            $"GetProductoFormulasWithCost?startDate={selectedStartDateForGrid.ToString()}&endDate={selectedEndDateForGrid.ToString()}");
        SpinnerVisible = false;
    }

    protected async Task OnSelectedViewChange()
    {
        if (classValue == "fa fa-table e-btn-icon")
        {
            SpinnerVisible = true;
            classValue = "fa fa-line-chart e-btn-icon";
            SpinnerVisible = false;
        }
        else if (classValue == "fa fa-line-chart e-btn-icon")
        {
            SpinnerVisible = true;
            classValue = "fa fa-table e-btn-icon";
            SpinnerVisible = false;
        }
    }

    public void ValueChangeFechaCostos(RangePickerEventArgs<DateTime> args)
    {
        selectedStartDate = args.StartDate;
        selectedEndDate = args.EndDate;
    }

    public void ValueChangeFechaForGrid(RangePickerEventArgs<DateTime> args)
    {
        selectedStartDateForGrid = args.StartDate;
        selectedEndDateForGrid = args.EndDate;
    }

    protected async Task OnInputCG_PROD(InputEventArgs args)
    {
        if (!string.IsNullOrWhiteSpace(args.Value))
        {
            Descripcion = string.IsNullOrEmpty(Descripcion) ? string.Empty : Descripcion;
            var query = $"Codigo={args.Value.Trim()}&Descripcion={Descripcion}";
            var respose = await HttpNew.GetFromJsonAsync<Producto>($"api/Prod/GetByFilter?{query}");

            if (respose.Response != null)
                Descripcion = respose.Response.DES_PROD;
            else
                Descripcion = "";
        }
    }

    protected async Task OnInputDES_PROD(InputEventArgs args)
    {
        if (args.Value != "")
        {
            CG_PRODlist = await Http.GetFromJsonAsync<List<Producto>>($"api/Prod/BuscarPorDES_PROD/{args.Value}");
            if (CG_PRODlist.Count > 0)
                Codigo = CG_PRODlist.FirstOrDefault()?.Id;
            else
                Codigo = "";
        }
    }

    protected async Task BuscarProducto()
    {
        if (string.IsNullOrEmpty(Descripcion) && string.IsNullOrEmpty(Codigo))
        {
            Busquedalist =
                await Http.GetFromJsonAsync<List<Producto>>(
                    $"api/Prevision/BuscarProductoPrevision/Vacio/Vacio/{CantidadMostrar}");
            IsVisible = true;
        }

        if (!string.IsNullOrEmpty(Descripcion) || !string.IsNullOrEmpty(Codigo))
        {
            if (string.IsNullOrEmpty(Descripcion))
                Busquedalist =
                    await Http.GetFromJsonAsync<List<Producto>>(
                        $"api/Prevision/BuscarProductoPrevision/{Codigo}/Vacio/{CantidadMostrar}");
            else if (string.IsNullOrEmpty(Codigo))
                Busquedalist =
                    await Http.GetFromJsonAsync<List<Producto>>(
                        $"api/Prevision/BuscarProductoPrevision/Vacio/{Descripcion}/{CantidadMostrar}");
            else
                Busquedalist =
                    await Http.GetFromJsonAsync<List<Producto>>(
                        $"api/Prevision/BuscarProductoPrevision/{Codigo}/{Descripcion}/{CantidadMostrar}");
            IsVisible = true;
        }
    }

    public async void OnSelected()
    {
        CantidadMostrar = 12;
        IsVisible = false;
        SpinnerVisible = true;
        Codigo = gridCostos.GetSelectedRecordsAsync().Result.FirstOrDefault()?.Id;
        Descripcion = gridCostos.GetSelectedRecordsAsync().Result.FirstOrDefault()?.DES_PROD;

        //busco el costo del producto
        var response = await Http.GetFromJsonAsync<decimal>($"api/Ingenieria/GetCostoByProd/{Codigo.Trim()}/1/1");
        if (response != default)
        {
            costoProd = response;
        }
        else
        {
            await errorOnResponse(
                $"Hubo un error al obtener el costo del producto {Codigo.Trim()}, \n tal vez no posee fórmula.");
            return;
        }

        //busco el factor de conversion del producto
        var response2 = await Http.GetFromJsonAsync<decimal>($"api/Prod/GetCG_DENSEG/{Codigo.Trim()}");
        if (response2 != default)
        {
            costoGen = response2;
        }
        else
        {
            await errorOnResponse($"Hubo un error al obtener el factor de conversion del producto {Codigo.Trim()}.");
            return;
        }

        //busco el precio del producto
        var response3 = await Http.GetFromJsonAsync<decimal>($"api/PrecioArticulos/GetPrecio/{Codigo.Trim()}");
        if (response3 != default)
        {
            precio = response3;
        }
        else
        {
            await errorOnResponse($"Hubo un error al obtener el precio del producto {Codigo.Trim()} o el precio es 0.");
            return;
        }

        SpinnerVisible = false;
        showCostResults = true;
        StateHasChanged();
    }

    private async Task errorOnResponse(string response)
    {
        await ToastMensajeError(response);
        showCostResults = false;
        SpinnerVisible = false;
        Descripcion = "";
        StateHasChanged();
    }

    protected async Task AgregarValores()
    {
        CantidadMostrar *= 2;
        await BuscarProducto();
    }

    protected async Task AbrirTrazabilidad()
    {
        await JsRuntime.InvokeVoidAsync("open", $"/ingenieria/consulta-formulas/{Codigo.Trim()}", "_blank");
    }

    protected async Task QueryCellInfoHandler(QueryCellInfoEventArgs<vIngenieriaProductosFormulas> args)
    {
        if (!args.Data.TIENE_FORM)
            args.Cell.AddClass(new[] { "rojas" });
        if (args.Data.TIENE_FORM && !args.Data.FORM_ACTIVA)
            args.Cell.AddClass(new[] { "amarillas" });
    }

    protected async Task CommandClickHandler(CommandClickEventArgs<vIngenieriaProductosFormulas> args)
    {
        if (args.CommandColumn.Title == "Despiece")
        {
            SpinnerVisible = true;
            ProdSelected = args.RowData;
            listaDespiece = await Http.GetFromJsonAsync<List<DespiecePlanificacion>>("api/Planificacion/Despiece/" +
                $"{args.RowData.CG_PROD.Trim()}/1/1");
            SpinnerVisible = false;
            await DialogDespieceRef.Show();
        }
    }

    private async Task ToastMensajeExito(string content = "Guardado con exito!")
    {
        await ToastObj.Show(new ToastModel
        {
            Title = "EXITO!",
            Content = content,
            CssClass = "e-toast-success",
            Icon = "e-success toast-icons",
            ShowCloseButton = true,
            ShowProgressBar = true
        });
    }

    private async Task ToastMensajeError(string content = "Ocurrio un Error.")
    {
        await ToastObj.ShowAsync(new ToastModel
        {
            Title = "Error!",
            Content = content,
            CssClass = "e-toast-warning",
            Icon = "e-warning toast-icons",
            ShowCloseButton = true,
            ShowProgressBar = true
        });
    }
}