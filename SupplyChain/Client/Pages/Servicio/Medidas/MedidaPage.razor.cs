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

namespace SupplyChain.Pages.Medidas;

public class MedidaPageBase : ComponentBase
{
    public bool Disabled = false;

    public bool Enabled = true;
    protected SfGrid<Medida> Grid;

    protected List<Medida> medidas = new();

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
        medidas = await Http.GetFromJsonAsync<List<Medida>>("api/Medida");

        await base.OnInitializedAsync();
    }

    public void ActionBeginHandler(ActionEventArgs<Medida> args)
    {
        if (args.RequestType == Action.BeginEdit)
            Enabled = false;
        else
            Enabled = true;
    }

    public async Task ActionBegin(ActionEventArgs<Medida> args)
    {
        if (args.RequestType == Action.Save)
        {
            HttpResponseMessage response;
            var found = medidas.Any(o => o.Id == args.Data.Id);
            var ur = new Medida();

            if (!found)
            {
                args.Data.Id = medidas.Max(s => s.Id) + 1;
                response = await Http.PostAsJsonAsync("api/Medida", args.Data);
            }
            else
            {
                response = await Http.PutAsJsonAsync($"api/Medida/{args.Data.Id}", args.Data);
            }

            if (response.StatusCode == HttpStatusCode.Created)
            {
            }
        }

        if (args.RequestType == Action.Delete) await EliminarServicio(args);
    }

    private async Task EliminarServicio(ActionEventArgs<Medida> args)
    {
        try
        {
            if (args.Data != null)
            {
                var isConfirmed = await JsRuntime.InvokeAsync<bool>("confirm",
                    "Seguro de que desea eliminar la medida / la reparacion?");
                if (isConfirmed)
                    //servicios.Remove(servicios.Find(m => m.PEDIDO == args.Data.PEDIDO));
                    await Http.DeleteAsync($"api/Medida/{args.Data.Id}");
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
                        "Seguro de que desea copiar la Medida / la reparacion?");
                    if (isConfirmed)
                    {
                        var Nuevo = new Medida();

                        Nuevo.Id = medidas.Max(s => s.Id) + 1;
                        Nuevo.Descripcion = selectedRecord.Descripcion;
                        Nuevo.Codigo = selectedRecord.Codigo;

                        var response = await Http.PostAsJsonAsync("api/Medida", Nuevo);

                        if (response.StatusCode == HttpStatusCode.Created)
                        {
                            Grid.Refresh();
                            var medida = await response.Content.ReadFromJsonAsync<Medida>();
                            await InvokeAsync(StateHasChanged);
                            Nuevo.Id = medida.Id;
                            medidas.Add(Nuevo);
                            var itemsJson = JsonSerializer.Serialize(medida);
                            Console.WriteLine(itemsJson);
                            //toastService.ShowToast($"Registrado Correctemente.Vale {StockGuardado.VALE}", TipoAlerta.Success);
                            medidas.OrderByDescending(o => o.Id);
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