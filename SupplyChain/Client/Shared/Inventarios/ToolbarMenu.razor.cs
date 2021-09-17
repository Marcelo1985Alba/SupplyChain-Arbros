using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SupplyChain.Client.Shared.Inventarios
{
    public partial class ToolbarMenuBase: ComponentBase
    {
        [Parameter] public bool DisabledEliminar { get; set; } = true;
        [Parameter] public bool DisabledGuardar { get; set; } = true;
        [Parameter] public bool DisabledImprimir { get; set; } = true;
        [Parameter] public EventCallback OnEliminarClick{ get; set; }
        [Parameter] public EventCallback OnNuevoClick{ get; set; }
        [Parameter] public EventCallback OnGuardarClick{ get; set; }
        [Parameter] public EventCallback OnImprimirClick{ get; set; }
        [Parameter] public Dictionary<string, object> HtmlAttribute { get; set; } =
        new Dictionary<string, object>() { { "type", "submit" } };


        protected async Task Eliminar()
        {
            await OnEliminarClick.InvokeAsync();
        }
        protected async Task Nuevo()
        {
            await OnNuevoClick.InvokeAsync();
        }
        protected async Task Guardar()
        {
            await OnGuardarClick.InvokeAsync();
        }
        protected async Task Imprimir()
        {
            await OnImprimirClick.InvokeAsync();
        }
    }
}
