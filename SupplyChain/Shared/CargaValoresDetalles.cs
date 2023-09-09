using System;
using System.ComponentModel.DataAnnotations.Schema;
using SupplyChain.Shared.Enum;

namespace SupplyChain.Shared;

[Table("Procesos2")]
public class CargaValoresDetalles : EntityBase<int>
{
    //[Key, Column("REGISTRO")]
    //new public int Id { get; set; }
    public int VALE { get; set; }
    public string DESPACHO { get; set; } = "";
    public DateTime FE_ENSAYO { get; set; } = DateTime.Now;
    public string CG_PROD { get; set; } = "";
    public string DESCAL { get; set; } = "";
    public string CARCAL { get; set; } = "";
    public string UNIDADM { get; set; } = "";
    public decimal CANTMEDIDA { get; set; } = 0;
    public string OBSERV { get; set; } = "";
    public string AVISO { get; set; } = "";
    public string OBSERV1 { get; set; } = "";
    public int CG_PROVE { get; set; } = 0;
    public string REMITO { get; set; } = "";
    public string VALORNC { get; set; } = "";
    public string LEYENDANC { get; set; } = "";
    public int O_COMPRA { get; set; } = 0;
    public string UNID { get; set; } = "";
    public int EVENTO { get; set; } = 0;
    public string ENSAYOS { get; set; } = "";
    public DateTime FECHA { get; set; }
    public string APROBADO { get; set; } = "";
    public string USUARIO { get; set; } = "";

    public int REGISTRO { get; set; } = 0;
    [NotMapped] public bool GUARDADO { get; set; }
    [NotMapped] public bool ESNUEVO { get; set; }
    [NotMapped] public EstadoItem Estado { get; set; }
}