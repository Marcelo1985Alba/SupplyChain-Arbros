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

namespace SupplyChain.Pages.Indicx;

public class IndicxPageBase : ComponentBase
{
    public bool Disabled = false;

    public bool Enabled = true;
    protected SfGrid<Indic> Grid;

    protected List<Indic> indics = new();

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
        indics = await Http.GetFromJsonAsync<List<Indic>>("api/Indic");

        await base.OnInitializedAsync();
    }

    public void ActionBeginHandler(ActionEventArgs<Indic> args)
    {
        if (args.RequestType == Action.BeginEdit)
            Enabled = false;
        else
            Enabled = true;
    }

    public async Task ActionBegin(ActionEventArgs<Indic> args)
    {
        if (args.RequestType == Action.Save)
        {
            HttpResponseMessage response;
            var found = indics.Any(o => o.REGISTRO == args.Data.REGISTRO);
            var ur = new Orificio();

            if (!found)
            {
                args.Data.REGISTRO = indics.Max(s => s.REGISTRO) + 1;
                response = await Http.PostAsJsonAsync("api/Indic", args.Data);
            }
            else
            {
                response = await Http.PutAsJsonAsync($"api/Indic/{args.Data.REGISTRO}", args.Data);
            }

            if (response.StatusCode == HttpStatusCode.Created)
            {
            }
        }

        if (args.RequestType == Action.Delete) await EliminarCeldas(args);
    }

    private async Task EliminarCeldas(ActionEventArgs<Indic> args)
    {
        try
        {
            if (args.Data != null)
            {
                var isConfirmed =
                    await JsRuntime.InvokeAsync<bool>("confirm", "Seguro de que desea eliminar la Areas?");
                if (isConfirmed)
                    //servicios.Remove(servicios.Find(m => m.PEDIDO == args.Data.PEDIDO));
                    await Http.DeleteAsync($"api/Indic/{args.Data.REGISTRO}");
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
                        var Nuevo = new Indic();

                        Nuevo.REGISTRO = indics.Max(s => s.REGISTRO) + 1;
                        Nuevo.DES_IND = selectedRecord.DES_IND;
                        Nuevo.VA_INDIC = selectedRecord.VA_INDIC;


                        Nuevo.VA_COMPRA = selectedRecord.VA_COMPRA;
                        Nuevo.FE_INDIC = selectedRecord.FE_INDIC;


                        var response = await Http.PostAsJsonAsync("api/Indic", Nuevo);

                        if (response.StatusCode == HttpStatusCode.Created)
                        {
                            Grid.Refresh();
                            var indi = await response.Content.ReadFromJsonAsync<Indic>();
                            await InvokeAsync(StateHasChanged);
                            Nuevo.REGISTRO = indi.REGISTRO;
                            indics.Add(Nuevo);
                            var itemsJson = JsonSerializer.Serialize(indi);
                            Console.WriteLine(itemsJson);
                            indics.OrderByDescending(o => o.REGISTRO);
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