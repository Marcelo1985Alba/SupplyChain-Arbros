using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SupplyChain.Shared;
using SupplyChain.Shared.Enum;

namespace SupplyChain;

public class PedCli : EntityBase<int>
{
    private string _color;

    [Key] [Column("REGISTRO")] public new int Id { get; set; } = 0;

    public DateTime FE_PED { get; set; }

    [Column("PRESUP")] public int PRESUPUESTOID { get; set; } = 0;

    public int PEDIDO { get; set; } = 0;
    public decimal CANTPED { get; set; }
    public int NUMOCI { get; set; } = 0;
    public int DPP { get; set; } = 0;
    public string MONEDA { get; set; }
    public int CG_CLI { get; set; } = 0;
    public string DES_CLI { get; set; } = "";
    public string UNID { get; set; } = "";
    public string ORCO { get; set; } = "";
    public string CG_ART { get; set; } = "";
    public string DES_ART { get; set; } = "";
    public decimal BONIFIC { get; set; } = 0;
    public decimal DESCUENTO { get; set; }
    public decimal VA_INDIC { get; set; } = 0;
    public int CG_TRANS { get; set; } = 0;

    [Column("IMPORTE1")] public decimal PREC_UNIT { get; set; }

    [Column("IMPORTE2")] public decimal PREC_UNIT_X_CANTIDAD { get; set; }

    [Column("IMPORTE3")] public decimal IMP_DESCUENTO { get; set; }

    [Column("IMPORTE4")] public decimal TOTAL { get; set; } = 0;

    public string OBSERITEM { get; set; } = "";
    public string DIRENT { get; set; } = "";
    public string CG_ESTADO { get; set; } = "";
    public int CG_COND_ENTREGA { get; set; } = 0;
    public int CG_ESTADPEDCLI { get; set; } = 0;
    public string ESTADO_LOGISTICA { get; set; } = "";
    public string LOTE { get; set; } = "";
    public string CAMPOCOM1 { get; set; } = "";
    public string CAMPOCOM6 { get; set; } = "";
    public string CAMPOCOM3 { get; set; } = "";
    public string CAMPOCOM4 { get; set; } = "";
    public string CAMPOCOM5 { get; set; } = "";
    public string CAMPOCOM2 { get; set; } = "";
    public string CAMPOCOM7 { get; set; } = "";
    public string CAMPOCOM8 { get; set; } = "";
    public string REMITO { get; set; } = "";
    public DateTime ENTRPREV { get; set; }
    public string CONDVEN { get; set; } = "";

    /// <summary>
    ///     Estado que sirve para determinar si el item se va a actualizar o agregar en la base dedatos
    /// </summary>
    [NotMapped]
    public EstadoItem ESTADO { get; set; }

    //[Column("FLAG")]
    [NotMapped] public bool CONFIRMADO { get; set; }

    [NotMapped]
    public string COLOR
    {
        get => _color;
        set
        {
            _color = ESTADO_LOGISTICA switch
            {
                "Remitir" => "red",
                "Inspeccion" => "yellow",
                "Ret.Planta" => "greenyellow",
                "Ret.CABA" => "greenyellow",
                "Entregar" => "pink",
                "Facturar" => "maroon",
                "Pago" => "blue",
                _ => "black"
            };
        }
    }

    [NotMapped] public int SolicitudId { get; set; } = new();
}