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

namespace SupplyChain.Client.Pages.ABM.TipoAreasP;

public class TipoAreaPageBase : ComponentBase
{
    protected bool popupFormVisible;
    protected SfGrid<TipoArea> refGrid;
    protected SfSpinner refSpinner;
    protected bool SpinnerVisible;
    protected List<TipoArea> tipoArea = new();
    protected TipoArea tipoAreaSeleccionado = new();

    protected SfToast ToastObj;

    protected List<object> Toolbaritems = new()
    {
        "Search",
        "Add",
        "Edit",
        "Delete",
        "Print",
        new ItemModel { Text = "Copia", TooltipText = "Copiar un tipo de area", PrefixIcon = "e-copy", Id = "Copy" },
        "ExcelExport"
    };

    [Inject] protected HttpClient Http { get; set; }
    [Inject] protected IJSRuntime jSRuntime { get; set; }
    [Inject] protected TipoAreaService TipoAreaService { get; set; }

    [CascadingParameter] private MainLayout MainLayout { get; set; }

    protected override async Task OnInitializedAsync()
    {
        MainLayout.Titulo = "TipoArea";

        SpinnerVisible = true;
        var response = await TipoAreaService.Get();
        if (!response.Error) tipoArea = response.Response;
        SpinnerVisible = false;
    }

    protected async Task OnToolbarHandler(ClickEventArgs args)
    {
        if (args.Item.Id == "Copy")
        {
            await CopiarTipoArea();
        }
        else if (args.Item.Id == "grdTipoArea_delete")
        {
            if ((await refGrid.GetSelectedRecordsAsync()).Count > 0)
            {
                var isConfirmed =
                    await jSRuntime.InvokeAsync<bool>("confirm", "Seguro de que desea eliminar el tipo de area?");
                if (isConfirmed)
                {
                    var tipoAreaABorrar = await refGrid.GetSelectedRecordsAsync();
                    var response = TipoAreaService.Eliminar(tipoAreaABorrar);
                    if (!response.IsCompletedSuccessfully)
                        await ToastObj.Show(new ToastModel
                        {
                            Title = "EXITO!",
                            Content = "los tipos de area seleccionados fueron eliminadas correctamente.",
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
        else if (args.Item.Id == "grdTipoArea_excelexport")
        {
            await refGrid.ExportToExcelAsync();
        }
    }

    private async Task CopiarTipoArea()
    {
        if (refGrid.SelectedRecords.Count == 1)
        {
            tipoAreaSeleccionado = new TipoArea();
            var selectedRecord = refGrid.SelectedRecords[0];
            var isConfirmed =
                await jSRuntime.InvokeAsync<bool>("confirm", "Seguro de que desea copiar el tipo de area?");
            if (isConfirmed)
            {
                tipoAreaSeleccionado.ESNUEVO = true;
                tipoAreaSeleccionado.DES_TIPOAREA = selectedRecord.DES_TIPOAREA;
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

    protected async Task OnActionBeginHandler(ActionEventArgs<TipoArea> args)
    {
        if (args.RequestType == Action.Add ||
            args.RequestType == Action.BeginEdit)
        {
            args.Cancel = true;
            args.PreventRender = false;
            popupFormVisible = true;
            tipoAreaSeleccionado = new TipoArea();
            tipoAreaSeleccionado.ESNUEVO = true;
        }

        if (args.RequestType == Action.BeginEdit)
        {
            tipoAreaSeleccionado = args.Data;
            tipoAreaSeleccionado.ESNUEVO = false;
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

    protected async Task OnActionCompleteHandler(ActionEventArgs<TipoArea> args)
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

    protected async Task Guardar(TipoArea tipoarea)
    {
        if (tipoarea.GUARDADO)
        {
            await ToastMensajeExito();
            popupFormVisible = false;
            if (tipoarea.ESNUEVO)
            {
                tipoArea.Add(tipoarea);
            }
            else
            {
                //actualizar datos sin ir a la base de datos
                var tipoAreaSinModificar = tipoArea.Where(p => p.Id == tipoarea.Id).FirstOrDefault();
                tipoAreaSinModificar.Id = tipoarea.Id;
                tipoAreaSinModificar.DES_TIPOAREA = tipoarea.DES_TIPOAREA;
                tipoArea.OrderByDescending(p => p.Id);
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

    protected const string APPNAME = "grdTipoAreaABM";
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