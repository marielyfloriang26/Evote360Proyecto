using Evote360.Application.DTOs.Alianza;
using Evote360.Application.DTOs.AlianzaPolitica;
using Evote360.Application.Interfaces;
using Evote360.Core.Entities;
using Evote360.Core.Interfaces;

namespace Evote360.Application.Services;

public class AlianzaPoliticaService : IAlianzaPoliticaService
{
    private readonly IAlianzaPoliticaRepository _allianceRepository;
    private readonly IRepositoryAsync<PartidoPolitico> _partyRepository;
    private readonly IRepositoryAsync<Eleccion> _electionRepository;
    private readonly IRepositoryAsync<AsignacionDirigente> _dirigenteRepository;
    private readonly IRepositoryAsync<Candidato> _candidateRepository; 
    private readonly IRepositoryAsync<AsignarCandidatoPuesto> _asignacionCandidatoRepository;
    

    public AlianzaPoliticaService(
        IAlianzaPoliticaRepository allianceRepository,
        IRepositoryAsync<PartidoPolitico> partyRepository, IRepositoryAsync<Eleccion> electionRepository, IRepositoryAsync<AsignacionDirigente> dirigenteRepository, IRepositoryAsync<Candidato> candidateRepository,
        IRepositoryAsync<AsignarCandidatoPuesto> asignacionCandidatoRepository)
    {
        _allianceRepository = allianceRepository;
        _partyRepository = partyRepository;
        _electionRepository = electionRepository;
        _dirigenteRepository = dirigenteRepository;
        _candidateRepository = candidateRepository;
        _asignacionCandidatoRepository = asignacionCandidatoRepository;
    }

    // Solicitudes recibidas de otros partidos
    public async Task<IEnumerable<SolicitudAlianzaDto>> GetSolicitudesRecibidasAsync(int partidoId)
    {
        var todasLasAlianzas = await _allianceRepository.GetAllAsync();
        var todosLosPartidos = await _partyRepository.GetAllAsync();

        // si es PartidoAliadoId y el Estado es false, significa que la enviaron y esta en espera de respuesta
        return todasLasAlianzas
            .Where(a => a.PartidoAliadoId == partidoId && a.Estado == false)
            .Select(a => {
                var solicitante = todosLosPartidos.FirstOrDefault(p => p.Id == a.PartidoMayoristaId);
                var receptor = todosLosPartidos.FirstOrDefault(p => p.Id == a.PartidoAliadoId);
                
                return new SolicitudAlianzaDto
                {
                    Id = a.Id,
                    PartidoSolicitanteId = a.PartidoMayoristaId,
                    PartidoSolicitanteNombre = solicitante?.Nombre ?? "Desconocido",
                    PartidoSolicitanteSiglas = solicitante?.Siglas ?? "N/A",
                    PartidoReceptorId = a.PartidoAliadoId,
                    PartidoReceptorNombre = receptor?.Nombre ?? "Desconocido",
                    PartidoReceptorSiglas = receptor?.Siglas ?? "N/A",
                    Estado = "En espera de respuesta" // el dto le pasa el string exacto 
                };
            }).ToList();
    }

    // Solicitudes realizadas 
    public async Task<IEnumerable<SolicitudAlianzaDto>> GetSolicitudesEnviadasAsync(int partidoId)
    {
        var todasLasAlianzas = await _allianceRepository.GetAllAsync();
        var todosLosPartidos = await _partyRepository.GetAllAsync();

        // Si es el PartidoMayoristaId, significa que inicio el registro
        return todasLasAlianzas
            .Where(a => a.PartidoMayoristaId == partidoId)
            .Select(a => {
                var solicitante = todosLosPartidos.FirstOrDefault(p => p.Id == a.PartidoMayoristaId);
                var receptor = todosLosPartidos.FirstOrDefault(p => p.Id == a.PartidoAliadoId);
                
                return new SolicitudAlianzaDto
                {
                    Id = a.Id,
                    PartidoSolicitanteId = a.PartidoMayoristaId,
                    PartidoSolicitanteNombre = solicitante?.Nombre ?? "Desconocido",
                    PartidoSolicitanteSiglas = solicitante?.Siglas ?? "N/A",
                    PartidoReceptorId = a.PartidoAliadoId,
                    PartidoReceptorNombre = receptor?.Nombre ?? "Desconocido",
                    PartidoReceptorSiglas = receptor?.Siglas ?? "N/A",
                    // Traduce el booleano al texto que el controlador mapeara al vm
                    Estado = a.Estado ? "Aceptada" : "En espera de respuesta" 
                };
            }).ToList();
    }

    // Alianzas vigentes 
    public async Task<IEnumerable<AlianzaPoliticaDto>> GetAlianzasVigentesAsync(int partidoId)
    {
        var todasLasAlianzas = await _allianceRepository.GetAllAsync();
        var todosLosPartidos = await _partyRepository.GetAllAsync();

        // Una alianza esta vigente si Estado == true y el partido participa 
        return todasLasAlianzas
            .Where(a => (a.PartidoMayoristaId == partidoId || a.PartidoAliadoId == partidoId) && a.Estado == true)
            .Select(a => {
                var mayorista = todosLosPartidos.FirstOrDefault(p => p.Id == a.PartidoMayoristaId);
                var aliado = todosLosPartidos.FirstOrDefault(p => p.Id == a.PartidoAliadoId);
                
                return new AlianzaPoliticaDto
                {
                    Id = a.Id,
                    EleccionId = a.EleccionId,
                    PartidoMayoristaId = a.PartidoMayoristaId,
                    PartidoMayoristaNombre = mayorista?.Nombre ?? "",
                    PartidoMayoristaSiglas = mayorista?.Siglas ?? "",
                    PartidoAliadoId = a.PartidoAliadoId,
                    PartidoAliadoNombre = aliado?.Nombre ?? "",
                    PartidoAliadoSiglas = aliado?.Siglas ?? ""
                };
            }).ToList();
    }

    // Llena el Select de creacion 
    public async Task<IEnumerable<SelectListItemDto>> GetPartidosDisponiblesParaAlianzaAsync(int partidoActualId)
    {
        var todosLosPartidos = await _partyRepository.GetAllAsync();
        var todasLasAlianzas = await _allianceRepository.GetAllAsync();

        // Partidos que esten activos y que no sea mi propio partido
        var partidosFiltrados = todosLosPartidos
            .Where(p => p.Id != partidoActualId && p.Estado == true)
            .ToList();

        var resultado = new List<SelectListItemDto>();

        foreach (var partido in partidosFiltrados)
        {
            // No debe existir ya ninguna relacion registrada (sea pendiente o aceptada) en ninguna direccion
            bool existeRelacion = todasLasAlianzas.Any(a => 
                (a.PartidoMayoristaId == partidoActualId && a.PartidoAliadoId == partido.Id) ||
                (a.PartidoMayoristaId == partido.Id && a.PartidoAliadoId == partidoActualId));

            if (existeRelacion) continue; // Si ya hay registro, se ignora y no aparece en el select

            // Si pasa los filtros se añade 
            resultado.Add(new SelectListItemDto
            {
                Value = partido.Id.ToString(),
                Text = $"{partido.Nombre} ({partido.Siglas})"
            });
        }

        return resultado;
    }


    public async Task<bool> ExisteEleccionActivaAsync()
    {
        var elecciones = await _electionRepository.GetAllAsync();
        return elecciones.Any(e => e.EstadoElectoral.Equals("Activa", StringComparison.OrdinalIgnoreCase));
    }

    // CREAR SOLICITUD DE ALIANZA
    public async Task<string?> CrearSolicitudAsync(SaveSolicitudAlianzaDto dto)
    {
        if (await ExisteEleccionActivaAsync())
            return "No se puede crear una solicitud de alianza mientras exista una elección activa.";

        if (dto.PartidoSolicitanteId == dto.PartidoReceptorId)
            return "No puede crear una solicitud de alianza hacia su propio partido político.";

        var todosLosPartidos = await _partyRepository.GetAllAsync();
        var receptor = todosLosPartidos.FirstOrDefault(p => p.Id == dto.PartidoReceptorId);
        if (receptor == null || receptor.Estado == false)
            return "No puede crear una solicitud de alianza con un partido político inactivo.";

        var todasLasAlianzas = await _allianceRepository.GetAllAsync();
        
        // Valida duplicados en ambas direcciones
        bool yaExisteRegistro = todasLasAlianzas.Any(a => 
            (a.PartidoMayoristaId == dto.PartidoSolicitanteId && a.PartidoAliadoId == dto.PartidoReceptorId) ||
            (a.PartidoMayoristaId == dto.PartidoReceptorId && a.PartidoAliadoId == dto.PartidoSolicitanteId));

        if (yaExisteRegistro)
        {
                var registroPrevio = todasLasAlianzas.FirstOrDefault(a => 
                (a.PartidoMayoristaId == dto.PartidoSolicitanteId && a.PartidoAliadoId == dto.PartidoReceptorId) ||
                (a.PartidoMayoristaId == dto.PartidoReceptorId && a.PartidoAliadoId == dto.PartidoSolicitanteId));

            if (registroPrevio!.Estado == true)
                return "Ya existe una alianza vigente con este partido político.";
            
            if (registroPrevio.PartidoMayoristaId == dto.PartidoSolicitanteId)
                return "Ya existe una solicitud de alianza pendiente enviada a este partido político.";
            else
                return "Ya existe una solicitud de alianza pendiente enviada por este partido político.";
        }

        // Busca una eleccion disponible para asociar el registro obligatorio
        var elecciones = await _electionRepository.GetAllAsync();
        var eleccionId = elecciones.FirstOrDefault()?.Id ?? 1;

        // Mapea del DTO a la Entidad antes de ir al repositorio
        var nuevaAlianza = new AlianzaPolitica
        {
            EleccionId = eleccionId,
            PartidoMayoristaId = dto.PartidoSolicitanteId,
            PartidoAliadoId = dto.PartidoReceptorId,
            Estado = false // false = "En espera de respuesta"
        };

        await _allianceRepository.AddAsync(nuevaAlianza);
        return null; // Éxito
    }

    // ACEPTAR SOLICITUD DE ALIANZA
    public async Task<string?> AceptarSolicitudAsync(int solicitudId, int partidoAutenticadoId)
    {
        var alianza = await _allianceRepository.GetByIdAsync(solicitudId);
        
        if (alianza == null)
            return "La solicitud de alianza seleccionada no existe.";

        if (alianza.PartidoAliadoId != partidoAutenticadoId)
            return "No tiene permisos para responder esta solicitud de alianza.";

        if (alianza.Estado == true)
            return "Esta solicitud de alianza ya fue respondida.";

        if (await ExisteEleccionActivaAsync())
            return "No se puede aceptar una solicitud de alianza mientras exista una elección activa.";

        var todosLosPartidos = await _partyRepository.GetAllAsync();
        var solicitante = todosLosPartidos.FirstOrDefault(p => p.Id == alianza.PartidoMayoristaId);
        var receptor = todosLosPartidos.FirstOrDefault(p => p.Id == alianza.PartidoAliadoId);

        if (solicitante == null || solicitante.Estado == false || receptor == null || receptor.Estado == false)
            return "No se puede proceder porque uno de los partidos políticos se encuentra inactivo.";

        // pasa a true (Aceptada / Alianza Vigente)
        alianza.Estado = true;
        await _allianceRepository.UpdateAsync(alianza);
        return null;
    }

    // RECHAZAR SOLICITUD DE ALIANZA
    public async Task<string?> RechazarSolicitudAsync(int solicitudId, int partidoAutenticadoId)
    {
        var alianza = await _allianceRepository.GetByIdAsync(solicitudId);
        
        if (alianza == null)
            return "La solicitud de alianza seleccionada no existe.";

        if (alianza.PartidoAliadoId != partidoAutenticadoId)
            return "No tiene permisos para responder esta solicitud de alianza.";

        if (alianza.Estado == true)
            return "Esta solicitud de alianza ya fue respondida.";

        if (await ExisteEleccionActivaAsync())
            return "No se puede rechazar una solicitud de alianza mientras exista una elección activa.";

        // al rechazarla ya no debe aparecer en el listado de pendientes, remueve fisicamente el registro de la tabla para limpiar la vista
        await _allianceRepository.DeleteAsync(alianza);
        return null;
    }

    // ELIMINAR SOLICITUD DE ALIANZA (Realizada)
    public async Task<string?> EliminarSolicitudAsync(int solicitudId, int partidoAutenticadoId)
    {
        var alianza = await _allianceRepository.GetByIdAsync(solicitudId);
        
        if (alianza == null)
            return "La solicitud de alianza seleccionada no existe o ya fue eliminada.";

        if (alianza.PartidoMayoristaId != partidoAutenticadoId)
            return "No tiene permisos para eliminar esta solicitud de alianza.";

        if (alianza.Estado == true)
            return "No se puede eliminar una solicitud aceptada porque ya generó una alianza vigente. Para terminarla debe eliminar la alianza desde el listado de alianzas vigentes.";

        if (await ExisteEleccionActivaAsync())
            return "No se puede eliminar una solicitud de alianza mientras exista una elección activa.";

        await _allianceRepository.DeleteAsync(alianza);
        return null;
    }

    // ELIMINAR ALIANZA VIGENTE
    public async Task<string?> EliminarAlianzaAsync(int alianzaId, int partidoAutenticadoId)
    {
        var alianza = await _allianceRepository.GetByIdAsync(alianzaId);
        
        if (alianza == null)
            return "La alianza política seleccionada no existe o ya fue eliminada.";

        if (alianza.PartidoMayoristaId != partidoAutenticadoId && alianza.PartidoAliadoId != partidoAutenticadoId)
            return "No tiene permisos para eliminar esta alianza política.";

        if (await ExisteEleccionActivaAsync())
            return "No se puede eliminar una alianza política mientras exista una elección activa.";

        // Verificar candidatos asignados en la eleccion de la alianza
        var todasLasAsignacionesCandidatos = await _asignacionCandidatoRepository.GetAllAsync();
        var todosLosCandidatos = await _candidateRepository.GetAllAsync();

        // Busca si hay asignaciones en esta eleccion que pertenezcan a candidatos de los partidos aliados
        bool tieneCandidatosAsignados = todasLasAsignacionesCandidatos.Any(asignacion => 
            asignacion.EleccionId == alianza.EleccionId && 
            todosLosCandidatos.Any(c => c.Id == asignacion.CandidatoId && 
            (c.PartidoId == alianza.PartidoMayoristaId || c.PartidoId == alianza.PartidoAliadoId))
        );

        if (tieneCandidatosAsignados) 
        {
            return "No se puede eliminar esta alianza porque existen candidatos aliados asignados entre estos partidos. Primero deben eliminarse las asignaciones correspondientes desde el módulo Asignar candidato a puesto.";
        }

        await _allianceRepository.DeleteAsync(alianza);
        return null;
    }
    
    
    public async Task<int?> ObtenerPartidoIdPorUsuarioIdAsync(int usuarioId)
    {
        // Traemos todas las asignaciones de dirigentes de la base de datos
        var asignaciones = await _dirigenteRepository.GetAllAsync();
        
        // Buscamos la asignación que le pertenezca al usuario que inició sesión
        var asignacionUsuario = asignaciones.FirstOrDefault(a => a.UsuarioId == usuarioId);
        
        // Si existe la asignación, retornamos el Id del Partido; si no, retornamos null
        return asignacionUsuario?.PartidoId;
    }
}