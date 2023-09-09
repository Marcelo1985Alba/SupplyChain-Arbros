using System.ComponentModel.DataAnnotations;

namespace SupplyChain.Shared.Models;

public class FormulaPlanificacion
{
    [Key] public int CG_FORM { get; set; } = 0;

    public string DES_FORM { get; set; } = "";
    public string ACTIVO { get; set; } = "";
    public int REVISION { get; set; } = 0;
}