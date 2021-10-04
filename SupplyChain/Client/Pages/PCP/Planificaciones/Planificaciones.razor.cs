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
using Syncfusion.Blazor.Inputs;
using Syncfusion.Blazor.Popups;
using SupplyChain.Shared;
using SupplyChain.Shared.PCP;

namespace SupplyChain.Client.Pages.PCP.Planificaciones
{
    public class PlanificacionesBase : ComponentBase
    {
        [Inject] protected HttpClient Http { get; set; }
        [Inject] protected IJSRuntime JsRuntime { get; set; }
        protected SfGrid<Planificacion> GridPlanificacion;
        protected SfGrid<DespiecePlanificacion> Grid2;
        protected SfGrid<FormulaPlanificacion> Grid3;
        protected SfGrid<Planificacion> Grid4;
        protected SfGrid<Producto> Grid5;
        protected SfDialog DialogDespieceRef;
        protected SfDialog refDialogEntrega;
        protected bool VisibleProperty { get; set; } = false;
        protected bool armadoCheck { get; set; } = true;
        protected bool emitidasCheck { get; set; } = true;

        public bool Enabled = true;
        public bool Disabled = false;
        protected int emitidas = 0;
        protected int armado = 0;
        protected int CantidadMostrar = 100;
        protected int CantidadMostrar2 = 100;
        protected int PreviousEstado = 0;
        protected int ActualEstado = 0;
        protected bool IsVisible { get; set; } = false;
        protected bool IsVisible2 { get; set; } = false;
        protected bool IsVisible3 { get; set; } = false;
        protected bool IsVisible4 { get; set; } = false;
        protected bool IsVisible5 { get; set; } = false;
        protected Planificacion Datos_paraFormula;
        protected Producto ProdSeleccionada;
        protected DialogSettings DialogParams = new DialogSettings { MinHeight = "400px", Width = "500px" };
        protected int OrdenFabricacionSelected = 0;
        //protected List<CatOpe> catopes = new List<CatOpe>();
        protected List<Planificacion> listaPlanificacion = new List<Planificacion>();
        protected List<DespiecePlanificacion> listaDespiece = new List<DespiecePlanificacion>();
        protected List<FormulaPlanificacion> listaFormula = new List<FormulaPlanificacion>();
        protected List<Planificacion> listaCerradasAnuladas = new List<Planificacion>();
        protected List<Producto> Busquedalist = new List<Producto>();
        protected List<Producto> CG_PRODlist = new List<Producto>();
        protected List<Producto> DES_PRODlist = new List<Producto>();
        protected string CgString = "";
        protected string DesString = "";

        protected List<Object> Toolbaritems = new List<Object>(){
        "Search",
        new ItemModel(){ Type = ItemType.Separator},
        //new ItemModel(){ Id= "Print", Text = "Imprimir", PrefixIcon = "fa fa-print", CssClass = "bagde badge-success", Type = ItemType.Button},
        "Print",
        new ItemModel(){ Type = ItemType.Separator},
        "ExcelExport",
        new ItemModel(){ Type = ItemType.Separator},
        new ItemModel { Text = "Seleccionar Columnas", TooltipText = "Seleccionar Columnas", Id = "Seleccionar Columnas" },
        };
        public class Estado
        {
            public string Texto { get; set; }
            public int Valor { get; set; }
        }

        public List<Estado> Estados = new List<Estado> {
            new Estado() {Texto= "EMITIDA", Valor = 0},
            new Estado() {Texto= "PLANEADA", Valor = 1},
            //new Estado() {Texto= "FIRME", Valor = 2},
            //new Estado() {Texto= "EN CURSO", Valor = 3},
            //new Estado() {Texto= "CERRADA", Valor = 4},
            new Estado() {Texto= "ANULADA", Valor = 5}
        };

        protected List<EstadosCargaMaquina> EstadosCargaMaquinas = new();
        protected SfDialog refDialogCerradasAnuladas;
        protected const string APPNAME = "grdPlanificacion";
        protected string state;
        protected const string APPNAME_OF_CERRADAS_ANULADAS = "OFCerradasAnuladas";
        protected string state_of_cerradas_anuladas;
        protected Planificacion PlanificacionSeleccionadaOFCerrada;
        protected bool SpinnerVisible = false;
        protected override async Task OnInitializedAsync()
        {

            EstadosCargaMaquinas = await Http.GetFromJsonAsync<List<EstadosCargaMaquina>>("api/EstadosCargaMaquinas");
            listaPlanificacion = await Http.GetFromJsonAsync<List<Planificacion>>("api/Planificacion/0/1");
            //await GridPlanificacion?.AutoFitColumns();
            await base.OnInitializedAsync();
        }

        public async Task DataBoundHandler()
        {
            GridPlanificacion.PreventRender();
            await GridPlanificacion?.AutoFitColumns();
            VisibleProperty = false;
        }

        public async Task ClickHandler(Syncfusion.Blazor.Navigations.ClickEventArgs args)
        {
            if (args.Item.Text == "Excel Export")
            {
                await this.GridPlanificacion.ExcelExport();
            }
            if (args.Item.Text == "Print")
            {
                await this.GridPlanificacion.Print();
            }

            if (args.Item.Text == "Seleccionar Columnas")
            {
                await GridPlanificacion.OpenColumnChooser();
            }
        }

        public async Task ActionBegin(ActionEventArgs<Planificacion> args)
        {
            if (args.RequestType == Syncfusion.Blazor.Grids.Action.BeginEdit)
            {
                PreviousEstado = args.Data.CG_ESTADOCARGA;
            }
            if (args.RequestType == Syncfusion.Blazor.Grids.Action.Save)
            {
                //decimal cantidad1 = Grid.SelectedRecords.FirstOrDefault().CANT;
                ActualEstado = args.Data.CG_ESTADOCARGA;
                if (PreviousEstado == 0 && ActualEstado == 2)
                {
                    await JsRuntime.InvokeAsync<bool>("confirm", "No puede pasar una órden de fabricación EMITIDA a estado EN FIRME");
                    args.Data.CG_ESTADOCARGA = 0;
                }
                else if (PreviousEstado == 0 && ActualEstado == 3)
                {
                    await JsRuntime.InvokeAsync<bool>("confirm", "No puede pasar una órden de fabricación EMITIDA a estado EN CURSO");
                    args.Data.CG_ESTADOCARGA = 0;
                }
                else if (PreviousEstado == 1 && ActualEstado == 3)
                {
                    await JsRuntime.InvokeAsync<bool>("confirm", "No puede pasar una órden de fabricación PLANEADA a estado EN CURSO");
                    args.Data.CG_ESTADOCARGA = 0;
                }
                else if (ActualEstado == 2)
                {
                    await JsRuntime.InvokeAsync<bool>("confirm", "Para pasar una órden de fabricación a estado FIRME hay que Abastecerla");
                    args.Data.CG_ESTADOCARGA = 0;
                }
                else if (ActualEstado == 4)
                {
                    await JsRuntime.InvokeAsync<bool>("confirm", "No se pueden cerrar ordenes desde esta pantalla");
                    args.Data.CG_ESTADOCARGA = 0;
                }
                HttpResponseMessage response;
                response = await Http.PutAsJsonAsync($"api/Planificacion/PutPlanif/{PreviousEstado}", args.Data);
                if (armadoCheck == true)
                {
                    if (emitidasCheck == true)
                    {
                        listaPlanificacion = await Http.GetFromJsonAsync<List<Planificacion>>("api/Planificacion/0/1");
                    }
                    else
                    {
                        listaPlanificacion = await Http.GetFromJsonAsync<List<Planificacion>>("api/Planificacion/0/0");
                    }
                }
                else
                {
                    if (emitidasCheck == true)
                    {
                        listaPlanificacion = await Http.GetFromJsonAsync<List<Planificacion>>("api/Planificacion/1/1");
                    }
                    else
                    {
                        listaPlanificacion = await Http.GetFromJsonAsync<List<Planificacion>>("api/Planificacion/1/0");
                    }
                }
                await GridPlanificacion.RefreshColumns();
                GridPlanificacion.Refresh();
                await GridPlanificacion.RefreshHeader();

            }
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
                GridPlanificacion.PreventRender();
                GridPlanificacion.Refresh();
                
                state = await GridPlanificacion.GetPersistData();
                await GridPlanificacion.AutoFitColumnsAsync();
                await GridPlanificacion.RefreshColumns();
                await GridPlanificacion.RefreshHeader();
                //VisibleProperty = false;
            }
        }

        protected async Task OnInputCG_PROD(InputEventArgs args)
        {
            if (args.Value != "")
            {
                CG_PRODlist = await Http.GetFromJsonAsync<List<Producto>>($"api/Prevision/BuscarPorCG_PROD/{args.Value}");
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
                CG_PRODlist = await Http.GetFromJsonAsync<List<Producto>>($"api/Prevision/BuscarPorDES_PROD/{args.Value}");
                if (CG_PRODlist.Count > 0)
                {
                    CgString = CG_PRODlist.FirstOrDefault().CG_PROD;
                }
                else
                {
                    CgString = "";
                }
            }
        }
        protected async Task BuscarProductoPrevision()
        {
            CantidadMostrar2 = 100;
            if (DesString == "")
            {
                Busquedalist = await Http.GetFromJsonAsync<List<Producto>>($"api/Prevision/BuscarProductoPrevision/{CgString}/Vacio/{CantidadMostrar2}");
            }
            else if (CgString == "")
            {
                Busquedalist = await Http.GetFromJsonAsync<List<Producto>>($"api/Prevision/BuscarProductoPrevision/Vacio/{DesString}/{CantidadMostrar2}");
            }
            else
            {
                Busquedalist = await Http.GetFromJsonAsync<List<Producto>>($"api/Prevision/BuscarProductoPrevision/{CgString}/{DesString}/{CantidadMostrar2}");
            }
            IsVisible5 = true;
        }

        protected async Task Emitir()
        {
            IsVisible4 = true;
        }
        public void Validation()
        {
            CantidadMostrar = 100;
        }
        public async Task CommandClickHandler(CommandClickEventArgs<Planificacion> args)
        {

            ProdSeleccionada = await Http.GetFromJsonAsync<Producto>($"api/Prod/GetByFilter?Codigo={args.RowData.CG_PROD}&Descripcion={string.Empty}");
            if (args.CommandColumn.Title == "Entrega")
            {
                OrdenFabricacionSelected = args.RowData.CG_ORDF;
                await JsRuntime.InvokeAsync<object>("open", $"inventario/10/true/{OrdenFabricacionSelected}", "_blank");

                await refDialogEntrega.Show(true);
            }
            if (args.CommandColumn.Title == "Despiece")
            {
                listaDespiece = await Http.GetFromJsonAsync<List<DespiecePlanificacion>>($"api/Planificacion/Despiece/{args.RowData.CG_PROD.Trim()}/{args.RowData.CG_FORM}/{args.RowData.CANT}");
                //IsVisible = true;
                await DialogDespieceRef.Show(true);
            }
            if (args.CommandColumn.Title == "Activo")
            {
                Datos_paraFormula = args.RowData;
                listaFormula = await Http.GetFromJsonAsync<List<FormulaPlanificacion>>($"api/Planificacion/Formula/{args.RowData.CG_PROD.Trim()}");
                IsVisible2 = true;
            }
        }
        public async Task OnSelected()
        {
            HttpResponseMessage response;
            response = await Http.PutAsJsonAsync($"api/Planificacion/PutFormula/{this.Grid3.GetSelectedRecords().Result.FirstOrDefault().CG_FORM}", Datos_paraFormula);
            IsVisible2 = false;
        }
        public void OnSelected2()
        {
            CgString = this.Grid2.GetSelectedRecords().Result.FirstOrDefault().CG_PROD; // return the details of selected record
            DesString = this.Grid2.GetSelectedRecords().Result.FirstOrDefault().DES_PROD; // return the details of selected record
            CantidadMostrar2 = 0;
            IsVisible5 = false;
        }
        public async Task OrdCerradas()
        {
            VisibleProperty = true;
            listaCerradasAnuladas = await Http.GetFromJsonAsync<List<Planificacion>>($"api/Planificacion/OrdenesCerradasYAnuladas/{CantidadMostrar}");
            IsVisible3 = true;
            VisibleProperty = false;
            await refDialogCerradasAnuladas.Show(true);
        }

        public async Task DataBoundHandlerOFCerradas()
        {
            Grid4.PreventRender();
            await Grid4?.AutoFitColumns();
            VisibleProperty = false;
        }

        public async Task ActionBeginOFCerradas(ActionEventArgs<Planificacion> args)
        {
            if (args.RequestType == Syncfusion.Blazor.Grids.Action.Grouping ||
                args.RequestType == Syncfusion.Blazor.Grids.Action.UnGrouping
                || args.RequestType == Syncfusion.Blazor.Grids.Action.ClearFiltering
                || args.RequestType == Syncfusion.Blazor.Grids.Action.CollapseAllComplete
                || args.RequestType == Syncfusion.Blazor.Grids.Action.ColumnState
                || args.RequestType == Syncfusion.Blazor.Grids.Action.ClearFiltering
                || args.RequestType == Syncfusion.Blazor.Grids.Action.Reorder)
            {
                state_of_cerradas_anuladas = await Grid4.GetPersistData();
            }
        }

        public async Task ClickHandlerOFCerradas(Syncfusion.Blazor.Navigations.ClickEventArgs args)
        {
            if (args.Item.Text == "Excel Export")
            {
                await Grid4.ExcelExport();
            }
            if (args.Item.Text == "Print")
            {
                await Grid4.Print();
            }

            if (args.Item.Text == "Seleccionar Columnas")
            {
                await Grid4.OpenColumnChooser();
            }
        }

        public async Task CheckCambio()
        {
            if (armadoCheck == true)
            {
                if (emitidasCheck == true)
                {
                    listaPlanificacion = await Http.GetFromJsonAsync<List<Planificacion>>("api/Planificacion/0/1");
                }
                else
                {
                    listaPlanificacion = await Http.GetFromJsonAsync<List<Planificacion>>("api/Planificacion/0/0");
                }
            }
            else
            {
                if (emitidasCheck == true)
                {
                    listaPlanificacion = await Http.GetFromJsonAsync<List<Planificacion>>("api/Planificacion/1/1");
                }
                else
                {
                    listaPlanificacion = await Http.GetFromJsonAsync<List<Planificacion>>("api/Planificacion/1/0");
                }
            }
            await GridPlanificacion.RefreshColumns();
            GridPlanificacion.Refresh();
            await GridPlanificacion.RefreshHeader();
        }
        protected async Task AgregarValores()
        {
            CantidadMostrar = CantidadMostrar + 100;
            listaCerradasAnuladas = await Http.GetFromJsonAsync<List<Planificacion>>($"api/Planificacion/OrdenesCerradasYAnuladas/{CantidadMostrar}");
        }
        protected async Task AgregarValores2()
        {
            CantidadMostrar2 = CantidadMostrar2 + 100;
            if (DesString == "")
            {
                Busquedalist = await Http.GetFromJsonAsync<List<Producto>>($"api/Prevision/BuscarProductoPrevision/{CgString}/Vacio/{CantidadMostrar2}");
            }
            else if (CgString == "")
            {
                Busquedalist = await Http.GetFromJsonAsync<List<Producto>>($"api/Prevision/BuscarProductoPrevision/Vacio/{DesString}/{CantidadMostrar2}");
            }
            else
            {
                Busquedalist = await Http.GetFromJsonAsync<List<Producto>>($"api/Prevision/BuscarProductoPrevision/{CgString}/{DesString}/{CantidadMostrar2}");
            }
        }
        public async Task QueryCellInfoHandler(QueryCellInfoEventArgs<Planificacion> args)
        {
            if (args.Column.Field == "CG_ESTADOCARGA")
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
            }
        }

        public async Task OnVistaSeleccionada(VistasGrillas vistasGrillas)
        {
            await GridPlanificacion.SetPersistData(vistasGrillas.Layout);
        }
        public async Task OnReiniciarGrilla()
        {
            await GridPlanificacion.ResetPersistData();
        }

        public async Task OnVistaSeleccionada_grdOFCerradasAnuladas(VistasGrillas vistasGrillas)
        {
            await Grid4.SetPersistData(vistasGrillas.Layout);
        }
        public async Task OnReiniciarGrilla_grdOFCerradasAnuladas()
        {
            await Grid4.ResetPersistData();
        }

        protected async Task OnChangeEstadoCarga(Syncfusion.Blazor.DropDowns.ChangeEventArgs<int,EstadosCargaMaquina> args)
        {
            int ValorAnterior = 4;
            if (args.ItemData.CG_ESTADO < 4)
            {
                var response = await Http.PutAsJsonAsync($"api/Planificacion/PutPlanif/{ValorAnterior}", PlanificacionSeleccionadaOFCerrada);
                if (response.IsSuccessStatusCode)
                {
                    listaCerradasAnuladas = listaCerradasAnuladas.Where(p => p.CG_ESTADOCARGA > 3)
                    .ToList();
                    //listaCerradasAnuladas.Remove(PlanificacionSeleccionadaOFCerrada);
                    //await Grid4.RefreshHeader();
                    //await Grid4.RefreshColumns();
                    //Grid4.Refresh();

                    //listaPlanificacion = await Http.GetFromJsonAsync<List<Planificacion>>("api/Planificacion/0/1");
                    //await GridPlanificacion.RefreshHeader();
                    //await GridPlanificacion.RefreshColumns();
                    //GridPlanificacion.Refresh();
                }
                
            }

            

        }
    }
}
