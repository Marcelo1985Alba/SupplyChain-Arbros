using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using SupplyChain.Shared.CDM;
using SupplyChain.Shared.Context;
using Syncfusion.Blazor.Gantt;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Spinner;

namespace SupplyChain.Client.Pages.PCP.Carga_de_Maquina
{
    public class CargaMaquinaIA : ComponentBase
    {
        [Inject] public HttpClient Http { get; set; }

        protected bool visible = false;
        protected List<Operario> operarios = new List<Operario>();
        protected List<PLANNER_CN1> ordenesCN1 = new List<PLANNER_CN1>();
        protected List<PLANNER_CN1> ordenesCN2 = new List<PLANNER_CN1>();
        protected List<GanttDataDetails> ordenes = new List<GanttDataDetails>();

        protected SfSpinner refSpinner;
        protected bool VisibleSpinner = false;

        public SfGantt<GanttDataDetails> Gantt;
        private DateTime projectStart;
        private DateTime projectEnd;
        private List<Maquina> Maquinas { get; set; }
        protected bool taskbarUpdate = true;

        protected override async Task OnInitializedAsync()
        {
            VisibleSpinner = true;
            operarios = await Http.GetFromJsonAsync<List<Operario>>("api/Operario");
            ordenesCN1 = await Http.GetFromJsonAsync<List<PLANNER_CN1>>("api/CargasIA/GetCN1");
            ordenesCN2 = await Http.GetFromJsonAsync<List<PLANNER_CN1>>("api/CargasIA/GetCN2");
            this.Maquinas = getMaquinas;
            projectStart = ordenesCN1.First().INICIO;
            projectEnd = ordenesCN1.Last().FIN;

            int id = 1;

            // agrego las maquinas
            foreach (var maquina in Maquinas)
            {
                ordenes.Add(new GanttDataDetails()
                {
                    Id = id++,
                    Name = maquina.Nombre,
                    Sdate = projectStart,
                    Edate = projectEnd,
                });
            }

            // agrego las ordenes de CN1
            foreach (var orden in ordenesCN1)
            {
                ordenes.Add(new GanttDataDetails()
                {
                    Id = id++,
                    Name = orden.DES_PROD,
                    Sdate = orden.INICIO,
                    Edate = orden.FIN,
                    Progress = "0",
                    ParentId = 1,
                    Notes = orden.CG_ORDF.ToString().Trim(),
                });
            }

            // agrego las ordenes de CN2
            foreach (var orden in ordenesCN2)
            {
                ordenes.Add(new GanttDataDetails()
                {
                    Id = id++,
                    Name = orden.DES_PROD,
                    Sdate = orden.INICIO,
                    Edate = orden.FIN,
                    Progress = "0",
                    ParentId = 2,
                    Notes = orden.CG_ORDF.ToString().Trim(),
                });
            }

            VisibleSpinner = false;
            await this.Gantt.RefreshAsync();
        }

        public class Maquina
        {
            public int Id { get; set; }
            public string Nombre { get; set; }
        }

        public static List<Maquina> getMaquinas = new List<Maquina>()
        {
            new Maquina() { Id = 1, Nombre = "CN1" },
            new Maquina() { Id = 2, Nombre = "CN2" },
        };

        public void RowDataBound(RowDataBoundEventArgs<GanttDataDetails> args)
        {
            if (!args.Data.ParentId.HasValue)
            {
                args.Row.AddClass(new string[] { "custom-row" });
            }
        }

        public void queryChart(QueryChartRowInfoEventArgs<GanttDataDetails> args)
        {
            if (!args.Data.ParentId.HasValue)
            {
                args.Row.AddClass(new string[] { "custom-row" });
            }
        }
    }
}
