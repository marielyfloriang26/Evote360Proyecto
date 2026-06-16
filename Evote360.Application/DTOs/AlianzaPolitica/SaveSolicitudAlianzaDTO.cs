namespace Evote360.Application.DTOs.Alianza;

public class SaveSolicitudAlianzaDto
{
    public int Id { get; set; }
    public int PartidoSolicitanteId { get; set; }
    public int PartidoReceptorId { get; set; }
    public string Estado { get; set; } = "En espera de respuesta";
}