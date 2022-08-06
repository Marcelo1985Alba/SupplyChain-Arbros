using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupplyChain.Shared.DTOs
{
    public class SolicitudDTO
    {
        public DateTime Fecha { get; set; } = DateTime.Now;
        [Required(ErrorMessage = "El Producto es requerido")]
        [StringLength(maximumLength: 15, MinimumLength = 3, ErrorMessage = "El producto debe tener entre 3 y 15 digitos")]
        public string Producto { get; set; } = string.Empty;
        public int CalcId { get; set; } = 0;
        public string Cuit { get; set; } = string.Empty;

        [Range(minimum: 1, maximum: 9999999, ErrorMessage = "La Cantidad es requerida")]
        public int Cantidad { get; set; }
        public string ContrapresionFija { get; set; }
        public string ContrapresionVariable { get; set; }
        public string PresionApertura { get; set; }
        public string DescripcionFluido { get; set; }
        public string TemperaturaDescargaT { get; set; }
        public string CapacidadRequerida { get; set; }
        public string DescripcionTag { get; set; }
    }
}
