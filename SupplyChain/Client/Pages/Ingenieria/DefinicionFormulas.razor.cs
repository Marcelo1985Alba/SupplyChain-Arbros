using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Identity;
using SupplyChain.Client.HelperService;
using SupplyChain.Client.Shared;
using SupplyChain.Shared;
using SupplyChain.Shared.Models;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Navigations;
using Syncfusion.Blazor.Notifications;
using Syncfusion.Blazor.Popups;
using Syncfusion.Blazor.RichTextEditor;
using Syncfusion.Blazor.Spinner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using SupplyChain.Client.RepositoryHttp;
using SupplyChain.Shared.PCP;
using Syncfusion.Blazor.Popups;

namespace SupplyChain.Client.Pages.Ingenieria
{
    public class BaseDefinicionFormulas : ComponentBase
    {
        [Inject] public HttpClient Http { get; set; }
        [Inject] public ExcelService ExcelService { get; set; }
        [Inject] protected IJSRuntime JsRuntime { get; set; }
        [Inject] public SfDialogService DialogService { get; set; }
        [CascadingParameter] public MainLayout MainLayout { get; set; }
        [Parameter] public string extern_codigo { get; set; }
        [CascadingParameter] public Task<AuthenticationState> authenticationState { get; set; }
        protected SfTab refSfTab;
        protected SfSpinner refSpinner;
        protected bool VisibleSpinner = false;
        protected bool edited = false;
        protected bool deleted = false;
        protected bool added = false;

        protected SfDialog EditFormulas { get; set; }
        protected Boolean dialogVisibleCopiar = false;
        protected Boolean dialogVisibleEditar = false;
        protected decimal valorDolar = 0;
        protected vIngenieriaProductosFormulas Selected { get; set; }

        protected SfToast ToastObj;

        protected List<Object> Toolbaritems = new()
        {
            "ExcelExport",
            new ItemModel() { Type = ItemType.Separator },
            new ItemModel()
            {
                Text = "Guardar Excel", Type = ItemType.Button, CssClass = "", ShowTextOn = DisplayMode.Both,
                TooltipText = "Guardar el despiece en formato excel", PrefixIcon = "e-update e-icons",
                Id = "GuardarExcel"
            }
        };

        protected bool PopupBuscadorProdVisible = false;

        protected List<Object> ToolbaritemsEditarFormula = new()
        {
            new ItemModel()
            {
                Text = "Agregar Insumo", Type = ItemType.Button, CssClass = "", ShowTextOn = DisplayMode.Both,
                TooltipText = "Buscar insumos", PrefixIcon = "e-search e-icons", Id = "BuscarInsumo"
            },
            new ItemModel()
            {
                Text = "Guardar", Type = ItemType.Button, CssClass = "", ShowTextOn = DisplayMode.Both,
                TooltipText = "Guardar el despiece en la base de datos", PrefixIcon = "e-update e-icons",
                Id = "GuardarDB"
            },
            "Delete"
        };

        protected List<Producto> Insumos = new List<Producto>();
        protected Producto insumoSeleccionado = new();

        protected SfDialog DialogDespieceRef;
        protected SfGrid<DespiecePlanificacion> GridDespiece;
        protected SfGrid<vIngenieriaProductosFormulas> GridProdForm;
        protected SfGrid<vIngenieriaProductosFormulas> DetailedGridProdForm;
        protected SfGrid<Formula> EditForm;
        protected List<DespiecePlanificacion> listaDespiece = new();

        protected List<vIngenieriaProductosFormulas> DataOrdeProductosFormulas { get; set; } =
            new List<vIngenieriaProductosFormulas>();

        protected List<vIngenieriaProductosFormulas> DataDetailedProductosFormulas { get; set; } =
            new List<vIngenieriaProductosFormulas>();

        protected List<Formula> DataEditarProductosFormulas { get; set; } = new List<Formula>();
        protected vIngenieriaProductosFormulas ProdSelected = new();

        public class TabData
        {
            public string Header { get; set; }
            public string CodigoInsumo { get; set; }
            public RenderFragment Content { get; set; }
        }

        protected List<TabData> TabItems = new();

        protected string tituloTabFormulas = string.Empty;
        protected bool mostrarCerrarTab = false;

        protected AuthenticationState authState;
        
        protected async override Task OnInitializedAsync()
        {
            MainLayout.Titulo = "Consulta de Fórmulas";
            VisibleSpinner = true;
            authState = await authenticationState;
            DataOrdeProductosFormulas =
                await Http.GetFromJsonAsync<List<vIngenieriaProductosFormulas>>("api/Ingenieria/GetProductoFormulas");
            if (extern_codigo != null)
            {
                ProdSelected = DataOrdeProductosFormulas.FirstOrDefault(s => s.CG_PROD.Trim() == extern_codigo.Trim());
                listaDespiece =
                    await Http.GetFromJsonAsync<List<DespiecePlanificacion>>(
                        $"api/Planificacion/Despiece/{extern_codigo.Trim()}/1/1");
                await DialogDespieceRef.Show();
            }

            VisibleSpinner = false;
        }

        protected async Task CommandClickHandler(CommandClickEventArgs<vIngenieriaProductosFormulas> args)
        {
            if (args.CommandColumn.Title == "Editar")
            {
                if(!args.RowData.TIENE_FORM)
                {
                    await ToastMensajeError($"{args.RowData.CG_PROD.Trim()} no tiene una formula.");
                    return;
                }
                Selected = args.RowData;
                DataEditarProductosFormulas = await Http.GetFromJsonAsync<List<Formula>>("api/Formulas/BuscarPorCgProdMaxRevision/" +
                    $"{args.RowData.CG_PROD.Trim()}");
                edited = false;
                deleted = false;
                dialogVisibleEditar = true;
            }
            else if (args.CommandColumn.Title == "Copiar")
            {
                if (args.RowData.TIENE_FORM)
                {
                    await ToastMensajeError($"{args.RowData.CG_PROD.Trim()} ya tiene una formula.");
                    return;
                }
                Selected = args.RowData;
                DataDetailedProductosFormulas =
                    (await Http.GetFromJsonAsync<List<vIngenieriaProductosFormulas>>(
                        "api/Ingenieria/GetProductoFormulas")).Where(s => s.TIENE_FORM && s.FORM_ACTIVA).ToList();
                dialogVisibleCopiar = true;
            }
            else if (args.CommandColumn.Title == "Despiece")
            {
                VisibleSpinner = true;
                ProdSelected = args.RowData;
                listaDespiece = await Http.GetFromJsonAsync<List<DespiecePlanificacion>>("api/Planificacion/Despiece/" +
                    $"{args.RowData.CG_PROD.Trim()}/1/1");


                VisibleSpinner = false;
                await DialogDespieceRef.Show();
            }
            else if (args.CommandColumn.Title == "Ver Plano")
            {
                VisibleSpinner = true;
                tituloTabFormulas = "Formulas";
                mostrarCerrarTab = true;
                TabItems.Add(new TabData
                {
                    Header = $"Plano ({args.RowData.CG_PROD.Trim()})",
                    //Verificar si es un semi o un prod, los semis obtener los primeros 7 digitos
                    CodigoInsumo = new string(args.RowData.CG_PROD.Take(2).ToArray()) == "C-"
                                   || new string(args.RowData.CG_PROD.Take(2).ToArray()) == "D-"
                        ? args.RowData.CG_PROD.Substring(0, 7)
                        : args.RowData.CG_PROD
                });


                //El primer tab no se encuentra dentro de la lista
                //le resto el que agregue para que no lo desactive
                //for (int i = 0; i < TabItems.Count + 1; i++)
                //{
                //    await refSfTab.EnableTab(i, false);
                //}


                //await refSfTab.EnableTab(TabItems.Count, true);
                //await refSfTab.Select(TabItems.Count);
                VisibleSpinner = false;
            }
            else if (args.CommandColumn.Title == "Ver Programas")
            {
                VisibleSpinner = true;
                tituloTabFormulas = "Formulas";
                mostrarCerrarTab = true;
                TabItems.Add(new TabData
                {
                    Header = $"Programas ({args.RowData.CG_PROD.Trim()})",
                    CodigoInsumo = args.RowData.CG_PROD.Trim()
                });


                //El primer tab no se encuentra dentro de la lista
                //le resto el que agregue para que no lo desactive
                //for (int i = 0; i < TabItems.Count + 1; i++)
                //{
                //    await refSfTab.EnableTab(i, false);
                //}


                //await refSfTab.EnableTab(TabItems.Count, true);
                //await refSfTab.Select(TabItems.Count);
                VisibleSpinner = false;
            }
        }
        
        protected async Task OnCommandClicked(CommandClickEventArgs<Formula> args)
        {
            if (args.CommandColumn.Type == CommandButtonType.Delete)
            {
                var form = DataEditarProductosFormulas.FirstOrDefault(f => f.Id == args.RowData.Id);
                if (form != null)
                {
                    DataEditarProductosFormulas.Remove(form);
                }
                await EditForm.Refresh();
            }
            else if (args.CommandColumn.Type == CommandButtonType.Save)
            {
                DataEditarProductosFormulas.First(f => f.Id == args.RowData.Id).CANT_MAT = args.RowData.CANT_MAT;
                DataEditarProductosFormulas.First(f => f.Id == args.RowData.Id).OBSERV = args.RowData.OBSERV;
                await EditForm.Refresh();
            }
        }

        protected async Task ToolbarProdFormClickHandler(ClickEventArgs args)
        {
            if (args.Item.Id == "ProdForm_excelexport") //Id is combination of Grid's ID and itemname
            {
                VisibleSpinner = true;
                await this.GridProdForm.ExcelExport();
                VisibleSpinner = false;
            }
        }

        protected async Task ToolbarEditFormClickHandler(ClickEventArgs args)
        {
            if (args.Item.Id == "BuscarInsumo")
            {
                VisibleSpinner = true;
                await BuscarInsumo();
                VisibleSpinner = false;
            }
            if(args.Item.Id == "GuardarDB")
            {
                VisibleSpinner = true;
                await GuardarDB();
                VisibleSpinner = false;
            }
            if(args.Item.Text == "Delete")
            {
                deleted = true;
                var form = DataEditarProductosFormulas.FirstOrDefault(f => f.Id == EditForm.SelectedRecords.First().Id);
                if (form != null)
                {
                    DataEditarProductosFormulas.Remove(form);
                }
            }
        }

        protected async Task BuscarInsumo()
        {
            Insumos = await Http.GetFromJsonAsync<List<Producto>>($"api/Prod/ByTipo/{true}/{true}/{true}");
            PopupBuscadorProdVisible = true;
        }

        protected async Task OnInsumoSeleccionado(Producto insumoSeleccionado)
        {
            PopupBuscadorProdVisible = false;
            if (insumoSeleccionado != null)
            {
                var nuevoInsumoFormula = new Formula()
                {
                    ACTIVO = "S",
                    CG_ORDEN = insumoSeleccionado.CG_ORDEN,
                    Cg_Mat = insumoSeleccionado.CG_ORDEN == 4 ? insumoSeleccionado.Id : string.Empty,
                    Cg_Se = insumoSeleccionado.CG_ORDEN == 3 ? insumoSeleccionado.Id : string.Empty,
                    CANTIDAD = 0M,
                    Cg_Prod = DataEditarProductosFormulas[0].Cg_Prod,
                    DES_PROD = insumoSeleccionado.DES_PROD,
                    REVISION = DataEditarProductosFormulas[0].REVISION,
                    USUARIO = authState.User.Identity != null ? authState.User.Identity.Name : "USER",
                    FE_FORM = DateTime.Now,
                    CG_FORM = DataEditarProductosFormulas[0].CG_FORM,
                    AUTORIZA = "",
                    CG_GRUPOMP = "",
                    CG_FORM_SE = 0,
                    CG_CLAS = 0,
                    CG_DEC = 0,
                    CG_DEC_OF = 0,
                    CANT_MAT = 1,
                    CATEGORIA = 0,
                    COSTO = 0M,
                    DES_FORM = "",
                    DES_REVISION = "",
                    DOSIS = 0M,
                    FE_VIGE = DateTime.Now,
                    FOTO = "",
                    MERMA = 0M,
                    NOMBREFOTO = "",
                    OBSERV = "",
                    TIPO = 0,
                    TIPOFORM = "",
                    UNIMAT = "",
                    UNIPROD = "",
                    Diferencial = false,
                    CANT_BASE = 0M,
                    CANT_MAT_BASE = 0M,
                    CANTIDAD_BASE = 0M
                };

                DataEditarProductosFormulas.Add(nuevoInsumoFormula);
                added = true;
                await EditForm.Refresh();
            }
        }
        
        public async Task CellSavedHandler(CellSaveArgs<Formula> args)
        {
            if (args.Value is decimal)
                DataEditarProductosFormulas.First(f => f.DES_PROD.Trim() == args.RowData.DES_PROD.Trim()).CANT_MAT = args.Value as decimal? ?? 0;
            else if (args.Value.GetType() == typeof(string))
                DataEditarProductosFormulas.First(f => f.DES_PROD.Trim() == args.RowData.DES_PROD.Trim()).OBSERV = args.Value as string;
            edited = true;
        }
        
        protected async Task GuardarDB()
        {
            if (added)
            {
                DataEditarProductosFormulas.ForEach(f => f.REVISION += 1);
                DataEditarProductosFormulas.ForEach(f => f.FE_FORM = DateTime.Now);
                DataEditarProductosFormulas.ForEach(f => f.USUARIO = authState.User.Identity != null ? authState.User.Identity.Name : "USER");
                await Http.PostAsJsonAsync<List<Formula>>($"api/Formulas/PostList", DataEditarProductosFormulas);
                edited = false;
                deleted = false;
                await ToastMensajeExito("Se guardo la nueva revision correctamente.");
                dialogVisibleEditar = false;
                return;
            }
            if (!edited && !deleted)
            {
                await ToastMensajeError("No se ha modificado la formula.");
                return;
            } 
            bool isConfirmed = await DialogService.ConfirmAsync("Quiere que se guarde como una nueva revisión?", 
                "Revisión actual: " + DataEditarProductosFormulas[0].REVISION, new DialogOptions()
                {
                    PrimaryButtonOptions = new DialogButtonOptions() { Content = "Crear revisión " + (DataEditarProductosFormulas[0].REVISION + 1)},
                    CancelButtonOptions = new DialogButtonOptions() { Content = "Actualizar revisión " + DataEditarProductosFormulas[0].REVISION},
                });
            if (isConfirmed)
            {
                DataEditarProductosFormulas.ForEach(f => f.REVISION += 1);
                DataEditarProductosFormulas.ForEach(f => f.FE_FORM = DateTime.Now);
                DataEditarProductosFormulas.ForEach(f => f.USUARIO = authState.User.Identity != null ? authState.User.Identity.Name : "USER");
                await Http.PostAsJsonAsync<List<Formula>>($"api/Formulas/PostList", DataEditarProductosFormulas);
                edited = false;
                deleted = false;
                await ToastMensajeExito("Se guardo la nueva revision correctamente.");
                dialogVisibleEditar = false;
                return;
            }
            List<Formula> formsOriginales = await Http.GetFromJsonAsync<List<Formula>>($"api/Formulas/BuscarPorCgProdMaxRevision/{DataEditarProductosFormulas[0].Cg_Prod.Trim()}");
            List<Formula> formsEliminar = formsOriginales.Where(f => DataEditarProductosFormulas.All(f2 => f2.Id != f.Id)).ToList();
            if (formsEliminar.Count > 0)
                await Http.PostAsJsonAsync<List<Formula>>($"api/Formulas/DeleteList", formsEliminar);
            await Http.PutAsJsonAsync<List<Formula>>($"api/Formulas/PutList", DataEditarProductosFormulas);
            edited = false;
            deleted = false;
            await ToastMensajeExito("La revision se actualizo correctamente.");
            dialogVisibleEditar = false;
        }

        protected async Task ToolbarClickHandler(ClickEventArgs args)
        {
            if (args.Item.Id == "Despiece_excelexport") //Id is combination of Grid's ID and itemname
            {
                VisibleSpinner = true;
                await this.GridDespiece.ExportToExcelAsync();
                VisibleSpinner = false;
            }

            if (args.Item.Text == "Guardar Excel")
            {
                VisibleSpinner = true;
                var dataEspecial = listaDespiece.Select(d => new
                {
                    d.CG_PROD,
                    d.CG_SE,
                    d.CG_MAT,
                    d.DES_PROD,
                    d.CG_FORM,
                    CANT = d.CANT_MAT
                }).ToDataTable();
                await ExcelService.CreateExcel(dataEspecial, ProdSelected.CG_PROD.Trim());
                VisibleSpinner = false;

                await ToastMensajeExito("Guardado Correctamente.");
            }
        }
        
        protected async Task QueryCellInfoHandler(QueryCellInfoEventArgs<vIngenieriaProductosFormulas> args)
        {
            if (!args.Data.TIENE_FORM)
                args.Cell.AddClass(new string[] { "rojas" });
            if (args.Data.TIENE_FORM && !args.Data.FORM_ACTIVA)
                args.Cell.AddClass(new string[] { "amarillas" });
        }

        protected async Task TabEliminando(RemoveEventArgs removeEventArgs)
        {
            var tab = refSfTab.Items[removeEventArgs.RemovedIndex];
            if (removeEventArgs.RemovedIndex == 0)
                removeEventArgs.Cancel = true;
        }

        protected async Task TabEliminado(RemoveEventArgs removeEventArgs)
        {
            if (refSfTab.Items.Count == 1)
            {
                mostrarCerrarTab = false;
                tituloTabFormulas = string.Empty;
            }
        }

        protected async Task CopiarFormula(MouseEventArgs args)
        {
            dialogVisibleCopiar = false;
            VisibleSpinner = true;
            if (DetailedGridProdForm.SelectedRecords.Count != 0)
            {
                if (Selected != null)
                {
                    vIngenieriaProductosFormulas SelectedFormula2 =
                        DetailedGridProdForm.SelectedRecords.FirstOrDefault();
                    if (SelectedFormula2 != null)
                    {
                        HttpResponseMessage httpResponse = await Http.GetAsync($"api/Formulas/BuscarPorCgProdCopiar/{SelectedFormula2.CG_PROD}");
                        List<Formula> responseContent = await httpResponse.Content.ReadFromJsonAsync<List<Formula>>();
                        if (responseContent.Count == 0)
                        {
                            await ToastMensajeError($"No se encontró la formula para copiar. Generela para {SelectedFormula2.CG_PROD}.");
                            return;
                        }
                        bool isError = !httpResponse.IsSuccessStatusCode;
                        HttpResponseWrapper<List<Formula>> responseFormulas =
                            new HttpResponseWrapper<List<Formula>>(responseContent, httpResponse, isError);
                        if (responseFormulas.Error)
                        {
                            var errorReason = responseFormulas.HttpResponseMessage?.ReasonPhrase;
                            Console.WriteLine(errorReason);
                            return;
                        }
                        else if (responseFormulas.Response != null)
                        {
                            try
                            {
                                List<Formula> formulas = responseFormulas.Response;
                                foreach (Formula form in formulas)
                                {
                                    form.Id = 0;
                                    form.REVISION = 0;
                                    form.ACTIVO = "N";
                                    form.CG_FORM = 1;
                                    form.USUARIO = authState.User.Identity != null ? authState.User.Identity.Name : "USER";
                                    form.Cg_Prod = Selected.CG_PROD;
                                    form.FE_FORM = DateTime.Now;
                                    form.OBSERV = "COPIADA DE FORMULA " + SelectedFormula2.CG_PROD;
                                }

                                await Http.PostAsJsonAsync<List<Formula>>($"api/Formulas/PostList", formulas);
                                await ToastMensajeExito("Las formulas se copiaron correctamente.");

                                HttpResponseMessage httpResponse2 = await Http.GetAsync($"api/Procun/{SelectedFormula2.CG_PROD}");
                                if(httpResponse2.Content.Headers.ContentLength == 0)
                                {
                                    await ToastMensajeError($"No se encontró una hoja de ruta para copiar de {SelectedFormula2.CG_PROD}, generela para {Selected.CG_PROD}.");
                                    await GridProdForm.Refresh();
                                    VisibleSpinner = false;
                                    return;
                                }
                                List<Procun> responseContent2 = await httpResponse2.Content.ReadFromJsonAsync<List<Procun>>();
                                isError = !httpResponse2.IsSuccessStatusCode;
                                
                                if (responseContent2.Count == 0 || isError)
                                {
                                    await ToastMensajeError($"No se encontró una hoja de ruta para copiar de {SelectedFormula2.CG_PROD}, generela para {Selected.CG_PROD}.");
                                    await GridProdForm.Refresh();
                                    VisibleSpinner = false;
                                    return;
                                }

                                HttpResponseWrapper<List<Procun>> responseProcuns = new HttpResponseWrapper<List<Procun>>(responseContent2, httpResponse2, isError);

                                if (responseProcuns.Response != null && responseProcuns.Response.Count > 0)
                                {
                                    bool isConfirmed = await JsRuntime.InvokeAsync<bool>("confirm",$"Existe una hoja de ruta para {SelectedFormula2.CG_PROD}, desea copiarla para {Selected.CG_PROD}?");
                                    if (isConfirmed)
                                    {
                                        dialogVisibleCopiar = false;
                                        
                                        if (responseProcuns.Error)
                                        {
                                            await ToastMensajeError("Error al copiar la hoja de ruta.");
                                            VisibleSpinner = false;
                                            return;
                                        }
                                        
                                        List<Procun> procuns = responseProcuns.Response;
                                        foreach (Procun proc in procuns)
                                        {
                                            proc.Id = 0;
                                            proc.CG_PROD = Selected.CG_PROD;
                                            proc.USUARIO = "USER";
                                            proc.FECHA = DateTime.Now;
                                            proc.OBSERV = "COPIADA DE PROCUN " + SelectedFormula2.CG_PROD;
                                        }

                                        await Http.PostAsJsonAsync<List<Procun>>($"api/Procun/PostList2", procuns);
                                        await ToastMensajeExito("Hoja de ruta copiada correctamente.");

                                        DataOrdeProductosFormulas = await Http.GetFromJsonAsync<List<vIngenieriaProductosFormulas>>("api/Ingenieria/GetProductoFormulas");
                                        await GridProdForm.Refresh();
                                    }
                                    else
                                    {
                                        DataOrdeProductosFormulas = await Http.GetFromJsonAsync<List<vIngenieriaProductosFormulas>>("api/Ingenieria/GetProductoFormulas");
                                        await GridProdForm.Refresh();
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                await ToastMensajeError(ex.Message);
                                VisibleSpinner = false;
                            }
                        }
                    }
                }
            }
            VisibleSpinner = false;
        }
        
        private async Task ToastMensajeExito(string content = "Guardado Correctamente.")
        {
            await this.ToastObj.Show(new ToastModel
            {
                Title = "EXITO!",
                Content = content,
                CssClass = "e-toast-success",
                Icon = "e-success toast-icons",
                ShowCloseButton = true,
                ShowProgressBar = true
            });
        }
        
        private async Task ToastMensajeError(string content = "Ocurrio un Error.")
        {
            await ToastObj.Show(new ToastModel
            {
                Title = "Error!",
                Content = content,
                CssClass = "e-toast-warning",
                Icon = "e-warning toast-icons",
                ShowCloseButton = true,
                ShowProgressBar = true
            });
        }
    }
}