using SupplyChain.Shared;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupplyChain
{
    [Table("Lineas")]
    public class Lineas : EntityBase<int>
    {
        [Key, Column("CG_LINEA")]
        new public int Id { get; set; } = 0;
        public string DES_LINEA { get; set; } = "";
        [NotMapped]
        public bool GUARDADO { get; set; }
        [NotMapped]
        public bool ESNUEVO { get; set; }
    }
}
