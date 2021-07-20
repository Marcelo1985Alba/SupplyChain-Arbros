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
using SupplyChain.Shared.PCP;
using SupplyChain.Client.Shared;
using SupplyChain.Shared;

namespace SupplyChain.Client.Pages.PCP.Pendientes_Fabricacion
{
    public class PendientesFabricacionBase : ComponentBase
    {
        [Inject] protected HttpClient Http { get; set; }
        [Inject] protected IJSRuntime JsRuntime { get; set; }
        protected SfGrid<vPendienteFabricar> Grid;
        protected bool VisibleProperty { get; set; } = false;

        public bool Enabled = true;
        public bool Disabled = false;

        protected DialogSettings DialogParams = new DialogSettings { MinHeight = "400px", Width = "500px" };

        //protected List<CatOpe> catopes = new List<CatOpe>();
        protected List<vPendienteFabricar> listaPendFab = new List<vPendienteFabricar>();

        protected List<Object> Toolbaritems = new List<Object>(){
        "Search",
        new ItemModel(){ Type = ItemType.Separator},
        "Print",
        new ItemModel(){ Type = ItemType.Separator},
        "ExcelExport"
        };

        protected NotificacionToast NotificacionObj;
        protected bool ToastVisible { get; set; } = false;
        protected const string APPNAME = "grdPendienteFabricar";
        protected string state;
        protected override async Task OnInitializedAsync()
        {
            VisibleProperty = true;
            listaPendFab = await Http.GetFromJsonAsync<List<vPendienteFabricar>>("api/PendientesFabricar");
            
            await base.OnInitializedAsync();
        }

        public async Task DataBoundHandler()
        {
            
            await Grid.AutoFitColumns();
            VisibleProperty = false;
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
        }

        public async Task ActionBegin(ActionEventArgs<vPendienteFabricar> args)
        {
            if (args.RequestType == Syncfusion.Blazor.Grids.Action.Save)
            {
                if (args.Data.CG_FORM != 1)
                {
                    bool isConfirmed = await JsRuntime.InvokeAsync<bool>("confirm", 
                        "El producto no tiene Fórmula o no tiene Fórmula Activa");

                    if (isConfirmed)
                    {
                    }
                }
                else
                {
                    if (args.Data.CANTEMITIR != 0)
                    {
                        HttpResponseMessage response;

                        //Convertir a modelo 
                        var modelo = new ModeloPendientesFabricar()
                        {
                            CALCULADO = args.Data.CALCULADO,
                            CANTEMITIR = args.Data.CANTEMITIR,
                            CANTPED = args.Data.CANTPED,
                            CG_ART = args.Data.CG_ART,
                            CG_FORM = args.Data.CG_FORM,
                            DES_ART = args.Data.DES_ART,
                            EXIGEOA = args.Data.EXIGEOA == "Armado",
                            LOPTIMO = args.Data.LOPTIMO,
                            PEDIDO = args.Data.PEDIDO,
                            PREVISION = args.Data.PREVISION,
                            REGISTRO = args.Data.REGISTRO,
                            STOCK =args.Data.STOCK,
                            STOCKENT = args.Data.STOCKENT,
                            STOCKMIN = args.Data.STOCKMIN
                        };

                        response = await Http
                            .PutAsJsonAsync($"api/PendientesFabricar/PutPenFab/{Convert.ToInt32(args.Data.REGISTRO)}", modelo);

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
                            listaPendFab = await Http.GetFromJsonAsync<List<vPendienteFabricar>>("api/PendientesFabricar");
                            Grid.Refresh();
                            await NotificacionObj.ShowAsync();
                        }

                        
                    }
                }
            }
            if (args.RequestType == Syncfusion.Blazor.Grids.Action.Grouping ||
                args.RequestType == Syncfusion.Blazor.Grids.Action.UnGrouping
                || args.RequestType == Syncfusion.Blazor.Grids.Action.ClearFiltering
                || args.RequestType == Syncfusion.Blazor.Grids.Action.CollapseAllComplete
                || args.RequestType == Syncfusion.Blazor.Grids.Action.ColumnState
                || args.RequestType == Syncfusion.Blazor.Grids.Action.ClearFiltering
                || args.RequestType == Syncfusion.Blazor.Grids.Action.Reorder
                || args.RequestType == Syncfusion.Blazor.Grids.Action.CollapseAllComplete)
            {
                VisibleProperty = false;
                state = await Grid.GetPersistData();
            }
        }
        protected async Task EmitirOrden()
        {
            int xCuantas = listaPendFab.Where(s => s.CANTEMITIR > 0).Count();
            if (xCuantas == 0)
            {
                bool isConfirmed = await JsRuntime.InvokeAsync<bool>("confirm", "No hay productos con 'Cantidad a emitir' en las 'Necesidades de stock' para emitir órdenes de fabricación");
                if (isConfirmed)
                {
                }
            }
            else
            {
                bool isConfirmed = await JsRuntime.InvokeAsync<bool>("confirm", "Va a emitir órdenes de fabricación según necesidades de stock \n\n¿Desea continuar?");
                if (isConfirmed)
                {
                    listaPendFab = await Http.GetFromJsonAsync<List<vPendienteFabricar>>("api/PendientesFabricar/EmitirOrdenes");
                    await NotificacionObj.ShowAsync();
                }
            }
        }
        public async Task QueryCellInfoHandler(QueryCellInfoEventArgs<vPendienteFabricar> args)
        {
            if (args.Data.CG_FORM == 0)
            {
                args.Cell.AddClass(new string[] { "rojas" });
            }
            if (args.Column.Field == "CANTEMITIR")
            {
                args.Cell.AddClass(new string[] { "gris" });
            }
        }

        public async Task OnVistaSeleccionada(VistasGrillas vistasGrillas)
        {
            await Grid.SetPersistData(vistasGrillas.Layout);
        }
        public async Task OnReiniciarGrilla()
        {
            await Grid.ResetPersistData();
        }
    }
}
