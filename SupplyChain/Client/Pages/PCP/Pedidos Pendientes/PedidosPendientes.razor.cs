using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SupplyChain.Shared;
using SupplyChain.Shared.Models;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Navigations;
using Syncfusion.Blazor.Notifications;
using Syncfusion.Blazor.Spinner;

namespace SupplyChain.Client.Pages.PCP.Pedidos_Pendientes;

public class PedidosPendientesBase : ComponentBase
{
    protected const string APPNAME = "grdPedPendPCP";
    public bool Disabled = false;

    public bool Enabled = true;
    protected SfGrid<ModeloPedidosPendientes> Grid;


    protected List<ModeloPedidosPendientes> listaPedPend = new();
    protected SfSpinner SpinnerObj;
    protected string state;
    protected SfToast ToasObj;

    protected List<object> Toolbaritems = new()
    {
        new ItemModel { Type = ItemType.Separator },
        "Search",
        new ItemModel { Type = ItemType.Separator },
        "Print",
        new ItemModel { Type = ItemType.Separator },
        "ExcelExport",
        new ItemModel
            { Text = "Seleccionar Columnas", TooltipText = "Seleccionar Columnas", Id = "Seleccionar Columnas" }
    };

    [Inject] protected HttpClient Http { get; set; }
    [Inject] protected IJSRuntime JsRuntime { get; set; }
    protected bool VisibleProperty { get; set; }

    protected override async Task OnInitializedAsync()
    {
        VisibleProperty = true;
        listaPedPend = await Http.GetFromJsonAsync<List<ModeloPedidosPendientes>>("api/PedidosPendientes");
        await Grid.AutoFitColumns();
        VisibleProperty = false;
    }

    public async Task Begin(ActionEventArgs<ModeloPedidosPendientes> args)
    {
        if (args.RequestType == Action.Grouping ||
            args.RequestType == Action.UnGrouping
            || args.RequestType == Action.ClearFiltering
            || args.RequestType == Action.CollapseAllComplete
            || args.RequestType == Action.ColumnState
            || args.RequestType == Action.ClearFiltering
            || args.RequestType == Action.Reorder ||
            args.RequestType == Action.Sorting
           )
        {
            //VisibleProperty = true;
            Grid.PreventRender();
            Grid.Refresh();

            state = await Grid.GetPersistData();
            await Grid.AutoFitColumnsAsync();
            await Grid.RefreshColumns();
            await Grid.RefreshHeader();
            //VisibleProperty = false;
        }

        if (args.RequestType == Action.RowDragAndDrop)
        {
        }
    }

    protected async Task OnDataBoundGrid(BeforeDataBoundArgs<ModeloPedidosPendientes> args)
    {
        await SpinnerObj.ShowAsync();
        VisibleProperty = true;
        Grid.PreventRender();
        VisibleProperty = false;
    }

    public async Task LoadGrid(object args)
    {
        await SpinnerObj.ShowAsync();
        VisibleProperty = true;
        await Grid.AutoFitColumns();
        VisibleProperty = false;
    }

    public async Task DataBoundHandler()
    {
        Grid.PreventRender();
        await Grid.AutoFitColumns();
        VisibleProperty = false;
    }

    public async Task ClickHandler(ClickEventArgs args)
    {
        if (args.Item.Text == "Seleccionar Columnas") await Grid.OpenColumnChooser();
        if (args.Item.Text == "Excel Export")
            //BORRARESTO = Grid.Columns.FirstOrDefault().Width.ToString();
            await Grid.ExcelExport();
        if (args.Item.Text == "Print") await Grid.Print();
    }

    public async Task QueryCellInfoHandler(QueryCellInfoEventArgs<ModeloPedidosPendientes> args)
    {
        if (args.Data.CG_ESTADOCARGA == 2 || args.Data.CG_ESTADOCARGA == 3)
            args.Cell.AddClass(new[] { "verdes" });
        else if (args.Data.CG_ESTADOCARGA == 4) args.Cell.AddClass(new[] { "amarillas" });
    }

    public async Task ActionComplete(ActionEventArgs<ModeloPedidosPendientes> args)
    {
        if (args.RequestType == Action.Save)
        {
            var respuesta = await Http.PutAsJsonAsync($"api/PedidosPendientes/{args.Data.PEDIDO}", args.Data);
            if (!respuesta.IsSuccessStatusCode)
                await ToasObj.Show(new ToastModel
                {
                    Title = "ERROR!",
                    Content = $"Ocurrrio un error.Error al intentar Guardar Pedido: {args.Data.PEDIDO} ",
                    CssClass = "e-toast-danger",
                    Icon = "e-error toast-icons",
                    ShowCloseButton = true,
                    ShowProgressBar = true
                });
            else
                await ToasObj.Show(new ToastModel
                {
                    Title = "ÉXITO!",
                    Content = $"Guardado Correctamente! Nro OF: {args.Data.PEDIDO}",
                    CssClass = "e-toast-success",
                    Icon = "e-success toast-icons",
                    ShowCloseButton = true,
                    ShowProgressBar = true
                });
            //await Grid.RefreshColumns();
            //Grid.Refresh();
            //await Grid.RefreshHeader();
        }
    }

    public async Task CommandClickHandler(CommandClickEventArgs<ModeloPedidosPendientes> args)
    {
        if (args.CommandColumn.Title == "Entrega")
        {
            var tipoo = 10;
            var prod = await Http.GetFromJsonAsync<Producto>($"api/Prod/{args.RowData.CG_ART.Trim()}");
            if (prod.EXIGEOA) tipoo = 28;


            //await JsRuntime.InvokeAsync<object>("open", $"inventario/{tipoo}/true/{args.RowData.CG_ORDF}", "_blank");               
            await JsRuntime.InvokeVoidAsync("open", $"inventario/{tipoo}/true/{args.RowData.CG_ORDF}", "_blank");
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