﻿using SupplyChain.Shared;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupplyChain
{
    [Table("AspectosAmbientales")]
    public class AspAmb : EntityBase<int>
    {
        [Key, Column("Id")]
        new public int Id { get; set; } = 0;
        public string descripcion { get; set; } = "";
        [NotMapped]
        public bool GUARDADO { get; set; }
        [NotMapped]
        public bool ESNUEVO { get; set; }
    }
}
