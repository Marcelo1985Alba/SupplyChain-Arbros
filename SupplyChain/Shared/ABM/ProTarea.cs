using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupplyChain;

[Table("ProTarea")]
public class ProTarea
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string TAREAPROC { get; set; } = ""; //

    public string DESCRIP { get; set; } = ""; //
    public string OBSERVAC { get; set; } = ""; //
    public int CG_CIA { get; set; } = 0; //

    //cg_cia
}