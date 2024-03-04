using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SupplyChain.Client.HelperService;
using SupplyChain.Client.Shared;
using SupplyChain.Shared;
using SupplyChain.Shared.Models;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Navigations;
using Syncfusion.Blazor.Notifications;
using Syncfusion.Blazor.Spinner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SupplyChain.Client.Pages.ABM.Prods
{
    public class ProductosPageBase : ComponentBase
    {
        [Inject] protected HttpClient Http { get; set; }
        [Inject] protected IJSRuntime jSRuntime { get; set; }
        [Inject] protected ProductoService ProductoService { get; set; }
        #region "Vista Grilla"
        protected const string APPNAME = "grdProdABM";
        protected string state;
        #endregion
        protected List<Object> Toolbaritems = new List<Object>(){
        "Search",
        "Add",
        "Edit",
        "Delete",
        "Print",
        new ItemModel { Text = "Copia", TooltipText = "Copiar un producto", PrefixIcon = "e-copy", Id = "Copy" },
        "ExcelExport"
        };
        protected List<Producto> Productos  = new();
        
        protected SfToast ToastObj;
        protected SfSpinner refSpinner;
        protected SfGrid<Producto> refGrid;
        protected Producto productoSeleccionado = new();
        protected FormProducto refFormProducto;
        protected bool SpinnerVisible = false;

        protected bool popupFormVisible = false;

        [CascadingParameter] MainLayout MainLayout { get; set; }
        protected override async Task OnInitializedAsync()
        {
            MainLayout.Titulo = "Productos";

            SpinnerVisible = true;
            var response = await ProductoService.Get();
            if (!response.Error)
            {
                Productos = response.Response;
            }
            //Productos = (await Http.GetFromJsonAsync<List<Producto>>("api/Prod")).OrderBy(s => s.CG_ORDEN).ToList();
            SpinnerVisible = false;
        }

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

        protected async Task OnToolbarHandler(ClickEventArgs args)
        {
            if (args.Item.Id == "Copy")
            {
                await CopiarProducto();
            }
            else if (args.Item.Id == "grdProd_excelexport")
            {
                await refGrid.ExportToExcelAsync();
            }
            else if (args.Item.Id == "grdProd_delete")
            {
                if ((await refGrid.GetSelectedRecordsAsync()).Count > 0)
                {
                    bool isConfirmed = await jSRuntime.InvokeAsync<bool>("confirm", "Seguro de que desea eliminar los productos seleccionados?");
                    if (isConfirmed)
                    {
                        List<Producto> productosABorrar = await refGrid.GetSelectedRecordsAsync();
                        var response = ProductoService.Eliminar(productosABorrar);
                        if (!response.IsCompletedSuccessfully)
                        {
                            await this.ToastObj.ShowAsync(new ToastModel
                            {
                                Title = "EXITO!",
                                Content = "los productos seleccionados fueron eliminados correctamente.",
                                CssClass = "e-toast-success",
                                Icon = "e-success toast-icons",
                                ShowCloseButton = true,
                                ShowProgressBar = true
                            });
                        }
                        else
                        {
                            await ToastMensajeError();
                        }
                    }
                }
            }
        }

        private async Task CopiarProducto()
        {
            if (refGrid.SelectedRecords.Count == 1)
            {
                productoSeleccionado = new();
                Producto selectedRecord = refGrid.SelectedRecords[0];
                bool isConfirmed = await jSRuntime.InvokeAsync<bool>("confirm", "Seguro de que desea copiar el producto?");
                if (isConfirmed)
                {
                    productoSeleccionado.ESNUEVO = true;
                    productoSeleccionado.DES_PROD = selectedRecord.DES_PROD;
                    productoSeleccionado.CAMPOCOM1 = selectedRecord.CAMPOCOM1;
                    productoSeleccionado.CAMPOCOM2 = selectedRecord.CAMPOCOM2;
                    productoSeleccionado.CAMPOCOM3 = selectedRecord.CAMPOCOM3;
                    productoSeleccionado.CAMPOCOM4 = selectedRecord.CAMPOCOM4;
                    productoSeleccionado.CAMPOCOM5 = selectedRecord.CAMPOCOM5;
                    productoSeleccionado.CAMPOCOM6 = selectedRecord.CAMPOCOM6;
                    productoSeleccionado.CAMPOCOM7 = selectedRecord.CAMPOCOM7;
                    productoSeleccionado.CAMPOCOM8 = selectedRecord.CAMPOCOM8;
                    productoSeleccionado.CAMPOCOM9 = selectedRecord.CAMPOCOM9;
                    productoSeleccionado.CAMPOCOM10 = selectedRecord.CAMPOCOM10;
                    productoSeleccionado.CG_ORDEN = selectedRecord.CG_ORDEN;
                    productoSeleccionado.TIPO = selectedRecord.TIPO;
                    productoSeleccionado.CG_CLAS = selectedRecord.CG_CLAS;
                    productoSeleccionado.UNID = selectedRecord.UNID;
                    productoSeleccionado.CG_DENSEG = selectedRecord.CG_DENSEG;
                    productoSeleccionado.UNIDSEG = selectedRecord.UNIDSEG;
                    productoSeleccionado.PESO = selectedRecord.PESO;
                    productoSeleccionado.UNIDPESO = selectedRecord.UNIDPESO;
                    productoSeleccionado.NORMA = selectedRecord.NORMA;
                    productoSeleccionado.EXIGEDESPACHO = selectedRecord.EXIGEDESPACHO;
                    productoSeleccionado.EXIGELOTE = selectedRecord.EXIGELOTE;
                    productoSeleccionado.EXIGESERIE = selectedRecord.EXIGESERIE;
                    productoSeleccionado.EXIGEOA = selectedRecord.EXIGEOA;
                    productoSeleccionado.STOCKMIN = selectedRecord.STOCKMIN;
                    productoSeleccionado.ESPECIF = selectedRecord.ESPECIF;
                    productoSeleccionado.LOPTIMO = selectedRecord.LOPTIMO;
                    productoSeleccionado.TIEMPOFAB = selectedRecord.TIEMPOFAB;
                    productoSeleccionado.COSTO = selectedRecord.COSTO;
                    productoSeleccionado.COSTOTER = selectedRecord.COSTOTER;
                    productoSeleccionado.MONEDA = selectedRecord.MONEDA;
                    productoSeleccionado.FE_UC = selectedRecord.FE_UC;
                    productoSeleccionado.CG_CELDA = selectedRecord.CG_CELDA;
                    productoSeleccionado.CG_AREA = selectedRecord.CG_AREA;
                    productoSeleccionado.CG_LINEA = selectedRecord.CG_LINEA;
                    productoSeleccionado.CG_TIPOAREA = selectedRecord.CG_TIPOAREA;

                    popupFormVisible = true;
                }

            }
            else
            {
                await this.ToastObj.ShowAsync(new ToastModel
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

        protected async Task OnActionBeginHandler(ActionEventArgs<Producto> args)
        {
            if (args.RequestType == Syncfusion.Blazor.Grids.Action.Add ||
                args.RequestType == Syncfusion.Blazor.Grids.Action.BeginEdit)
            {
                args.Cancel = true;
                args.PreventRender = false;
                popupFormVisible = true;
                productoSeleccionado = new();
                productoSeleccionado.ESNUEVO = true;
            }



            if (args.RequestType == Syncfusion.Blazor.Grids.Action.BeginEdit)
            {
                productoSeleccionado = args.Data;
                productoSeleccionado.ESNUEVO = false;
                await refFormProducto.Refrescar(productoSeleccionado);
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
                //VisibleProperty = true;
                refGrid.PreventRender();
                refGrid.Refresh();

                state = await refGrid.GetPersistDataAsync();
                await refGrid.AutoFitColumnsAsync();
                await refGrid.RefreshColumnsAsync();
                await refGrid.RefreshHeaderAsync();
            }
        }

        protected async Task OnActionCompleteHandler(ActionEventArgs<Producto> args)
        {
            if (args.RequestType == Syncfusion.Blazor.Grids.Action.BeginEdit)
            {
                args.Cancel = true;
                args.PreventRender = false;
                popupFormVisible = true;
            }
        }

        protected void OnCerraDialog()
        {
            popupFormVisible = false;
        }

        protected async Task Guardar(Producto producto)
        {
            if (producto.GUARDADO)
            {
                await ToastMensajeExito();
                popupFormVisible = false;
                if (producto.ESNUEVO)
                {
                    Productos.Add(producto);
                }
                else
                {
                    //actualizar datos sin ir a la base de datos
                    var prodSinModificar = Productos.Where(p => p.Id == producto.Id).FirstOrDefault();
                    prodSinModificar.Id = producto.Id;
                    prodSinModificar.DES_PROD = producto.DES_PROD;
                    prodSinModificar.CAMPOCOM1 = producto.CAMPOCOM1;
                    prodSinModificar.CAMPOCOM2 = producto.CAMPOCOM2;
                    prodSinModificar.CAMPOCOM3 = producto.CAMPOCOM3;
                    prodSinModificar.CAMPOCOM4 = producto.CAMPOCOM4;
                    prodSinModificar.CAMPOCOM5 = producto.CAMPOCOM5;
                    prodSinModificar.CAMPOCOM6 = producto.CAMPOCOM6;
                    prodSinModificar.CAMPOCOM7 = producto.CAMPOCOM7;
                    prodSinModificar.CAMPOCOM8 = producto.CAMPOCOM8;
                    prodSinModificar.CAMPOCOM9 = producto.CAMPOCOM9;
                    prodSinModificar.CAMPOCOM10 = producto.CAMPOCOM10;
                    prodSinModificar.CG_ORDEN = producto.CG_ORDEN;
                    prodSinModificar.TIPO = producto.TIPO;
                    prodSinModificar.CG_CLAS = producto.CG_CLAS;
                    prodSinModificar.UNID = producto.UNID;
                    prodSinModificar.CG_DENSEG = producto.CG_DENSEG;
                    prodSinModificar.UNIDSEG = producto.UNIDSEG;
                    prodSinModificar.PESO = producto.PESO;
                    prodSinModificar.UNIDPESO = producto.UNIDPESO;
                    prodSinModificar.NORMA = producto.NORMA;
                    prodSinModificar.EXIGEDESPACHO = producto.EXIGEDESPACHO;
                    prodSinModificar.EXIGELOTE = producto.EXIGELOTE;
                    prodSinModificar.EXIGESERIE = producto.EXIGESERIE;
                    prodSinModificar.EXIGEOA = producto.EXIGEOA;
                    prodSinModificar.STOCKMIN = producto.STOCKMIN;
                    prodSinModificar.ESPECIF = producto.ESPECIF;
                    prodSinModificar.LOPTIMO = producto.LOPTIMO;
                    prodSinModificar.TIEMPOFAB = producto.TIEMPOFAB;
                    prodSinModificar.COSTO = producto.COSTO;
                    prodSinModificar.COSTOTER = producto.COSTOTER;
                    prodSinModificar.MONEDA = producto.MONEDA;
                    prodSinModificar.FE_UC = producto.FE_UC;
                    prodSinModificar.CG_CELDA = producto.CG_CELDA;
                    prodSinModificar.CG_AREA = producto.CG_AREA;
                    prodSinModificar.CG_LINEA = producto.CG_LINEA;
                    prodSinModificar.CG_TIPOAREA = producto.CG_TIPOAREA;
                    prodSinModificar.FE_UC = producto.FE_UC;
                    Productos.OrderByDescending(p => p.Id);

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
            await this.ToastObj.ShowAsync(new ToastModel
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
            await ToastObj.ShowAsync(new ToastModel
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
