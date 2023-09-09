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

namespace SupplyChain.Pages.Paradax;

public class ParadaxPageBase : ComponentBase
{
    public bool Disabled = false;

    public bool Enabled = true;
    protected SfGrid<Parada> Grid;

    protected List<Parada> paradas = new();

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
        paradas = await Http.GetFromJsonAsync<List<Parada>>("api/Parada");

        await base.OnInitializedAsync();
    }

    public void ActionBeginHandler(ActionEventArgs<Parada> args)
    {
        if (args.RequestType == Action.BeginEdit)
            Enabled = false;
        else
            Enabled = true;
    }

    public async Task ActionBegin(ActionEventArgs<Parada> args)
    {
        if (args.RequestType == Action.Save)
        {
            HttpResponseMessage response;
            var found = paradas.Any(o => o.CP == args.Data.CP);
            var ur = new Orificio();

            if (!found)
            {
                args.Data.CP = paradas.Max(s => s.CP) + 1;
                response = await Http.PostAsJsonAsync("api/Parada", args.Data);
            }
            else
            {
                response = await Http.PutAsJsonAsync($"api/Parada/{args.Data.CP}", args.Data);
            }

            if (response.StatusCode == HttpStatusCode.Created)
            {
            }
        }

        if (args.RequestType == Action.Delete) await EliminarCeldas(args);
    }

    private async Task EliminarCeldas(ActionEventArgs<Parada> args)
    {
        try
        {
            if (args.Data != null)
            {
                var isConfirmed =
                    await JsRuntime.InvokeAsync<bool>("confirm", "Seguro de que desea eliminar la Areas?");
                if (isConfirmed)
                    //servicios.Remove(servicios.Find(m => m.PEDIDO == args.Data.PEDIDO));
                    await Http.DeleteAsync($"api/Parada/{args.Data.CP}");
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
                        var Nuevo = new Parada();

                        Nuevo.CP = paradas.Max(s => s.CP) + 1;
                        Nuevo.DESCRIP = selectedRecord.DESCRIP;
                        Nuevo.CG_CIA = selectedRecord.CG_CIA;


                        var response = await Http.PostAsJsonAsync("api/Parada", Nuevo);

                        if (response.StatusCode == HttpStatusCode.Created)
                        {
                            Grid.Refresh();
                            var para = await response.Content.ReadFromJsonAsync<Parada>();
                            await InvokeAsync(StateHasChanged);
                            Nuevo.CP = para.CP;
                            paradas.Add(Nuevo);
                            var itemsJson = JsonSerializer.Serialize(para);
                            Console.WriteLine(itemsJson);
                            paradas.OrderByDescending(o => o.CP);
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