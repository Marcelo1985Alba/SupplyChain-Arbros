using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace SupplyChain.Shared
{
    [Table("PRECIOS_ARTICULOS")]
    public class PrecioArticulo: EntityBase<string>
    {
        [Required(ErrorMessage ="La Descripcion es requerida")]
        public string Descripcion { get; set; }
        [Required(ErrorMessage ="El Precio es requerido")]
        public decimal Precio { get; set; } = 0;
        [Required(ErrorMessage = "La Moneda es requerida")]
        public string Moneda { get; set; }
        [StringLength(maximumLength:500, ErrorMessage ="La Marca debete tener como Maximo 500 caracteres")]
        public string Marca { get; set; }
        [StringLength(maximumLength:500, ErrorMessage ="La Construccion debete tener como Maximo 500 caracteres")]
        public string Construccion { get; set; }
    }
}
