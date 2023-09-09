using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SupplyChain.Shared.Models;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Navigations;
using Syncfusion.Blazor.Spinner;
using Action = Syncfusion.Blazor.Grids.Action;

namespace SupplyChain.Client.Pages.PCP.Abastecimiento;

public class AbastecimeintoBase : ComponentBase
{
    protected string claseMP = "btn btn-sm btn-outline-primary";
    protected string claseSE = "btn btn-sm btn-primary active";

    protected DialogSettings DialogParams = new() { MinHeight = "400px", Width = "500px" };
    protected bool Disabled = false;

    protected bool Enabled = true;
    protected SfGrid<ModeloAbastecimiento> GridMP;
    protected SfGrid<ModeloAbastecimiento> GridSE;

    //protected List<CatOpe> catopes = new List<CatOpe>();
    protected List<ModeloAbastecimiento> listaAbastMP = new();
    protected List<ModeloAbastecimiento> listaAbastSE = new();
    protected bool ShowMatP;
    protected bool ShowSemi;
    protected SfSpinner SpinnerObjMP;
    protected SfSpinner SpinnerObjSE;

    protected List<object> ToolbaritemsMP = new()
    {
        "Search",
        new ItemModel { Type = ItemType.Separator },
        "Print",
        new ItemModel { Type = ItemType.Separator },
        "ExcelExport"
    };

    protected List<object> ToolbaritemsSE = new()
    {
        "Search",
        new ItemModel { Type = ItemType.Separator },
        "Print",
        new ItemModel { Type = ItemType.Separator },
        "ExcelExport",
        new ItemModel { Type = ItemType.Separator },
        new ItemModel
            { Text = "Seleccionar Columnas", TooltipText = "Seleccionar Columnas", Id = "Seleccionar Columnas" }
    };

    [Inject] protected HttpClient Http { get; set; }
    [Inject] protected IJSRuntime JsRuntime { get; set; }
    protected bool VisiblePropertyMP { get; set; }

    protected bool VisiblePropertySE { get; set; }

    //protected NotificacionToast NotificacionObj;
    //protected bool ToastVisible { get; set; } = false;
    protected override async Task OnInitializedAsync()
    {
        try
        {
            VisiblePropertySE = true;
            VisiblePropertyMP = true;
            //HttpResponseMessage respuesta;
            var listAbastecimiento = await Http.GetFromJsonAsync<List<ModeloAbastecimiento>>("api/Abastecimiento");
            listaAbastMP = listAbastecimiento.Where(a => a.CG_ORDEN == 4).ToList();
            listaAbastSE = listAbastecimiento.Where(a => a.CG_ORDEN == 3).ToList();
            //listaAbastMP = await Http.GetFromJsonAsync<List<ModeloAbastecimiento>>("api/Abastecimiento/AbastecimientoMP");
            //listaAbastSE =  await Http.GetFromJsonAsync<List<ModeloAbastecimiento>>("api/Abastecimiento/AbastecimientoSE");
            //if (respuesta.StatusCode == System.Net.HttpStatusCode.BadRequest)
            //{
            //    var mensServidor = await respuesta.Content.ReadAsStringAsync();

            //    Console.WriteLine($"Error: {mensServidor}");
            //    //await NotificacionObj.ShowAsyncError();
            //}
            //else
            //{
            //    listaAbastSE = await respuesta.Content.ReadFromJsonAsync<List<ModeloAbastecimiento>>();
            //}

            //await InvokeAsync(StateHasChanged);
            GridSE.PreventRender();
            await GridSE.AutoFitColumnsAsync();
            VisiblePropertyMP = false;
            VisiblePropertySE = false;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }


    protected async Task OnDataBoundGridSE(BeforeDataBoundArgs<ModeloAbastecimiento> args)
    {
        VisiblePropertySE = true;
        GridSE.PreventRender();
        await GridSE.AutoFitColumnsAsync();
        VisiblePropertySE = false;
    }

    public async Task DataBoundHandler(object args)
    {
        GridSE.PreventRender();
        await GridSE.AutoFitColumnsAsync();
        VisiblePropertySE = false;
    }

    public async Task ClickHandlerMP(ClickEventArgs args)
    {
        if (args.Item.Text == "Excel Export") await GridMP.ExcelExport();
        if (args.Item.Text == "Print") await GridMP.Print();
    }

    public async Task ClickHandlerSE(ClickEventArgs args)
    {
        if (args.Item.Text == "Excel Export") await GridSE.ExportToExcelAsync();
        if (args.Item.Text == "Print") await GridSE.PrintAsync();
        if (args.Item.Text == "Seleccionar Columnas") await GridSE.OpenColumnChooserAsync();
    }

    public async Task ActionBeginMP(ActionEventArgs<ModeloAbastecimiento> args)
    {
        if (args.RequestType == Action.Save)
        {
            HttpResponseMessage response;
            response = await Http.PutAsJsonAsync("api/Abastecimiento/PutAbMP", args.Data);
            listaAbastMP =
                await Http.GetFromJsonAsync<List<ModeloAbastecimiento>>("api/Abastecimiento/AbastecimientoMPX");
            await GridMP.Refresh();
        }

        if (args.RequestType == Action.Delete)
        {
        }
    }

    public async Task ActionBeginSE(ActionEventArgs<ModeloAbastecimiento> args)
    {
        if (args.RequestType == Action.Save)
        {
            HttpResponseMessage response;
            response = await Http.PutAsJsonAsync($"api/Abastecimiento/PutAbSE/{args.Data.CG_MAT}", args.Data);
            await GridSE.RefreshHeaderAsync();
            await GridSE.RefreshColumnsAsync();
            await GridSE.Refresh();
        }

        if (args.RequestType == Action.Delete)
        {
        }
    }

    protected void Semis()
    {
        ShowSemi = true;
        ShowMatP = false;
    }

    protected async Task MatP()
    {
        var isConfirmed =
            await JsRuntime.InvokeAsync<bool>("confirm", "Asegúrese de haber Abastecido Semi-Elaborados primero.");
        if (isConfirmed)
        {
            ShowMatP = true;
            ShowSemi = false;
        }
    }

    protected async Task EmitirMP()
    {
        var xCuantas = listaAbastMP.Where(s => s.CALCULADO != 0).Count();
        if (xCuantas != 0)
        {
            var isConfirmed = await JsRuntime.InvokeAsync<bool>("confirm",
                "Hay Semi-Elaborados con Fabricación sugerida. \n\n¿Desea continuar?",
                "Abastecimiento de Materias Primas");
            if (isConfirmed)
            {
                var isConfirmed2 = await JsRuntime.InvokeAsync<bool>("confirm",
                    "Va a ejecutar el proceso de Emitir Preparación de Compras Materias primas. \n\n¿Desea continuar?",
                    "Abastecimiento de Materias Primas");
                if (isConfirmed2)
                {
                    listaAbastMP =
                        await Http.GetFromJsonAsync<List<ModeloAbastecimiento>>("api/Abastecimiento/AbastecerMP");
                    listaAbastMP =
                        await Http.GetFromJsonAsync<List<ModeloAbastecimiento>>("api/Abastecimiento/AbastecimientoMPX");
                    GridMP.Refresh();
                }
            }
        }
    }

    protected async Task EmitirSE()
    {
        var isConfirmed = await JsRuntime.InvokeAsync<bool>("confirm",
            "Va a ejecutar el proceso de abastecer. \n\n¿Desea continuar?", "Abastecimiento de Semi-Elaborados");
        if (isConfirmed)
        {
            listaAbastSE = await Http.GetFromJsonAsync<List<ModeloAbastecimiento>>("api/Abastecimiento/AbastecerSE");
            listaAbastSE =
                await Http.GetFromJsonAsync<List<ModeloAbastecimiento>>("api/Abastecimiento/AbastecimientoSEX");
            await GridSE.RefreshHeaderAsync();
            await GridSE.RefreshColumnsAsync();
            await GridSE.Refresh();
        }
    }

    public void QueryCellInfoHandler(QueryCellInfoEventArgs<ModeloAbastecimiento> args)
    {
        if (args.Column.Field == "ACOMPRAR") args.Cell.AddClass(new[] { "gris" });

        if (args.Data.CantProcesos < 3 && args.Data.CG_ORDEN == 3) args.Cell.AddClass(new[] { "alerta-procesos" });
    }

    protected async Task OnLoadGridSE(object args)
    {
        GridSE.PreventRender();
        VisiblePropertySE = false;
    }

    protected async Task OnLoadGridMP(object args)
    {
        VisiblePropertyMP = true;
    }

    protected void Selecting(SelectingEventArgs args)
    {
        VisiblePropertySE = false;
    }
}