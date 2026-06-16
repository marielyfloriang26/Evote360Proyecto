using Evote360.Application.DTOs.Alianza;
using Evote360.Application.DTOs.AlianzaPolitica;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Evote360.Application.Interfaces;

public interface IAlianzaPoliticaService
{
    // Obtiene todas las solicitudes y alianzas de un partido especifico para la pantalla principal
    Task<IEnumerable<SolicitudAlianzaDto>> GetSolicitudesRecibidasAsync(int partidoId);
    Task<IEnumerable<SolicitudAlianzaDto>> GetSolicitudesEnviadasAsync(int partidoId);
    Task<IEnumerable<AlianzaPoliticaDto>> GetAlianzasVigentesAsync(int partidoId);

    // Obtiene los partidos activos con los que el partido actual si puede solicitar una alianza
    Task<IEnumerable<SelectListItemDto>> GetPartidosDisponiblesParaAlianzaAsync(int partidoActualId);

    // acciones principales
    Task<string?> CrearSolicitudAsync(SaveSolicitudAlianzaDto dto);
    Task<string?> AceptarSolicitudAsync(int solicitudId, int partidoAutenticadoId);
    Task<string?> RechazarSolicitudAsync(int solicitudId, int partidoAutenticadoId);
    Task<string?> EliminarSolicitudAsync(int solicitudId, int partidoAutenticadoId);
    Task<string?> EliminarAlianzaAsync(int alianzaId, int partidoAutenticadoId);

    
    Task<bool> ExisteEleccionActivaAsync();
    Task<int?> ObtenerPartidoIdPorUsuarioIdAsync(int usuarioId);
}