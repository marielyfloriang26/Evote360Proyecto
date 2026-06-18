namespace Evote360.Application.ViewModels.AlianzaPolitica;
public class SolicitudAlianzaViewModel
{
    public int Id { get; set; }
    public string PartidoNombre { get; set; } = null!;
    public string PartidoSiglas { get; set; } = null!;
    public string FechaSolicitud { get; set; } = null!; // Formateada como string para la vista
    public string Estado { get; set; } = null!;
}