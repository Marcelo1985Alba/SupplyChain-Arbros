using System.ComponentModel.DataAnnotations;

namespace SupplyChain.Shared.Models;

public class Tire
{
    public int Tipoo { get; set; } = 0;

    [Required(ErrorMessage = "Ingresar Tipo de Operacion")]
    public string Descrip { get; set; } = "";
}