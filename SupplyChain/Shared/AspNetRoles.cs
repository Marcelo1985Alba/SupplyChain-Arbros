using SupplyChain.Shared;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupplyChain
{
    [Table("aspnetroles")]
    public class AspNetRoles : EntityBase<String>
    {
        [Key, Column("Id")]
        new public String Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string NormalizedName { get; set; } = "";
        public string ConcurrencyStamp { get; set; } = "";
    }
}
