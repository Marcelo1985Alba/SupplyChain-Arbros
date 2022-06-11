using SupplyChain.Shared.Models;
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
        [Required(ErrorMessage = "El Producto es requerido")]
        [StringLength(maximumLength: 15, MinimumLength = 3, ErrorMessage = "El producto debe tener entre 3 y 15 digitos")]
        public string Producto { get; set; } = "";
        //[Range(minimum:1, maximum:9999999, ErrorMessage ="El Cliente es requerido")]
        public int CG_CLI { get; set; } = 0;
        public int CalcId { get; set; } = 0;
        public string Cuit { get; set; } = "";

        [Range(minimum: 1, maximum: 9999999, ErrorMessage = "La Cantidad es requerida")]
        public int Cantidad { get; set; }
        public bool TienePresupuesto { get; set; }

        public string Medidas { get; set; } = "";
        public string Orificio { get; set; } = "";
        public string SerieEntrada { get; set; } = "";
        public string TipoEntrada { get; set; } = "";
        public string SerieSalida { get; set; } = "";
        public string TipoSalida { get; set; } = "";
        public string Accesorios { get; set; } = "";
        public string Asiento { get; set; } = "";
        public string Bonete { get; set; } = "";
        public string Cuerpo { get; set; } = "";
        public string Resorte { get; set; } = "";
        public string Disco { get; set; } = "";
        public string Tobera { get; set; } = "";
        public PresupuestoDetalle PresupuestoDetalle { get; set; }
        [NotMapped] public PreciosArticulos PrecioArticulo { get; set; }
        [NotMapped] public string Des_Cli { get; set; } = "";
        [NotMapped] public string Des_Prod { get; set; } = "";
        [NotMapped] public bool Guardado { get; set; } = false;
        [NotMapped] public bool EsNuevo { get; set; } = false;
    }
}
