using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupplyChain.Shared
{
    public class ApplicationUser : IdentityUser
    {
        public int Cg_Cli { get; set; } = 0;
        public virtual ICollection<ChatMessage> ChatMessagesFromUsers { get; set; }
        public virtual ICollection<ChatMessage> ChatMessagesToUsers { get; set; }
        public ApplicationUser()
        {
            ChatMessagesFromUsers = new HashSet<ChatMessage>();
            ChatMessagesToUsers = new HashSet<ChatMessage>();
        }


        [NotMapped]
        public string NombreCliente { get; set; } = string.Empty;

        [NotMapped]
        public List<string> Roles { get; set; } = new();

        [NotMapped]
        public bool EsNuevo { get; set; } = false;
    }
}
