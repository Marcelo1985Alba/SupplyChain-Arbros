using SupplyChain.Shared;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupplyChain
{
    [Table("MantCeldas")]
    public class MantCeldas : EntityBase<int>
    {
        [Key, Column("Id")]
        public new int Id { get; set; }
        public string Cg_Celda { get; set; } = "";
        public string Des_Celda { get; set; } = "";
        public DateTime? Fecha { get; set; }
        public string Mantenimiento { get; set; } = "";
        public string Tarea { get; set; } = "";
        public string Causa { get; set; } = "";
        public decimal TiempoParada { get; set; } = 0;
        public string Repuesto { get; set; } = "";
        public decimal Costo { get; set; } = 0;
        public string Operario { get; set; } = "";
        public string Operador { get; set; } = "";
        public string Proveedor { get; set; } = "";
        public string Estado { get; set; } = "";
        public DateTime? FechaCumplido { get; set; }
		[NotMapped]
        public bool GUARDADO { get; set; }
        [NotMapped]
        public bool ESNUEVO { get; set; }
    }
}




      