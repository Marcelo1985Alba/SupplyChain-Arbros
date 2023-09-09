using System;

namespace SupplyChain.Shared;

public class vEstadPresupuestos
{
    public int PRESUP { get; set; }
    public DateTime FECHA { get; set; }
    public string CG_ART { get; set; }
    public string DES_ART { get; set; }
    public decimal CANTENT { get; set; }
    public decimal UNIDEQUI { get; set; }
    public int MES { get; set; }

    public int ANIO { get; set; }

    //public DateTime FECHA_PREV { get; set; }
    //public int MES_PREV { get; set; }
    //public int ANIO_PREV { get; set; }
    //public int SEMANA_PREV { get; set; }
    public double COTIZ { get; set; }

    public double TOT_DOL { get; set; }
    //public bool ESTADO { get; set; }
}