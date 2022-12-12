using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupplyChain.Shared
{
    public class vUsuario : EntityBase<string>
    {
        public string USUARIO { get; set; }
        public string EMAIL { get; set; }
        public string? CLIENTE { get; set; }
    }
}
