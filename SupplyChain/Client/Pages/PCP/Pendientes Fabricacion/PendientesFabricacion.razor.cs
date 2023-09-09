using System;
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
using SupplyChain.Shared.PCP;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Navigations;
using Action = Syncfusion.Blazor.Grids.Action;

namespace SupplyChain.Client.Pages.PCP.Pendientes_Fabricacion;

public class PendientesFabricacionBase : ComponentBase
{
    protected const string APPNAME = "grdPendienteFabricar";

    protected DialogSettings DialogParams = new() { MinHeight = "400px", Width = "500px" };
    public bool Disabled = false;

    public bool Enabled = true;
    protected SfGrid<vPendienteFabricar> Grid;

    //protected List<CatOpe> catopes = new List<CatOpe>();
    protected List<vPendienteFabricar> listaPendFab = new();

    protected NotificacionToast NotificacionObj;
    protected string state;

    protected List<object> Toolbaritems = new()
    {
        "Search",
        new ItemModel { Type = ItemType.Separator },
        "Print",
        new ItemModel { Type = ItemType.Separator },
        "ExcelExport"
    };

    [Inject] protected HttpClient Http { get; set; }
    [Inject] protected IJSRuntime JsRuntime { get; set; }
    protected bool VisibleProperty { get; set; }
    protected bool ToastVisible { get; set; } = false;

    protected override async Task OnInitializedAsync()
    {
        VisibleProperty = true;
        listaPendFab = await Http.GetFromJsonAsync<List<vPendienteFabricar>>("api/PendientesFabricar");

        await base.OnInitializedAsync();
    }

    public async Task DataBoundHandler()
    {
        await Grid.AutoFitColumns();
        VisibleProperty = false;
    }

    public async Task ClickHandler(ClickEventArgs args)
    {
        if (args.Item.Text == "Excel Export") await Grid.ExcelExport();
        if (args.Item.Text == "Print") await Grid.Print();
    }

    public async Task ActionBegin(ActionEventArgs<vPendienteFabricar> args)
    {
        if (args.RequestType == Action.Save)
        {
            if (args.Data.CG_FORM != 1)
            {
                var isConfirmed = await JsRuntime.InvokeAsync<bool>("confirm",
                    "El producto no tiene Fórmula o no tiene Fórmula Activa");

                if (isConfirmed)
                {
                }
            }
            else
            {
                if (args.Data.CANTEMITIR != 0)
                {
                    HttpResponseMessage response;

                    //Convertir a modelo 
                    var modelo = new ModeloPendientesFabricar
                    {
                        CALCULADO = args.Data.CALCULADO,
                        CANTEMITIR = args.Data.CANTEMITIR,
                        CANTPED = args.Data.CANTPED,
                        CG_ART = args.Data.CG_ART,
                        CG_FORM = args.Data.CG_FORM,
                        DES_ART = args.Data.DES_ART,
                        EXIGEOA = args.Data.EXIGEOA == "Armado",
                        LOPTIMO = args.Data.LOPTIMO,
                        PEDIDO = args.Data.PEDIDO,
                        PREVISION = args.Data.PREVISION,
                        REGISTRO = args.Data.REGISTRO,
                        STOCK = args.Data.STOCK,
                        STOCKENT = args.Data.STOCKENT,
                        STOCKMIN = args.Data.STOCKMIN
                    };

                    response = await Http
                        .PutAsJsonAsync($"api/PendientesFabricar/PutPenFab/{Convert.ToInt32(args.Data.REGISTRO)}",
                            modelo);

                    if (response.StatusCode == HttpStatusCode.BadRequest
                        || response.StatusCode == HttpStatusCode.NotFound
                        || response.StatusCode == HttpStatusCode.Conflict)
                    {
                        var mensServidor = await response.Content.ReadAsStringAsync();

                        Console.WriteLine($"Error: {mensServidor}");
                        await NotificacionObj.ShowAsyncError();
                    }
                    else
                    {
                        listaPendFab = await Http.GetFromJsonAsync<List<vPendienteFabricar>>("api/PendientesFabricar");
                        Grid.Refresh();
                        await NotificacionObj.ShowAsync();
                    }
                }
            }
        }

        if (args.RequestType == Action.Grouping ||
            args.RequestType == Action.UnGrouping
            || args.RequestType == Action.ClearFiltering
            || args.RequestType == Action.CollapseAllComplete
            || args.RequestType == Action.ColumnState
            || args.RequestType == Action.ClearFiltering
            || args.RequestType == Action.Reorder
            || args.RequestType == Action.CollapseAllComplete)
        {
            VisibleProperty = false;
            state = await Grid.GetPersistData();
        }
    }

    protected async Task EmitirOrden()
    {
        var xCuantas = listaPendFab.Where(s => s.CANTEMITIR > 0).Count();
        if (xCuantas == 0)
        {
            var isConfirmed = await JsRuntime.InvokeAsync<bool>("confirm",
                "No hay productos con 'Cantidad a emitir' en las 'Necesidades de stock' para emitir órdenes de fabricación");
            if (isConfirmed)
            {
            }
        }
        else
        {
            var isConfirmed = await JsRuntime.InvokeAsync<bool>("confirm",
                "Va a emitir órdenes de fabricación según necesidades de stock \n\n¿Desea continuar?");
            if (isConfirmed)
            {
                listaPendFab =
                    await Http.GetFromJsonAsync<List<vPendienteFabricar>>("api/PendientesFabricar/EmitirOrdenes");
                await NotificacionObj.ShowAsync();
            }
        }
    }

    public async Task QueryCellInfoHandler(QueryCellInfoEventArgs<vPendienteFabricar> args)
    {
        if (args.Data.CG_FORM == 0) args.Cell.AddClass(new[] { "rojas" });
        if (args.Column.Field == "CANTEMITIR") args.Cell.AddClass(new[] { "gris" });
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