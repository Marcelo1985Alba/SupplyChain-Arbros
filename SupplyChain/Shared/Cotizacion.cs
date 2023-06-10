using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupplyChain.Shared
{
    [Table("ARBROS.dbo.ERP_COTIZACIONES")]
    public class Cotizaciones : EntityBase<int>
    {
        [Key, Column("ID")]
        new public int Id { get; set; }
        public double COTIZACION { get; set; }
        public DateTime? FEC_ULT_ACT { get; set; }
    }
}
