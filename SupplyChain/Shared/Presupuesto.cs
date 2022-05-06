using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupplyChain.Shared
{
    [Table("Prescli")]
    public class Presupuesto : EntityBase<int>
    {
        [Key, Column("REGISTRO")]
        new public int Id { get; set; }
        [Column("FE_PED")]
        public DateTime FE_PRESP { get; set; } = DateTime.Now;
        public int PRESUP { get; set; }
        [StringLength(maximumLength: 15, MinimumLength = 15)]
        public string CG_ART { get; set; }
        public string DES_ART { get; set; }
        public string UNID { get; set; }
        public decimal CANTENT { get; set; }
        public int CG_CLI { get; set; } = 0;
        public string DES_CLI { get; set; }
        public DateTime FE_REG { get; set; } = DateTime.Now;

        public int OBRA { get; set; } = 0;
    }
}
