using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupplyChain.Shared.PCP;

[Table("Procesos2")]
public class Procesos2 : EntityBase<int>
{
    public string VALE { get; set; } = "";
    public int DESPACHO { get; set; } = 0;
    public DateTime FE_ENSAYO { get; set; } = DateTime.Today;
    public string CG_PROD { get; set; } = "";
    public string DESCAL { get; set; } = "";
    public string CARCAL { get; set; } = "";
    public string UNIDADM { get; set; } = "";
    public int CANTMEDIDA { get; set; } = 0;
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
    public DateTime FECHA { get; set; } = DateTime.Today;
    public string APROBADO { get; set; } = "";
    public string Usuario { get; set; } = "";
    public int REGISTRO { get; set; } = 0;

    [NotMapped] public bool GUARDADO { get; set; }
    [NotMapped] public bool ESNUEVO { get; set; }
}