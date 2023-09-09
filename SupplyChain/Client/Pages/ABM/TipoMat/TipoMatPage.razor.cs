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

namespace SupplyChain.Pages.TipoMatx;

public class TipoMatxPageBase : ComponentBase
{
    public bool Disabled = false;

    public bool Enabled = true;
    protected SfGrid<TipoMat> Grid;

    protected List<TipoMat> tipomats = new();

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
        tipomats = await Http.GetFromJsonAsync<List<TipoMat>>("api/TipoMat");

        await base.OnInitializedAsync();
    }

    public void ActionBeginHandler(ActionEventArgs<TipoMat> args)
    {
        if (args.RequestType == Action.BeginEdit)
            Enabled = false;
        else
            Enabled = true;
    }

    public async Task ActionBegin(ActionEventArgs<TipoMat> args)
    {
        if (args.RequestType == Action.Save)
        {
            HttpResponseMessage response;
            var found = tipomats.Any(o => o.TIPO == args.Data.TIPO);
            var ur = new Orificio();

            if (!found)
            {
                args.Data.TIPO = tipomats.Max(s => s.TIPO) + 1;
                response = await Http.PostAsJsonAsync("api/TipoMat", args.Data);
            }
            else
            {
                response = await Http.PutAsJsonAsync($"api/TipoMat/{args.Data.TIPO}", args.Data);
            }

            if (response.StatusCode == HttpStatusCode.Created)
            {
            }
        }

        if (args.RequestType == Action.Delete) await EliminarCeldas(args);
    }

    private async Task EliminarCeldas(ActionEventArgs<TipoMat> args)
    {
        try
        {
            if (args.Data != null)
            {
                var isConfirmed =
                    await JsRuntime.InvokeAsync<bool>("confirm", "Seguro de que desea eliminar la Areas?");
                if (isConfirmed)
                    //servicios.Remove(servicios.Find(m => m.PEDIDO == args.Data.PEDIDO));
                    await Http.DeleteAsync($"api/TipoMat/{args.Data.TIPO}");
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
                        var Nuevo = new TipoMat();

                        Nuevo.TIPO = tipomats.Max(s => s.TIPO) + 1;

                        var response = await Http.PostAsJsonAsync("api/TipoMat", Nuevo);

                        if (response.StatusCode == HttpStatusCode.Created)
                        {
                            Grid.Refresh();
                            var tipo = await response.Content.ReadFromJsonAsync<TipoMat>();
                            await InvokeAsync(StateHasChanged);
                            Nuevo.TIPO = tipo.TIPO;
                            tipomats.Add(Nuevo);
                            var itemsJson = JsonSerializer.Serialize(tipo);
                            Console.WriteLine(itemsJson);
                            tipomats.OrderByDescending(o => o.TIPO);
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