using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SupplyChain.Client.HelperService;
using SupplyChain.Client.RepositoryHttp;
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

namespace SupplyChain.Client.Pages.ABM.Precios
{
    public class PreciosPageBase : ComponentBase
    {
        [Inject] protected PrecioArticuloService PrecioArticuloService { get; set; }
        [Inject] public IJSRuntime JsRuntime { get; set; }
        [Inject] public HttpClient Http { get; set; }
        protected SfSpinner refSpinner;
        protected SfGrid<PreciosArticulos> Grid;
        protected SfToast ToastObj;
        public bool SpinnerVisible = false;
        protected bool IsVisible { get; set; } = false;

        public bool Enabled = true;
        public bool Disabled = false;
        public bool isAdding = false;


        protected List<PreciosArticulos> preciosArts = new();
        public PreciosArticulos precioArt = new();

        protected List<Moneda> monedas = new();

        protected const string APPNAME = "grdPreciosArtABM";
        protected string state;

        protected List<Object> Toolbaritems = new List<Object>(){
            "Search",
            "Add",
            "Edit",
            "Delete",
            "Print",
            new ItemModel { Text = "Copia", TooltipText = "Copy", PrefixIcon = "e-copy", Id = "copy" },
            "ExcelExport"
        };
        [CascadingParameter] MainLayout MainLayout { get; set; }

        protected override async Task OnInitializedAsync()
        {
            MainLayout.Titulo = "Precios de Articulos";

            SpinnerVisible = true;
            //preciosArts = await Http.GetFromJsonAsync<List<PreciosArticulos>>("api/PreciosArt");
            await GetPrecioArticulos();

            SpinnerVisible = false;
        }

        protected async Task GetPrecioArticulos()
        {
            var respose = await PrecioArticuloService.Get();
            if (respose.Error)
            {
                Console.WriteLine(await respose.HttpResponseMessage.Content.ReadAsStringAsync());
                await ToastMensajeError();
            }
            else
            {
                preciosArts = respose.Response;
                monedas = await Http.GetFromJsonAsync<List<Moneda>>("api/Monedas");
            }
        }

        public async Task Cerrar()
        {
            precioArt = new();
        }

        public async Task guardarPrecioArt()
        {
            if (isAdding == true)
            {
                bool existe = await PrecioArticuloService.Existe(precioArt.Id);
                if (!existe)
                {
                    await AgregarNuevoArticulo();
                }
                else
                {
                    await ToastMensajeError($"El Articulo {precioArt.Id} ya existe en la base de datos.");
                }
            }
            else
            {
                await ActualizarArticulo();
            }
        }

        private async Task ActualizarArticulo()
        {
            var response = await PrecioArticuloService.Actualizar(precioArt.Id, precioArt);

            if (!response.Error)
            {
                var precioArtNuevo = (PreciosArticulos)response.Response;
                precioArtNuevo.Id = precioArt.Id;
                var precioArtModificar = preciosArts.Where(p => p.Id == precioArt.Id).FirstOrDefault();
                precioArtModificar.Id = precioArtNuevo.Id;
                precioArtModificar.Descripcion = precioArtNuevo.Descripcion;
                precioArtModificar.Precio = precioArtNuevo.Precio;
                precioArtModificar.Moneda = precioArtNuevo.Moneda;
                precioArtModificar.Marca = precioArtNuevo.Marca;
                precioArtModificar.Construccion = precioArtNuevo.Construccion;
                preciosArts.OrderByDescending(p => p.Id);
                await ToastMensajeExito($"Precio de articulo {precioArt.Id} editado Correctamente.");
                Grid.Refresh();
                IsVisible = false;
                await Cerrar();
            }
        }

        private async Task AgregarNuevoArticulo()
        {
            var response = await PrecioArticuloService.Agregar(precioArt);

            if (!response.Error)
            {
                var precioArtNuevo = response.Response;
                preciosArts.Add(precioArtNuevo);
                preciosArts.OrderByDescending(p => p.Id);
                await Grid.RefreshColumns();
                Grid.Refresh();
                await Grid.RefreshHeader();

                await ToastMensajeExito($"Precio de articulo {precioArt.Id} Guardado Correctamente.");
                isAdding = false; //DESPUES DE AGREGAR
                IsVisible = false;
                await Cerrar();
            }
            else
            {
                await ToastMensajeError("Error al agregar Articulo.");
            }
        }

        public async Task ClickHandler(Syncfusion.Blazor.Navigations.ClickEventArgs args)
        {
            if (args.Item.Text == "Edit")
            {
                if (this.Grid.SelectedRecords.Count < 2)
                {
                    foreach (PreciosArticulos selectedRecord in this.Grid.SelectedRecords)
                    {
                        precioArt.Id = selectedRecord.Id;
                        precioArt.Descripcion = selectedRecord.Descripcion;
                        precioArt.Precio = selectedRecord.Precio;
                        precioArt.Moneda = selectedRecord.Moneda;
                        precioArt.Marca = selectedRecord.Marca;
                        precioArt.Construccion = selectedRecord.Construccion;
                    }
                    isAdding = false;
                    IsVisible = true;
                }
                else
                {
                    await this.ToastObj.ShowAsync(new ToastModel
                    {
                        Title = "ERROR!",
                        Content = "Solo se puede editar un item",
                        CssClass = "e-toast-danger",
                        Icon = "e-error toast-icons",
                        ShowCloseButton = true,
                        ShowProgressBar = true
                    });
                }
            }

            if (args.Item.Text == "Copy" || args.Item.Text == "Add")
            {
                if (this.Grid.SelectedRecords.Count < 2)
                {
                    precioArt = new();
                    foreach (PreciosArticulos selectedRecord in this.Grid.SelectedRecords)
                    {
                        bool isConfirmed = await JsRuntime.InvokeAsync<bool>("confirm", "Seguro de que desea copiar el producto?");
                        if (isConfirmed)
                        {
                            precioArt.Descripcion = selectedRecord.Descripcion;
                            precioArt.Precio = selectedRecord.Precio;
                            precioArt.Moneda = selectedRecord.Moneda;
                            precioArt.Marca = selectedRecord.Marca;
                            precioArt.Construccion = selectedRecord.Construccion;
                        }
                    }
                    IsVisible = true;
                    isAdding = true;
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

            if (args.Item.Text == "Exportar Excel")
            {
                await this.Grid.ExcelExport();
            }

            if (args.Item.Text == "Eliminar")
            {
                if ((await Grid.GetSelectedRecordsAsync()).Count > 0)
                {
                    foreach (PreciosArticulos selectedRecord in this.Grid.SelectedRecords)
                    {
                        bool isConfirmed = await JsRuntime.InvokeAsync<bool>("confirm", "Seguro de que desea eliminar el precio de articulo?");
                        if (isConfirmed)
                        {
                            await Http.DeleteAsync($"api/PreciosArt/{selectedRecord.Id}");
                        }
                    }
                }
            }
        }
        protected async Task OnActionBeginHandler(ActionEventArgs<PreciosArticulos> args)
        {
            if (args.RequestType == Syncfusion.Blazor.Grids.Action.BeginEdit)
            {
                precioArt = args.Data;
                args.PreventRender = false;
                args.Cancel = true;
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
                Grid.PreventRender();
                Grid.Refresh();

                state = await Grid.GetPersistData();
                await Grid.AutoFitColumnsAsync();
                await Grid.RefreshColumns();
                await Grid.RefreshHeader();
            }
        }
        protected async Task OnActionCompleteHandler(ActionEventArgs<PreciosArticulos> args)
        {
            if (args.RequestType == Syncfusion.Blazor.Grids.Action.BeginEdit)
            {
                precioArt = args.Data;
                args.PreventRender = false;
            }
        }
        protected async Task OnVistaSeleccionada(VistasGrillas vistasGrillas)
        {
            await Grid.SetPersistData(vistasGrillas.Layout);
        }
        protected async Task OnReiniciarGrilla()
        {
            await Grid.ResetPersistData();
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
