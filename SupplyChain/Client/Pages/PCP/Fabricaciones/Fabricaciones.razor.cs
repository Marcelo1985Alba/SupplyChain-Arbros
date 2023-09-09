using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SupplyChain.Client.HelperService;
using SupplyChain.Shared;
using SupplyChain.Shared.Models;
using SupplyChain.Shared.PCP;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Navigations;
using Syncfusion.Blazor.Notifications;
using Syncfusion.Blazor.Spinner;

namespace SupplyChain.Pages.Fab;

public class FabricPageBase : ComponentBase
{
    protected const string APPNAME = "grdFabricaciones";

    protected DialogSettings DialogParams = new() { MinHeight = "400px", Width = "500px" };
    public bool Disabled = false;

    public bool Enabled = true;
    protected SfGrid<Fabricacion> Grid;
    protected List<Celdas> listaCelda = new();
    protected List<EstadosCargaMaquina> listaEstadosCargaMaquina = new();

    //protected List<CatOpe> catopes = new List<CatOpe>();
    protected List<Fabricacion> listaFab;
    protected SfSpinner refSpinner;
    protected string state;

    protected SfToast ToasObj;

    protected List<object> Toolbaritems = new()
    {
        "Search",
        new ItemModel { Type = ItemType.Separator },
        "Print",
        new ItemModel { Type = ItemType.Separator },
        "ExcelExport",
        "Actualizar Fecha Ordenes en Curso"
    };

    [Inject] protected HttpClient Http { get; set; }
    [Inject] protected IJSRuntime JsRuntime { get; set; }
    [Inject] protected CeldasService CeldasService { get; set; }
    protected bool VisibleProperty { get; set; }

    protected override async Task OnInitializedAsync()
    {
        VisibleProperty = true;
        listaFab = await Http.GetFromJsonAsync<List<Fabricacion>>("api/Fabricacion");
        var response = await CeldasService.Get();
        if (!response.Error) listaCelda = response.Response;
        //listaCelda = await Http.GetFromJsonAsync<List<Celdas>>("api/Celdas");
        listaEstadosCargaMaquina = await Http.GetFromJsonAsync<List<EstadosCargaMaquina>>("api/EstadosCargaMaquinas");
        VisibleProperty = false;
    }

    public async Task DataBoundHandler()
    {
        await Grid.AutoFitColumns();
    }

    public async Task ClickHandler(ClickEventArgs args)
    {
        if (args.Item.Text == "Excel Export")
        {
            await Grid.ExcelExport();
        }
        else if (args.Item.Text == "Print")
        {
            await Grid.Print();
        }
        else if (args.Item.Text == "Actualizar Fecha Ordenes en Curso")
        {
            VisibleProperty = true;
            var reponse = await Http.PutAsJsonAsync("api/OrdenesFabricacion/actualizarFechaCursoPrimeraCelda", args);
            if (reponse.IsSuccessStatusCode)
                await ToasObj.Show(new ToastModel
                {
                    Title = "Exito!",
                    Content = "Guardado Correctamente!",
                    CssClass = "e-toast-success",
                    Icon = "e-success toast-icons",
                    ShowCloseButton = true,
                    ShowProgressBar = true
                });
            else
                await ToasObj.Show(new ToastModel
                {
                    Title = "ERROR!",
                    Content = "Ocurrrio un error.Error al intentar actualizar fechas de curso para ordenes en curso. ",
                    CssClass = "e-toast-danger",
                    Icon = "e-error toast-icons",
                    ShowCloseButton = true,
                    ShowProgressBar = true
                });
            VisibleProperty = false;
        }
    }

    public async Task Begin(ActionEventArgs<Fabricacion> args)
    {
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
            Grid.PreventRender();
            Grid.Refresh();

            state = await Grid.GetPersistData();
            await Grid.AutoFitColumnsAsync();
            await Grid.RefreshColumns();
            await Grid.RefreshHeader();
            //VisibleProperty = false;
        }

        if (args.RequestType == Action.RowDragAndDrop)
        {
        }
    }

    public async Task ActionComplete(ActionEventArgs<Fabricacion> args)
    {
        if (args.RequestType == Action.Save)
        {
            var respuesta = await Http.PutAsJsonAsync($"api/OrdenesFabricacion/PutFromModeloOF/{args.Data.CG_ORDF}",
                args.Data);
            if (respuesta.StatusCode == HttpStatusCode.BadRequest)
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
                await ToasObj.Show(new ToastModel
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

        if (args.RequestType == Action.RowDragAndDrop)
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