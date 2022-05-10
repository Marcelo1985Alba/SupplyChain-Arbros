using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupplyChain.Shared
{
    [Table("Solicitud")]
    public class Solicitud : EntityBase<int>
    {
        public DateTime Fecha { get; set; } = DateTime.Now;
        [Required(ErrorMessage ="El Producto es requerido")]
        [StringLength(maximumLength: 15, MinimumLength = 15)]
        public string Producto { get; set; }
        [Range(minimum:1, maximum:9999999, ErrorMessage ="El Cliente es requerido")]
        public int CG_CLI { get; set; } = 0;
        public int TagId { get; set; } = 0;
        public string Cuit { get; set; } = "";

        [Range(minimum: 1, maximum: 9999999, ErrorMessage = "La Cantidad es requerida")]
        public int Cantidad { get; set; }
        public bool TienePresupuesto { get; set; }

        [NotMapped]
        public string Des_Cli { get; set; } = "";
        [NotMapped]
        public string Des_Prod { get; set; } = "";
        [NotMapped]
        public bool Guardado { get; set; } = false;
        [NotMapped]
        public bool EsNuevo { get; set; } = false;
    }
}
