namespace Evote360.Application.DTOs.AlianzaPolitica;

public class SolicitudAlianzaDto
{
    public int Id { get; set; }
    
    // Partido que envia la solicitud
    public int PartidoSolicitanteId { get; set; }
    public string PartidoSolicitanteNombre { get; set; } = null!;
    public string PartidoSolicitanteSiglas { get; set; } = null!;
    
    // Partido que recibe la solicitud
    public int PartidoReceptorId { get; set; }
    public string PartidoReceptorNombre { get; set; } = null!;
    public string PartidoReceptorSiglas { get; set; } = null!;
    
    public DateTime FechaSolicitud { get; set; }
    
    public string Estado { get; set; } = null!; 
}