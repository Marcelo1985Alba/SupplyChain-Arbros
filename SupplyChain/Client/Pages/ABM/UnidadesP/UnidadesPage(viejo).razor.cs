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

namespace SupplyChain.Pages.Unidad
{
    public class UnidadPageBase : ComponentBase
    {
        [Inject] protected HttpClient Http { get; set; }
        [Inject] protected IJSRuntime JsRuntime { get; set; }
        protected SfGrid<Unidades> Grid;
        protected string state;

        public bool Enabled = true;
        public bool Disabled = false;

        protected List<Unidades> unidades = new List<Unidades>();
        protected Unidades unidadSeleccionado = new();

        protected bool popupFormVisible = false;

        protected List<Object> Toolbaritems = new List<Object>(){
        "Search",
        "Add",
        "Edit",
        "Delete"
        };
        protected bool SpinnerVisible = false;

        protected override async Task OnInitializedAsync()
        {
            SpinnerVisible = true;
            unidades = await Http.GetFromJsonAsync<List<Unidades>>("api/Unidades");
            SpinnerVisible = false;
        }

        protected async Task OnActionBeginHandler(ActionEventArgs<Unidades> args)
        {
            if (args.RequestType == Syncfusion.Blazor.Grids.Action.Add ||
                args.RequestType == Syncfusion.Blazor.Grids.Action.BeginEdit)
            {
                args.Cancel = true;
                args.PreventRender = false;
                popupFormVisible = true;
                unidadSeleccionado = new();
                unidadSeleccionado.ESNUEVO = true;
            }

            if (args.RequestType == Syncfusion.Blazor.Grids.Action.BeginEdit)
            {
                unidadSeleccionado = args.Data;
                unidadSeleccionado.ESNUEVO = false;
            }

            if (args.RequestType == Syncfusion.Blazor.Grids.Action.Grouping
                || args.RequestType == Syncfusion.Blazor.Grids.Action.UnGrouping
                || args.RequestType == Syncfusion.Blazor.Grids.Action.ClearFiltering
                || args.RequestType == Syncfusion.Blazor.Grids.Action.CollapseAllComplete
                || args.RequestType == Syncfusion.Blazor.Grids.Action.ColumnState
                || args.RequestType == Syncfusion.Blazor.Grids.Action.ClearFiltering
                || args.RequestType == Syncfusion.Blazor.Grids.Action.Reorder
                || args.RequestType == Syncfusion.Blazor.Grids.Action.Sorting
                )
            {
                Grid.PreventRender();
                Grid.Refresh();

                state = await Grid.GetPersistData();
                await Grid.AutoFitColumnsAsync();
                await Grid.RefreshColumns();
                await Grid.RefreshHeader();
            }
        }
        /*public async Task ActionBegin(ActionEventArgs<Unidades> args)
        {
            if (args.RequestType == Syncfusion.Blazor.Grids.Action.Save)
            {
                HttpResponseMessage response;
                bool found = unidades.Any(o => o.UNID == args.Data.UNID);
                Orificio ur = new Orificio();

                if (!found)
                {
                    args.Data.UNID = unidades.Max(s => s.UNID) + 1;
                    response = await Http.PostAsJsonAsync("api/Unidades", args.Data);
                }
                else
                {
                    response = await Http.PutAsJsonAsync($"api/Unidades/{args.Data.UNID}", args.Data);
                }
            }

            if (args.RequestType == Syncfusion.Blazor.Grids.Action.Delete)
            {
                await EliminarCeldas(args);
            }
        }*/
        /*private async Task EliminarCeldas(ActionEventArgs<Unidades> args)
        {
            try
            {
                if (args.Data != null)
                {
                    bool isConfirmed = await JsRuntime.InvokeAsync<bool>("confirm", "Seguro de que desea eliminar la Areas?");
                    if (isConfirmed)
                    {
                        //servicios.Remove(servicios.Find(m => m.PEDIDO == args.Data.PEDIDO));
                        await Http.DeleteAsync($"api/Unidades/{args.Data.UNID}");
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }*/

        protected async Task OnToolbarHandler(ClickEventArgs args)
        {
        }
        protected async Task OnActionCompleteHandler(ActionEventArgs<Unidades> args)
        {
            if (args.RequestType == Syncfusion.Blazor.Grids.Action.BeginEdit)
            {
                args.Cancel = true;
                args.PreventRender = false;
                popupFormVisible = true;
            }
        }

        public async Task ClickHandler(Syncfusion.Blazor.Navigations.ClickEventArgs args)
        {
            if (args.Item.Text == "Copy")
            {
                if (this.Grid.SelectedRecords.Count > 0)
                {
                    foreach (Unidades selectedRecord in this.Grid.SelectedRecords)
                    {
                        bool isConfirmed = await JsRuntime.InvokeAsync<bool>("confirm", "Seguro de que desea copiar el area?");
                        if (isConfirmed)
                        {
                            Unidades Nuevo = new Unidades();

                            Nuevo.Id = unidades.Max(s => s.Id) + 1;
                            Nuevo.DES_UNID = selectedRecord.DES_UNID;
                            
                        
                            Nuevo.TIPOUNID = selectedRecord.TIPOUNID;
                            Nuevo.CG_DENBASICA = selectedRecord.CG_DENBASICA;
                            Nuevo.CODIGO = selectedRecord.CODIGO;



                            var response = await Http.PostAsJsonAsync("api/Unidades", Nuevo);

                            if (response.StatusCode == System.Net.HttpStatusCode.Created)
                            {
                                Grid.Refresh();
                                var unidad = await response.Content.ReadFromJsonAsync<Unidades>();
                                await InvokeAsync(StateHasChanged);
                                Nuevo.Id = unidad.Id;
                                unidades.Add(Nuevo);
                                var itemsJson = JsonSerializer.Serialize(unidad);
                                Console.WriteLine(itemsJson);
                                unidades.OrderByDescending(o => o.Id);
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
