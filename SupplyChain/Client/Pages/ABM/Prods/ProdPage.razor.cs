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
namespace SupplyChain.Pages.Prods
{
    public class ProdsPageBase : ComponentBase
    {
        [Inject] protected HttpClient Http { get; set; }
        [Inject] protected IJSRuntime JsRuntime { get; set; }
        protected SfGrid<Producto> Grid;

        public bool Enabled = true;
        public bool Disabled = false;


        protected List<Producto> prods = new List<Producto>();


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
            prods = await Http.GetFromJsonAsync<List<Producto>>("api/Prod");

            await base.OnInitializedAsync();
        }

        public void ActionBeginHandler(ActionEventArgs<Producto> args)
        {
            if (args.RequestType == Syncfusion.Blazor.Grids.Action.BeginEdit)
            {
                this.Enabled = false;
            }
            else
            {
                this.Enabled = true;
            }
        }
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

        public async Task ActionBegin(ActionEventArgs<Producto> args)
        {
            //if (args.RequestType == Syncfusion.Blazor.Grids.Action.Save)
            //{
            //    HttpResponseMessage response;
            //    bool found = prods.Any(p => p.CG_PROD == args.Data.CG_PROD);
            //    Producto ur = new Producto();

            //    if (!found)
            //    {
            //        args.Data.CG_CIA = 1;
            //        args.Data.USUARIO = "User";
            //        response = await Http.PostAsJsonAsync("api/Prod", args.Data);
            //        args.Data.CG_PROD = prods.Max(s => s.CG_PROD) + 1;
            //    }
            //    else
            //    {
            //        response = await Http.PutAsJsonAsync($"api/Prod/{args.Data.CG_PROD}", args.Data);
            //    }

            //    if (response.StatusCode == System.Net.HttpStatusCode.Created)
            //    {

            //    }
            //}

            if (args.RequestType == Syncfusion.Blazor.Grids.Action.Delete)
            {
                await EliminarProd(args);
            }
        }

        private async Task EliminarProd(ActionEventArgs<Producto> args)
        {
            try
            {
                if (args.Data != null)
                {
                    bool isConfirmed = await JsRuntime.InvokeAsync<bool>("confirm", "Seguro de que desea eliminar el Operario?");
                    if (isConfirmed)
                    {
                        //operarios.Remove(operarios.Find(m => m.CG_OPER == args.Data.CG_OPER));
                        await Http.DeleteAsync($"api/Prod/{args.Data.CG_PROD}");
                    }
                }
            }
            catch (Exception ex)
            {

            }
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
                                var prod = await response.Content.ReadFromJsonAsync<Prod>();
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
                Refresh();
            }
            if (args.Item.Text == "Excel Export")
            {
                await this.Grid.ExcelExport();
            }
        }

        public async Task CellSaveHandler(CellSaveArgs<Producto> args)
        {

            HttpResponseMessage response;
            bool found = prods.Any(p => p.CG_PROD == args.RowData.CG_PROD);
            Producto ur = new Producto();

            if (!found)
            {
                //args.RowData.CG_CIA = 1;
                //args.RowData.USUARIO = "User";
                response = await Http
                    .PostAsJsonAsync($"api/Prod/ActualizarDatos/{args.ColumnName}/{args.Value.ToString()}/{args.RowData.CG_PROD}", args.RowData);
                args.RowData.CG_PROD = prods.Max(s => s.CG_PROD) + 1;
            }
            else
            {
                response = await Http.PostAsJsonAsync($"api/Prod/ActualizarDatos/{args.ColumnName}/{args.Value.ToString()}/{args.RowData.CG_PROD}", args.RowData);
                
            }

            if (response.StatusCode == System.Net.HttpStatusCode.Created)
            {

            }



        }
        public void Refresh()
        {
            Grid.Refresh();

        }
    }
}
