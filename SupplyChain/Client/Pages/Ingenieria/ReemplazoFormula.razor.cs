using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using SupplyChain.Client.HelperService;
using SupplyChain.Client.Shared;
using SupplyChain.Shared;
using SupplyChain.Shared.Models;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Navigations;
using Syncfusion.Blazor.Notifications;
using Syncfusion.Blazor.Popups;
using Syncfusion.Blazor.Spinner;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using SupplyChain.Client.RepositoryHttp;
using Syncfusion.Blazor.Buttons;
using Syncfusion.Blazor.Inputs;
using ChangeEventArgs = Syncfusion.Blazor.Navigations.ChangeEventArgs;

namespace SupplyChain.Client.Pages.Ingenieria
{
    public class BaseReemplazoFormula : ComponentBase
    {
        [Inject] public HttpClient Http { get; set; }
        [Inject] protected IRepositoryHttp HttpNew { get; set; }
        [Inject] protected IJSRuntime JsRuntime { get; set; }
        [Inject] public SfDialogService DialogService { get; set; }
        [CascadingParameter] public Task<AuthenticationState> authenticationState { get; set; }
        [CascadingParameter] public MainLayout MainLayout { get; set; }

        protected SfToast ToastObj;

        protected SearchModel searchModel = new SearchModel();

        protected List<Formula> resultados = new List<Formula>();
        protected bool IsVisible { get; set; } = false;
        protected SfGrid<Producto> gridProductos;
        protected SfGrid<Formula> gridFormulas;
        protected List<Producto> Busquedalist = new();
        protected List<Producto> CG_PRODlist = new();
        protected int CantidadMostrar = 12;
        protected string editing = "";

        protected AuthenticationState authState;
        
        protected SfSpinner refSpinner;
        protected bool SpinnerVisible { get; set; } = false;
        
        protected override async Task OnInitializedAsync()
        {
            MainLayout.Titulo = "Reemplazo de Formula";
            authState = await authenticationState;
        }

        public void OnSelected()
        {
            CantidadMostrar = 12;
            IsVisible = false;
            SpinnerVisible = true;
            if (searchModel.Tipo == "MP")
            {
                if(editing == "busqueda")
                {
                    searchModel.TextoBusqueda = this.gridProductos.GetSelectedRecordsAsync().Result.FirstOrDefault()?.Id;
                    searchModel.DescripcionBusqueda = this.gridProductos.GetSelectedRecordsAsync().Result.FirstOrDefault()?.DES_PROD;
                }
                else if (editing == "reemplazo")
                {
                    searchModel.TextoReemplazo = this.gridProductos.GetSelectedRecordsAsync().Result.FirstOrDefault()?.Id;
                    searchModel.DescripcionReemplazo = this.gridProductos.GetSelectedRecordsAsync().Result.FirstOrDefault()?.DES_PROD;
                }
            }
            else if (searchModel.Tipo == "SE")
            {
                if(editing == "busqueda")
                {
                    searchModel.TextoBusqueda = this.gridProductos.GetSelectedRecordsAsync().Result.FirstOrDefault()?.Id;
                    searchModel.DescripcionBusqueda = this.gridProductos.GetSelectedRecordsAsync().Result.FirstOrDefault()?.DES_PROD;
                }
                else if (editing == "reemplazo")
                {
                    searchModel.TextoReemplazo = this.gridProductos.GetSelectedRecordsAsync().Result.FirstOrDefault()?.Id;
                    searchModel.DescripcionReemplazo = this.gridProductos.GetSelectedRecordsAsync().Result.FirstOrDefault()?.DES_PROD;
                }
            }
            
            SpinnerVisible = false;
            StateHasChanged();
        }
        
        protected async Task AgregarValores()
        {
            CantidadMostrar *= 2;
            if(editing == "busqueda")
                await BuscarProducto("busqueda");
            else
                await BuscarProducto("reemplazo");
        }
        
        protected async Task BuscarProducto(string tipo)
        {
            if (searchModel.Tipo == "MP")
            {
                if (tipo == "busqueda")
                {
                    editing = "busqueda";
                    if (string.IsNullOrEmpty(searchModel.DescripcionBusqueda) && string.IsNullOrEmpty(searchModel.TextoBusqueda))
                    {
                        Busquedalist = await Http.GetFromJsonAsync<List<Producto>>($"api/Prevision/BuscarPorCgOrden/Vacio/Vacio/4/{CantidadMostrar}");
                        IsVisible = true;
                    }
                    if (!string.IsNullOrEmpty(searchModel.DescripcionBusqueda) || !string.IsNullOrEmpty(searchModel.TextoBusqueda))
                    {
                        if (string.IsNullOrEmpty(searchModel.DescripcionBusqueda))
                            Busquedalist = await Http.GetFromJsonAsync<List<Producto>>($"api/Prevision/BuscarPorCgOrden/{searchModel.TextoBusqueda}/Vacio/4/{CantidadMostrar}");
                        else if (string.IsNullOrEmpty(searchModel.TextoBusqueda))
                            Busquedalist = await Http.GetFromJsonAsync<List<Producto>>($"api/Prevision/BuscarPorCgOrden/Vacio/{searchModel.DescripcionBusqueda}/4/{CantidadMostrar}");
                        else
                            Busquedalist = await Http.GetFromJsonAsync<List<Producto>>($"api/Prevision/BuscarPorCgOrden/{searchModel.TextoBusqueda}/{searchModel.DescripcionBusqueda}/4/{CantidadMostrar}");
                        IsVisible = true;
                    }
                }
                else
                {
                    editing = "reemplazo";
                    if (string.IsNullOrEmpty(searchModel.DescripcionReemplazo) && string.IsNullOrEmpty(searchModel.TextoReemplazo))
                    {
                        Busquedalist = await Http.GetFromJsonAsync<List<Producto>>($"api/Prevision/BuscarPorCgOrden/Vacio/Vacio/4/{CantidadMostrar}");
                        IsVisible = true;
                    }
                    if (!string.IsNullOrEmpty(searchModel.DescripcionReemplazo) || !string.IsNullOrEmpty(searchModel.TextoReemplazo))
                    {
                        if (string.IsNullOrEmpty(searchModel.DescripcionReemplazo))
                            Busquedalist = await Http.GetFromJsonAsync<List<Producto>>($"api/Prevision/BuscarPorCgOrden/{searchModel.TextoReemplazo}/Vacio/4/{CantidadMostrar}");
                        else if (string.IsNullOrEmpty(searchModel.TextoReemplazo))
                            Busquedalist = await Http.GetFromJsonAsync<List<Producto>>($"api/Prevision/BuscarPorCgOrden/Vacio/{searchModel.DescripcionReemplazo}/4/{CantidadMostrar}");
                        else
                            Busquedalist = await Http.GetFromJsonAsync<List<Producto>>($"api/Prevision/BuscarPorCgOrden/{searchModel.TextoReemplazo}/{searchModel.DescripcionReemplazo}/4/{CantidadMostrar}");
                        IsVisible = true;
                    }
                }
            } else if (searchModel.Tipo == "SE")
            {
                if (tipo == "busqueda")
                {
                    editing = "busqueda";
                    if (string.IsNullOrEmpty(searchModel.DescripcionBusqueda) && string.IsNullOrEmpty(searchModel.TextoBusqueda))
                    {
                        Busquedalist = await Http.GetFromJsonAsync<List<Producto>>($"api/Prevision/BuscarPorCgOrden/Vacio/Vacio/3/{CantidadMostrar}");
                        IsVisible = true;
                    }
                    if (!string.IsNullOrEmpty(searchModel.DescripcionBusqueda) || !string.IsNullOrEmpty(searchModel.TextoBusqueda))
                    {
                        if (string.IsNullOrEmpty(searchModel.DescripcionBusqueda))
                            Busquedalist = await Http.GetFromJsonAsync<List<Producto>>($"api/Prevision/BuscarPorCgOrden/{searchModel.TextoBusqueda}/Vacio/3/{CantidadMostrar}");
                        else if (string.IsNullOrEmpty(searchModel.TextoBusqueda))
                            Busquedalist = await Http.GetFromJsonAsync<List<Producto>>($"api/Prevision/BuscarPorCgOrden/Vacio/{searchModel.DescripcionBusqueda}/3/{CantidadMostrar}");
                        else
                            Busquedalist = await Http.GetFromJsonAsync<List<Producto>>($"api/Prevision/BuscarPorCgOrden/{searchModel.TextoBusqueda}/{searchModel.DescripcionBusqueda}/3/{CantidadMostrar}");
                        IsVisible = true;
                    }
                }
                else
                {
                    editing = "reemplazo";
                    if (string.IsNullOrEmpty(searchModel.DescripcionReemplazo) && string.IsNullOrEmpty(searchModel.TextoReemplazo))
                    {
                        Busquedalist = await Http.GetFromJsonAsync<List<Producto>>($"api/Prevision/BuscarPorCgOrden/Vacio/Vacio/3/{CantidadMostrar}");
                        IsVisible = true;
                    }
                    if (!string.IsNullOrEmpty(searchModel.DescripcionReemplazo) || !string.IsNullOrEmpty(searchModel.TextoReemplazo))
                    {
                        if (string.IsNullOrEmpty(searchModel.DescripcionReemplazo))
                            Busquedalist = await Http.GetFromJsonAsync<List<Producto>>($"api/Prevision/BuscarPorCgOrden/{searchModel.TextoReemplazo}/Vacio/3/{CantidadMostrar}");
                        else if (string.IsNullOrEmpty(searchModel.TextoReemplazo))
                            Busquedalist = await Http.GetFromJsonAsync<List<Producto>>($"api/Prevision/BuscarPorCgOrden/Vacio/{searchModel.DescripcionReemplazo}/3/{CantidadMostrar}");
                        else
                            Busquedalist = await Http.GetFromJsonAsync<List<Producto>>($"api/Prevision/BuscarPorCgOrden/{searchModel.TextoReemplazo}/{searchModel.DescripcionReemplazo}/3/{CantidadMostrar}");
                        IsVisible = true;
                    }
                }
            }
        }
        
        protected async Task Buscar()
        {
            SpinnerVisible = true;
            if(searchModel.Tipo == "MP")
                resultados = await Http.GetFromJsonAsync<List<Formula>>($"api/Formulas/containsCG_MAT/{searchModel.TextoBusqueda}");
            else if (searchModel.Tipo == "SE")
                resultados = await Http.GetFromJsonAsync<List<Formula>>($"api/Formulas/containsCG_SE/{searchModel.TextoBusqueda}");
            await InvokeAsync(StateHasChanged);
            if(gridFormulas != null)
                await gridFormulas.Refresh();
            SpinnerVisible = false;
        }
        
        protected void BusquedaTextChanged(InputEventArgs args)
        {
            if (args.Value != null)
                searchModel.TextoBusqueda = args.Value.ToString().ToUpper();
            searchModel.DescripcionBusqueda = "";
        }

        protected void DescripcionBusquedaTextChanged(InputEventArgs args)
        {
            if (args.Value != null)
                searchModel.DescripcionBusqueda = args.Value.ToString().ToUpper();
            searchModel.TextoBusqueda = "";
        }
        
        protected void ReemplazoTextChanged(InputEventArgs args)
        {
            if (args.Value != null)
                searchModel.TextoReemplazo = args.Value.ToString().ToUpper();
            searchModel.DescripcionReemplazo = "";
        }
        
        protected void DescripcionReemplazoTextChanged(InputEventArgs args)
        {
            if (args.Value != null)
                searchModel.DescripcionReemplazo = args.Value.ToString().ToUpper();
            searchModel.TextoReemplazo = "";
        }

        protected async Task Reemplazar()
        {
            SpinnerVisible = true;
            if (string.IsNullOrEmpty(searchModel.TextoReemplazo) || string.IsNullOrEmpty(searchModel.DescripcionReemplazo))
            {
                if(searchModel.Tipo == "MP")
                    await ToastMensajeError("Ingrese la materia prima de reemplazo");
                else if (searchModel.Tipo == "SE")
                    await ToastMensajeError("Ingrese el semielaborado de reemplazo");
                SpinnerVisible = false;
                return;
            }

            var formulas = gridFormulas.GetSelectedRecordsAsync().Result;
            if (formulas.Count == 0)
            {
                await ToastMensajeError("Debe seleccionar al menos una formula");
                SpinnerVisible = false;
                return;
            }
            List<Formula> formulasAEditar = new List<Formula>();
            foreach (var formula in formulas)
            {
                var formulasAEditarAux = await Http.GetFromJsonAsync<List<Formula>>($"api/Formulas/BuscarPorCgProdMaxRevision/{formula.Cg_Prod.Trim()}");
                foreach (var formulaAEditar in formulasAEditarAux)
                {
                    formulaAEditar.REVISION += 1;
                    formulaAEditar.FE_FORM = DateTime.Now;
                    formulaAEditar.FE_VIGE = DateTime.Now;
                    formulaAEditar.USUARIO = authState.User.Identity != null ? authState.User.Identity.Name : "USER";
                    if(formulaAEditar.Cg_Mat == formula.Cg_Mat)
                        if(searchModel.Tipo == "MP")
                            formulaAEditar.Cg_Mat = searchModel.TextoReemplazo;
                    if(formulaAEditar.Cg_Se == formula.Cg_Se)
                        if(searchModel.Tipo == "SE")
                            formulaAEditar.Cg_Se = searchModel.TextoReemplazo;
                }
                formulasAEditar.AddRange(formulasAEditarAux);
            }

            try
            {
                await Http.PostAsJsonAsync<List<Formula>>($"api/Formulas/PostList", formulasAEditar);
                await ToastMensajeExito("Formulas reemplazadas correctamente.");
                resultados = new List<Formula>();
                IsVisible = false;
            }
            catch
            {
                await ToastMensajeError("Ocurrio un error al reemplazar las formulas.");
            }
            SpinnerVisible = false;
        }
        
        protected class SearchModel
        {
            public string Tipo { get; set; } = "MP";
            public string TextoBusqueda { get; set; } = "";
            public string DescripcionBusqueda { get; set; } = "";
            public string TextoReemplazo { get; set; } = "";
            public string DescripcionReemplazo { get; set; } = "";
        }
        
        public void OnRadioChange(ChangeArgs<string> args)
        {
            searchModel.Tipo = args.Value;
            searchModel.TextoBusqueda = "";
            searchModel.TextoReemplazo = "";
            searchModel.DescripcionBusqueda = "";
            searchModel.DescripcionReemplazo = "";
            resultados = new List<Formula>();
            IsVisible = false;
        }
        
        protected async Task QueryCellInfoHandler(QueryCellInfoEventArgs<Formula> args)
        {
            if (args.Data != null)
                if (args.Data.ACTIVO.Trim() == "N")
                    args.Cell.AddClass(new string[] { "rojas" });
        }
        
        public void CustomizeCell(QueryCellInfoEventArgs<Formula> args)
        {
            if(args.Data.ACTIVO.Trim() == "N")
                args.Cell.AddClass(new String[] { "rojas" });
        }
        
        private async Task ToastMensajeExito(string content = "Guardado Correctamente.")
        {
            await this.ToastObj.ShowAsync(new ToastModel
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
            await ToastObj.ShowAsync(new ToastModel
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