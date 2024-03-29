﻿using Microsoft.AspNetCore.Components;
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
using SupplyChain.Client.Shared;
using Syncfusion.Blazor.Spinner;
using Syncfusion.Blazor.Notifications;

namespace SupplyChain.Pages.Area
{
    public class AreaPageBase : ComponentBase
    {
        [Inject] protected HttpClient Http { get; set; }
        [Inject] protected IJSRuntime JsRuntime { get; set; }
        [CascadingParameter] MainLayout MainLayout { get; set; }

        protected SfSpinner refSpinner;
        protected SfGrid<Areas> Grid;
        protected SfToast ToastObj;
        public bool SpinnerVisible = false;
        public bool isAdding = false;
        public bool habilitaCodigo = false;
        protected bool IsVisible { get; set; } = false;

        protected List<Areas> areas = new();
        public Areas area = new Areas();

        protected List<Object> Toolbaritems = new List<Object>(){
        "Search",
        "Add",
        "Edit",
        "Delete",
        "Print",
        new ItemModel { Text = "Copy", TooltipText = "Copy", PrefixIcon = "e-copy", Id = "copy" },
        "ExcelExport"
        };

        protected override async Task OnInitializedAsync()
        {
            MainLayout.Titulo = "Areas";

            SpinnerVisible = true;
            areas = await Http.GetFromJsonAsync<List<Areas>>("api/Areas");

            SpinnerVisible = false;
            await base.OnInitializedAsync();
        }

        public async Task ClickHandler(Syncfusion.Blazor.Navigations.ClickEventArgs args)
        {
            if (args.Item.Text == "Edit")
            {
                if (this.Grid.SelectedRecords.Count < 2)
                {
                    foreach (Areas selectedRecord in this.Grid.SelectedRecords)
                    {
                        area.Id = selectedRecord.Id;
                        area.DES_AREA = selectedRecord.DES_AREA;
                    }
                    isAdding = false;
                    habilitaCodigo = false;
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
                    area = new();
                    foreach (Areas selectedRecord in this.Grid.SelectedRecords)
                    {
                        bool isConfirmed = await JsRuntime.InvokeAsync<bool>("confirm", "Seguro de que desea copiar el area?");
                        if (isConfirmed)
                        {
                            area.Id = selectedRecord.Id;
                            area.DES_AREA = selectedRecord.DES_AREA;
                        }
                    }
                    IsVisible = true;
                    isAdding = true;
                    habilitaCodigo = true;
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
                var areas = await Grid.GetSelectedRecordsAsync();
                if (areas.Count > 0)
                {
                    bool isConfirmed = await JsRuntime.InvokeAsync<bool>("confirm", "Seguro de que desea eliminar las areas seleccionadas?");
                    if (isConfirmed)
                    {
                        var areasSeleccionadas = areas.Select(p => p.Id).ToList();
                        var response = await Http.PostAsJsonAsync("api/Areas/PostList", areas);
                        if (response.IsSuccessStatusCode)
                        {
                            await this.ToastObj.Show(new ToastModel
                            {
                                Title = "EXITO!",
                                Content = "las areas seleccionadas fueron eliminadas correctamente.",
                                CssClass = "e-toast-success",
                                Icon = "e-success toast-icons",
                                ShowCloseButton = true,
                                ShowProgressBar = true
                            });
                            args.Cancel = false;
                        }
                        else
                        {
                            await this.ToastObj.Show(new ToastModel
                            {
                                Title = "ERROR!",
                                Content = "Hubo un problema al eliminar las areas seleccionadas",
                                CssClass = "e-toast-danger",
                                Icon = "e-error toast-icons",
                                ShowCloseButton = true,
                                ShowProgressBar = true
                            });
                            args.Cancel = true;
                        }
                    }
                }
                else
                {
                    await this.ToastObj.Show(new ToastModel
                    {
                        Title = "ERROR!",
                        Content = "No ha seleccionado productos para eliminar.",
                        CssClass = "e-toast-danger",
                        Icon = "e-error toast-icons",
                        ShowCloseButton = true,
                        ShowProgressBar = true
                    });
                }
            }
        }

        public async Task guardarArea()
        {
            if (isAdding == true)
            {
                var existe = await Http.GetFromJsonAsync<bool>($"api/Areas/AreaExists/{area.Id}");

                if (!existe)
                {
                    var response = await Http.PostAsJsonAsync("api/Areas", area);

                    if (response.StatusCode == System.Net.HttpStatusCode.Created)
                    {
                        var newArea = await response.Content.ReadFromJsonAsync<Areas>();
                        areas.Add(newArea);
                        areas.OrderByDescending(p => p.Id);
                        await Grid.RefreshColumns();
                        Grid.Refresh();
                        await Grid.RefreshHeader();

                        await this.ToastObj.Show(new ToastModel
                        {
                            Title = "EXITO!",
                            Content = $"AREA {area.Id} Guardada Correctamente.",
                            CssClass = "e-toast-success",
                            Icon = "e-success toast-icons",
                            ShowCloseButton = true,
                            ShowProgressBar = true
                        });
                        isAdding = false;
                        habilitaCodigo = false;
                        IsVisible = false;
                        area = new();
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
                var response = await Http.PutAsJsonAsync($"api/Areas/{area.Id}", area);

                if (response.IsSuccessStatusCode)
                {
                    var newArea = await response.Content.ReadFromJsonAsync<Areas>();
                    newArea.Id = area.Id;
                    var areaSinModificar = areas.Where(p => p.Id == area.Id).FirstOrDefault();
                    areaSinModificar.Id = newArea.Id;
                    areaSinModificar.DES_AREA = newArea.DES_AREA;
                    areas.OrderByDescending(p => p.Id);
                    await this.ToastObj.Show(new ToastModel
                    {
                        Title = "EXITO!",
                        Content = $"AREA {area.Id} editada Correctamente.",
                        CssClass = "e-toast-success",
                        Icon = "e-success toast-icons",
                        ShowCloseButton = true,
                        ShowProgressBar = true
                    });
                    Grid.Refresh();
                    IsVisible = false;
                    area = new();
                }
            }
        }
    }
}
