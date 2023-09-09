using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SupplyChain.Shared;
using SupplyChain.Shared.Models;
using SupplyChain.Shared.PCP;
using Syncfusion.Blazor.DropDowns;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Navigations;
using Syncfusion.Blazor.Popups;
using Action = Syncfusion.Blazor.Grids.Action;

namespace SupplyChain.Client.Pages.PCP.Planificaciones;

public class PlanificacionesBase : ComponentBase
{
    protected const string APPNAME = "grdPlanificacion";
    protected const string APPNAME_OF_CERRADAS_ANULADAS = "OFCerradasAnuladas";
    protected int ActualEstado;
    protected int armado = 0;
    protected List<Producto> Busquedalist = new();

    protected decimal cantEmitir = 0;
    protected int CantidadMostrar = 100;
    protected int CantidadMostrar2 = 100;
    protected int cg_form = 0;
    protected List<Producto> CG_PRODlist = new();
    protected string CgString = "";
    protected Planificacion Datos_paraFormula;
    protected List<Producto> DES_PRODlist = new();
    protected string DesString = "";
    protected SfDialog DialogDespieceRef;
    protected DialogSettings DialogParams = new() { MinHeight = "400px", Width = "500px" };
    public bool Disabled = false;
    protected int emitidas = 0;

    public bool Enabled = true;

    public List<Estado> Estados = new()
    {
        new() { Texto = "EMITIDA", Valor = 0 },
        new() { Texto = "PLANEADA", Valor = 1 },
        //new Estado() {Texto= "FIRME", Valor = 2},
        //new Estado() {Texto= "EN CURSO", Valor = 3},
        //new Estado() {Texto= "CERRADA", Valor = 4},
        new() { Texto = "ANULADA", Valor = 5 }
    };

    protected List<EstadosCargaMaquina> EstadosCargaMaquinas = new();
    protected DateTime fe_Entrega = DateTime.Today;
    protected SfGrid<DespiecePlanificacion> Grid2;
    protected SfGrid<FormulaPlanificacion> Grid3;
    protected SfGrid<Planificacion> Grid4;
    protected SfGrid<Producto> Grid5;
    protected SfGrid<Planificacion> GridPlanificacion;
    protected List<Planificacion> listaCerradasAnuladas = new();
    protected List<DespiecePlanificacion> listaDespiece = new();

    protected List<FormulaPlanificacion> listaFormula = new();

    //protected List<CatOpe> catopes = new List<CatOpe>();
    protected List<Planificacion> listaPlanificacion = new();
    protected int OrdenFabricacionSelected;
    protected Planificacion PlanificacionSeleccionadaOFCerrada;
    protected int PreviousEstado;
    protected Producto ProdSeleccionada;
    protected SfDialog refDialogCerradasAnuladas;
    protected SfDialog refDialogEntrega;
    protected bool spinnerEmitirOrden;
    protected bool SpinnerVisible = false;
    protected string state;
    protected string state_of_cerradas_anuladas;
    protected string Tipo = "";

    protected List<object> Toolbaritems = new()
    {
        "Search",
        new ItemModel { Type = ItemType.Separator },
        //new ItemModel(){ Id= "Print", Text = "Imprimir", PrefixIcon = "fa fa-print", CssClass = "bagde badge-success", Type = ItemType.Button},
        "Print",
        new ItemModel { Type = ItemType.Separator },
        "ExcelExport",
        new ItemModel { Type = ItemType.Separator },
        new ItemModel
            { Text = "Seleccionar Columnas", TooltipText = "Seleccionar Columnas", Id = "Seleccionar Columnas" }
    };

    [Inject] protected HttpClient Http { get; set; }
    [Inject] protected IJSRuntime JsRuntime { get; set; }
    protected bool VisibleProperty { get; set; }
    protected bool armadoCheck { get; set; } = true;
    protected bool emitidasCheck { get; set; } = true;
    protected bool IsVisible { get; set; } = false;
    protected bool IsVisible2 { get; set; }
    protected bool IsVisible3 { get; set; } = false;
    protected bool IsVisible4 { get; set; }
    protected bool IsVisible5 { get; set; }
    protected bool IsVisible6 { get; set; }

    protected override async Task OnInitializedAsync()
    {
        EstadosCargaMaquinas = await Http.GetFromJsonAsync<List<EstadosCargaMaquina>>("api/EstadosCargaMaquinas");
        listaPlanificacion = await Http.GetFromJsonAsync<List<Planificacion>>("api/Planificacion/0/1");
        //await GridPlanificacion?.AutoFitColumns();
        await base.OnInitializedAsync();
        VisibleProperty = false;
    }

    public async Task DataBoundHandler()
    {
        //GridPlanificacion.PreventRender();
        //await GridPlanificacion?.AutoFitColumns();
        //VisibleProperty = false;
    }

    public async Task ClickHandler(ClickEventArgs args)
    {
        if (args.Item.Text == "Excel Export") await GridPlanificacion.ExcelExport();
        if (args.Item.Text == "Print") await GridPlanificacion.Print();

        if (args.Item.Text == "Seleccionar Columnas") await GridPlanificacion.OpenColumnChooser();
    }

    public async Task ActionBegin(ActionEventArgs<Planificacion> args)
    {
        if (args.RequestType == Action.BeginEdit) PreviousEstado = args.Data.CG_ESTADOCARGA;
        if (args.RequestType == Action.Save)
        {
            //decimal cantidad1 = Grid.SelectedRecords.FirstOrDefault().CANT;
            ActualEstado = args.Data.CG_ESTADOCARGA;
            if (PreviousEstado == 0 && ActualEstado == 2)
            {
                await JsRuntime.InvokeAsync<bool>("confirm",
                    "No puede pasar una órden de fabricación EMITIDA a estado EN FIRME");
                args.Data.CG_ESTADOCARGA = 0;
            }
            else if (PreviousEstado == 0 && ActualEstado == 3)
            {
                await JsRuntime.InvokeAsync<bool>("confirm",
                    "No puede pasar una órden de fabricación EMITIDA a estado EN CURSO");
                args.Data.CG_ESTADOCARGA = 0;
            }
            else if (PreviousEstado == 1 && ActualEstado == 3)
            {
                await JsRuntime.InvokeAsync<bool>("confirm",
                    "No puede pasar una órden de fabricación PLANEADA a estado EN CURSO");
                args.Data.CG_ESTADOCARGA = 0;
            }
            else if (ActualEstado == 2)
            {
                await JsRuntime.InvokeAsync<bool>("confirm",
                    "Para pasar una órden de fabricación a estado FIRME hay que Abastecerla");
                args.Data.CG_ESTADOCARGA = 0;
            }
            else if (ActualEstado == 4)
            {
                await JsRuntime.InvokeAsync<bool>("confirm", "No se pueden cerrar ordenes desde esta pantalla");
                args.Data.CG_ESTADOCARGA = 0;
            }

            HttpResponseMessage response;
            response = await Http.PutAsJsonAsync($"api/Planificacion/PutPlanif/{PreviousEstado}", args.Data);
            if (armadoCheck)
            {
                if (emitidasCheck)
                    listaPlanificacion = await Http.GetFromJsonAsync<List<Planificacion>>("api/Planificacion/0/1");
                else
                    listaPlanificacion = await Http.GetFromJsonAsync<List<Planificacion>>("api/Planificacion/0/0");
            }
            else
            {
                if (emitidasCheck)
                    listaPlanificacion = await Http.GetFromJsonAsync<List<Planificacion>>("api/Planificacion/1/1");
                else
                    listaPlanificacion = await Http.GetFromJsonAsync<List<Planificacion>>("api/Planificacion/1/0");
            }

            await GridPlanificacion.Refresh();
        }

        if (args.RequestType == Action.Grouping ||
            args.RequestType == Action.UnGrouping
            || args.RequestType == Action.ClearFiltering
            || args.RequestType == Action.CollapseAllComplete
            || args.RequestType == Action.ColumnState
            || args.RequestType == Action.ClearFiltering
            || args.RequestType == Action.Reorder ||
            args.RequestType == Action.Sorting
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

    protected async Task BuscarProductoPrevision()
    {
        CantidadMostrar2 = 100;
        if (DesString == "")
            Busquedalist =
                await Http.GetFromJsonAsync<List<Producto>>(
                    $"api/Prevision/BuscarProductoPrevision/{CgString}/Vacio/{CantidadMostrar2}");
        else if (CgString == "")
            Busquedalist =
                await Http.GetFromJsonAsync<List<Producto>>(
                    $"api/Prevision/BuscarProductoPrevision/Vacio/{DesString}/{CantidadMostrar2}");
        else
            Busquedalist =
                await Http.GetFromJsonAsync<List<Producto>>(
                    $"api/Prevision/BuscarProductoPrevision/{CgString}/{DesString}/{CantidadMostrar2}");
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
        ProdSeleccionada =
            await Http.GetFromJsonAsync<Producto>(
                $"api/Prod/GetByFilter?Codigo={args.RowData.CG_PROD}&Descripcion={string.Empty}");
        if (args.CommandColumn.Title == "Entrega")
        {
            var tipoo = 10;
            OrdenFabricacionSelected = args.RowData.CG_ORDF;
            if (ProdSeleccionada.EXIGEOA) tipoo = 28;


            await JsRuntime.InvokeVoidAsync("open", $"inventario/{tipoo}/true/{OrdenFabricacionSelected}", "_blank");
            await GridPlanificacion.Refresh();
        }

        if (args.CommandColumn.Title == "Despiece")
        {
            listaDespiece = await Http.GetFromJsonAsync<List<DespiecePlanificacion>>(
                $"api/Planificacion/Despiece/{args.RowData.CG_PROD.Trim()}/{args.RowData.CG_FORM}/{args.RowData.CANT}");
            //IsVisible = true;
            await DialogDespieceRef.ShowAsync(true);
        }

        if (args.CommandColumn.Title == "Activo")
        {
            Datos_paraFormula = args.RowData;
            listaFormula =
                await Http.GetFromJsonAsync<List<FormulaPlanificacion>>(
                    $"api/Planificacion/Formula/{args.RowData.CG_PROD.Trim()}");
            IsVisible2 = true;
        }
    }

    public async Task OnSelected()
    {
        HttpResponseMessage response;
        response = await Http.PutAsJsonAsync(
            $"api/Planificacion/PutFormula/{Grid3.GetSelectedRecordsAsync().Result.FirstOrDefault().CG_FORM}",
            Datos_paraFormula);
        IsVisible2 = false;
    }

    public void OnSelected2()
    {
        CgString = Grid5.GetSelectedRecordsAsync().Result.FirstOrDefault().Id; // return the details of selected record
        DesString = Grid5.GetSelectedRecordsAsync().Result.FirstOrDefault()
            .DES_PROD; // return the details of selected record
        CantidadMostrar2 = 0;
        IsVisible5 = false;
        IsVisible6 = true;
    }

    public async Task OrdCerradas()
    {
        VisibleProperty = true;
        listaCerradasAnuladas =
            await Http.GetFromJsonAsync<List<Planificacion>>(
                $"api/Planificacion/OrdenesCerradasYAnuladas/{CantidadMostrar}");
        VisibleProperty = false;
        if (Grid4 is not null) await Grid4?.AutoFitColumnsAsync();

        await refDialogCerradasAnuladas.ShowAsync(true);
    }


    public async Task ActionBeginOFCerradas(ActionEventArgs<Planificacion> args)
    {
        if (args.RequestType == Action.Grouping ||
            args.RequestType == Action.UnGrouping
            || args.RequestType == Action.ClearFiltering
            || args.RequestType == Action.CollapseAllComplete
            || args.RequestType == Action.ColumnState
            || args.RequestType == Action.ClearFiltering
            || args.RequestType == Action.Reorder)
            state_of_cerradas_anuladas = await Grid4.GetPersistData();
    }

    public async Task ClickHandlerOFCerradas(ClickEventArgs args)
    {
        if (args.Item.Text == "Excel Export") await Grid4.ExcelExport();
        if (args.Item.Text == "Print") await Grid4.Print();

        if (args.Item.Text == "Seleccionar Columnas") await Grid4.OpenColumnChooser();
    }

    public async Task CheckCambio()
    {
        if (armadoCheck)
        {
            if (emitidasCheck)
                listaPlanificacion = await Http.GetFromJsonAsync<List<Planificacion>>("api/Planificacion/0/1");
            else
                listaPlanificacion = await Http.GetFromJsonAsync<List<Planificacion>>("api/Planificacion/0/0");
        }
        else
        {
            if (emitidasCheck)
                listaPlanificacion = await Http.GetFromJsonAsync<List<Planificacion>>("api/Planificacion/1/1");
            else
                listaPlanificacion = await Http.GetFromJsonAsync<List<Planificacion>>("api/Planificacion/1/0");
        }

        await GridPlanificacion.RefreshColumns();
        GridPlanificacion.Refresh();
        await GridPlanificacion.RefreshHeader();
    }

    protected async Task AgregarValores()
    {
        CantidadMostrar = CantidadMostrar + 100;
        listaCerradasAnuladas =
            await Http.GetFromJsonAsync<List<Planificacion>>(
                $"api/Planificacion/OrdenesCerradasYAnuladas/{CantidadMostrar}");
    }

    protected async Task AgregarValores2()
    {
        CantidadMostrar2 = CantidadMostrar2 + 100;
        if (DesString == "")
            Busquedalist =
                await Http.GetFromJsonAsync<List<Producto>>(
                    $"api/Prevision/BuscarProductoPrevision/{CgString}/Vacio/{CantidadMostrar2}");
        else if (CgString == "")
            Busquedalist =
                await Http.GetFromJsonAsync<List<Producto>>(
                    $"api/Prevision/BuscarProductoPrevision/Vacio/{DesString}/{CantidadMostrar2}");
        else
            Busquedalist =
                await Http.GetFromJsonAsync<List<Producto>>(
                    $"api/Prevision/BuscarProductoPrevision/{CgString}/{DesString}/{CantidadMostrar2}");
    }

    protected async Task EmitirOrden()
    {
        spinnerEmitirOrden = true;
        if (string.IsNullOrEmpty(Tipo)) Tipo = "Vacio";

        //var url = "api/Planificacion/EmitirOrden";
        //url += $"?tipo={Tipo}&cgArt={CgString}&cantEmitir={cantEmitir}&feEntrega={fe_Entrega.ToString("MM-dd-yyyy")}&" +
        //    $"cgform={cg_form}&cgestadoCarga=0&semOrigen=4&pedido=0";

        var fecha = fe_Entrega.ToString("yyyy-MM-ddTHH:mm:sszzz");
        var response = await Http.GetFromJsonAsync<List<Planificacion>>($"api/Planificacion/EmitirOrden/{Tipo}/" +
                                                                        $"{CgString}/{cantEmitir}/{fecha}/{cg_form}/0/4/0");
        //var response = await Http.GetFromJsonAsync<List<Planificacion>>(url);
        spinnerEmitirOrden = false;
        listaPlanificacion.AddRange(response);
        IsVisible4 = false;
        IsVisible6 = false;
    }

    public async Task QueryCellInfoHandler(QueryCellInfoEventArgs<Planificacion> args)
    {
        if (args.Column.Field == "CG_ESTADOCARGA") args.Cell.AddClass(new[] { "gris" });
        if (args.Column.Field == "FE_ENTREGA") args.Cell.AddClass(new[] { "gris" });
        if (args.Column.Field == "FE_EMIT") args.Cell.AddClass(new[] { "gris" });
        if (args.Column.Field == "FE_PLAN") args.Cell.AddClass(new[] { "gris" });
        if (args.Column.Field == "FE_FIRME") args.Cell.AddClass(new[] { "gris" });
        if (args.Column.Field == "FE_CURSO") args.Cell.AddClass(new[] { "gris" });
        if (args.Column.Field == "CANT") args.Cell.AddClass(new[] { "gris" });
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

    protected async Task OnChangeEstadoCarga(ChangeEventArgs<int, EstadosCargaMaquina> args,
        Planificacion planificacion)
    {
        if (args.ItemData.CG_ESTADO < 4)
        {
            var response = await Http.PutAsJsonAsync("api/Planificacion/RehabilitarOrden", planificacion);
            if (response.IsSuccessStatusCode)
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

    public class Estado
    {
        public string Texto { get; set; }
        public int Valor { get; set; }
    }
}