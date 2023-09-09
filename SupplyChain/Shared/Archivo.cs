namespace SupplyChain.Shared;

public class Archivo
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Directorio { get; set; }
    public string[] Contenido { get; set; }
    public byte[] ContenidoByte { get; set; }
    public string DocumentPath { get; set; }
    public bool IsAzure { get; set; } = false;
}