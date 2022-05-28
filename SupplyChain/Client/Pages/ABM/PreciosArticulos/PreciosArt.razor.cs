using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SupplyChain;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Navigations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using SupplyChain.Shared.Models;
using SupplyChain.Client.Shared;
using Syncfusion.Blazor.Spinner;
using Syncfusion.Blazor.Notifications;
using SupplyChain.Shared;

namespace SupplyChain.Pages.PreciosArt
{
    public class PreciosArtPageBase : ComponentBase
    {
        [Inject] protected HttpClient Http { get; set; }
        [Inject] protected IJSRuntime JsRuntime { get; set; }

        protected SfSpinner refSpinner;
        protected SfGrid<PreciosArticulos> Grid;
        protected SfToast ToastObj;
        public bool SpinnerVisible = false;
        protected bool IsVisible { get; set; } = false;

        public bool Enabled = true;
        public bool Disabled = false;
        public bool isAdding = false;


        protected List<PreciosArticulos> preciosArts = new();
        public PreciosArticulos precioArt = new PreciosArticulos();

        protected List<Moneda> monedas = new List<Moneda>();

        protected const string APPNAME = "grdPreciosArtABM";
        protected string state;

        protected List<Object> Toolbaritems = new List<Object>(){
        "Search",
        "Add",
        "Edit",
        "Delete",
        "Print",
        new ItemModel { Text = "Copy", TooltipText = "Copy", PrefixIcon = "e-copy", Id = "copy" },
        "ExcelExport"
    };
        [CascadingParameter] MainLayout MainLayout { get; set; }
        protected override async Task OnInitializedAsync()
        {
            MainLayout.Titulo = "Precios de Articulos";

            SpinnerVisible = true;
            preciosArts = await Http.GetFromJsonAsync<List<PreciosArticulos>>("api/PreciosArt");
            monedas = await Http.GetFromJsonAsync<List<Moneda>>("api/Monedas");

            SpinnerVisible = false;
            await base.OnInitializedAsync();
        }

        public async Task Begin(ActionEventArgs<PreciosArticulos> args)
        {
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

            if (args.RequestType == Syncfusion.Blazor.Grids.Action.RowDragAndDrop)
            {

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
                var existe = await Http.GetFromJsonAsync<bool>($"api/PreciosArt/PreciosArtExists/{precioArt.Id}");

                if (!existe)
                {
                    var response = await Http.PostAsJsonAsync("api/PreciosArt", precioArt);

                    if (response.StatusCode == System.Net.HttpStatusCode.Created)
                    {
                        var precioArtNuevo = await response.Content.ReadFromJsonAsync<PreciosArticulos>();
                        preciosArts.Add(precioArtNuevo);
                        preciosArts.OrderByDescending(p => p.Id);
                        await Grid.RefreshColumns();
                        Grid.Refresh();
                        await Grid.RefreshHeader();

                        await this.ToastObj.Show(new ToastModel
                        {
                            Title = "EXITO!",
                            Content = $"Precio de articulo {precioArt.Id} Guardado Correctamente.",
                            CssClass = "e-toast-success",
                            Icon = "e-success toast-icons",
                            ShowCloseButton = true,
                            ShowProgressBar = true
                        }); //DESPUES DE AGREGAR
                        isAdding = false; //DESPUES DE AGREGAR
                        IsVisible = false;
                        await Cerrar();
                    }
                    else
                    {
                        await this.ToastObj.Show(new ToastModel
                        {
                            Title = "ERROR!",
                            Content = "Error al verificar datos",
                            CssClass = "e-toast-danger",
                            Icon = "e-error toast-icons",
                            ShowCloseButton = true,
                            ShowProgressBar = true
                        });
                    }
                }
                else
                {
                    await this.ToastObj.Show(new ToastModel
                    {
                        Title = "ERROR!",
                        Content = "Error al verificar datos",
                        CssClass = "e-toast-danger",
                        Icon = "e-error toast-icons",
                        ShowCloseButton = true,
                        ShowProgressBar = true
                    });
                }
            }
            else
            {
                var response = await Http.PutAsJsonAsync($"api/PreciosArt/{precioArt.Id}", precioArt);

                if (response.IsSuccessStatusCode)
                {
                    var precioArtNuevo = await response.Content.ReadFromJsonAsync<PreciosArticulos>();
                    precioArtNuevo.Id = precioArt.Id;
                    var precioArtModificar = preciosArts.Where(p => p.Id == precioArt.Id).FirstOrDefault();
                    precioArtModificar.Id = precioArtNuevo.Id;
                    precioArtModificar.Descripcion = precioArtNuevo.Descripcion;
                    precioArtModificar.Precio = precioArtNuevo.Precio;
                    precioArtModificar.Moneda = precioArtNuevo.Moneda;
                    precioArtModificar.Marca = precioArtNuevo.Marca;
                    precioArtModificar.Construccion = precioArtNuevo.Construccion;
                    preciosArts.OrderByDescending(p => p.Id);
                    await this.ToastObj.Show(new ToastModel
                    {
                        Title = "EXITO!",
                        Content = $"Precio de articulo {precioArt.Id} editado Correctamente.",
                        CssClass = "e-toast-success",
                        Icon = "e-success toast-icons",
                        ShowCloseButton = true,
                        ShowProgressBar = true
                    }); //DESPUES DE EDITAR
                    Grid.Refresh();
                    IsVisible = false;
                    await Cerrar();
                }
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
                    await this.ToastObj.Show(new ToastModel
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
        public async Task OnVistaSeleccionada(VistasGrillas vistasGrillas)
        {
            await Grid.SetPersistData(vistasGrillas.Layout);
        }
        public async Task OnReiniciarGrilla()
        {
            await Grid.ResetPersistData();
        }
    }
}
