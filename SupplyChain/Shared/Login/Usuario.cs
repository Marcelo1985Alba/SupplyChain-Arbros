using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupplyChain.Shared.Login
{
    public class Usuario
    {
        public string Id { get; set; }
        public string Nombre { get; set; }
        public string Email { get; set; }
        //[NotMapped]
        //public byte[] Foto{ get; set; }
        public byte[] Foto { get; set; }
        

    }
}
