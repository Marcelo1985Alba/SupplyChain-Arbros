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

public class CateOperariosPageBase : ComponentBase
{
    protected List<EstaActivo> ActivoData = new()
    {
        new EstaActivo() { BActivo = true, Text = "SI" },
        new EstaActivo() { BActivo = false, Text = "NO" }
    };


    protected List<CatOpe> catopes = new();
    public bool Disabled = false;

    public bool Enabled = true;
    protected SfGrid<CatOpe> Grid;

    private List<Moneda> MonedaData = new()
    {
        new Moneda() { ID = "Mon1", Text = "Peso Argentino" },
        new Moneda() { ID = "Mon2", Text = "Dolar" },
        new Moneda() { ID = "Mon3", Text = "Euro" }
    };


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
        catopes = await Http.GetFromJsonAsync<List<CatOpe>>("api/CatOpe");


        await base.OnInitializedAsync();
    }

    public void ActionBeginHandler(ActionEventArgs<CatOpe> args)
    {
        if (args.RequestType == Action.BeginEdit)
            Enabled = false;
        else
            Enabled = true;
    }

    public async Task ActionBegin(ActionEventArgs<CatOpe> args)
    {
        if (args.RequestType == Action.Save)
        {
            HttpResponseMessage response;
            var found = catopes.Any(p => p.CG_CATEOP == args.Data.CG_CATEOP);
            var ur = new CatOpe();

            if (!found)
                response = await Http.PostAsJsonAsync("api/CatOpe", args.Data);
            else
                response = await Http.PutAsJsonAsync($"api/CatOpe/{args.Data.CG_CATEOP}", args.Data);

            if (response.StatusCode == HttpStatusCode.Created)
            {
            }
        }

        if (args.RequestType == Action.Delete) await EliminarOperario(args);
    }

    private async Task EliminarOperario(ActionEventArgs<CatOpe> args)
    {
        try
        {
            if (args.Data != null)
            {
                var isConfirmed =
                    await JsRuntime.InvokeAsync<bool>("confirm", "Seguro de que desea eliminar la clase?");
                if (isConfirmed)
                    //operarios.Remove(operarios.Find(m => m.CG_OPER == args.Data.CG_OPER));
                    await Http.DeleteAsync($"api/CatOpe/{args.Data.CG_CATEOP}");
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
                        await JsRuntime.InvokeAsync<bool>("confirm", "Seguro de que desea copiar la Clase?");
                    if (isConfirmed)
                    {
                        var Nuevo = new CatOpe();

                        //Nuevo.CG_OPER = operarios.Max(s => s.CG_OPER) + 1;
                        Nuevo.DES_CATEOP = selectedRecord.DES_CATEOP;
                        Nuevo.VALOR_HORA = selectedRecord.VALOR_HORA;
                        Nuevo.MONEDA = selectedRecord.MONEDA;


                        var response = await Http.PostAsJsonAsync("api/CateOpe", Nuevo);

                        if (response.StatusCode == HttpStatusCode.Created)
                        {
                            Grid.Refresh();
                            var cateope = await response.Content.ReadFromJsonAsync<CatOpe>();
                            await InvokeAsync(StateHasChanged);
                            Nuevo.CG_CATEOP = cateope.CG_CATEOP;
                            catopes.Add(Nuevo);
                            var itemsJson = JsonSerializer.Serialize(cateope);
                            Console.WriteLine(itemsJson);
                            //toastService.ShowToast($"Registrado Correctemente.Vale {StockGuardado.VALE}", TipoAlerta.Success);
                            catopes.OrderByDescending(p => p.CG_CATEOP);
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