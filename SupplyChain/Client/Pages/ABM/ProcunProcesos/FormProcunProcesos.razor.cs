using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Net.Http;

namespace SupplyChain.Client.Pages.ABM.ProcunProcesos
{
    public class FormProcunProcesosBase : ComponentBase
    {
        [Inject] protected HttpClient Http { get; set; }
        [Inject] protected IJSRuntime JSRuntime { get; set; }
        [Inject] protected 
    }
}
