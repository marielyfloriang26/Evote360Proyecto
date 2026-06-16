using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Evote360.Core.Entities;
using Evote360.Core.Interfaces;
using Evote360.Application.Interfaces;
using Evote360.Application.ViewModels.Dirigente;

namespace Evote360.Application.Services.Implementations
{
    public class DirigenteService : IDirigenteService
    {
        private readonly IRepositoryAsync<Usuario> _usuarioRepository;
        private readonly IRepositoryAsync<PartidoPolitico> _partidoRepository;
        private readonly IRepositoryAsync<Candidato> _candidatoRepository;
        private readonly IRepositoryAsync<AlianzaPolitica> _alianzaRepository;
        private readonly IRepositoryAsync<AsignarCandidatoPuesto> _asignacionCandidatoRepository;
        private readonly IRepositoryAsync<AsignacionDirigente> _asignacionDirigenteRepository;

        public DirigenteService(
            IRepositoryAsync<Usuario> usuarioRepository,
            IRepositoryAsync<PartidoPolitico> partidoRepository,
            IRepositoryAsync<Candidato> candidatoRepository,
            IRepositoryAsync<AlianzaPolitica> alianzaRepository,
            IRepositoryAsync<AsignarCandidatoPuesto> asignacionCandidatoRepository,
            IRepositoryAsync<AsignacionDirigente> asignacionDirigenteRepository)
        {
            _usuarioRepository = usuarioRepository;
            _partidoRepository = partidoRepository;
            _candidatoRepository = candidatoRepository;
            _alianzaRepository = alianzaRepository;
            _asignacionCandidatoRepository = asignacionCandidatoRepository;
            _asignacionDirigenteRepository = asignacionDirigenteRepository;
        }

        public async Task<HomeDirigenteViewModel> ObtenerDashboardDirigenteAsync(string nombreUsuario)
        {
            var model = new HomeDirigenteViewModel();

            // 1. Validar que el usuario existe en el sistema
            var usuarios = await _usuarioRepository.GetAllAsync();
            var usuarioAuth = usuarios.FirstOrDefault(u => u.NombreUsuario.Equals(nombreUsuario, StringComparison.OrdinalIgnoreCase));

            if (usuarioAuth == null || !usuarioAuth.Estado)
            {
                model.ErrorAcceso = "Usuario inactivo o no encontrado.";
                return model;
            }

            // 2. Buscar la asignación directamente en su repositorio correspondiente para evitar problemas de Include/LazyLoading
            var todasLasAsignaciones = await _asignacionDirigenteRepository.GetAllAsync() ?? new List<AsignacionDirigente>();
            var asignacion = todasLasAsignaciones.FirstOrDefault(a => a.UsuarioId == usuarioAuth.Id);

            if (asignacion == null)
            {
                model.ErrorAcceso = "No tiene un partido político asignado. Por favor, póngase en contacto con un administrador.";
                return model;
            }

            // Obtener el ID del partido desde la asignación encontrada
            int partidoId = asignacion.PartidoId;
            var partido = await _partidoRepository.GetByIdAsync(partidoId);

            // 3. Validar si el partido político asignado se encuentra activo
            if (partido == null || !partido.Estado)
            {
                model.ErrorAcceso = "El partido político asignado a este usuario se encuentra inactivo.";
                return model;
            }

            // Mapear los datos de cabecera del Partido Político en el ViewModel
            model.PartidoId = partido.Id;
            model.NombrePartido = partido.Nombre;
            model.Siglas = partido.Siglas;
            model.LogoUrl = partido.LogoUrl;

            // --- CÁLCULO DE INDICADORES EN BASE AL ALCANCE DEL PARTIDO (DATA SCOPING) ---
            
            // Carga de datos relacionales requeridos en memoria de forma segura
            var todosLosCandidatos = await _candidatoRepository.GetAllAsync() ?? new List<Candidato>();
            var todasLasAlianzas = await _alianzaRepository.GetAllAsync() ?? new List<AlianzaPolitica>();
            var todasLasAsignacionesPuestos = await _asignacionCandidatoRepository.GetAllAsync() ?? new List<AsignarCandidatoPuesto>();

            // Filtrar la lista global de candidatos para dejar únicamente los pertenecientes al partido del dirigente
            var candidatosDelPartido = todosLosCandidatos.Where(c => c.PartidoId == partidoId).ToList();

            // Indicador 1: Cantidad de candidatos activos
            model.CantidadCandidatosActivos = candidatosDelPartido.Count(c => c.Estado);

            // Indicador 2: Cantidad de candidatos inactivos
            model.CantidadCandidatosInactivos = candidatosDelPartido.Count(c => !c.Estado);

            // Indicador 3: Cantidad de alianzas políticas aprobadas (Estado = true)
            // Se contabiliza si el partido participa como solicitante (Mayorista) o receptor (Aliado)
            model.CantidadAlianzasPoliticas = todasLasAlianzas.Count(a => 
                a.Estado && (a.PartidoMayoristaId == partidoId || a.PartidoAliadoId == partidoId));

            // Indicador 4: Cantidad de solicitudes de alianzas pendientes de responder
            // Condiciones: Enviadas por otro partido (Mayorista != miPartido), dirigidas a mí (Aliado == miPartido) y en espera (Estado == false)
            model.CantidadSolicitudesPendientes = todasLasAlianzas.Count(a => 
                !a.Estado && a.PartidoAliadoId == partidoId && a.PartidoMayoristaId != partidoId);

            // Indicador 5: Cantidad de candidatos asignados a puestos electivos vigentes
            model.CantidadCandidatosAsignados = todasLasAsignacionesPuestos.Count(asig => 
                asig.Estado && asig.Candidato != null && asig.Candidato.PartidoId == partidoId);

            return model;
        }
    }
}