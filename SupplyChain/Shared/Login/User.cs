using SupplyChain.Shared.Login;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SupplyChain.Shared.Models
{
    public class Usuarios : EntityBase<int>
    {
        [Key]
        public string Id { get; set; }
        
        [Required(ErrorMessage = "Nombre de Usuario Requerido")]
        public string Usuario { get; set; } = ""; 
        public int Cg_TipoUsu { get; set; } = 0;
        public string Nombre { get; set; } = "";
        public string Email { get; set; } = "";
        [Required(ErrorMessage = "Contraseña Requerida")]
        public string Contras { get; set; } = "";
        public int Derechos { get; set; } = 0;
        public int Cg_Cia { get; set; } = 0;
        public string UltimoPuntoVenta { get; set; } = "";
        public int Cg_Area { get; set; } = 0;
        public bool CG_CUENTRAPI { get; set; } = false;
        public int RolId { get; set; }
        public virtual Rol Rol { get; set; }
    }
}
