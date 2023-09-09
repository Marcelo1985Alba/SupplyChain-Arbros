using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SupplyChain.Shared;

namespace SupplyChain;

[Table("ISO")]
public class ISO : EntityBase<int>
{
    [Key] [Column("Id")] public new int Id { get; set; } = 0;

    public int Identificacion { get; set; } = 0;
    public DateTime Fecha { get; set; }
    public string Descripcion { get; set; } = "";
    public string Detalle { get; set; } = "";
    public string Factor { get; set; } = "";
    public string Proceso { get; set; } = "";
    public string FODA { get; set; } = "";
    public string ImpAmb { get; set; } = "";
    public int AspAmb { get; set; } = 0;
    public string Frecuencia { get; set; } = "";
    public string Impacto { get; set; } = "";
    public string CondOperacion { get; set; } = "";
    public string CondControl { get; set; } = "";
    public string NaturalezaDelImpacto { get; set; } = "";
    public string Gestion { get; set; } = "";
    public string Comentarios { get; set; } = "";
    public string Medidas { get; set; } = "";
    public DateTime? FechaCumplido { get; set; }
    public string USER { get; set; } = "";

    [NotMapped] public int PUNTAJE => getPuntaje();

    [NotMapped] public bool GUARDADO { get; set; }

    [NotMapped] public bool ESNUEVO { get; set; }

    protected int getPuntaje()
    {
        var prob = 0;
        var imp = 0;
        var gestion = 0;
        if (Gestion.Trim() == "Optima")
            gestion = -10;
        else if (Gestion.Trim() == "Sin gestion")
            gestion = 10;
        switch (Frecuencia.Trim())
        {
            case "Muy baja":
                prob = 1;
                break;
            case "Baja":
                prob = 3;
                break;
            case "Media":
                prob = 5;
                break;
            case "Alta":
                prob = 7;
                break;
            case "Muy alta":
                prob = 9;
                break;
        }

        switch (Impacto.Trim())
        {
            case "Muy poco":
                imp = 1;
                break;
            case "Poco":
                imp = 3;
                break;
            case "Moderado":
                imp = 5;
                break;
            case "Alto":
                imp = 7;
                break;
            case "Muy alto":
                imp = 9;
                break;
        }

        return prob * imp + gestion;
    }
}