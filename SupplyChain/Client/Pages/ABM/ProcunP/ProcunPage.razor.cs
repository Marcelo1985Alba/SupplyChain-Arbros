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

namespace SupplyChain.Client.Pages.ABM.ProcunP;

public class ProcunPageBase : ComponentBase
{
    protected bool popupFormVisible;
    protected Procun procSeleccionado = new();
    protected List<Procun> procuns = new();
    protected SfGrid<Procun> refGrid;
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
        new ItemModel { Text = "Copia", TooltipText = "Copiar una celda", PrefixIcon = "e-copy", Id = "Copy" },
        "ExcelExport"
    };

    [Inject] protected HttpClient Http { get; set; }
    [Inject] protected IJSRuntime jSRuntime { get; set; }
    [Inject] protected ProcunService ProcunService { get; set; }

    [CascadingParameter] private MainLayout MainLayout { get; set; }

    protected override async Task OnInitializedAsync()
    {
        MainLayout.Titulo = "Procun";

        SpinnerVisible = true;
        var response = await ProcunService.Get();
        if (!response.Error) procuns = response.Response;
        SpinnerVisible = false;
    }

    protected async Task OnToolbarHandler(ClickEventArgs args)
    {
        if (args.Item.Id == "Copy")
        {
            await CopiarProcun();
        }
        else if (args.Item.Id == "grdProcun_delete")
        {
            if ((await refGrid.GetSelectedRecordsAsync()).Count > 0)
            {
                var isConfirmed =
                    await jSRuntime.InvokeAsync<bool>("confirm", "Seguro de que desea eliminar el procun?");
                if (isConfirmed)
                {
                    var procunsABorrar = await refGrid.GetSelectedRecordsAsync();
                    var response = ProcunService.Eliminar(procunsABorrar);
                    if (!response.IsCompletedSuccessfully)
                        await ToastObj.Show(new ToastModel
                        {
                            Title = "EXITO!",
                            Content = "los procun seleccionados fueron eliminadas correctamente.",
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
        else if (args.Item.Id == "grdProcun_excelexport")
        {
            await refGrid.ExportToExcelAsync();
        }
    }

    private async Task CopiarProcun()
    {
        if (refGrid.SelectedRecords.Count == 1)
        {
            procSeleccionado = new Procun();
            var selectedRecord = refGrid.SelectedRecords[0];
            var isConfirmed = await jSRuntime.InvokeAsync<bool>("confirm", "Seguro de que desea copiar el procun?");
            if (isConfirmed)
            {
                procSeleccionado.ESNUEVO = true;
                procSeleccionado.ORDEN = selectedRecord.ORDEN;
                procSeleccionado.CG_PROD = selectedRecord.CG_PROD;
                procSeleccionado.CG_AREA = selectedRecord.CG_AREA;
                procSeleccionado.CG_LINEA = selectedRecord.CG_LINEA;
                procSeleccionado.CG_CELDA = selectedRecord.CG_CELDA;
                procSeleccionado.PROCESO = selectedRecord.PROCESO;
                procSeleccionado.TIEMPO1 = selectedRecord.TIEMPO1;
                procSeleccionado.TS1 = selectedRecord.TS1;
                procSeleccionado.PROPORC = selectedRecord.PROPORC;
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

    protected async Task OnActionBeginHandler(ActionEventArgs<Procun> args)
    {
        if (args.RequestType == Action.Add ||
            args.RequestType == Action.BeginEdit)
        {
            args.Cancel = true;
            args.PreventRender = false;
            popupFormVisible = true;
            procSeleccionado = new Procun();
            procSeleccionado.ESNUEVO = true;
        }

        if (args.RequestType == Action.BeginEdit)
        {
            procSeleccionado = args.Data;
            procSeleccionado.ESNUEVO = false;
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

    protected async Task OnActionCompleteHandler(ActionEventArgs<Procun> args)
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

    protected async Task Guardar(Procun proc)
    {
        if (proc.GUARDADO)
        {
            await ToastMensajeExito();
            popupFormVisible = false;
            if (proc.ESNUEVO)
            {
                procuns.Add(proc);
            }
            else
            {
                //actualizar datos sin ir a la base de datos
                var procunSinModificar = procuns.Where(p => p.Id == proc.Id).FirstOrDefault();
                procunSinModificar.Id = proc.Id;
                procunSinModificar.ORDEN = proc.ORDEN;
                procunSinModificar.CG_PROD = proc.CG_PROD;
                procunSinModificar.CG_FORM = proc.CG_FORM;
                procunSinModificar.CG_AREA = proc.CG_AREA;
                procunSinModificar.CG_LINEA = proc.CG_LINEA;
                procunSinModificar.CG_CELDA = proc.CG_CELDA;
                procunSinModificar.PROCESO = proc.PROCESO;
                procunSinModificar.DESCRIP = proc.DESCRIP;
                procunSinModificar.OBSERV = proc.OBSERV;
                procunSinModificar.DESPROC = proc.DESPROC;
                procunSinModificar.TIEMPO1 = proc.TIEMPO1;
                procunSinModificar.TS1 = proc.TS1;
                procunSinModificar.FRECU = proc.FRECU;
                procunSinModificar.CG_CALI1 = proc.CG_CALI1;
                procunSinModificar.PROPORC = proc.PROPORC;
                procunSinModificar.TOLE1 = proc.TOLE1;
                procunSinModificar.CG_CALI2 = proc.CG_CALI2;
                procunSinModificar.VALOR1 = proc.VALOR1;
                procunSinModificar.TOLE2 = proc.TOLE2;
                procunSinModificar.CG_CALI3 = proc.CG_CALI3;
                procunSinModificar.VALOR2 = proc.VALOR2;
                procunSinModificar.TOLE3 = proc.TOLE3;
                procunSinModificar.CG_CALI4 = proc.CG_CALI4;
                procunSinModificar.VALOR3 = proc.VALOR3;
                procunSinModificar.TOLE4 = proc.TOLE4;
                procunSinModificar.CG_CALI5 = proc.CG_CALI5;
                procunSinModificar.CG_CALI6 = proc.CG_CALI6;
                procunSinModificar.CG_CALI7 = proc.CG_CALI7;
                procunSinModificar.CG_OPER = proc.CG_OPER;
                procunSinModificar.FECHA = proc.FECHA;
                procunSinModificar.COSTO = proc.COSTO;
                procunSinModificar.VALOR4 = proc.VALOR4;
                procunSinModificar.COSTOCOMB = proc.COSTOCOMB;
                procunSinModificar.COSTOENERG = proc.COSTOENERG;
                procunSinModificar.PLANTEL = proc.PLANTEL;
                procunSinModificar.CG_CATEOP = proc.CG_CATEOP;
                procunSinModificar.COSTAC = proc.COSTAC;
                procunSinModificar.OCUPACION = proc.OCUPACION;
                procunSinModificar.COEFI = proc.COEFI;
                procunSinModificar.TAREAPROC = proc.TAREAPROC;
                procunSinModificar.ESTANDAR = proc.ESTANDAR;
                procunSinModificar.RELEVAN = proc.RELEVAN;
                procunSinModificar.REVISION = proc.REVISION;
                procunSinModificar.USUARIO = proc.USUARIO;
                procunSinModificar.AUTORIZA = proc.AUTORIZA;
                procuns.OrderByDescending(p => p.Id);
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

    protected const string APPNAME = "grdCeldasABM";
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