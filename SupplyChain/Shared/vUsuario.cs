using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata;
using static System.Net.Mime.MediaTypeNames;

namespace SupplyChain.Shared
{
    public class vUsuario : EntityBase<string>
    {
        public string USUARIO { get; set; }
        public string EMAIL { get; set; }
        public string? CLIENTE { get; set; }
        public byte[] FOTO { get; set; }  
        
    }
}
