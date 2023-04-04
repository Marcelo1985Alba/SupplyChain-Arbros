using System;

namespace SupplyChain.Shared
{
    public class vUsuario : EntityBase<string>
    {
        public string USUARIO { get; set; }
        public string EMAIL { get; set; }
        public string? CLIENTE { get; set; }
    }
}
