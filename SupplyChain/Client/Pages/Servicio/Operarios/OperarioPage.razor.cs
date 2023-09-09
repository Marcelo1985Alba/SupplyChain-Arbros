using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Navigations;
using Action = Syncfusion.Blazor.Grids.Action;

namespace SupplyChain.Pages.Operarios;

public class OperarioPageBase : ComponentBase
{
    protected List<CatOpe> categorias = new();

    protected DialogSettings DialogParams = new() { MinHeight = "400px", Width = "700px" };
    public bool Disabled = false;

    public bool Enabled = true;
    protected SfGrid<Operario> Grid;

    protected List<Operario> operarios = new();

    public List<SIoNO> SIoNOData = new()
    {
        new() { Texto = "SI", Valor = true },
        new() { Texto = "NO", Valor = false }
    };

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

    [Inject] protected HttpClient Http { get; set; }
    [Inject] protected IJSRuntime JsRuntime { get; set; }

    protected override async Task OnInitializedAsync()
    {
        operarios = await Http.GetFromJsonAsync<List<Operario>>("api/Operario");
        categorias = await Http.GetFromJsonAsync<List<CatOpe>>("api/CatOpe");

        await base.OnInitializedAsync();
    }

    public void ActionBeginHandler(ActionEventArgs<Operario> args)
    {
        if (args.RequestType == Action.BeginEdit)
            Enabled = false;
        else
            Enabled = true;
    }

    public async Task ActionBegin(ActionEventArgs<Operario> args)
    {
        if (args.RequestType == Action.Save)
        {
            HttpResponseMessage response;
            var found = operarios.Any(o => o.CG_OPER == args.Data.CG_OPER);
            var ur = new Operario();

            if (!found)
            {
                args.Data.CG_OPER = operarios.Max(s => s.CG_OPER) + 1;
                response = await Http.PostAsJsonAsync("api/Operario", args.Data);
            }
            else
            {
                response = await Http.PutAsJsonAsync($"api/Operario/{args.Data.CG_OPER}", args.Data);
            }

            if (response.StatusCode == HttpStatusCode.Created)
            {
            }
        }

        if (args.RequestType == Action.Delete) await EliminarServicio(args);
    }

    private async Task EliminarServicio(ActionEventArgs<Operario> args)
    {
        try
        {
            if (args.Data != null)
            {
                var isConfirmed = await JsRuntime.InvokeAsync<bool>("confirm",
                    "Seguro de que desea eliminar el OPERARIO / la reparacion?");
                if (isConfirmed)
                    //servicios.Remove(servicios.Find(m => m.PEDIDO == args.Data.PEDIDO));
                    await Http.DeleteAsync($"api/Operario/{args.Data.CG_OPER}");
            }
        }
        catch (Exception ex)
        {
        }
    }

    public async Task ClickHandler(ClickEventArgs args)
    {
        if (args.Item.Text == "Copy")
            if (Grid.SelectedRecords.Count > 0)
                foreach (var selectedRecord in Grid.SelectedRecords)
                {
                    var isConfirmed = await JsRuntime.InvokeAsync<bool>("confirm",
                        "Seguro de que desea copiar el OPERARIO / la reparacion?");
                    if (isConfirmed)
                    {
                        var Nuevo = new Operario();

                        Nuevo.CG_OPER = operarios.Max(s => s.CG_OPER) + 1;
                        Nuevo.DES_OPER = selectedRecord.DES_OPER;
                        Nuevo.CG_TURNO = selectedRecord.CG_TURNO;
                        Nuevo.RENDIM = selectedRecord.RENDIM;
                        Nuevo.FE_FINAL = selectedRecord.FE_FINAL;
                        Nuevo.HS_FINAL = selectedRecord.HS_FINAL;
                        Nuevo.CG_CATEOP = selectedRecord.CG_CATEOP;
                        Nuevo.VALOR_HORA = selectedRecord.VALOR_HORA;
                        Nuevo.MONEDA = selectedRecord.MONEDA;
                        Nuevo.ACTIVO = selectedRecord.ACTIVO;
                        Nuevo.CG_CIA = selectedRecord.CG_CIA;
                        Nuevo.USUARIO = selectedRecord.USUARIO;


                        var response = await Http.PostAsJsonAsync("api/Operario", Nuevo);

                        if (response.StatusCode == HttpStatusCode.Created)
                        {
                            Grid.Refresh();
                            var operario = await response.Content.ReadFromJsonAsync<Operario>();
                            await InvokeAsync(StateHasChanged);
                            Nuevo.CG_OPER = operario.CG_OPER;
                            operarios.Add(Nuevo);
                            var itemsJson = JsonSerializer.Serialize(operario);
                            Console.WriteLine(itemsJson);
                            //toastService.ShowToast($"Registrado Correctemente.Vale {StockGuardado.VALE}", TipoAlerta.Success);
                            operarios.OrderByDescending(o => o.CG_OPER);
                        }
                    }
                }

        if (args.Item.Text == "Excel Export") await Grid.ExcelExport();
    }

    public void Refresh()
    {
        Grid.Refresh();
    }

    public class SIoNO
    {
        public string Texto { get; set; }
        public bool Valor { get; set; }
    }
}