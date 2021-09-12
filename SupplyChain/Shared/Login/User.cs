using SupplyChain.Shared.Login;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SupplyChain.Shared.Models
{
    public class Usuarios
    {
        [Key]
        public string Id { get; set; }
        [Required(ErrorMessage = "Nombre de Usuario Requerido")] public string Usuario { get; set; } = ""; 
        public int Cg_TipoUsu { get; set; } = 0;
        public string Nombre { get; set; } = "";
        public string Email { get; set; } = "";
        
        [Required(ErrorMessage = "Nombre de Contraseña Requerido")] public string Contras { get; set; } = "";
        public List<Rol> Roles { get; set; } = new List<Rol>();
    }
}
