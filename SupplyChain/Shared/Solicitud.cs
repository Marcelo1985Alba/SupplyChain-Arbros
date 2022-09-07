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
        public string Producto { get; set; } = string.Empty;
        //[Range(minimum:1, maximum:9999999, ErrorMessage ="El Cliente es requerido")]
        public int CG_CLI { get; set; } = 0;
        public int CalcId { get; set; } = 0;
        public string Cuit { get; set; } = string.Empty;

        [Range(minimum: 1, maximum: 9999999, ErrorMessage = "La Cantidad es requerida")]
        public int Cantidad { get; set; }
        public string ContrapresionFija { get; set; } = string.Empty;
        public string ContrapresionVariable { get; set; } = string.Empty;
        public string PresionApertura { get; set; } = string.Empty;
        public string DescripcionFluido { get; set; } = string.Empty;
        public string TemperaturaDescargaT { get; set; } = string.Empty;
        public string CapacidadRequerida { get; set; } = string.Empty;
        public string DescripcionTag { get; set; } = string.Empty;
        [StringLength(maximumLength:500, ErrorMessage = "*Las observaciones tienen un maximo de 500 caracteres")]
        public string Observaciones { get; set; } = string.Empty;
        public bool TienePresupuesto { get; set; } = false;
        public PresupuestoDetalle PresupuestoDetalle { get; set; }

        [NotMapped] public PreciosArticulos PrecioArticulo { get; set; }
        [NotMapped] public string Des_Cli { get; set; } = string.Empty;
        [NotMapped] public string Des_Prod { get; set; } = string.Empty;
        [NotMapped] public bool Guardado { get; set; } = false;
        [NotMapped] public bool EsNuevo { get; set; } = false;
    }
}
