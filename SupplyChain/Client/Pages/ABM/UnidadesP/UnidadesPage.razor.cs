using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SupplyChain.Client.HelperService;
using SupplyChain.Client.Shared;
using SupplyChain.Shared;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Navigations;
using Syncfusion.Blazor.Notifications;
using Syncfusion.Blazor.Spinner;

namespace SupplyChain.Client.Pages.ABM.UnidadesP;

public class UnidadPageBase : ComponentBase
{
    protected bool popupFormVisible;
    protected SfGrid<Unidades> refGrid;
    protected SfSpinner refSpinner;
    protected bool SpinnerVisible;

    protected SfToast ToastObj;

    protected List<object> Toolbaritems = new()
    {
        "Search",
        "Add",
        "Edit",
        "Delete",
        "Print",
        new ItemModel { Text = "Copia", TooltipText = "Copiar una unidad", PrefixIcon = "e-copy", Id = "Copy" },
        "ExcelExport"
    };

    protected List<Unidades> unidades = new();
    protected Unidades unidadSeleccionada = new();
    [Inject] protected HttpClient Http { get; set; }
    [Inject] protected IJSRuntime jSRuntime { get; set; }
    [Inject] protected UnidadesService UnidadesService { get; set; }

    [CascadingParameter] private MainLayout MainLayout { get; set; }

    protected override async Task OnInitializedAsync()
    {
        MainLayout.Titulo = "Unidades";

        SpinnerVisible = true;
        var response = await UnidadesService.Get();
        if (!response.Error) unidades = response.Response;
        SpinnerVisible = false;
    }

    protected async Task OnToolbarHandler(ClickEventArgs args)
    {
        if (args.Item.Id == "Copy")
        {
            await CopiarUnidad();
        }
        else if (args.Item.Id == "grdUnidades_delete")
        {
            if ((await refGrid.GetSelectedRecordsAsync()).Count > 0)
            {
                var isConfirmed =
                    await jSRuntime.InvokeAsync<bool>("confirm", "Seguro de que desea eliminar la unidad?");
                if (isConfirmed)
                {
                    var unidadesABorrar = await refGrid.GetSelectedRecordsAsync();
                    var response = UnidadesService.Eliminar(unidadesABorrar);
                    if (!response.IsCompletedSuccessfully)
                        await ToastObj.Show(new ToastModel
                        {
                            Title = "EXITO!",
                            Content = "las unidades seleccionadas fueron eliminadas correctamente.",
                            CssClass = "e-toast-success",
                            Icon = "e-success toast-icons",
                            ShowCloseButton = true,
                            ShowProgressBar = true
                        });
                    else
                        await ToastMensajeError();
                }
            }
        }
        else if (args.Item.Id == "grdUnidad_excelexport")
        {
            await refGrid.ExportToExcelAsync();
        }
    }

    private async Task CopiarUnidad()
    {
        if (refGrid.SelectedRecords.Count == 1)
        {
            unidadSeleccionada = new Unidades();
            var selectedRecord = refGrid.SelectedRecords[0];
            var isConfirmed = await jSRuntime.InvokeAsync<bool>("confirm", "Seguro de que desea copiar la unidad?");
            if (isConfirmed)
            {
                unidadSeleccionada.ESNUEVO = true;
                unidadSeleccionada.DES_UNID = selectedRecord.DES_UNID;
                unidadSeleccionada.TIPOUNID = selectedRecord.TIPOUNID;
                unidadSeleccionada.CG_DENBASICA = selectedRecord.CG_DENBASICA;
                unidadSeleccionada.CODIGO = selectedRecord.CODIGO;
                popupFormVisible = true;
            }
        }
        else
        {
            await ToastObj.Show(new ToastModel
            {
                Title = "ERROR!",
                Content = "Solo se puede copiar un item",
                CssClass = "e-toast-danger",
                Icon = "e-error toast-icons",
                ShowCloseButton = true,
                ShowProgressBar = true
            });
        }
    }

    protected async Task OnActionBeginHandler(ActionEventArgs<Unidades> args)
    {
        if (args.RequestType == Action.Add ||
            args.RequestType == Action.BeginEdit)
        {
            args.Cancel = true;
            args.PreventRender = false;
            popupFormVisible = true;
            unidadSeleccionada = new Unidades();
            unidadSeleccionada.ESNUEVO = true;
        }

        if (args.RequestType == Action.BeginEdit)
        {
            unidadSeleccionada = args.Data;
            unidadSeleccionada.ESNUEVO = false;
        }

        if (args.RequestType == Action.Grouping
            || args.RequestType == Action.UnGrouping
            || args.RequestType == Action.ClearFiltering
            || args.RequestType == Action.CollapseAllComplete
            || args.RequestType == Action.ColumnState
            || args.RequestType == Action.ClearFiltering
            || args.RequestType == Action.Reorder
            || args.RequestType == Action.Sorting
           )
        {
            //VisibleProperty = true;
            refGrid.PreventRender();
            refGrid.Refresh();

            state = await refGrid.GetPersistData();
            await refGrid.AutoFitColumnsAsync();
            await refGrid.RefreshColumns();
            await refGrid.RefreshHeader();
        }
    }

    protected async Task OnActionCompleteHandler(ActionEventArgs<Unidades> args)
    {
        if (args.RequestType == Action.BeginEdit)
        {
            args.Cancel = true;
            args.PreventRender = false;
            popupFormVisible = true;
        }
    }

    protected void OnCerrarDialog()
    {
        popupFormVisible = false;
    }

    protected async Task Guardar(Unidades unidad)
    {
        if (unidad.GUARDADO)
        {
            await ToastMensajeExito();
            popupFormVisible = false;
            if (unidad.ESNUEVO)
            {
                unidades.Add(unidad);
            }
            else
            {
                //actualizar datos sin ir a la base de datos
                var unidadSinModificar = unidades.Where(p => p.Id == unidad.Id).FirstOrDefault();
                unidadSinModificar.Id = unidad.Id;
                unidadSinModificar.DES_UNID = unidad.DES_UNID;
                unidadSinModificar.TIPOUNID = unidad.TIPOUNID;
                unidadSinModificar.CG_DENBASICA = unidad.CG_DENBASICA;
                unidadSinModificar.CODIGO = unidad.CODIGO;
                unidades.OrderByDescending(p => p.Id);
            }

            await refGrid.RefreshHeaderAsync();
            refGrid.Refresh();
            await refGrid.RefreshColumnsAsync();
        }
        else
        {
            await ToastMensajeError();
        }
    }

    private async Task ToastMensajeExito()
    {
        await ToastObj.Show(new ToastModel
        {
            Title = "EXITO!",
            Content = "Guardado Correctamente.",
            CssClass = "e-toast-success",
            Icon = "e-success toast-icons",
            ShowCloseButton = true,
            ShowProgressBar = true
        });
    }

    private async Task ToastMensajeError()
    {
        await ToastObj.Show(new ToastModel
        {
            Title = "Error!",
            Content = "Ocurrio un Error.",
            CssClass = "e-toast-warning",
            Icon = "e-warning toast-icons",
            ShowCloseButton = true,
            ShowProgressBar = true
        });
    }

    #region "Vista Grilla"

    protected const string APPNAME = "grdUnidadesABM";
    protected string state;

    #endregion

    #region "Eventos Vista Grilla"

    protected async Task OnVistaSeleccionada(VistasGrillas vistasGrillas)
    {
        await refGrid.SetPersistData(vistasGrillas.Layout);
    }

    protected async Task OnReiniciarGrilla()
    {
        await refGrid.ResetPersistData();
    }

    #endregion
}