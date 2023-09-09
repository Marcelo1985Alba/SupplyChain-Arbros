using System;

namespace SupplyChain.Shared;

public class vEventos
{
    public DateTime FE_EMIT { get; set; }
    public int Cg_NoConf { get; set; }
    public int Cg_TipoNc { get; set; }
    public string Des_TipoNc { get; set; }
    public string Observaciones { get; set; }
    public DateTime? Fe_Ocurrencia { get; set; }
    public DateTime? Fe_Aprobacion { get; set; }
    public decimal Cg_Cli { get; set; }
    public string Cg_Prod { get; set; }
    public string? DES_CLI { get; set; }
    public string? DES_PROVE { get; set; }
    public string DES_PROD { get; set; }
    public string Lote { get; set; }
    public string Serie { get; set; }
    public string Despacho { get; set; }
    public int Cg_Ordf { get; set; }
    public int Pedido { get; set; }
    public string Usuario { get; set; }
    public int OCOMPRA { get; set; }
    public decimal CANT { get; set; }
    public int? Cg_NoConfAcc { get; set; }
    public string? Texto { get; set; }
    public int? Orden { get; set; }
    public string? ObservacionesAccion { get; set; }
    public DateTime? fe_implemen { get; set; }
    public string Origen { get; set; }
    public DateTime? FE_SOLUC { get; set; }
    public DateTime? fe_cierre { get; set; }
    public bool Aprob { get; set; }
    public string Comentarios { get; set; }
}