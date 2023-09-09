using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using SupplyChain.Client.HelperService;
using Syncfusion.Blazor.DropDowns;
using Syncfusion.Blazor.Notifications;
using Syncfusion.Blazor.Spinner;
using SFHeatMap = Syncfusion.Blazor.HeatMap;

namespace SupplyChain.Client.Pages.ABM.ISOP;

public class GraphicIso : ComponentBase
{
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
    public List<int> idByImp;
    protected int idForImpSelected;

    public List<BaseOption> Impactos = new()
    {
        new() { Text = "AIRE" },
        new() { Text = "AGUA" },
        new() { Text = "SUELO" },
        new() { Text = "RRNN" },
        new() { Text = "BIOTA" },
        new() { Text = "QVIDA" },
        new() { Text = "RIESGO" },
        new() { Text = "OPORTUNIDAD" }
    };

    protected string impAmb = "RIESGO";

    protected List<ISO> isos = new();
    protected SfSpinner refSpinner;
    protected bool SpinnerVisible = false;
    protected SfToast ToastObj;
    protected string[] XLabels = { "Muy Baja", "Baja", "Media", "Alta", "Muy Alta" };
    protected string[] YLabels = { "Muy Poco", "Poco", "Moderado", "Alto", "Muy Alto" };
    [Inject] protected HttpClient Http { get; set; }
    [Inject] protected NavigationManager NavigationManager { get; set; }
    [Inject] public ISOService isoService { get; set; }
    public object HeatMapColors { get; set; }

    [Parameter] public bool Show { get; set; }
    [Parameter] public int pedido { get; set; }

    protected override async Task OnInitializedAsync()
    {
        HeatMapColors = GetDefaultColors();
        var response = await isoService.Get();
        if (!response.Error) isos = response.Response;
        idByImp = isos.Where(s => s.ImpAmb == impAmb).Select(s => s.Identificacion).OrderBy(s => s).ToList();
        idByImp.Add(0);
    }

    private int[,] GetDefaultColors()
    {
        int[,] dataSource =
        {
            { I1F1, I2F1, I3F1, I4F1, I5F1 },
            { I1F2, I2F2, I3F2, I4F2, I5F2 },
            { I1F3, I2F3, I3F3, I4F3, I5F3 },
            { I1F4, I2F4, I3F4, I4F4, I5F4 },
            { I1F5, I2F5, I3F5, I4F5, I5F5 }
        };
        return dataSource;
    }

    protected void ChangeImpAmb(ChangeEventArgs<string, BaseOption> args)
    {
        impAmb = args.Value;
        idByImp = isos.Where(s => s.ImpAmb == impAmb).Select(s => s.Identificacion).OrderBy(s => s).ToList();
        idByImp.Add(0);
    }

    protected void ChangeId(ChangeEventArgs<int, int> args)
    {
        idForImpSelected = args.Value;
    }

    protected async Task TooltipRendering(SFHeatMap.TooltipEventArgs args)
    {
        List<ISO> registros;
        if (idForImpSelected == 0)
            registros = isos.Where(p => p.ImpAmb == impAmb && p.Frecuencia == args.XLabel && p.Impacto == args.YLabel)
                .ToList();
        else
            registros = isos.Where(p =>
                p.ImpAmb == impAmb && p.Frecuencia == args.XLabel && p.Impacto == args.YLabel &&
                p.Identificacion == idForImpSelected).ToList();
        //string content = $@"Impacto: {args.YLabel}.<br>Frecuencia: {args.XLabel}.";
        var content = " ";
        //string content = "";
        for (var i = 0; i < registros.Count; i++)
            /*
            string toShow = registros[i].Detalle;
            int comienzo = 0;
            content += $"<br>";
            while(toShow.Length > comienzo){
                if(toShow.Length < comienzo || toShow.Length < 50)
                    content += $"{registros[i].Detalle.Substring(comienzo, toShow.Length-1)}";
                else
                    content += $"{registros[i].Detalle.Substring(comienzo, comienzo+50-1)}";
                comienzo += 50;
            }
            */
            content += $"<br>* {registros[i].Detalle}";
        if (content == " ")
            content = "-";
        args.Content = new[] { content };
    }

    public void onClick()
    {
        NavigationManager.NavigateTo("/Abms/Iso");
    }

    public class BaseOption
    {
        public string Text { get; set; }
    }
}