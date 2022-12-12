using SupplyChain.Shared;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupplyChain
{
    [Table("ISO")]
    public class ISO : EntityBase<int>
    {
        [Key, Column("Id")]
        new public int Id { get; set; } = 0;
        public int Identificacion { get; set; } = 0;
        public DateTime Fecha { get; set; }
        public string Descripcion { get; set; } = "";
        public string Detalle { get; set; } = "";
        public string Factor { get; set; } = "";
        public string Proceso { get; set; } = "";
        public string FODA { get; set; } = "";
        public string ImpAmb { get; set; } = "";
        public int AspAmb { get; set; } = 0;
        public string Frecuencia { get; set; } = "";
        public string Impacto { get; set; } = "";
        public string CondOperacion { get; set; } = "";
        public string CondControl { get; set; } = "";
        public string NaturalezaDelImpacto { get; set; } = "";
        public string Gestion { get; set; } = "";
        public string Comentarios { get; set; } = "";
        public string Medidas { get; set; } = "";
        public string USER { get; set; } = "";
        [NotMapped]
        public bool GUARDADO { get; set; }
        [NotMapped]
        public bool ESNUEVO { get; set; }
    }
}
