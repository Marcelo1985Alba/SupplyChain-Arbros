using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupplyChain.Shared.Models;

[Table("Proveedores_compras")]
public class Proveedores_compras
{
    [Key] public int NROCLTE { get; set; }

    public string DES_PROVE { get; set; }
}