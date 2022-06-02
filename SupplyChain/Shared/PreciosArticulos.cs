using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupplyChain.Shared.Models
{
    [Table("PRECIOS_ARTICULOS")]
    public class PreciosArticulos :  EntityBase<string>
    {
        [ColumnaGridViewAtributo(Name = "Descripcion")]
        public string Descripcion { get; set; } = "";
        [ColumnaGridViewAtributo(Name = "Precio")]
        public decimal Precio { get; set; } = 0;
        [ColumnaGridViewAtributo(Name = "Moneda")]
        public string Moneda { get; set; } = "";
        [ColumnaGridViewAtributo(Name = "Marca")]
        public string Marca { get; set; } = "";
        [ColumnaGridViewAtributo(Name = "Construccion")]
        public string Construccion { get; set; } = "";

        [NotMapped]
        public bool ESNUEVO { get; set; }

        [NotMapped]
        public bool GUARDADO { get; set; }
    }
}