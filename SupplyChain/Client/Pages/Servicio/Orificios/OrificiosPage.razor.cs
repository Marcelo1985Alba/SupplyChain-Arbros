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

namespace SupplyChain.Pages.Orificios;

public class OrificioPageBase : ComponentBase
{
    public bool Disabled = false;

    public bool Enabled = true;
    protected SfGrid<Orificio> Grid;

    protected List<Orificio> orificios = new();

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
        orificios = await Http.GetFromJsonAsync<List<Orificio>>("api/Orificio");

        await base.OnInitializedAsync();
    }

    public void ActionBeginHandler(ActionEventArgs<Orificio> args)
    {
        if (args.RequestType == Action.BeginEdit)
            Enabled = false;
        else
            Enabled = true;
    }

    public async Task ActionBegin(ActionEventArgs<Orificio> args)
    {
        if (args.RequestType == Action.Save)
        {
            HttpResponseMessage response;
            var found = orificios.Any(o => o.Id == args.Data.Id);
            var ur = new Orificio();

            if (!found)
            {
                args.Data.Id = orificios.Max(s => s.Id) + 1;
                args.Data.CG_ORDEN = 1;
                response = await Http.PostAsJsonAsync("api/Orificio", args.Data);
            }
            else
            {
                response = await Http.PutAsJsonAsync($"api/Orificio/{args.Data.Id}", args.Data);
            }

            if (response.StatusCode == HttpStatusCode.Created)
            {
            }
        }

        if (args.RequestType == Action.Delete) await EliminarServicio(args);
    }

    private async Task EliminarServicio(ActionEventArgs<Orificio> args)
    {
        try
        {
            if (args.Data != null)
            {
                var isConfirmed = await JsRuntime.InvokeAsync<bool>("confirm",
                    "Seguro de que desea eliminar el orificio / la reparacion?");
                if (isConfirmed)
                    //servicios.Remove(servicios.Find(m => m.PEDIDO == args.Data.PEDIDO));
                    await Http.DeleteAsync($"api/Orificio/{args.Data.Id}");
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
                        "Seguro de que desea copiar el Orificio / la reparacion?");
                    if (isConfirmed)
                    {
                        var Nuevo = new Orificio();

                        Nuevo.Id = orificios.Max(s => s.Id) + 1;
                        Nuevo.Codigo = selectedRecord.Codigo;
                        Nuevo.Descripcion = selectedRecord.Descripcion;
                        Nuevo.CG_ORDEN = selectedRecord.CG_ORDEN;

                        var response = await Http.PostAsJsonAsync("api/Orificio", Nuevo);

                        if (response.StatusCode == HttpStatusCode.Created)
                        {
                            Grid.Refresh();
                            var orificio = await response.Content.ReadFromJsonAsync<Orificio>();
                            await InvokeAsync(StateHasChanged);
                            Nuevo.Id = orificio.Id;
                            orificios.Add(Nuevo);
                            var itemsJson = JsonSerializer.Serialize(orificio);
                            Console.WriteLine(itemsJson);
                            //toastService.ShowToast($"Registrado Correctemente.Vale {StockGuardado.VALE}", TipoAlerta.Success);
                            orificios.OrderByDescending(o => o.Id);
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