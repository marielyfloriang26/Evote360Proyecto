namespace Evote360.Application.ViewModels.AlianzaPolitica;

public class AlianzaVigenteViewModel
{
    public int Id { get; set; }
    public string PartidoAliadoNombre { get; set; } = null!;
    public string PartidoAliadoSiglas { get; set; } = null!;
    public string FechaAceptacion { get; set; } = null!; // Formateada de manera limpia
}