using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupplyChain.Shared
{
    public class SolCotEmail : EntityBase<decimal>
    {
        [Key, Column("REGISTRO"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public new decimal Id { get; set; }
        public DateTime FE_SOLCOT { get; set; }
        public string USUARIO { get; set; }
        public decimal CG_CIA { get; set; }
        public string CG_MAT { get; set; }
        public decimal CG_PROVE { get; set; }
        public string CONTACTO { get; set; }
        public string EMAIL { get; set; }
        public decimal REGISTRO_COMPRAS { get; set; }
        public DateTime FE_PREV { get; set; }
        public decimal CANTIDAD { get; set; }
        public string UNIDAD { get; set; }
        public string ASUNTO_EMAIL { get; set; }
        public string MENSAJE_EMAIL { get; set; }

        [NotMapped]
        public string Proveedor { get; set; }
        [NotMapped]
        public string NombreInsumo { get; set; }
    }
}
