using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupplyChain.Shared
{
    [Table("Form2")]
    public class Formula : EntityBase<int>
    {
        [Key, Column("Registro")]
        new public int Id { get; set; }
        public string Cg_Prod { get; set; }
        public string Cg_Se { get; set; }
        public string Cg_Mat { get; set; }
        public string CANT_MAT { get; set; }
    }
}
