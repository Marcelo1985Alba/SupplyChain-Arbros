using System;

namespace SupplyChain.Shared.DTOs;

public class SolicitudDTO
{
    public DateTime Fecha { get; set; }
    public string Producto { get; set; }
    public int Cantidad { get; set; } = 1;
    public int CalcId { get; set; }
    public string Cuit { get; set; }
    public string ContrapresionFija { get; set; } = string.Empty;
    public string ContrapresionVariable { get; set; } = string.Empty;
    public string PresionApertura { get; set; } = string.Empty;
    public string DescripcionFluido { get; set; } = string.Empty;
    public string TemperaturaDescargaT { get; set; } = string.Empty;
    public string CapacidadRequerida { get; set; } = string.Empty;
    public string DescripcionTag { get; set; } = string.Empty;
}