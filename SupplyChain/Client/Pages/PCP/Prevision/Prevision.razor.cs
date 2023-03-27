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
using SupplyChain.Shared.Models;
using Microsoft.Extensions.Configuration;
using System.IO;
using Syncfusion.Blazor.Inputs;
using Newtonsoft.Json;
using Syncfusion.Blazor.Notifications;
using SupplyChain.Client.Shared;
using Syncfusion.Blazor.Popups;

namespace SupplyChain.Client.Pages.Prev
{
    public class PrevisionesPageBase : ComponentBase
    {
        [Inject] protected HttpClient Http { get; set; }
        [Inject] protected IJSRuntime JsRuntime { get; set; }

        protected SfGrid<PresAnual> Grid;
        protected SfGrid<Producto> Grid2;
        protected SfGrid<DespiecePlanificacion> GridDespiece;
        protected SfDialog DialogDespieceRef;
        protected bool VisibleProperty { get; set; } = false;

        public bool Enabled = true;
        public bool Disabled = false;
        public bool Showgrid = true;
        protected List<DespiecePlanificacion> listaDespiece = new List<DespiecePlanificacion>();
        protected List<PresAnual> previsiones = new();
        protected List<PresAnual> prueba = new();
        protected List<Producto> CG_PRODlist = new();
        protected List<Producto> DES_PRODlist = new();
        protected List<Producto> Busquedalist = new();
        protected List<Producto> Agregarlist = new();
        protected string CgString = "";
        protected string DesString = "";
        protected int CantidadMostrar = 100;
        protected bool IsVisible { get; set; } = false;

        protected DialogSettings DialogParams = new() { MinHeight = "400px", Width = "500px" };

        protected List<Object> Toolbaritems = new(){
        "Search",
        new ItemModel(){ Type = ItemType.Separator},
        "Delete",
        new ItemModel(){ Type = ItemType.Separator},
        "Print",
        new ItemModel(){ Type = ItemType.Separator},
        "ExcelExport"
        };

        protected NotificacionToast NotificacionObj;
        protected bool ToastVisible { get; set; } = false;
        protected override async Task OnInitializedAsync()
        {
            previsiones = await Http.GetFromJsonAsync<List<PresAnual>>("api/Prevision");
        }

        public async Task CommandClickHandler(CommandClickEventArgs<PresAnual> args)
        {

            if (args.CommandColumn.Title == "Despiece")
            {
                listaDespiece = await Http.GetFromJsonAsync<List<DespiecePlanificacion>>($"api/Planificacion/Despiece/" +
                    $"{args.RowData.CG_ART.Trim()}/1/{args.RowData.CANTPED}");
                //IsVisible = true;
                await DialogDespieceRef.Show();
            }
        }

        public async Task ClickHandler(Syncfusion.Blazor.Navigations.ClickEventArgs args)
        {
            if (args.Item.Text == "Excel Export")
            {
                await this.Grid.ExcelExport();
            }
            if (args.Item.Text == "Print")
            {
                await this.Grid.Print();
            }


            if (args.Item.Text == "Eliminar" || args.Item.Text == "Delete")
            {
                bool isConfirmed = await JsRuntime.InvokeAsync<bool>("confirm", "Seguro de que desea eliminar el producto?");
                if (isConfirmed)
                {
                    await Http.GetFromJsonAsync<List<PresAnual>>($"api/Prevision/BorrarPrevision/{this.Grid.GetSelectedRecords().Result.FirstOrDefault().Id}");
                    previsiones = await Http.GetFromJsonAsync<List<PresAnual>>("api/Prevision");
                    Grid.Refresh();
                }
            }
        }

        public async Task ActionBegin(ActionEventArgs<PresAnual> args)
        {
            if (args.RequestType == Syncfusion.Blazor.Grids.Action.Save)
            {
                HttpResponseMessage response;
                response = await Http.PutAsJsonAsync($"api/Prevision/PutPrev/{args.Data.Id}", args.Data);
                previsiones = await Http.GetFromJsonAsync<List<PresAnual>>("api/Prevision");
                Grid.Refresh();
            }
            if (args.RequestType == Syncfusion.Blazor.Grids.Action.ColumnState)
            {
                await Grid.AutoFitColumnsAsync();
                Grid.Refresh();
            }
        }

        public void OnSelected()
        {
            CgString = this.Grid2.GetSelectedRecords().Result.FirstOrDefault().Id; // return the details of selected record
            DesString = this.Grid2.GetSelectedRecords().Result.FirstOrDefault().DES_PROD; // return the details of selected record
            CantidadMostrar = 0;
            IsVisible = false;
        }

        protected async Task OnInputCG_PROD(InputEventArgs args)
        {
            if (args.Value != "")
            {
                CG_PRODlist = await Http.GetFromJsonAsync<List<Producto>>($"api/Prod/BuscarPorCG_PROD/{args.Value}");
                if (CG_PRODlist.Count > 0)
                {
                    DesString = CG_PRODlist.FirstOrDefault().DES_PROD;
                }
                else
                {
                    DesString = "";
                }
            }
        }

        protected async Task OnInputDES_PROD(InputEventArgs args)
        {
            if (args.Value != "")
            {
                CG_PRODlist = await Http.GetFromJsonAsync<List<Producto>>($"api/Prod/BuscarPorDES_PROD/{args.Value}");
                if (CG_PRODlist.Count > 0)
                {
                    CgString = CG_PRODlist.FirstOrDefault().Id;
                }
                else
                {
                    CgString = "";
                }
            }
        }

        protected async Task BuscarProductoPrevision()
        {
            if (string.IsNullOrEmpty(DesString) && string.IsNullOrEmpty(CgString))
            {
                Busquedalist = await Http.GetFromJsonAsync<List<Producto>>($"api/Prevision/BuscarProductoPrevision/Vacio" +
                        $"/Vacio/{CantidadMostrar}");
            }


            if (!string.IsNullOrEmpty(DesString) || !string.IsNullOrEmpty(CgString))
            {
                CantidadMostrar = 100;
                if (string.IsNullOrEmpty(DesString))
                {
                    Busquedalist = await Http.GetFromJsonAsync<List<Producto>>($"api/Prevision/BuscarProductoPrevision/{CgString}" +
                        $"/Vacio/{CantidadMostrar}");
                }
                else if (string.IsNullOrEmpty(CgString))
                {
                    Busquedalist = await Http.GetFromJsonAsync<List<Producto>>($"api/Prevision/BuscarProductoPrevision/Vacio/{DesString}/{CantidadMostrar}");
                }
                else
                {
                    //CgString ??= string.Empty;
                    //DesString ??= string.Empty;
                    Busquedalist = await Http.GetFromJsonAsync<List<Producto>>($"api/Prevision/BuscarProductoPrevision/{CgString}/{DesString}/{CantidadMostrar}");
                }
                IsVisible = true;
            }

            
        }
        protected async Task AgregarProductoPrevision()
        {
            //previsiones = await Http.GetFromJsonAsync<List<PresAnual>>($"api/Prevision/AgregarProductoPrevision/{CgString}");
            var producto = await Http.GetFromJsonAsync<Producto>($"api/Prod/{CgString}");
            var response = await Http.PostAsJsonAsync($"api/Prevision/AgregarProductoPrevision", producto);
            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest
                || response.StatusCode == System.Net.HttpStatusCode.NotFound
                || response.StatusCode == System.Net.HttpStatusCode.Conflict)
            {
                var mensServidor = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"Error: {mensServidor}");
                await NotificacionObj.ShowAsyncError();
            }
            else
            {
                CgString = "";
                DesString = "";
                //previsiones = await Http.GetFromJsonAsync<List<PresAnual>>("api/Prevision");
                previsiones = await response.Content.ReadFromJsonAsync<List<PresAnual>>();
                Grid.Refresh();

                await NotificacionObj.ShowAsync();
            }

            
        }
        protected async Task AgregarValores()
        {
            CantidadMostrar = CantidadMostrar + 100;
            if (DesString == "")
            {
                Busquedalist = await Http.GetFromJsonAsync<List<Producto>>($"api/Prevision/BuscarProductoPrevision/{CgString}/Vacio/{CantidadMostrar}");
            }
            else if (CgString == "")
            {
                Busquedalist = await Http.GetFromJsonAsync<List<Producto>>($"api/Prevision/BuscarProductoPrevision/Vacio/{DesString}/{CantidadMostrar}");
            }
            else
            {
                Busquedalist = await Http.GetFromJsonAsync<List<Producto>>($"api/Prevision/BuscarProductoPrevision/{CgString}/{DesString}/{CantidadMostrar}");
            }
        }
        protected async Task GuardarDatos(CellSaveArgs<PresAnual> args)
        {
            if (args.ColumnName == "CANTPED")
            {
                previsiones = await Http.GetFromJsonAsync<List<PresAnual>>($"api/Prevision/UpdateCant/{args.RowData.Id}/{args.Value}");
                await Grid.UpdateCell(args.RowData.Id, "CANTPED", args.Value);
            }
            else if (args.ColumnName == "FE_PED")
            {
                string Dia = ((DateTime)args.Value).Day.ToString();
                string Mes = ((DateTime)args.Value).Month.ToString();
                string Anio = ((DateTime)args.Value).Year.ToString();
                previsiones = await Http.GetFromJsonAsync<List<PresAnual>>($"api/Prevision/UpdateFecha/{args.RowData.Id}/{Dia}/{Mes}/{Anio}");
                //await Grid.UpdateCell(args.RowData.REGISTRO, "FE_PED", args.Value);
                Grid.Refresh();
                Showgrid = false;
            }
            //Grid.Refresh();
            //StateHasChanged();
            Showgrid = true;
            Grid.Refresh();
            StateHasChanged();
        }
        public async Task CellSaveHandlerAsync(CellSaveArgs<PresAnual> args)
        {
            if (args.ColumnName == "CANTPED")
            {
                previsiones = await Http.GetFromJsonAsync<List<PresAnual>>($"api/Prevision/UpdateCant/{args.RowData.Id}/{args.Value}");
            }
            else if (args.ColumnName == "FE_PED")
            {
                string Dia = ((DateTime)args.Value).Day.ToString();
                string Mes = ((DateTime)args.Value).Month.ToString();
                string Anio = ((DateTime)args.Value).Year.ToString();
                prueba = await Http.GetFromJsonAsync<List<PresAnual>>($"api/Prevision/UpdateFecha/{args.RowData.Id}/{Dia}/{Mes}/{Anio}");
            }
            await Grid.EndEdit();
        }
        public void QueryCellInfoHandler(QueryCellInfoEventArgs<PresAnual> args)
        {
            if (args.Column.Field == "CANTPED")
            {
                args.Cell.AddClass(new string[] { "gris" });
            }
        }
    }
}
