using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace SupplyChain.Shared;

public class ApplicationUser : IdentityUser
{
    public ApplicationUser()
    {
        ChatMessagesFromUsers = new HashSet<ChatMessage>();
        ChatMessagesToUsers = new HashSet<ChatMessage>();
    }

    public int Cg_Cli { get; set; } = 0;
    public byte[]? Foto { get; set; }
    public virtual ICollection<ChatMessage> ChatMessagesFromUsers { get; set; }
    public virtual ICollection<ChatMessage> ChatMessagesToUsers { get; set; }


    [NotMapped] public string NombreCliente { get; set; } = string.Empty;

    [NotMapped] public List<string> Roles { get; set; } = new();

    [NotMapped] public bool EsNuevo { get; set; } = false;
}