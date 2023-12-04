using System;

namespace SupplyChain.Shared.CDM
{
    public class PLANNER_CN1
    {
        public long NewId { get; set; }
        public int Max_T_CICLO { get; set; }
        public string DES_PROD { get; set; }
        public string CG_PROD { get; set; }
        public long CG_ORDF { get; set; }
        public DateTime INICIO { get; set; }
        public DateTime FIN { get; set; }
        public int CANT_ACTUALIZADA { get; set; }
    }
}
