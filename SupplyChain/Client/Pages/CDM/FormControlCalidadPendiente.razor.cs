using Microsoft.AspNetCore.Components;
using SupplyChain.Client.HelperService;
using Syncfusion.Blazor.Grids;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Syncfusion.Blazor.Spinner;
using Syncfusion.Blazor.Notifications;
using SupplyChain.Shared;
using Microsoft.JSInterop;
using System.Security.Authentication.ExtendedProtection;
using SupplyChain.Client.Shared;
using System.Linq;
using System.Net.Http.Json;
using SupplyChain.Shared.Models;
using SupplyChain.Client.Pages.Ventas._2_Pedidos;
using SupplyChain.Shared.PCP;

namespace SupplyChain.Client.Pages.CDM
{
    public class FormCargaValoresBase : ComponentBase
    {
        [Inject] protected HttpClient Http { get; set; }
        [Inject] protected InventarioService InventarioService { get; set; }
        [Inject] protected ProcesoService ProcesoService { get; set; }
        [Inject] protected IJSRuntime jSRuntime { get; set; }
        [Parameter] public Pedidos controlCalidadPendientes { get; set; }
        [Parameter] public bool Show { get; set; } = false;
        [Parameter] public EventCallback OnCerrar { get; set; }
        protected const string APPNAME = "grdCargaProcesos";
        [CascadingParameter] MainLayout MainLayout { get; set; }

        protected SfGrid<vControlCalidadPendientes> refGridItems;

        protected SfSpinner refSpinnerCli;
        protected SfSpinner refSpinner;
        protected SfToast ToastObj;

        protected List<vControlCalidadPendientes> segGrilla = new();
        protected bool popupFormVisible = false;
        protected bool SpinnerVisible = false;
        protected string state;

        protected bool BotonGuardarDisabled = false;
        protected Producto prodList = new();


        protected Dictionary<string, object> HtmlAttributeSubmit = new()
        {
            { "type", "submit" },
            { "form", "formControlCalidad" }
        };

        protected Dictionary<string, object> HtmlAttribute = new()
        {
            { "type", "button" }
        };


        protected override async Task OnInitializedAsync()
        {
            MainLayout.Titulo = "Control de Calidad";

            SpinnerVisible = true;
            segGrilla = (await InventarioService.GetSegundaGrilla()).Where(s => s.VALE == controlCalidadPendientes.VALE && s.DESPACHO == controlCalidadPendientes.DESPACHO).ToList();
            SpinnerVisible = false;
        }
        protected async Task OnActionBeginHandler(ActionEventArgs<Pedidos> args)
        {
            if (args.RequestType == Syncfusion.Blazor.Grids.Action.BeginEdit)
            {
                args.Cancel = true;
                args.PreventRender = false;
                popupFormVisible = true;
            }
            if (args.RequestType == Syncfusion.Blazor.Grids.Action.Grouping
                || args.RequestType == Syncfusion.Blazor.Grids.Action.UnGrouping
                || args.RequestType == Syncfusion.Blazor.Grids.Action.ClearFiltering
                || args.RequestType == Syncfusion.Blazor.Grids.Action.CollapseAllComplete
                || args.RequestType == Syncfusion.Blazor.Grids.Action.ColumnState
                || args.RequestType == Syncfusion.Blazor.Grids.Action.Reorder
                || args.RequestType == Syncfusion.Blazor.Grids.Action.Sorting
                )
            {
                refGridItems.PreventRender();
                refGridItems.Refresh();

                state = await refGridItems.GetPersistData();
                await refGridItems.AutoFitColumnsAsync();
                await refGridItems.RefreshColumns();
                await refGridItems.RefreshHeader();
            }
        } 

        protected async Task OnActionCompleteHandler(ActionEventArgs<Pedidos> args)
        {
            if (args.RequestType == Syncfusion.Blazor.Grids.Action.BeginEdit)
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
        protected async Task openedDialog()
        {
            List<Procesos> porDespacho = await Http.GetFromJsonAsync<List<Procesos>>($"api/Proceso/BuscarPorDESPACHO/{controlCalidadPendientes.DESPACHO}");
            if (porDespacho.Count != 0)
            {
                bool isConfirmed = await jSRuntime.InvokeAsync<bool>("confirm", "Se han encontrado procesos guardados con el mismo despacho, desea copiarlos?");
                if (isConfirmed)
                {
                    Show = false;
                    porDespacho = porDespacho.Where(s => s.VALE == porDespacho[0].VALE).ToList();
                    foreach(Procesos item in porDespacho)
                    {
                        item.FECHA = DateTime.Now;
                        item.FE_REG = DateTime.Now;
                        item.FE_ENSAYO = DateTime.Now;
                        item.CG_PROD = controlCalidadPendientes.CG_ART;
                        item.Id = 0;
                        item.VALE = controlCalidadPendientes.VALE.ToString();
                    }
                    var response = await ProcesoService.GuardarLista(porDespacho);
                    if (response.Error)
                    {
                        await ToastMensajeError($"Error al guardar en Procesos\n\rDetalle:{response.HttpResponseMessage.ReasonPhrase}");
                        return;
                    }

                    if (response.Response.Any(p => p.APROBADO == "N"))
                    {
                        controlCalidadPendientes.CG_DEP = 3;
                    }
                    else
                    {
                        controlCalidadPendientes.CG_DEP = 4;
                    }

                    var response2 = await Http.PutAsJsonAsync($"api/Pedidos/ActualizaDeposito/{controlCalidadPendientes.Id}", controlCalidadPendientes);
                    if (!response2.IsSuccessStatusCode)
                    {
                        await ToastMensajeError("Error al actualizar deposito");
                    }
                    await refGridItems.Refresh();
                    return;
                }
            }

            SpinnerVisible = true;
            segGrilla = await InventarioService.GetSegundaGrilla();
            prodList = await Http.GetFromJsonAsync<Producto>($"api/Prod/{controlCalidadPendientes.CG_ART}");
            segGrilla = segGrilla.Where(s => s.VALE == controlCalidadPendientes.VALE && s.DESPACHO == controlCalidadPendientes.DESPACHO && s.CG_LINEA == prodList.CG_LINEA && s.CG_PROD == controlCalidadPendientes.CG_ART).ToList();
            SpinnerVisible = false;
        }
        protected async Task<bool> Guardar()
        {
            popupFormVisible = false;
            BotonGuardarDisabled = true;
            foreach (var item in segGrilla)
            {
                if (item.TOLE1 == 0)
                {
                    if (item.VALOR > item.TOLE2)
                    {
                        await ToastMensajeError($"En el elemento {item.UNIDADM} se ingresó {item.VALOR} y debería estar entre los valores {item.TOLE1} y {item.TOLE2}.");
                        BotonGuardarDisabled = false;
                        return false;
                    }
                }
                else if (item.TOLE2 == 0)
                {
                    if (item.TOLE1 != 0 && item.VALOR < item.TOLE1)
                    {
                        await ToastMensajeError($"En el elemento {item.UNIDADM} se ingresó {item.VALOR} y debería estar entre los valores {item.TOLE1} y {item.TOLE2}.");
                        BotonGuardarDisabled = false;
                        return false;
                    }
                }
                else if (item.VALOR < item.TOLE1 || item.VALOR > item.TOLE2)
                {
                    await ToastMensajeError($"En el elemento {item.UNIDADM} se ingresó {item.VALOR} y debería estar entre los valores {item.TOLE1} y {item.TOLE2}.");
                    BotonGuardarDisabled = false;
                    return false;
                }
            }
            await ToastMensajeExito("Todos los datos fueron ingresador correctamente, guardando.");

            List<Procesos> toSaveList = new List<Procesos>();

            foreach (var item in segGrilla)
            {
                Procesos toSave = new Procesos();
                toSave.VALE = controlCalidadPendientes.VALE.ToString();
                toSave.DESPACHO = controlCalidadPendientes.DESPACHO;
                toSave.FE_ENSAYO = DateTime.Now;
                toSave.CG_PROD = controlCalidadPendientes.CG_ART;
                toSave.CG_ORDEN = (int)controlCalidadPendientes.CG_ORDEN;
                toSave.DESCAL = item.DESCAL;
                toSave.CARCAL = item.CARCAL;
                toSave.UNIDADM = item.UNIDADM;
                toSave.CANTMEDIDA = 0;
                if (item.VALOR == null) 
                {
                    toSave.MEDIDA = 0;
                }
                toSave.MEDIDA = item.VALOR;
                toSave.TOLE1 = item.TOLE1;
                toSave.TOLE2 = item.TOLE2;
                toSave.OBSERV = controlCalidadPendientes.OBS1;
                toSave.AVISO = item.AVISO;
                toSave.MEDIDA1 = 0;
                toSave.OBSERV1 = controlCalidadPendientes.OBS2;
                toSave.CG_PROVE = (int)controlCalidadPendientes.CG_PROVE;
                toSave.REMITO = controlCalidadPendientes.REMITO;
                toSave.VALORNC = "";
                toSave.LEYENDANC = "";
                toSave.LOTE = controlCalidadPendientes.LOTE;
                toSave.CG_ORDF = (int)controlCalidadPendientes.CG_ORDF;
                toSave.UNID = controlCalidadPendientes.UNID;
                toSave.NUM_PASE = 0;
                toSave.ENSAYOS = "";
                toSave.FECHA = DateTime.Now;
                toSave.APROBADO = "S";
                toSave.TIPO = "";
                toSave.CG_CLI = 0;
                toSave.USUARIO = "";
                toSave.FE_REG = DateTime.Now;
                toSaveList.Add(toSave);
            }


            var response = await ProcesoService.GuardarLista(toSaveList);
            if (response.Error)
            {
                await ToastMensajeError($"Error al guardar en Procesos\n\rDetalle:{response.HttpResponseMessage.ReasonPhrase}");
                return false;
            }

            if (response.Response.Any(p => p.APROBADO == "N"))
            {
                controlCalidadPendientes.CG_DEP = 3;
            }
            else
            {
                controlCalidadPendientes.CG_DEP = 4;
            }

            var response2 = await Http.PutAsJsonAsync($"api/Pedidos/ActualizaDeposito/{controlCalidadPendientes.Id}", controlCalidadPendientes);
            if (!response2.IsSuccessStatusCode)
            {
                await ToastMensajeError("Error al actualizar deposito");
                return false;
            }
            await refGridItems.Refresh();
            BotonGuardarDisabled = false;
            await ToastMensajeExito("Guardado");
            Show = false;
            return true;
        }

        protected bool IsAdd { get; set; }

        protected async Task BatchDeleteHandler(BeforeBatchDeleteArgs<vControlCalidadPendientes> args)
        {

        }
        public void BatchAddHandler(BeforeBatchAddArgs<vControlCalidadPendientes> args)
        {
            IsAdd = true;
        }
        public void BatchSaveHandler(BeforeBatchSaveArgs<vControlCalidadPendientes> args)
        {
            IsAdd=false;
        }
        public async void CellSavedHandler(CellSaveArgs<vControlCalidadPendientes> args)
        {
            segGrilla.Where(s => s.UNIDADM == args.Data.UNIDADM).FirstOrDefault().VALOR = args.Data.VALOR;
            await refGridItems.Refresh();
        }

        protected async Task QueryCellInfoHandler(QueryCellInfoEventArgs<vControlCalidadPendientes> args)
        {
            if (args.Data.VALOR == 0)
            {
                args.Cell.AddClass(new string[] { "blancas" });
            }
            else if (args.Data.TOLE2 == 0)
            {
                if (args.Data.TOLE1 == 0 || args.Data.VALOR >= args.Data.TOLE1)
                {
                    args.Cell.AddClass(new string[] { "verdes" });
                }
                else
                {
                    args.Cell.AddClass(new string[] { "rojas" });
                }
            }
            else if (args.Data.TOLE1 == 0)
            {
                if (args.Data.VALOR <= args.Data.TOLE2)
                {
                    args.Cell.AddClass(new string[] { "verdes" });
                }
                else
                {
                    args.Cell.AddClass(new string[] { "rojas" });
                }
            }
            else if (args.Data.VALOR >= args.Data.TOLE1 && args.Data.VALOR <= args.Data.TOLE2)
            {
                args.Cell.AddClass(new string[] { "verdes" });
            }
            else
            {
                args.Cell.AddClass(new string[] { "rojas" });
            }
        }
        public async Task Hide()
        {
            Show = false;
        }

        private async Task ToastMensajeExito(string content = "Guardado Correctamente.")
        {
            await this.ToastObj.ShowAsync(new ToastModel
            {
                Title = "EXITO!",
                Content = content,
                CssClass = "e-toast-success",
                Icon = "e-warning toast-icons",
                ShowCloseButton = true,
                ShowProgressBar = true
            });
        }
        private async Task ToastMensajeError(string content = "Ocurrio un error.")
        {
            await ToastObj.ShowAsync(new ToastModel
            {
                Title = "EXITO!",
                Content = content,
                CssClass = "e-toast-success",
                Icon = "e-warning toast-icons",
                ShowCloseButton = true,
                ShowProgressBar = true
            });

        }
    }
}

