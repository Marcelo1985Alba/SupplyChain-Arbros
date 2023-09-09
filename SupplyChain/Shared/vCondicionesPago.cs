namespace SupplyChain.Shared;

public class vCondicionesPago : EntityBase<int>
{
    public string DESCRIPCION { get; set; }
    public int DIAS { get; set; }
    public bool CONTADO { get; set; }
}