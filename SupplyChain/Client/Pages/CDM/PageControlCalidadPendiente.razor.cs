using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.JSInterop;
using SupplyChain;
using SupplyChain.Client.HelperService;
using SupplyChain.Client.Shared;
using SupplyChain.Shared;
using SupplyChain.Shared.Enum;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Kanban.Internal;
using Syncfusion.Blazor.LinearGauge.Internal;
using Syncfusion.Blazor.Navigations;
using Syncfusion.Blazor.Notifications;
using Syncfusion.Blazor.Spinner;
using Syncfusion.XlsIO.FormatParser.FormatTokens;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace SupplyChain.Client.Pages.CDM
{
    public class PageControlCalidadPendientesBase : ComponentBase
    {
        //[Inject] public ControlCalidadService ControlCalidadService { get; set; }
        [Inject] public InventarioService InventarioService{ get; set; }
        [Inject] public ControlCalidadService ControlCalidadService { get; set; }

        [Inject] protected HttpClient Http { get; set; }
        [Inject] protected IJSRuntime jSRuntime { get; set; }
        //[Inject] protected CargaValoresService CargaValoresService { get; set; }
        #region "Vista Grilla"
        protected const string APPNAME = "grdCargaValores";
        protected string state;
        #endregion
        protected List<Object> Toolbaritems = new List<Object>()
        {
               "Search", "Add", "Edit", "Delete", "Print",
            new ItemModel {Text = "Copia", TooltipText="Copiar un Proceso", PrefixIcon="e-copy", Id="Copy"},
            "ExcelExport"
        };

        //protected List<Valores> valor = new();
        //protected List<vControlCalidadPendientes> ControlCalPendientes = new();
        //protected SfGrid<vControlCalidadPendientes> refGrid;
        protected SfGrid<vControlCalidadPendientes> refGrid;
        protected List<vControlCalidadPendientes> valor = new();
        protected SfToast ToastObj;
        protected SfSpinner refSpinner;
        //protected SfGrid<Valores> refGrid;
        //protected Valores valoresSeleccionada = new();
        protected List<vControlCalidadPendientes> controlesCalidad = new();
        protected List<vControlCalidadPendientes> controlCalidadSeleccionado = new();
        protected FormControlCalidadPendiente refControlCalidadSeleccionado = new();
        protected FormControlCalidadPendiente controlCalidadPendientes = new();
        protected bool SpinnerVisible = false;
        protected bool popupFormVisible = false;
        [CascadingParameter] MainLayout MainLayout { get; set; }
        protected override async Task OnInitializedAsync()
        {
            MainLayout.Titulo = "Control de Calidad";

            SpinnerVisible = true;
            //CAMBIAR A INVENTARIOSERVICE
            valor = await InventarioService.GetPendienteAprobacion();
            //if (!response.Error)
            //{
            //    pedidos = response.Response;
            //}
            SpinnerVisible = false;
        }
        
        protected async Task OnVistaSeleccionada(VistasGrillas vistasGrillas)
        {
            await refGrid.SetPersistDataAsync(vistasGrillas.Layout);
        }
        
        protected async Task OnReiniciarGrilla()
        {
            await refGrid.ResetPersistData();
        }

        protected async Task OnToolHandler(ClickEventArgs args)
        {
            if (args.Item.Id == "Copy")
            {
                await CopiarCargaValores();
            }
            else if (args.Item.Id == "grdCargaValores_delete")
            {
                if ((await refGrid.GetSelectedRecordsAsync()).Count > 0)
                {
                    bool isConfirmed = await jSRuntime.InvokeAsync<bool>(
                        "confirm",
                        "Seguro que deseaa eliminar la linea?");
                    if (isConfirmed)
                    {
                        List<vControlCalidadPendientes> valoresABorrar = await refGrid.GetSelectedRecordsAsync();
                        //var response = InventarioService.Eliminar(valoresABorrar);
                        //if (!response.IsCompletedSuccessfully)
                        //{
                        //    await this.ToastObj.Show(new ToastModel
                        //    {
                        //        Title = "EXITO!",
                        //        Content = "los procesos seleccionados fueron eliminados correctamente.",
                        //        CssClass = "e-toast-success",
                        //        Icon = "e-success toast-icons",
                        //        ShowCloseButton = true,
                        //        ShowProgressBar = true,
                        //    });
                        //}
                        //else
                        //{
                        //    await ToastMensajeError();
                        //}
                    }
                }
            }
            else if (args.Item.Id == "grdCargaValores_excelexport")
            {
                await refGrid.ExportToExcelAsync();
            }
        }

        private async Task CopiarCargaValores()
        {
            //if (refGrid.SelectedRecords.Count == 1)
            //{
            //    controlesCalidad = new();
            //    vControlCalidadPendientes selectedRecord = refGrid.SelectedRecords[0];
            //    bool isConfirmed = await jSRuntime.InvokeAsync<bool>("confirm", "Seguro que desea copiar la materia?");
            //    if (isConfirmed)
            //    {
            //        controlCalidadSeleccionado.VALE = selectedRecord.VALE;
            //        controlCalidadSeleccionado.REGISTRO = selectedRecord.REGISTRO;
            //        controlCalidadSeleccionado.DESPACHO = selectedRecord.DESPACHO;
            //        controlCalidadSeleccionado.CG_PROD = selectedRecord.CG_PROD;
            //        controlCalidadSeleccionado.CG_DEP = selectedRecord.CG_DEP;
            //        controlCalidadSeleccionado.CG_LINEA = selectedRecord.CG_LINEA;
            //        controlCalidadSeleccionado.DESCAL = selectedRecord.DESCAL;
            //        controlCalidadSeleccionado.CARCAL = selectedRecord.CARCAL;
            //        controlCalidadSeleccionado.UNIDADM = selectedRecord.UNIDADM;
            //        controlCalidadSeleccionado.TOLE1 = selectedRecord.TOLE1;
            //        controlCalidadSeleccionado.TOLE2 = selectedRecord.TOLE2;
            //        controlCalidadSeleccionado.AVISO = selectedRecord.AVISO;
            //        controlCalidadSeleccionado.CG_PROVE = selectedRecord.CG_PROVE;
            //        controlCalidadSeleccionado.REMITO = selectedRecord.REMITO;
            //        controlCalidadSeleccionado.OCOMPRA = selectedRecord.OCOMPRA;


            //    }
            //    popupFormVisible = true;
            //}
            //else
            //{
            //    await this.ToastObj.Show(new ToastModel
            //    {
            //        Title = "ERROR!",
            //        Content = "Solo se puede copiar un item",
            //        CssClass = "e-toast-danger",
            //        Icon = "e-error toast-icons",

            //        ShowCloseButton = true,
            //        ShowProgressBar = true
            //    });
            //}
        }
     

        protected async Task OnActionBeginHandler(ActionEventArgs<vControlCalidadPendientes> args)
        {
            if (args.RequestType == Syncfusion.Blazor.Grids.Action.Add ||
               args.RequestType == Syncfusion.Blazor.Grids.Action.BeginEdit)
            {
                args.Cancel = true;
                args.PreventRender = false;
                popupFormVisible = true;
                controlesCalidad = new();
                ////valoresSeleccionada.ESNUEVO = true;
            }
            if (args.RequestType == Syncfusion.Blazor.Grids.Action.Add)
            {
                var responsecontrolcalidad = await ControlCalidadService.GetControlCalidad(args.Data.REGISTRO);
                if (responsecontrolcalidad.Error)
                {

                }
                else
                {
                    controlCalidadSeleccionado = new();
                    controlCalidadSeleccionado = responsecontrolcalidad.Response;
                }
                
                //valoresSeleccionada.ESNUEVO = false;
            }
            if (args.RequestType == Syncfusion.Blazor.Grids.Action.Grouping
                || args.RequestType == Syncfusion.Blazor.Grids.Action.UnGrouping
                || args.RequestType == Syncfusion.Blazor.Grids.Action.ClearFiltering
                || args.RequestType == Syncfusion.Blazor.Grids.Action.CollapseAllComplete
                || args.RequestType == Syncfusion.Blazor.Grids.Action.ColumnState
                || args.RequestType == Syncfusion.Blazor.Grids.Action.ClearFiltering
                || args.RequestType == Syncfusion.Blazor.Grids.Action.Reorder
                || args.RequestType == Syncfusion.Blazor.Grids.Action.Sorting
                )
            {
                ////VisibleProperty = true;
                refGrid.PreventRender();
                refGrid.Refresh();

                state = await refGrid.GetPersistData();
                await refGrid.AutoFitColumnsAsync();
                await refGrid.RefreshColumns();
                await refGrid.RefreshHeader();
            }
        }

        protected async Task OnActionCompleteHandler(ActionEventArgs<vControlCalidadPendientes> args)
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

        protected async Task Guardar(vControlCalidadPendientes valorg)
        {
            if (valorg.GUARDADO)
            {
                await ToastMensajeExito();
                popupFormVisible = false;
                if (valorg.ESNUEVO)
                {
                    valor.Add(valorg);
                }
                else
                {
                    //actualiza los datos sin ir a BD
                    var valoresSinModificar = valor.Where(v => v.VALE == valorg.VALE).FirstOrDefault();
                    valoresSinModificar.VALE  = valorg.VALE;
                    valoresSinModificar.DESPACHO= valorg.DESPACHO;
                    valoresSinModificar.CG_PROD= valorg.CG_PROD;
                    valoresSinModificar.CG_DEP= valorg.CG_DEP;
                    valoresSinModificar.CG_LINEA= valorg.CG_LINEA;
                    valoresSinModificar.DESCAL= valorg.DESCAL;
                    valoresSinModificar.CARCAL = valorg.CARCAL;
                    valoresSinModificar.UNIDADM= valorg.UNIDADM;
                    valoresSinModificar.TOLE1=valorg.TOLE1;
                    valoresSinModificar.TOLE2=valorg.TOLE2;
                    valoresSinModificar.AVISO=valorg.AVISO;
                    valoresSinModificar.CG_PROVE= valorg.CG_PROVE;
                    valoresSinModificar.REMITO= valorg.REMITO;
                    valoresSinModificar.OCOMPRA=valorg.OCOMPRA;

                    valor.OrderByDescending(v => v.VALE);
                }
                await refGrid.RefreshHeaderAsync();
                refGrid.Refresh();
                await refGrid.RefreshColumnsAsync();
            }
            else
            {
                await ToastMensajeError();
            }


            //valoresSinModificar.VALE = valorg.VALE;
            //valoresSinModificar.FE_MOV = valorg.FE_MOV;
            //valoresSinModificar.VOUCHER = valorg.VOUCHER;
            //valoresSinModificar.COMPROB = valorg.COMPROB;
            //valoresSinModificar.FACTURA = valorg.FACTURA;
            //valoresSinModificar.LEYENDA = valorg.LEYENDA;
            //valoresSinModificar.REMITO = valorg.REMITO;
            //valoresSinModificar.TIPO = valorg.TIPO;
            //valoresSinModificar.PEDIDO = valorg.PEDIDO;
            //valoresSinModificar.NUMOCI = valorg.NUMOCI;
            //valoresSinModificar.OCOMPRA = valorg.OCOMPRA;
            //valoresSinModificar.CG_ORDF = valorg.CG_ORDF;
            //valoresSinModificar.OBSERVACIONES = valorg.OBSERVACIONES;
            //valoresSinModificar.OBSERITEM = valorg.OBSERITEM;
            //valoresSinModificar.OBS1 = valorg.OBS1;
            //valoresSinModificar.OBS2 = valorg.OBS2;
            //valoresSinModificar.OBS3 = valorg.OBS3;
            //valoresSinModificar.OBS4 = valorg.OBS4;
            //valoresSinModificar.AVISO = valorg.AVISO;
            //valoresSinModificar.DIRENT = valorg.DIRENT;
            //valoresSinModificar.CG_ORDEN = valorg.CG_ORDEN;
            //valoresSinModificar.CG_ART = valorg.CG_ART;
            //valoresSinModificar.DES_ART = valorg.DES_ART;
            //valoresSinModificar.TIPO = valorg.TIPO;
            //valoresSinModificar.LOTE = valorg.LOTE;
            //valoresSinModificar.SERIE = valorg.SERIE;
            //valoresSinModificar.DESPACHO = valorg.DESPACHO;
            //valoresSinModificar.UBICACION = valorg.UBICACION;
            //valoresSinModificar.CG_DEP = valorg.CG_DEP;
            //valoresSinModificar.CANTENT = valorg.CANTENT;
            //valoresSinModificar.STOCK = valorg.STOCK;
            //valoresSinModificar.UNID = valorg.UNID;
            //valoresSinModificar.CG_DEN = valorg.CG_DEN;
            //valoresSinModificar.STOCKA = valorg.STOCKA;
            //valoresSinModificar.UNIDA = valorg.UNIDA;
            //valoresSinModificar.CANTENTA = valorg.CANTENTA;
            //valoresSinModificar.ENTRREAL = valorg.ENTRREAL;
            //valoresSinModificar.MONEDA = valorg.MONEDA;
            //valoresSinModificar.IMPORTE1 = valorg.IMPORTE1;
            //valoresSinModificar.IMPORTE2 = valorg.IMPORTE2;
            //valoresSinModificar.IMPORTE3 = valorg.IMPORTE3;
            //valoresSinModificar.IMPORTE4 = valorg.IMPORTE4;
            //valoresSinModificar.IMPORTE6 = valorg.IMPORTE6;
            //valoresSinModificar.DESCUENTO = valorg.DESCUENTO;
            //valoresSinModificar.BONIFIC = valorg.BONIFIC;
            //valoresSinModificar.IVA = valorg.IVA;
            //valoresSinModificar.VA_INDIC = valorg.VA_INDIC;
            //valoresSinModificar.CG_CUENT = valorg.CG_CUENT;
            //valoresSinModificar.CUIT = valorg.CUIT;
            //valoresSinModificar.CG_PROVE = valorg.CG_PROVE;
            //valoresSinModificar.CG_CLI = valorg.CG_CLI;
            //valoresSinModificar.DIRECC = valorg.DIRECC;
            //valoresSinModificar.LOCALIDAD = valorg.LOCALIDAD;
            //valoresSinModificar.CG_POSTA = valorg.CG_POSTA;
            //valoresSinModificar.CANTPED = valorg.CANTPED;
            //valoresSinModificar.USUARIO = valorg.USUARIO;
            //valoresSinModificar.FE_REG = valorg.FE_REG;
            //valoresSinModificar.CG_CIA = valorg.CG_CIA;
            //valoresSinModificar.CG_COND_ENTREGA = valorg.CG_COND_ENTREGA;
            //valoresSinModificar.CG_CONDICION_PAGO = valorg.CG_CONDICION_PAGO;
            //valoresSinModificar.CG_TRANS = valorg.CG_TRANS;
            //valoresSinModificar.Control1 = valorg.Control1;
            //valoresSinModificar.Control2 = valorg.Control2;
            //valoresSinModificar.Control3 = valorg.Control3;
            //valoresSinModificar.Control4 = valorg.Control4;
            //valoresSinModificar.Control5 = valorg.Control5;
            //valoresSinModificar.Control6 = valorg.Control6;


        }

                private async Task ToastMensajeExito()
        {
            await this.ToastObj.Show(new ToastModel
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
    }
}
