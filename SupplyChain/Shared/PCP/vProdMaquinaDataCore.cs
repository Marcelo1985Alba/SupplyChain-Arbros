using System.ComponentModel.DataAnnotations;

namespace SupplyChain.Shared.PCP;

public class vProdMaquinaDataCore
{
    [Key] public int Id { get; set; }

    public string FechaFin { get; set; }
    public int Año { get; set; }
    public int Mes { get; set; }
    public string Maquina { get; set; }
    public decimal ParadasPlanHoras { get; set; }
    public decimal SetupRealHoras { get; set; }
    public decimal TiempoNetoHoras { get; set; }
    public string Orden { get; set; }
    public string Operador { get; set; }
}