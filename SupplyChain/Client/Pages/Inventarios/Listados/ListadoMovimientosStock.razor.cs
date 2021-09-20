﻿using Microsoft.AspNetCore.Components;
using SupplyChain.Client.Shared;
using SupplyChain.Shared;
using SupplyChain.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SupplyChain.Client.Pages.Inventarios
{
    public class ListadoMovimientosStockBase : ComponentBase
    {
        [Inject] public HttpClient Http { get; set; }
        [CascadingParameter] public MainLayout ML { get; set; }
        protected List<MovimientoStockSP> DataSource;
        protected bool spinnerVisible = false;
        protected DateTime desde = DateTime.Now.AddMonths(-1);
        protected DateTime hasta = DateTime.Now;
        protected FilterMovimientosStock filter = new();



        protected async override Task OnInitializedAsync()
        {
            ML.Titulo = "Lisatado de Movimientos de Stock";
        }

        protected async Task Buscar()
        {
            filter.Desde = desde.ToString("dd/MM/yyyy");
            filter.Hasta = hasta.ToString("dd/MM/yyyy");
            spinnerVisible = true;
            DataSource = await Http.GetFromJsonAsync<List<MovimientoStockSP>>(GeneraUrl());
            spinnerVisible = false;
        }

        private string GeneraUrl()
        {

            string api = "api/Stock/MovimientosStock";

            api += $"?Tipoo={filter.Tipoo}&Desde={filter.Desde}&Hasta={filter.Hasta}";

            return api;
        }

        protected async Task ChangeTipoo(Tire tire)
        {
            filter.Tipoo = tire.Tipoo;
        }
        protected async Task ChangeDeposito(Deposito deposito)
        {
            filter.Deposito = deposito.CG_DEP;
        }

        protected async Task LimpiarFiltros()
        {
            desde = DateTime.Now.AddMonths(-1);
            hasta = DateTime.Now;

            filter = new()
            {
                Tipoo = 0,
                Desde = desde.ToString("dd/MM/yyyy"),
                Hasta = hasta.ToString("dd/MM/yyyy")
            };
    }
    }
}