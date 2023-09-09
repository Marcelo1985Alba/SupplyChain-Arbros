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

namespace SupplyChain.Pages.ABM.Depositos.DepositosPageBase;

public class DepositosPageBase : ComponentBase
{
    protected List<Deposito> deposito = new();
    public bool Disabled = false;

    public bool Enabled = true;
    protected SfGrid<Deposito> Grid;

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
        deposito = await Http.GetFromJsonAsync<List<Deposito>>("api/Deposito");

        await base.OnInitializedAsync();
    }

    public void ActionBeginHandler(ActionEventArgs<Deposito> args)
    {
        if (args.RequestType == Action.BeginEdit)
            Enabled = false;
        else
            Enabled = true;
    }

    public async Task ActionBegin(ActionEventArgs<Deposito> args)
    {
        if (args.RequestType == Action.Save)
        {
            HttpResponseMessage response;
            var found = deposito.Any(o => o.CG_DEP == args.Data.CG_DEP);
            var ur = new Orificio();

            if (!found)
            {
                args.Data.CG_DEP = deposito.Max(s => s.CG_DEP) + 1;
                response = await Http.PostAsJsonAsync("api/Deposito", args.Data);
            }
            else
            {
                response = await Http.PutAsJsonAsync($"api/Deposito/{args.Data.CG_DEP}", args.Data);
            }

            if (response.StatusCode == HttpStatusCode.Created)
            {
            }
        }

        if (args.RequestType == Action.Delete) await EliminarCeldas(args);
    }

    private async Task EliminarCeldas(ActionEventArgs<Deposito> args)
    {
        try
        {
            if (args.Data != null)
            {
                var isConfirmed =
                    await JsRuntime.InvokeAsync<bool>("confirm", "Seguro de que desea eliminar la Areas?");
                if (isConfirmed)
                    //servicios.Remove(servicios.Find(m => m.PEDIDO == args.Data.PEDIDO));
                    await Http.DeleteAsync($"api/Deposito/{args.Data.CG_DEP}");
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
                    var isConfirmed =
                        await JsRuntime.InvokeAsync<bool>("confirm", "Seguro de que desea copiar el area?");
                    if (isConfirmed)
                    {
                        var Nuevo = new Deposito();

                        Nuevo.CG_DEP = deposito.Max(s => s.CG_DEP) + 1;
                        Nuevo.DES_DEP = selectedRecord.DES_DEP;


                        Nuevo.TIPO_DEP = selectedRecord.TIPO_DEP;
                        Nuevo.CG_CIA = selectedRecord.CG_CIA;
                        Nuevo.CG_CLI = selectedRecord.CG_CLI;
                        Nuevo.CG_PROVE = selectedRecord.CG_PROVE;


                        var response = await Http.PostAsJsonAsync("api/Deposito", Nuevo);

                        if (response.StatusCode == HttpStatusCode.Created)
                        {
                            Grid.Refresh();
                            var depositos = await response.Content.ReadFromJsonAsync<Deposito>();
                            await InvokeAsync(StateHasChanged);
                            Nuevo.CG_DEP = depositos.CG_DEP;
                            deposito.Add(Nuevo);
                            var itemsJson = JsonSerializer.Serialize(depositos);
                            Console.WriteLine(itemsJson);
                            deposito.OrderByDescending(o => o.CG_DEP);
                        }
                    }
                }

        if (args.Item.Text == "Excel Export") await Grid.ExcelExport();
    }

    public void Refresh()
    {
        Grid.Refresh();
    }
}