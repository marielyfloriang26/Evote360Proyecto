namespace Evote360.Application.ViewModels.AlianzaPolitica;

public class AlianzaPoliticaViewModel
{
    public bool ExisteEleccionActiva { get; set; }

    // Solicitudes de alianza enviadas por otros partidos al partido del dirigente autenticado
    public List<SolicitudAlianzaViewModel> SolicitudesPendientes { get; set; } = new();

    // Solicitudes de alianza enviadas por el partido del dirigente autenticado a otros partidos
    public List<SolicitudAlianzaViewModel> SolicitudesRealizadas { get; set; } = new();

    // Alianzas aceptadas donde participa el partido del dirigente autenticado
    public List<AlianzaVigenteViewModel> AlianzasVigentes { get; set; } = new();
}

