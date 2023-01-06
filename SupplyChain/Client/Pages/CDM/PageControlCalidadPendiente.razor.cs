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
        [Inject] public ControlCalidadService ControlCalidadService { get; set; }
        [Inject] protected HttpClient Http { get; set; }
        [Inject] protected IJSRuntime jSRuntime { get; set; }
        //[Inject] protected CargaValoresService CargaValoresService { get; set; }
        [Inject] protected InventarioService InventarioService { get; set; }
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
        protected SfGrid<Pedidos> refGrid;
        protected List<Pedidos> valor = new();
        protected SfToast ToastObj;
        protected SfSpinner refSpinner;
        //protected SfGrid<Valores> refGrid;
        //protected Valores valoresSeleccionada = new();
        protected Procesos controlesCalidad = new();
        protected vControlCalidadPendientes controlCalidadSeleccionado = new();
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
                        List<Pedidos> valoresABorrar = await refGrid.GetSelectedRecordsAsync();
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
            if (refGrid.SelectedRecords.Count == 1)
            {
                controlesCalidad = new();
                Pedidos selectedRecord = refGrid.SelectedRecords[0];
                bool isConfirmed = await jSRuntime.InvokeAsync<bool>("confirm", "Seguro que desea copiar la materia?");
                if (isConfirmed)
                {
                    //valoresSeleccionada.ESNUEVO = true;
                    controlesCalidad.FE_REG = selectedRecord.FE_REG;
                    //valoresSeleccionada.CANTMEDIDA = selectedRecord.CANTMEDIDA;
                    controlesCalidad.DESPACHO = selectedRecord.DESPACHO;
                    //valoresSeleccionada.CG_PROD = selectedRecord.CG_PROD;
                    //ProcalMP
                    //valoresSeleccionada.DESCAL = selectedRecord.DESCAL;
                    //Procalmp
                    //valoresSeleccionada.UNIDADM = selectedRecord.UNIDADM;
                    //valoresSeleccionada.CANTMEDIDA = selectedRecord.CANTMEDIDA;
                    //valoresSeleccionada.OBSERV = selectedRecord.OBSERV;
                    //Procalm
                    controlesCalidad.AVISO = selectedRecord.AVISO;
                    //valoresSeleccionada.CG_PROVE = selectedRecord.CG_PROVE;
                    controlesCalidad.REMITO = selectedRecord.REMITO;
                    //valoresSeleccionada.VALORNC = selectedRecord.VALORNC;
                    //valoresSeleccionada.LEYENDANC = selectedRecord.LEYENDANC;
                    //valoresSeleccionada.O_COMPRA = selectedRecord.O_COMPRA;
                    controlesCalidad.UNID = selectedRecord.UNID;
                    //valoresSeleccionada.EVENTO = selectedRecord.EVENTO;
                    //valoresSeleccionada.ENSAYOS = selectedRecord.ENSAYOS;
                    //valoresSeleccionada.FECHA = selectedRecord.FECHA;
                    //valoresSeleccionada.APROBADO = selectedRecord.APROBADO;
                    controlesCalidad.USUARIO = selectedRecord.USUARIO;
                    //valoresSeleccionada.REGISTRO = selectedRecord.REGISTRO;

                    
                }
                popupFormVisible = true;
            }
            else
            {
                await this.ToastObj.Show(new ToastModel
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
        //COMENTADO UT
        //protected async Task OnActionBeginHandler(ActionEventArgs<vControlCalidadPendientes> args)
        //{
        //    if (args.RequestType == Syncfusion.Blazor.Grids.Action.Add ||
        //        args.RequestType == Syncfusion.Blazor.Grids.Action.BeginEdit)
        //    {
        //        args.Cancel = true;
        //        args.PreventRender = false;
        //    }

        //    if (args.RequestType == Syncfusion.Blazor.Grids.Action.Add)
        //    {
        //        SpinnerVisible = true;
        //        controlCalidadSeleccionado = new();
        //        await refControlCalidadSeleccionado.ShowAsync(0);
        //        popupFormVisible= true;
        //        SpinnerVisible = false;

        //    }
        //    if(args.RequestType== Syncfusion.Blazor.Grids.Action.BeginEdit)
        //    {
        //        SpinnerVisible = true;
        //        var response = await ControlCalidadService.GetById(args.Data.Id);
        //        if (response.Error)
        //        {
        //            await ToastMensajeError();
        //        }
        //        else
        //        {
        //            controlCalidadSeleccionado = response.Response;
        //            await refControlCalidadSeleccionado.ShowAsync(args.Data.Id);
        //        }
        //    }
        //}

        protected async Task OnActionBeginHandler(ActionEventArgs<Pedidos> args)
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
            if (args.RequestType == Syncfusion.Blazor.Grids.Action.BeginEdit)
            {
                //controlesCalidad = args.Data;
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

        protected async Task Guardar(Pedidos valorg)
        {
            //if (valorg.GUARDADO)
            //{
            //    await ToastMensajeExito();
            //    popupFormVisible = false;
            //    if (valorg.ESNUEVO)
            //    {
            //        valor.Add(valorg);
            //    }
            //    else
            //    {
            //        //actualiza los datos sin ir a BD
            //        var valoresSinModificar = valor.Where(v => v.Id == valorg.Id).FirstOrDefault();
            ////valoresSinModificar.Id = valorg.Id;
            ////valoresSinModificar.FE_ENSAYO = valorg.FE_ENSAYO;
            ////valoresSinModificar.CANTMEDIDA = valorg.CANTMEDIDA;
            ////valoresSinModificar.DESPACHO = valorg.DESPACHO;
            ////valoresSinModificar.CG_ART = valorg.CG_ART;
            ////valoresSinModificar.DESCAL = valorg.DESCAL;
            ////valoresSinModificar.UNIDADM = valorg.UNIDADM;
            ////valoresSinModificar.CANTMEDIDA = valorg.CANTMEDIDA;
            ////valoresSinModificar.OBSERV = valorg.OBSERV;
            ////valoresSinModificar.AVISO = valorg.AVISO;
            ////valoresSinModificar.OBSERV1 = valorg.OBSERV1;
            ////valoresSinModificar.CG_PROVE = valorg.CG_PROVE;
            ////valoresSinModificar.REMITO = valorg.REMITO;
            ////valoresSinModificar.VALORNC = valorg.VALORNC;
            ////valoresSinModificar.LEYENDANC = valorg.LEYENDANC;
            ////valoresSinModificar.O_COMPRA = valorg.O_COMPRA;
            ////valoresSinModificar.UNID = valorg.UNID;
            ////valoresSinModificar.EVENTO = valorg.EVENTO;
            ////valoresSinModificar.ENSAYOS = valorg.ENSAYOS;
            ////valoresSinModificar.FECHA = valorg.FECHA;
            ////valoresSinModificar.APROBADO = valorg.APROBADO;
            ////valoresSinModificar.USUARIO = valorg.USUARIO;
            ////valoresSinModificar.REGISTRO = valorg.REGISTRO;



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

            //valor.OrderByDescending(v => v.Id);
            //    }
            //    await refGrid.RefreshHeaderAsync();
            //    refGrid.Refresh();
            //    await refGrid.RefreshColumnsAsync();
            //}
            //else
            //{
            //    await ToastMensajeError();
            //}
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
