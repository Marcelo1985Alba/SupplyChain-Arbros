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

namespace SupplyChain;

public class TipoAreaPageBase : ComponentBase
{
    protected List<EstaActivo> ActivoData = new()
    {
        new EstaActivo() { BActivo = true, Text = "SI" },
        new EstaActivo() { BActivo = false, Text = "NO" }
    };

    public bool Disabled = false;

    public bool Enabled = true;
    protected SfGrid<TipoArea> Grid;

    private List<Moneda> MonedaData = new()
    {
        new Moneda() { ID = "Mon1", Text = "Peso Argentino" },
        new Moneda() { ID = "Mon2", Text = "Dolar" },
        new Moneda() { ID = "Mon3", Text = "Euro" }
    };


    protected List<TipoArea> tipoareas = new();


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
        tipoareas = await Http.GetFromJsonAsync<List<TipoArea>>("api/TipoArea");


        await base.OnInitializedAsync();
    }

    public void ActionBeginHandler(ActionEventArgs<TipoArea> args)
    {
        if (args.RequestType == Action.BeginEdit)
            Enabled = false;
        else
            Enabled = true;
    }

    public async Task ActionBegin(ActionEventArgs<TipoArea> args)
    {
        if (args.RequestType == Action.Save)
        {
            HttpResponseMessage response;
            var found = tipoareas.Any(p => p.Id == args.Data.Id);
            var ur = new TipoArea();

            if (!found)
                response = await Http.PostAsJsonAsync("api/TipoArea", args.Data);
            else
                response = await Http.PutAsJsonAsync($"api/TipoArea/{args.Data.Id}", args.Data);

            if (response.StatusCode == HttpStatusCode.Created)
            {
            }
        }

        if (args.RequestType == Action.Delete) await EliminarOperario(args);
    }

    private async Task EliminarOperario(ActionEventArgs<TipoArea> args)
    {
        try
        {
            if (args.Data != null)
            {
                var isConfirmed =
                    await JsRuntime.InvokeAsync<bool>("confirm", "Seguro de que desea eliminar el Operario?");
                if (isConfirmed)
                    //operarios.Remove(operarios.Find(m => m.CG_OPER == args.Data.CG_OPER));
                    await Http.DeleteAsync($"api/TipoArea/{args.Data.Id}");
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
                        await JsRuntime.InvokeAsync<bool>("confirm", "Seguro de que desea copiar el Operario?");
                    if (isConfirmed)
                    {
                        var Nuevo = new TipoArea();

                        //Nuevo.CG_OPER = operarios.Max(s => s.CG_OPER) + 1;
                        Nuevo.DES_TIPOAREA = selectedRecord.DES_TIPOAREA;


                        var response = await Http.PostAsJsonAsync("api/TipoArea", Nuevo);

                        if (response.StatusCode == HttpStatusCode.Created)
                        {
                            Grid.Refresh();
                            var tipoarea = await response.Content.ReadFromJsonAsync<TipoArea>();
                            await InvokeAsync(StateHasChanged);
                            Nuevo.Id = tipoarea.Id;
                            tipoareas.Add(Nuevo);
                            var itemsJson = JsonSerializer.Serialize(tipoarea);
                            Console.WriteLine(itemsJson);
                            //toastService.ShowToast($"Registrado Correctemente.Vale {StockGuardado.VALE}", TipoAlerta.Success);
                            tipoareas.OrderByDescending(p => p.Id);
                        }
                    }
                }

        if (args.Item.Text == "Excel Export") await Grid.ExcelExport();
    }

    public void Refresh()
    {
        Grid.Refresh();
    }

    public class Moneda
    {
        public string ID { get; set; }
        public string Text { get; set; }
    }

    public class EstaActivo
    {
        public bool BActivo { get; set; }
        public string Text { get; set; }
    }
}