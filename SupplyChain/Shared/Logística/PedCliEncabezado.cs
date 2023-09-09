using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupplyChain.Shared.Logística;

public class PedCliEncabezado
{
    [Key] public int PEDIDO { get; set; } = 0;

    public int PRESUP { get; set; } = 0;
    public int NUMOCI { get; set; } = 0;
    public string ORCO { get; set; }

    [Column("VA_INDIC")] public double TC { get; set; } = 0;

    public string MONEDA { get; set; }

    [Required(ErrorMessage = "* La Condicion de Pago es requerida")]
    [Column("DPP")]
    public int CONDICION_PAGO { get; set; } = 0;

    public int CG_CLI { get; set; } = 0;
    public string DES_CLI { get; set; } = string.Empty;

    public DateTime FE_MOV { get; set; } = DateTime.Now;

    //[Required(ErrorMessage = "La Direccion de Entrega es requerida")]
    public string DIRENT { get; set; }

    [Required(ErrorMessage = "* La Condicion de Entrega es requerida")]
    public int CG_COND_ENTREGA { get; set; } = 0;

    public decimal BONIFIC { get; set; } = 0;
    public int CG_TRANS { get; set; } = 0;
    public decimal BONIFICACION_IMPORTE { get; set; } = 0;
    public decimal TOTAL { get; set; } = 0;
    public string CONDVEN { get; set; }
    public string USUARIO { get; set; }

    [ValidateComplexType] public List<PedCli> Items { get; set; } = new();

    [NotMapped] public List<vDireccionesEntrega> DireccionesEntregas { get; set; } = new();
}