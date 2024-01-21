using System;

namespace SupplyChain.Shared.CDM
{
    public class PLANNER
    {
        public int NewId { get; set; }
        public int Max_T_CICLO { get; set; }
        public string DES_PROD { get; set; }
        public string CG_PROD { get; set; }
        public int CG_ORDF { get; set; }
        public int? PRIORIDAD { get; set; }
        public string CG_CELDA { get; set; }
        public int? PEDIDO { get; set; }
        public int? CG_CLI { get; set; }
        public int? ULT_ASOC { get; set; }
        public DateTime INICIO { get; set; }
        public DateTime FE_ENTREGA { get; set; }
        public DateTime FIN { get; set; }
        public string PROCESO { get; set; }
        public int CANT { get; set; }
        public int CG_ORDFASOC { get; set; }
    }
}
