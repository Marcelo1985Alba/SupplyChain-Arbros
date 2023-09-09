using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SupplyChain.Shared.Enum;

namespace SupplyChain.Shared;

[Table("PRESUPUESTO_DETALLE")]
public class PresupuestoDetalle : EntityBase<int>
{
    public int PRESUPUESTOID { get; set; }
    public int SOLICITUDID { get; set; } = 0;

    [StringLength(15, MinimumLength = 4)] public string CG_ART { get; set; }

    [Range(1, 100, ErrorMessage = "La cantidad es requerida")]
    public decimal CANTIDAD { get; set; }

    public decimal PREC_UNIT { get; set; }
    public decimal PREC_UNIT_X_CANTIDAD { get; set; }
    public decimal IMP_DESCUENTO { get; set; }
    public string OBSERITEM { get; set; } = "";
    public decimal DESCUENTO { get; set; }

    [Range(1, 100, ErrorMessage = "Dias Plazo Entrega es requerido")]
    public int DIAS_PLAZO_ENTREGA { get; set; } = 0;

    public decimal TOTAL { get; set; } = 0;
    public Presupuesto Presupuesto { get; set; }
    public Solicitud Solicitud { get; set; }


    [NotMapped] public string CONTRAPRESIONFIJA { get; set; } = string.Empty;

    [NotMapped] public string CONTRAPRESIONVARIABLE { get; set; } = string.Empty;

    [NotMapped] public string PRESIONAPERTURA { get; set; } = string.Empty;

    [NotMapped] public string DESCRIPCIONFLUIDO { get; set; } = string.Empty;

    [NotMapped] public string TEMPERATURADESCARGAT { get; set; } = string.Empty;

    [NotMapped] public string CAPACIDADREQUERIDA { get; set; } = string.Empty;

    [NotMapped] public string DESCRIPCIONTAG { get; set; } = string.Empty;


    [NotMapped] public string DES_ART { get; set; }

    [NotMapped] public EstadoItem Estado { get; set; }
}