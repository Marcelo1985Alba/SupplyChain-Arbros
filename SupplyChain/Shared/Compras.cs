using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupplyChain.Shared;

public class Compras : EntityBase<int>
{
    private string _color;

    [Key] [Column("REGISTRO")] public new int Id { get; set; } = 0;

    public int NUMERO { get; set; }
    public DateTime? FE_EMIT { get; set; }
    public string CG_PREP { get; set; }
    public int CG_ORDEN { get; set; }
    public string CG_MAT { get; set; }
    public string CG_MATE { get; set; }
    public string DES_MAT { get; set; }
    public string TIPO { get; set; }
    public string TILDE { get; set; }
    public string TILDE1 { get; set; }
    public string TILDE2 { get; set; }
    public decimal NECESARIO { get; set; }
    public decimal SOLICITADO { get; set; }
    public string UNID { get; set; }
    public decimal AUTORIZADO { get; set; }
    public decimal CG_DEN { get; set; }
    public string UNID1 { get; set; }
    public decimal PRECIO { get; set; }
    public decimal PRECIONETO { get; set; }
    public decimal PRECIOTOT { get; set; }
    public string MONEDA { get; set; }
    public int NROCLTE { get; set; }
    public string DES_PROVE { get; set; }
    public int ENTREGA { get; set; }
    public DateTime? FE_PREV { get; set; }
    public DateTime? FE_REAL { get; set; }
    public DateTime? FE_VENC { get; set; }
    public DateTime? FE_CIERRE { get; set; }
    public string CONDVEN { get; set; }
    public string CONDPREC { get; set; }
    public string CONDVENEX { get; set; }
    public int CG_DEPOSM { get; set; }
    public decimal PRECIOPOND { get; set; }
    public int PEDIDO { get; set; }
    public int? CG_EST { get; set; }
    public decimal CG_CUENT { get; set; }
    public DateTime? FE_PREC { get; set; }
    public int? DIASVIGE { get; set; }
    public decimal CANTLOTE { get; set; }
    public decimal CANTMIN { get; set; }
    public string ESPECIFICA { get; set; }
    public string? ESPEGEN { get; set; }
    public string CG_PROD { get; set; }
    public string NOPROD { get; set; }
    public int CG_COS { get; set; }
    public string ESTADO { get; set; }
    public string MODALI { get; set; }
    public string CG_GRUPOMP { get; set; }
    public DateTime? FE_DISP { get; set; }
    public int NUMANULA { get; set; }
    public int NUMCOMP { get; set; }
    public string TIPOPREC { get; set; }
    public int CG_IMPORT { get; set; }
    public int CG_CIA { get; set; }
    public int IMPRESA { get; set; }
    public int MARCA1 { get; set; }
    public int AbiertoPreparacion { get; set; }
    public string USUARIO { get; set; }
    public DateTime? FE_REG { get; set; }
    public int REGISTRO { get; set; }
    public int NUMEROQ { get; set; }
    public DateTime? FE_REQ { get; set; }
    public DateTime? FE_AUTREQ { get; set; }
    public int CG_PROVEREQ { get; set; }
    public string OBSEREQ { get; set; }
    public string MARCAREQ { get; set; }
    public decimal AVANCE { get; set; }
    public string TXTOBSERVADO { get; set; }
    public string TXTCORREGIFO { get; set; }
    public string USUARIO_AUT { get; set; }
    public DateTime? FE_AUT { get; set; }
    public DateTime? FE_CIERREQ { get; set; }
    public string USUREQ { get; set; }
    public int CG_ORDF { get; set; }
    public int CG_PROY { get; set; }
    public int ESTADO_CAB { get; set; }
    public int ESTADO_IT { get; set; }
    public decimal NECESARIO_ORI { get; set; }
    public decimal NUM_SOLCOT { get; set; }
    public string CG_MAT2 { get; set; }
    public int MODIF_INGRESO { get; set; }
    public decimal PENDIENTE { get; set; }
    public string? TILDE3 { get; set; }
    public string Observaciones { get; set; }
    public decimal DESCUENTO { get; set; }

    [NotMapped] public bool CONFIRMADO { get; set; }
}