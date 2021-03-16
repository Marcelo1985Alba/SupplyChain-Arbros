using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupplyChain.Shared.Login
{

    public class Rol
    {
        [Key]
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public bool Estados { get; set; }
    }
}
