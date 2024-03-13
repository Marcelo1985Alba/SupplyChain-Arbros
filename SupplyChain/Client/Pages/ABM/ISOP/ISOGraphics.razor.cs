using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using SupplyChain.Client.HelperService;
using SupplyChain.Shared.Models;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.HeatMap;
using Syncfusion.Blazor.Notifications;
using Syncfusion.Blazor.Spinner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using SFHeatMap = Syncfusion.Blazor.HeatMap;

namespace SupplyChain.Client.Pages.ABM.ISOP
{
    public class GraphicIso : ComponentBase
    {
        [Inject] protected HttpClient Http { get; set; }
        [Inject] protected NavigationManager NavigationManager { get; set; }
        [Inject] public ISOService isoService { get; set; }

        protected List<ISO> isos = new();
        protected List<ISO> allIsos = new();
        protected SfSpinner refSpinner;
        protected bool SpinnerVisible = false;
        protected SfToast ToastObj;

		protected string impAmb = "RIESGO";
		protected int idForImpSelected = 0;
		protected string[] XLabels = new string[] { "Muy Baja", "Baja", "Media", "Alta", "Muy Alta" };
		protected string[] YLabels = new string[] { "Muy Poco", "Poco", "Moderado", "Alto", "Muy Alto" };
		protected int I1F1 = 11;
		protected int I1F2 = 12;
		protected int I1F3 = 13;
		protected int I1F4 = 14;
		protected int I1F5 = 15;
		protected int I2F1 = 21;
		protected int I2F2 = 22;
		protected int I2F3 = 23;
		protected int I2F4 = 24;
		protected int I2F5 = 25;
		protected int I3F1 = 31;
		protected int I3F2 = 32;
		protected int I3F3 = 33;
		protected int I3F4 = 34;
		protected int I3F5 = 35;
		protected int I4F1 = 41;
		protected int I4F2 = 42;
		protected int I4F3 = 43;
		protected int I4F4 = 44;
		protected int I4F5 = 45;
		protected int I5F1 = 51;
		protected int I5F2 = 52;
		protected int I5F3 = 53;
		protected int I5F4 = 54;
		protected int I5F5 = 55;
		public object HeatMapColors { get; set; }

		[Parameter] public bool Show { get; set; } = false;
        [Parameter] public int pedido { get; set; } = 0;

        public class BaseOption
		{
			public string Text { get; set; }
		}
		public List<BaseOption> Impactos = new List<BaseOption> {
			new BaseOption() {Text= "AIRE" },
			new BaseOption() {Text= "AGUA" },
			new BaseOption() {Text= "SUELO" },
			new BaseOption() {Text= "RRNN" },
			new BaseOption() {Text= "BIOTA" },
			new BaseOption() {Text= "QVIDA" },
			new BaseOption() {Text= "RIESGO" },
			new BaseOption() {Text= "OPORTUNIDAD" },
		};
		public List<BaseOption> Filtros = new List<BaseOption> {
			new BaseOption() {Text= "Ambos" },
			new BaseOption() {Text= "9001" },
			new BaseOption() {Text= "14001" },
		};
		public List<int> idByImp;
		protected override async Task OnInitializedAsync()
		{
			HeatMapColors = GetDefaultColors();
			var response = await isoService.Get();
			if (!response.Error)
			{
				isos = response.Response;
				allIsos = isos;
            }
            idByImp = isos.Where(s => s.ImpAmb == impAmb).Select(s => s.Identificacion).OrderBy(s => s).ToList();
            idByImp.Add(0);
        }
		int[,] GetDefaultColors()
		{
			int[,] dataSource = new int[,]
			{
			{I1F1, I2F1, I3F1, I4F1, I5F1},
			{I1F2, I2F2, I3F2, I4F2, I5F2},
			{I1F3, I2F3, I3F3, I4F3, I5F3},
			{I1F4, I2F4, I3F4, I4F4, I5F4},
			{I1F5, I2F5, I3F5, I4F5, I5F5},
			};
			return dataSource;
		}
		protected void ChangeImpAmb(Syncfusion.Blazor.DropDowns.ChangeEventArgs<string, BaseOption> args)
		{
			impAmb = args.Value;
            idByImp = isos.Where(s => s.ImpAmb == impAmb).Select(s => s.Identificacion).OrderBy(s => s).ToList();
            idByImp.Add(0);
        }
		protected void ChangeFiltro(Syncfusion.Blazor.DropDowns.ChangeEventArgs<string, BaseOption> args)
		{
			if (args.Value == "Ambos")
				isos = allIsos;
			else if (args.Value == "9001")
				isos = allIsos.Where(s => s.ImpAmb == "OPORTUNIDAD" || s.ImpAmb == "RIESGO").ToList();
			else if (args.Value == "14001")
				isos = allIsos.Where(s => s.ImpAmb != "OPORTUNIDAD" && s.ImpAmb != "RIESGO").ToList();
		}
        protected void ChangeId(Syncfusion.Blazor.DropDowns.ChangeEventArgs<int, int> args)
        {
			idForImpSelected = args.Value;
        }

        protected async Task TooltipRendering(SFHeatMap.TooltipEventArgs args)
		{
			List<ISO> registros;
            if (idForImpSelected == 0)
                registros = isos.Where(p => p.ImpAmb == impAmb && p.Frecuencia == args.XLabel && p.Impacto == args.YLabel).ToList();
            else
				registros = isos.Where(p => p.ImpAmb == impAmb && p.Frecuencia == args.XLabel && p.Impacto == args.YLabel && p.Identificacion == idForImpSelected).ToList();
            //string content = $@"Impacto: {args.YLabel}.<br>Frecuencia: {args.XLabel}.";
            string content = " ";
            //string content = "";
			for (int i = 0; i < registros.Count; i++)
				content += $"<br>* {registros[i].Detalle}";
			if (content == " ")
				content = "-";
			args.Content = new[] { content };
		}

        public void onClick()
        {
            NavigationManager.NavigateTo("/Abms/Iso");
        }
    }
}
