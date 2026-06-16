namespace Evote360.Application.DTOs.AlianzaPolitica;

public class AlianzaPoliticaDto
{
    public int Id { get; set; }
    public int EleccionId { get; set; }
    public string EleccionNombre { get; set; } = null!;

    // Datos del partido mayorista 
    public int PartidoMayoristaId { get; set; }
    public string PartidoMayoristaNombre { get; set; } = null!;
    public string PartidoMayoristaSiglas { get; set; } = null!;

    // Datos del partido aliado
    public int PartidoAliadoId { get; set; }
    public string PartidoAliadoNombre { get; set; } = null!;
    public string PartidoAliadoSiglas { get; set; } = null!;

    // Fecha en que se consolido la alianza 
    public DateTime FechaAceptacion { get; set; }
}