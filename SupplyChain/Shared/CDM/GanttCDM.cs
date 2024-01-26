using System;

namespace SupplyChain.Shared.CDM
{
    public class GanttCDM
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string CG_PROD { get; set; }
        public string DES_PROD { get; set; }
        public long CG_ORDF { get; set; }
        public DateTime INICIO { get; set; }
        public DateTime FIN { get; set; }
        public int CANT_ACTUALIZADA { get; set; }
        public string parentID { get; set; }
        public string progress { get; set; }
        public string duration { get; set; }
    }
}
