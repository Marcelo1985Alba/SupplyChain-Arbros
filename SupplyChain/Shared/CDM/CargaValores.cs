using SupplyChain.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace SupplyChain
{
    [Table("CargaValores")]
    public class Valores : EntityBase<int>
    {
        [Key, Column("REGISTRO")]
        new public int Id { get; set; } = 0;
        new public string CERTIFIC { get; set; } = "";
        new public DateTime FE_ENSAYO { get; set; }
        new public string CG_PROD { get; set; } = "";
        new public string DESCAL { get; set; } = "";
        new public string CARCAL { get; set; } = "";
        new public string UNIDADM { get; set; } = "";
        new public int CANTMEDIDA { get; set; } = 0;
        new public string OBSERV { get; set; } = "";
        new public string AVISO { get; set; } = "";
        new public string OBSERV1 { get; set; } = "";
        new public int CG_PROVE { get; set; } = 0;
        new public int REMITO { get; set; } = 0;
        new public string VALORNC { get; set; } = "";
        new public string LEYENDANC { get; set; } = "";
        new public int O_COMPRA { get; set; } = 0;
        new public string UNID { get; set; } = "";
        new public int EVENTO { get; set; } = 0;
        new public string ENSAYOS { get; set; } = "";
        new public DateTime FECHA { get; set; }
        new public string APROBADO { get; set; } = "";
        new public string USUARIO { get; set; } = "";
        
        new public int PRIORIDAD { get; set; } = 0;
        [NotMapped] public bool GUARDADO { get; set; }
        [NotMapped] public bool ESNUEVO { get; set; }
    }   
}
