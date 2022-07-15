using SupplyChain.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SupplyChain
{
    public class Solution : EntityBase<string>
    {
        [Key, Column("Registro")]
        new public int Id { get; set; }
        public string CAMPO { get; set; }
        public string VALORC { get; set; }
        public string DESCRIP { get; set; }
    }
}
