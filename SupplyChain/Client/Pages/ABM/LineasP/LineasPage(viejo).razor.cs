using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SupplyChain;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Navigations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace SupplyChain.Pages.Linea
{
    public class LineaPageBase : ComponentBase
    {
        [Inject] protected HttpClient Http { get; set; }
        [Inject] protected IJSRuntime JsRuntime { get; set; }
        protected SfGrid<Lineas> Grid;

        public bool Enabled = true;
        public bool Disabled = false;

        protected List<Lineas> lineas = new List<Lineas>();

        protected List<Object> Toolbaritems = new List<Object>(){
        "Search",
        "Add",
        "Edit",
        "Delete",
        "Print",
        new ItemModel { Text = "Copy", TooltipText = "Copy", PrefixIcon = "e-copy", Id = "copy" },
        "ExcelExport"
    };

        protected override async Task OnInitializedAsync()
        {
            lineas = await Http.GetFromJsonAsync<List<Lineas>>("api/Lineas");

            await base.OnInitializedAsync();
        }

        public void ActionBeginHandler(ActionEventArgs<Lineas> args)
        {
            if (args.RequestType == Syncfusion.Blazor.Grids.Action.BeginEdit)
            {
                this.Enabled = false;
            }
            else
            {
                this.Enabled = true;
            }
        }
        public async Task ActionBegin(ActionEventArgs<Lineas> args)
        {
            if (args.RequestType == Syncfusion.Blazor.Grids.Action.Save)
            {
                HttpResponseMessage response;
                bool found = lineas.Any(o => o.Id == args.Data.Id);
                Orificio ur = new Orificio();

                if (!found)
                {
                    args.Data.Id = lineas.Max(s => s.Id) + 1;
                    response = await Http.PostAsJsonAsync("api/Lineas", args.Data);
                }
                else
                {
                    response = await Http.PutAsJsonAsync($"api/Lineas/{args.Data.Id}", args.Data);
                }

                if (response.StatusCode == System.Net.HttpStatusCode.Created)
                {

                }
            }

            if (args.RequestType == Syncfusion.Blazor.Grids.Action.Delete)
            {
                await EliminarCeldas(args);
            }
        }

        private async Task EliminarCeldas(ActionEventArgs<Lineas> args)
        {
            try
            {
                if (args.Data != null)
                {
                    bool isConfirmed = await JsRuntime.InvokeAsync<bool>("confirm", "Seguro de que desea eliminar la Areas?");
                    if (isConfirmed)
                    {
                        //servicios.Remove(servicios.Find(m => m.PEDIDO == args.Data.PEDIDO));
                        await Http.DeleteAsync($"api/Lineas/{args.Data.Id}");
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        public async Task ClickHandler(Syncfusion.Blazor.Navigations.ClickEventArgs args)
        {
            if (args.Item.Text == "Copy")
            {
                if (this.Grid.SelectedRecords.Count > 0)
                {
                    foreach (Lineas selectedRecord in this.Grid.SelectedRecords)
                    {
                        bool isConfirmed = await JsRuntime.InvokeAsync<bool>("confirm", "Seguro de que desea copiar el area?");
                        if (isConfirmed)
                        {
                            Lineas Nuevo = new Lineas();

                            Nuevo.Id = lineas.Max(s => s.Id) + 1;
                            Nuevo.DES_LINEA = selectedRecord.DES_LINEA;

                            var response = await Http.PostAsJsonAsync("api/Lineas", Nuevo);

                            if (response.StatusCode == System.Net.HttpStatusCode.Created)
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
                }
            }
            if (args.Item.Text == "Excel Export")
            {
                await this.Grid.ExcelExport();
            }
        }

        public void Refresh()
        {
            Grid.Refresh();
        }
    }
}
