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

namespace SupplyChain.Client.Pages.CDM
{
    public class FormCargaValoresBase : ComponentBase
    {
        [Inject] protected HttpClient Http { get; set; }
        [Inject] protected InventarioService InventarioService { get; set; }
        [Inject] protected IJSRuntime jSRuntime { get; set; }
        [Parameter] public Pedidos controlCalidadPendientes { get; set; }
        [Parameter] public bool Show { get; set; } = false;
        [Parameter] public EventCallback OnCerrar { get; set; }
        protected const string APPNAME = "grdCargaProcesos";
        [CascadingParameter] MainLayout MainLayout { get; set; }

        protected SfGrid<vControlCalidadPendientes> refGridItems;

        protected SfGrid<Pedidos> refGrid;
        protected SfSpinner refSpinnerCli;
        protected SfSpinner refSpinner;
        protected SfToast ToastObj;

        protected List<vControlCalidadPendientes> segGrilla = new();
        protected bool popupFormVisible = false;
        protected bool SpinnerVisible = false;
        protected string state;

        protected bool BotonGuardarDisabled = false;

        protected Dictionary<string, object> HtmlAttributeSubmit = new()
        {
            { "type", "submit" }
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
            SpinnerVisible = true;
            segGrilla = await InventarioService.GetSegundaGrilla();
            segGrilla = segGrilla.Where(s => s.VALE == controlCalidadPendientes.VALE && s.DESPACHO == controlCalidadPendientes.DESPACHO).ToList();
            SpinnerVisible = false;
        }
        protected async Task<bool> Guardar()
        {
            popupFormVisible = false;
            BotonGuardarDisabled = true;
            foreach (var item in segGrilla)
            {
                if (true)
                {
                    //aca deberia terminar
                    await ToastMensajeError("Alguno de los elementos no cumple con la tolerancia.");
                    return false;
                }
            }
            //aca deberia hacer el nuevo proceso y guardarlo en la base de datos
            /*
            Procesos toSave = new Procesos();
            toSave.VALE = controlCalidadPendientes.VALE.ToString();
            toSave.DESPACHO = controlCalidadPendientes.DESPACHO;
            toSave.FE_ENSAYO = DateTime.Now;
            toSave.CG_PROD = controlCalidadPendientes.CG_ART;
            toSave.CG_ORDEN = (int)controlCalidadPendientes.CG_ORDEN;

            //aca va todo
            var response = await InventarioService.Existe(controlCalidadPendientes.VALE);
            if (!response)
            {
                var response2 = await InventarioService.Agregar(controlCalidadPendientes);
                if (response2.Error)
                {
                    Console.WriteLine(await response2.HttpResponseMessage.Content.ReadAsStringAsync());
                    await ToastMensajeError("Error al intentar Guardar un Proceso.");
                    return false;
                }
                controlCalidadPendientes = response2.Response;
                return true;
            }
            await ToastMensajeError($"El proceso con codigo{controlCalidadPendientes.VALE} ya existe.\n\rO el proceso no es permitido.");
            return false;
            */
            BotonGuardarDisabled = false;
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
        
        public void CellSavedHandler(CellSaveArgs<vControlCalidadPendientes> args)
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
        //no se porque no funciona bien lo de abajo
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
                } else
                {
                    args.Cell.AddClass(new string[] { "rojas" });
                }
            }
            else if(args.Data.TOLE1 == 0)
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
            else if(args.Data.VALOR >= args.Data.TOLE1 && args.Data.VALOR <= args.Data.TOLE2)
            {
                args.Cell.AddClass(new string[] { "verdes" });
            }
            else
            {
                args.Cell.AddClass(new string[] { "rojas" });
            }
        }

        private async Task ToastMensajeExito(string content = "Guardado Correctamente.")
        {
            await this.ToastObj.Show(new ToastModel
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
            await ToastObj.Show(new ToastModel
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

