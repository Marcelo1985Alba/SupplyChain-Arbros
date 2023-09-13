using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupplyChain.Shared
{
    public class vProveedorItris : EntityBase<int>
    {
        [Key, Column("CG_PROVE")]
        public new int Id { get; set; }
        public string DESCRIPCION { get; set; }
        public string CUIT { get; set; }
        public string? NOMBRE_CONTACTO { get; set; }
        public string? EMAIL_CONTACTO { get; set; }
        public string CALLE { get; set; }    
        public string? TE { get; set; }
    }
}
