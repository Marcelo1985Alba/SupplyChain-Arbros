using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupplyChain.Shared
{
    [Table("CampoComodines")]
    public class CampoComodin : EntityBase<int>
    {
        [Key]
        public new int Id { get; set; } = 0;
        [Column("Tabla")] public string Tabla { get; set; } = "";
        [Column("Campocom1")] public string Presion { get; set; } = "";
        [Column("Campocom2")] public string Resorte { get; set; } = "";
        [Column("Campocom3")] public string Fluido { get; set; } = "";
        [Column("Campocom4")] public string Ajuste_Banco{ get; set; } = "";
        [Column("Campocom5")] public string Contra_Presion{ get; set; } = "";
        [Column("Campocom6")] public string Temperatura{ get; set; } = "";
        [Column("Campocom7")] public string CampoCom7 { get; set; } = "";
        [Column("Campocom8")] public string CampoCom8 { get; set; } = "";
        [NotMapped] public bool ESNUEVO {  get; set; }
        [NotMapped] public bool GUARDADO{ get; set; }

    }
}
