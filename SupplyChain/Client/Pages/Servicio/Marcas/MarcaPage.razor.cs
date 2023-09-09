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

namespace SupplyChain.Pages.Marcas;

public class MarcaPageBase : ComponentBase
{
    public bool Disabled = false;

    public bool Enabled = true;
    protected SfGrid<Marca> Grid;

    protected List<Marca> marcas = new();

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
        marcas = await Http.GetFromJsonAsync<List<Marca>>("api/Marca");

        await base.OnInitializedAsync();
    }

    public void ActionBeginHandler(ActionEventArgs<Marca> args)
    {
        if (args.RequestType == Action.BeginEdit)
            Enabled = false;
        else
            Enabled = true;
    }

    public async Task ActionBegin(ActionEventArgs<Marca> args)
    {
        if (args.RequestType == Action.Save)
        {
            HttpResponseMessage response;
            var found = marcas.Any(o => o.MARCA == args.Data.MARCA);
            var ur = new Marca();

            if (!found)
                response = await Http.PostAsJsonAsync("api/Marca", args.Data);
            else
                response = await Http.PutAsJsonAsync($"api/Marca/{args.Data.MARCA}", args.Data);

            if (response.StatusCode == HttpStatusCode.Created)
            {
            }
        }

        if (args.RequestType == Action.Delete) await EliminarServicio(args);
    }

    private async Task EliminarServicio(ActionEventArgs<Marca> args)
    {
        try
        {
            if (args.Data != null)
            {
                var isConfirmed = await JsRuntime.InvokeAsync<bool>("confirm",
                    "Seguro de que desea eliminar el servicio / la reparacion?");
                if (isConfirmed)
                    //servicios.Remove(servicios.Find(m => m.PEDIDO == args.Data.PEDIDO));
                    await Http.DeleteAsync($"api/Marca/{args.Data.MARCA}");
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
                        "Seguro de que desea copiar el Marcas / la reparacion?");
                    if (isConfirmed)
                    {
                        var Nuevo = new Marca();
                        Nuevo.MARCA = selectedRecord.MARCA;

                        var response = await Http.PostAsJsonAsync("api/Marca", Nuevo);
                        if (response.StatusCode == HttpStatusCode.Created)
                        {
                            Grid.Refresh();
                            var marca = await response.Content.ReadFromJsonAsync<Marca>();
                            await InvokeAsync(StateHasChanged);
                            Nuevo.MARCA = marca.MARCA;
                            marcas.Add(Nuevo);
                            var itemsJson = JsonSerializer.Serialize(marca);
                            Console.WriteLine(itemsJson);
                            //toastService.ShowToast($"Registrado Correctemente.Vale {StockGuardado.VALE}", TipoAlerta.Success);
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