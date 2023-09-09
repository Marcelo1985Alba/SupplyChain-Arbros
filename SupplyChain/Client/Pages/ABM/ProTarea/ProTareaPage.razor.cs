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

namespace SupplyChain.Pages.ProTareax;

public class ProTareaxPageBase : ComponentBase
{
    public bool Disabled = false;

    public bool Enabled = true;
    protected SfGrid<ProTarea> Grid;

    protected List<ProTarea> pts = new();

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
        pts = await Http.GetFromJsonAsync<List<ProTarea>>("api/ProTarea");

        await base.OnInitializedAsync();
    }

    public void ActionBeginHandler(ActionEventArgs<ProTarea> args)
    {
        if (args.RequestType == Action.BeginEdit)
            Enabled = false;
        else
            Enabled = true;
    }

    public async Task ActionBegin(ActionEventArgs<ProTarea> args)
    {
        if (args.RequestType == Action.Save)
        {
            HttpResponseMessage response;
            var found = pts.Any(o => o.TAREAPROC == args.Data.TAREAPROC);
            var ur = new Orificio();

            if (!found)
            {
                args.Data.TAREAPROC = pts.Max(s => s.TAREAPROC) + 1;
                response = await Http.PostAsJsonAsync("api/ProTarea", args.Data);
            }
            else
            {
                response = await Http.PutAsJsonAsync($"api/ProTarea/{args.Data.TAREAPROC}", args.Data);
            }

            if (response.StatusCode == HttpStatusCode.Created)
            {
            }
        }

        if (args.RequestType == Action.Delete) await EliminarCeldas(args);
    }

    private async Task EliminarCeldas(ActionEventArgs<ProTarea> args)
    {
        try
        {
            if (args.Data != null)
            {
                var isConfirmed =
                    await JsRuntime.InvokeAsync<bool>("confirm", "Seguro de que desea eliminar la Areas?");
                if (isConfirmed)
                    //servicios.Remove(servicios.Find(m => m.PEDIDO == args.Data.PEDIDO));
                    await Http.DeleteAsync($"api/ProTarea/{args.Data.TAREAPROC}");
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
                        var Nuevo = new ProTarea();

                        Nuevo.TAREAPROC = pts.Max(s => s.TAREAPROC) + 1;
                        Nuevo.DESCRIP = selectedRecord.DESCRIP;


                        Nuevo.OBSERVAC = selectedRecord.OBSERVAC;
                        Nuevo.CG_CIA = selectedRecord.CG_CIA;


                        var response = await Http.PostAsJsonAsync("api/ProTarea", Nuevo);

                        if (response.StatusCode == HttpStatusCode.Created)
                        {
                            Grid.Refresh();
                            var pt = await response.Content.ReadFromJsonAsync<ProTarea>();
                            await InvokeAsync(StateHasChanged);
                            Nuevo.TAREAPROC = pt.TAREAPROC;
                            pts.Add(Nuevo);
                            var itemsJson = JsonSerializer.Serialize(pt);
                            Console.WriteLine(itemsJson);
                            pts.OrderByDescending(o => o.TAREAPROC);
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