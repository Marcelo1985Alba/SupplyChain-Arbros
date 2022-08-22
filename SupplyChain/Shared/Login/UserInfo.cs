﻿using System.ComponentModel.DataAnnotations;

namespace SupplyChain.Shared.Login
{
    public class UserInfo
    {
        [Required(ErrorMessage = "El Nombre de Usuario requerido")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "La Contraseña es requerida")]
        public string Password { get; set; }

        public int Cg_Cli { get; set; } = 0;

    }
}