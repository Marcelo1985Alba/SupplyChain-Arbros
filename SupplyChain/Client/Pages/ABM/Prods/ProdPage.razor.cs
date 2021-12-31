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

namespace SupplyChain.Pages.Prods
{
    public class ProdsPageBase : ComponentBase
    {
        [Inject] protected HttpClient Http { get; set; }
        [Inject] protected IJSRuntime JsRuntime { get; set; }

        protected SfSpinner refSpinner;
        protected SfGrid<Producto> Grid;
        protected SfToast ToastObj;
        public bool SpinnerVisible = false;

        public bool Enabled = true;
        public bool Disabled = false;


        protected List<Producto> prods = new();


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
        
        public class Moneda
        {
            public string ID { get; set; }
            public string Text { get; set; }
        }
        List<Moneda> MonedaData = new List<Moneda> {
            new Moneda() { ID= "Mon1", Text= "Peso Argentino"},
            new Moneda() { ID= "Mon2", Text= "Dolar"},
            new Moneda() { ID= "Mon3", Text= "Euro"}
        };

        public class EstaActivo
        {
            public bool BActivo { get; set; }
            public string Text { get; set; }
        }
        protected List<EstaActivo> ActivoData = new List<EstaActivo> {
            new EstaActivo() { BActivo= true, Text= "SI"},
            new EstaActivo() { BActivo= false, Text= "NO"}};

        protected override async Task OnInitializedAsync()
        {
            MainLayout.Titulo = "Productos";

            SpinnerVisible = true;
            prods = await Http.GetFromJsonAsync<List<Producto>>("api/Prod");
            SpinnerVisible = false;
            await base.OnInitializedAsync();
        }

        public async Task ClickHandler(Syncfusion.Blazor.Navigations.ClickEventArgs args)
        {
            if (args.Item.Text == "Copy")
            {
                if (this.Grid.SelectedRecords.Count > 0)
                {
                    foreach (Producto selectedRecord in this.Grid.SelectedRecords)
                    {
                        bool isConfirmed = await JsRuntime.InvokeAsync<bool>("confirm", "Seguro de que desea copiar el Operario?");
                        if (isConfirmed)
                        {
                            Producto Nuevo = new Producto();

                            Nuevo.DES_PROD = selectedRecord.DES_PROD;
                            Nuevo.CAMPOCOM1 = selectedRecord.CAMPOCOM1;
                            Nuevo.CAMPOCOM2 = selectedRecord.CAMPOCOM2;
                            Nuevo.CAMPOCOM3 = selectedRecord.CAMPOCOM3;
                            Nuevo.CAMPOCOM4 = selectedRecord.CAMPOCOM4;
                            Nuevo.CAMPOCOM5 = selectedRecord.CAMPOCOM5;
                            Nuevo.CAMPOCOM6 = selectedRecord.CAMPOCOM6;
                            Nuevo.CAMPOCOM7 = selectedRecord.CAMPOCOM7;
                            Nuevo.CAMPOCOM8 = selectedRecord.CAMPOCOM8;
                            Nuevo.CAMPOCOM9 = selectedRecord.CAMPOCOM9;
                            Nuevo.CAMPOCOM10 = selectedRecord.CAMPOCOM10;
                            Nuevo.CG_ORDEN = selectedRecord.CG_ORDEN;
                            Nuevo.TIPO = selectedRecord.TIPO;
                            Nuevo.CG_CLAS = selectedRecord.CG_CLAS;
                            Nuevo.UNID = selectedRecord.UNID;
                            Nuevo.CG_DENSEG = selectedRecord.CG_DENSEG;
                            Nuevo.UNIDSEG = selectedRecord.UNIDSEG;
                            Nuevo.PESO = selectedRecord.PESO;
                            Nuevo.UNIDPESO = selectedRecord.UNIDPESO;
                            Nuevo.ESPECIF = selectedRecord.ESPECIF;
                            Nuevo.NORMA = selectedRecord.NORMA;
                            Nuevo.EXIGEDESPACHO = selectedRecord.EXIGEDESPACHO;
                            Nuevo.EXIGELOTE = selectedRecord.EXIGELOTE;
                            Nuevo.EXIGESERIE = selectedRecord.EXIGESERIE;
                            Nuevo.EXIGEOA = selectedRecord.EXIGEOA;
                            Nuevo.STOCKMIN = selectedRecord.STOCKMIN;
                            Nuevo.LOPTIMO = selectedRecord.LOPTIMO;
                            //Nuevo.ACTIVO = selectedRecord.ACTIVO;
                            Nuevo.TIEMPOFAB = selectedRecord.TIEMPOFAB;
                            Nuevo.COSTO = selectedRecord.COSTO;
                            Nuevo.COSTOTER = selectedRecord.COSTOTER;
                            Nuevo.MONEDA = selectedRecord.MONEDA;
                            //Nuevo.COSTOUCLOCAL = selectedRecord.COSTOUCLOCAL;
                            //Nuevo.COSTOUCDOL = selectedRecord.COSTOUCDOL;
                            Nuevo.FE_UC = selectedRecord.FE_UC;
                            Nuevo.CG_CELDA = selectedRecord.CG_CELDA;
                            Nuevo.CG_AREA = selectedRecord.CG_AREA;
                            Nuevo.CG_LINEA = selectedRecord.CG_LINEA;
                            Nuevo.CG_TIPOAREA = selectedRecord.CG_TIPOAREA;
                            //Nuevo.FE_REG = selectedRecord.FE_REG;
                            //Nuevo.CG_CIA = 1;
                            //Nuevo.USUARIO = "User";

                            var response = await Http.PostAsJsonAsync("api/Prod", Nuevo);
                            Nuevo.CG_PROD = prods.Max(s => s.CG_PROD) + 1;

                            if (response.StatusCode == System.Net.HttpStatusCode.Created)
                            {
                                Grid.Refresh();
                                var prod = await response.Content.ReadFromJsonAsync<Producto>();
                                await InvokeAsync(StateHasChanged);
                                Nuevo.CG_PROD = prod.CG_PROD;
                                prods.Add(Nuevo);
                                var itemsJson = JsonSerializer.Serialize(prod);
                                Console.WriteLine(itemsJson);
                                //toastService.ShowToast($"Registrado Correctemente.Vale {StockGuardado.VALE}", TipoAlerta.Success);
                                prods.OrderByDescending(p => p.CG_PROD);
                            }

                        }
                    }
                }
            }
            if (args.Item.Text == "Exportar Excel")
            {
                await this.Grid.ExcelExport();
            }

            if (args.Item.Text == "Eliminar")
            {
                //Verificar si tienen formula o esta dentro
                if ((await Grid.GetSelectedRecordsAsync()).Count > 0)
                {
                    var productos = await Grid.GetSelectedRecordsAsync();
                    var productosSeleccionados = productos.Select(p=> p.CG_PROD.Trim()).ToList();
                    var query = string.Empty;
                    var i = 0;
                    foreach (var item in productosSeleccionados)
                    {
                        
                        if (i == 0)
                        {
                            query += $"insumos={item}";
                        }
                        else
                        {
                            query += $"&&insumos={item}";
                        }

                        i++;
                    }


                    bool HayInsumoEnFormula = await Http.GetFromJsonAsync<bool>($"api/Formulas/VerificaFormula?{query}");
                    if (!HayInsumoEnFormula)
                    {
                        bool isConfirmed = await JsRuntime.InvokeAsync<bool>("confirm", "Seguro de que desea eliminar los insumos seleccionados?");
                        if (isConfirmed)
                        {
                            var response = await Http.PostAsJsonAsync("api/Prod/PostList", productos);
                            if (response.IsSuccessStatusCode)
                            {
                                await this.ToastObj.Show(new ToastModel
                                {
                                    Title = "EXITO!",
                                    Content = "Insumos Eliminados Correctamente.",
                                    CssClass = "e-toast-success",
                                    Icon = "e-success toast-icons",
                                    ShowCloseButton = true,
                                    ShowProgressBar = true
                                });

                                //await Grid.ClearSelectionAsync();

                            }
                            else
                            {
                                //cancela eliminacion en grilla por error
                                args.Cancel = true;
                                await this.ToastObj.Show(new ToastModel
                                {
                                    Title = "ERROR!",
                                    Content = "Error al Eliminar insumos: contactar al administrador",
                                    CssClass = "e-toast-danger",
                                    Icon = "e-error toast-icons",
                                    ShowCloseButton = true,
                                    ShowProgressBar = true
                                });
                            }
                        }
                        else
                        {
                            //cancela eliminacion en grilla por cancelacion
                            args.Cancel = true;
                        }
                    }
                    else
                    {
                        //cancela eliminacion en grilla por estaar en una formula
                        args.Cancel = true;
                        await this.ToastObj.Show(new ToastModel
                        {
                            Title = "Atención!",
                            Content = "No se puede Eliminar insumos: insumos relacionados con formula",
                            CssClass = "e-toast-warning",
                            Icon = "e-warning toast-icons",
                            ShowCloseButton = true,
                            ShowProgressBar = true
                        });
                    }
                }
                
            }
        }
    }
}
