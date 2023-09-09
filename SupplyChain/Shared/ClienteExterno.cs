using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SupplyChain.Shared;

namespace SupplyChain;

public class ClienteExterno : EntityBase<string>
{
    public int? ID_CON_VEN { get; set; } = 0;
    public int? ID_CON_ENT { get; set; } = 0;
    public decimal? DESC_COMERCIAL { get; set; } = 0;

    [Display(Name = "Codigo")] public string CG_CLI { get; set; } = "";

    [Display(Name = "Descripcion")] public string DESCRIPCION { get; set; } = "";

    [Display(Name = "Cuit")] public string CUIT { get; set; } = "";

    public string PROVINCIA { get; set; } = "";
    public string PAIS { get; set; } = "";
    public string RUBRO { get; set; } = "";
    public string MERCADO { get; set; } = "";

    [NotMapped] public List<vDireccionesEntrega> DireccionesEntrega { get; set; } = new();
}