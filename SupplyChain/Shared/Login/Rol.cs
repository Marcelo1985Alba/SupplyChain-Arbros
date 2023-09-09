using System.ComponentModel.DataAnnotations;

namespace SupplyChain.Shared.Login;

public class Rol
{
    [Key] public int Id { get; set; }

    public string Descripcion { get; set; }
    public bool Estados { get; set; }
}