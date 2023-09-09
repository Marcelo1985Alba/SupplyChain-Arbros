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

namespace SupplyChain.Pages.Linea;

public class LineaPageBase : ComponentBase
{
    public bool Disabled = false;

    public bool Enabled = true;
    protected SfGrid<Lineas> Grid;

    protected List<Lineas> lineas = new();

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
        lineas = await Http.GetFromJsonAsync<List<Lineas>>("api/Lineas");

        await base.OnInitializedAsync();
    }

    public void ActionBeginHandler(ActionEventArgs<Lineas> args)
    {
        if (args.RequestType == Action.BeginEdit)
            Enabled = false;
        else
            Enabled = true;
    }

    public async Task ActionBegin(ActionEventArgs<Lineas> args)
    {
        if (args.RequestType == Action.Save)
        {
            HttpResponseMessage response;
            var found = lineas.Any(o => o.Id == args.Data.Id);
            var ur = new Orificio();

            if (!found)
            {
                args.Data.Id = lineas.Max(s => s.Id) + 1;
                response = await Http.PostAsJsonAsync("api/Lineas", args.Data);
            }
            else
            {
                response = await Http.PutAsJsonAsync($"api/Lineas/{args.Data.Id}", args.Data);
            }

            if (response.StatusCode == HttpStatusCode.Created)
            {
            }
        }

        if (args.RequestType == Action.Delete) await EliminarCeldas(args);
    }

    private async Task EliminarCeldas(ActionEventArgs<Lineas> args)
    {
        try
        {
            if (args.Data != null)
            {
                var isConfirmed =
                    await JsRuntime.InvokeAsync<bool>("confirm", "Seguro de que desea eliminar la Areas?");
                if (isConfirmed)
                    //servicios.Remove(servicios.Find(m => m.PEDIDO == args.Data.PEDIDO));
                    await Http.DeleteAsync($"api/Lineas/{args.Data.Id}");
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
                        var Nuevo = new Lineas();

                        Nuevo.Id = lineas.Max(s => s.Id) + 1;
                        Nuevo.DES_LINEA = selectedRecord.DES_LINEA;

                        var response = await Http.PostAsJsonAsync("api/Lineas", Nuevo);

                        if (response.StatusCode == HttpStatusCode.Created)
                        {
                            Grid.Refresh();
                            var linea = await response.Content.ReadFromJsonAsync<Lineas>();
                            await InvokeAsync(StateHasChanged);
                            Nuevo.Id = linea.Id;
                            lineas.Add(Nuevo);
                            var itemsJson = JsonSerializer.Serialize(linea);
                            Console.WriteLine(itemsJson);
                            lineas.OrderByDescending(o => o.Id);
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