﻿using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SupplyChain.Shared;
using SupplyChain.Shared.Models;
using SupplyChain.Shared.PCP;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Navigations;
using Syncfusion.Blazor.Notifications;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SupplyChain.Pages.Fab
{
    public class FabricPageBase : ComponentBase
    {
        [Inject] protected HttpClient Http { get; set; }
        [Inject] protected IJSRuntime JsRuntime { get; set; }
        protected SfGrid<Fabricacion> Grid;
        protected bool VisibleProperty { get; set; } = false;

        public bool Enabled = true;
        public bool Disabled = false;

        protected DialogSettings DialogParams = new DialogSettings { MinHeight = "400px", Width = "500px" };

        //protected List<CatOpe> catopes = new List<CatOpe>();
        protected List<Fabricacion> listaFab = new List<Fabricacion>();
        protected List<Celdas> listaCelda = new List<Celdas>();
        protected List<EstadosCargaMaquina> listaEstadosCargaMaquina = new List<EstadosCargaMaquina>();
        protected List<Object> Toolbaritems = new List<Object>(){
            "Search",
            new ItemModel(){ Type = ItemType.Separator},
            "Print",
            new ItemModel(){ Type = ItemType.Separator},
            "ExcelExport"
        };

        protected SfToast ToasObj;

        protected const string APPNAME = "grdFabricaciones";
        protected string state;
        protected override async Task OnInitializedAsync()
        {
            
            listaFab = await Http.GetFromJsonAsync<List<Fabricacion>>("api/Fabricacion");
            listaCelda = await Http.GetFromJsonAsync<List<Celdas>>("api/Celdas");
            listaEstadosCargaMaquina = await Http.GetFromJsonAsync<List<EstadosCargaMaquina>>("api/EstadosCargaMaquinas");
            await base.OnInitializedAsync();
        }
        
        public async Task DataBoundHandler()
        {
            await Grid.AutoFitColumns();
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

        public async Task Begin(ActionEventArgs<Fabricacion> args)
        {
            if (args.RequestType == Syncfusion.Blazor.Grids.Action.Grouping ||
                args.RequestType == Syncfusion.Blazor.Grids.Action.UnGrouping
                || args.RequestType == Syncfusion.Blazor.Grids.Action.ClearFiltering
                || args.RequestType == Syncfusion.Blazor.Grids.Action.CollapseAllComplete
                || args.RequestType == Syncfusion.Blazor.Grids.Action.ColumnState
                || args.RequestType == Syncfusion.Blazor.Grids.Action.ClearFiltering
                || args.RequestType == Syncfusion.Blazor.Grids.Action.Reorder ||
                args.RequestType == Syncfusion.Blazor.Grids.Action.Sorting
                )
            {
                //VisibleProperty = true;
                Grid.PreventRender();
                Grid.Refresh();

                state = await Grid.GetPersistData();
                await Grid.AutoFitColumnsAsync();
                await Grid.RefreshColumns();
                await Grid.RefreshHeader();
                //VisibleProperty = false;
            }

            if (args.RequestType == Syncfusion.Blazor.Grids.Action.RowDragAndDrop)
            {

            }
        }

        public async Task ActionComplete(ActionEventArgs<Fabricacion> args)
        {
            if (args.RequestType == Syncfusion.Blazor.Grids.Action.Save)
            {

                var respuesta = await Http.PutAsJsonAsync($"api/OrdenesFabricacion/PutFromModeloOF/{args.Data.CG_ORDF}", args.Data);
                if (respuesta.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    await ToasObj.Show(new ToastModel
                    {
                        Title = "ERROR!",
                        Content = $"Ocurrrio un error.Error al intentar Guardar OF: {args.Data.CG_ORDF} ",
                        CssClass = "e-toast-danger",
                        Icon = "e-error toast-icons",
                        ShowCloseButton = true,
                        ShowProgressBar = true
                    });
                }
                else
                {
                    await this.ToasObj.Show(new ToastModel
                    {
                        Title = "Exito!",
                        Content = $"Guardado Correctamente! Nro OF: {args.Data.CG_ORDF}",
                        CssClass = "e-toast-success",
                        Icon = "e-success toast-icons",
                        ShowCloseButton = true,
                        ShowProgressBar = true
                    });


                    await Grid.RefreshColumns();
                    Grid.Refresh();
                    await Grid.RefreshHeader();
                }
            }
            if (args.RequestType == Syncfusion.Blazor.Grids.Action.RowDragAndDrop)
            {

            }
        }
    

        public void QueryCellInfoHandler(QueryCellInfoEventArgs<Fabricacion> args)
        {
            /*if (args.Column.Field == "CG_ESTADOCARGA")
            {
                args.Cell.AddClass(new string[] { "gris" });
            }
            if (args.Column.Field == "FE_ENTREGA")
            {
                args.Cell.AddClass(new string[] { "gris" });
            }
            if (args.Column.Field == "FE_EMIT")
            {
                args.Cell.AddClass(new string[] { "gris" });
            }
            if (args.Column.Field == "FE_PLAN")
            {
                args.Cell.AddClass(new string[] { "gris" });
            }
            if (args.Column.Field == "FE_FIRME")
            {
                args.Cell.AddClass(new string[] { "gris" });
            }
            if (args.Column.Field == "FE_CURSO")
            {
                args.Cell.AddClass(new string[] { "gris" });
            }
            if (args.Column.Field == "CANT")
            {
                args.Cell.AddClass(new string[] { "gris" });
            }*/
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
