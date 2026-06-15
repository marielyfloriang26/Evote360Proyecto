using Evote360.Application.Services.Interfaces;
using Evote360.Application.ViewModels.Asignaciones;
using Evote360.Core.Common;
using Evote360.Core.Entities;
using Evote360.Core.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Evote360.Application.Services.Implementations
{
    public class AsignarCandidatoPuestoService : IAsignarCandidatoPuestoService
    {
        private readonly IAsignarCandidatoPuestoRepository _asignarRepo;
        private readonly IAsignacionDirigenteRepository _dirigenteRepo;
        private readonly IEleccionRepository _eleccionRepo;
        private readonly IRepositoryAsync<Candidato> _candidatoRepo;
        private readonly IRepositoryAsync<PuestoElectivo> _puestoRepo;
        private readonly IRepositoryAsync<AlianzaPolitica> _alianzaRepo;

        public AsignarCandidatoPuestoService(
            IAsignarCandidatoPuestoRepository asignarRepo,
            IAsignacionDirigenteRepository dirigenteRepo,
            IEleccionRepository eleccionRepo,
            IRepositoryAsync<Candidato> candidatoRepo,
            IRepositoryAsync<PuestoElectivo> puestoRepo,
            IRepositoryAsync<AlianzaPolitica> alianzaRepo)
        {
            _asignarRepo = asignarRepo;
            _dirigenteRepo = dirigenteRepo;
            _eleccionRepo = eleccionRepo;
            _candidatoRepo = candidatoRepo;
            _puestoRepo = puestoRepo;
            _alianzaRepo = alianzaRepo;
        }

        private async Task<(int PartidoId, int EleccionId, string Error)> ValidarDirigenteYEleccionAsync(int userId)
        {
            if (await _eleccionRepo.ExisteEleccionActivaAsync())
                return (0, 0, "No se pueden modificar asignaciones de candidatos a puestos mientras exista una elección activa.");

            var asignacionesDirigente = await _dirigenteRepo.GetAllAsync();
            var dirigente = asignacionesDirigente.FirstOrDefault(a => a.UsuarioId == userId);
            
            if (dirigente == null || dirigente.Partido == null)
                return (0, 0, "El usuario autenticado debe tener un partido político asignado.");

            if (!dirigente.Partido.Estado)
                return (0, 0, "El partido político del dirigente no se encuentra activo.");

            var elecciones = await _eleccionRepo.GetAllAsync();
            var eleccionPendiente = elecciones.FirstOrDefault(e => e.EstadoElectoral == EstadosEleccion.Pendiente);
            if (eleccionPendiente == null)
                return (0, 0, "No existe un proceso electoral pendiente en el sistema para registrar asignaciones.");

            return (dirigente.PartidoId, eleccionPendiente.Id, string.Empty);
        }

        public async Task<(bool Success, string ErrorMessage, AsignacionPuestoListViewModel Data)> GetIndexDataAsync(int userId)
        {
            var model = new AsignacionPuestoListViewModel();
            model.TieneEleccionActiva = await _eleccionRepo.ExisteEleccionActivaAsync();

            var asignacionesDirigente = await _dirigenteRepo.GetAllAsync();
            var dirigente = asignacionesDirigente.FirstOrDefault(a => a.UsuarioId == userId);
            if (dirigente == null)
                return (false, "El usuario autenticado debe tener un partido político asignado.", model);

            var todasAsignaciones = await _asignarRepo.GetAllAsync();
            var alianzas = await _alianzaRepo.GetAllAsync();

            var partidosAliadosIds = alianzas
                .Where(al => al.PartidoMayoristaId == dirigente.PartidoId || al.PartidoAliadoId == dirigente.PartidoId)
                .Select(al => al.PartidoMayoristaId == dirigente.PartidoId ? al.PartidoAliadoId : al.PartidoMayoristaId)
                .ToList();

            var asignacionesFiltradas = todasAsignaciones
                .Where(a => a.Candidato.PartidoId == dirigente.PartidoId || partidosAliadosIds.Contains(a.Candidato.PartidoId))
                .Select(a => new AsignacionPuestoIndexViewModel
                {
                    Id = a.Id,
                    NombreCandidato = a.Candidato.Nombre,
                    ApellidoCandidato = a.Candidato.Apellido,
                    PartidoOrigenCandidato = a.Candidato.Partido.Nombre,
                    PuestoElectivoAsociado = a.Puesto.Nombre,
                    TipoCandidatura = a.Candidato.PartidoId == dirigente.PartidoId ? "Propio" : "Aliado"
                }).ToList();

            model.Asignaciones = asignacionesFiltradas;
            return (true, string.Empty, model);
        }

        public async Task<(bool Success, string ErrorMessage, CrearAsignacionViewModel Form)> GetFormFieldsAsync(int userId)
        {
            var form = new CrearAsignacionViewModel();
            var (partidoId, _, error) = await ValidarDirigenteYEleccionAsync(userId);
            if (!string.IsNullOrEmpty(error)) return (false, error, form);

            var todosCandidatos = await _candidatoRepo.GetAllAsync();
            var todosPuestos = await _puestoRepo.GetAllAsync();
            var todasAsignaciones = await _asignarRepo.GetAllAsync();
            var todasAlianzas = await _alianzaRepo.GetAllAsync();

            var puestosOcupados = todasAsignaciones
                .Where(a => a.Candidato.PartidoId == partidoId)
                .Select(a => a.PuestoId)
                .ToList();

            form.PuestosDisponibles = todosPuestos
                .Where(p => p.Estado && !puestosOcupados.Contains(p.Id))
                .Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.Nombre })
                .ToList();

            var candidatosPropios = todosCandidatos
                .Where(c => c.Estado && c.PartidoId == partidoId && !todasAsignaciones.Any(a => a.CandidatoId == c.Id))
                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = $"{c.Nombre} {c.Apellido} (Propio)" });

            var partidosAliadosIds = todasAlianzas
                .Where(al => al.PartidoMayoristaId == partidoId || al.PartidoAliadoId == partidoId)
                .Select(al => al.PartidoMayoristaId == partidoId ? al.PartidoAliadoId : al.PartidoMayoristaId)
                .ToList();

            var candidatosAliados = todosCandidatos
                .Where(c => c.Estado && c.Partido.Estado && partidosAliadosIds.Contains(c.PartidoId))
                .Where(c => todasAsignaciones.Any(a => a.CandidatoId == c.Id && a.Candidato.PartidoId == c.PartidoId)) 
                .Where(c => !todasAsignaciones.Any(a => a.CandidatoId == c.Id && a.Candidato.PartidoId != c.PartidoId)) 
                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = $"{c.Nombre} {c.Apellido} ({c.Partido.Siglas} - Aliado)" });

            form.CandidatosDisponibles = candidatosPropios.Concat(candidatosAliados).ToList();
            return (true, string.Empty, form);
        }

        public async Task<(bool Success, string ErrorMessage)> RegistrarAsignacionAsync(int userId, CrearAsignacionViewModel model)
        {
            var (partidoId, eleccionId, error) = await ValidarDirigenteYEleccionAsync(userId);
            if (!string.IsNullOrEmpty(error)) return (false, error);

            var candidato = await _candidatoRepo.GetByIdAsync(model.CandidatoId!.Value);
            var puesto = await _puestoRepo.GetByIdAsync(model.PuestoId!.Value);

            if (candidato == null || !candidato.Estado) return (false, "El candidato seleccionado no existe o está inactivo.");
            if (puesto == null || !puesto.Estado) return (false, "El puesto electivo seleccionado no existe o está inactivo.");

            var todasAsignaciones = await _asignarRepo.GetAllAsync();

            if (todasAsignaciones.Any(a => a.PuestoId == puesto.Id && a.Candidato.PartidoId == partidoId))
                return (false, "Este puesto electivo ya tiene un candidato asignado dentro del partido.");

            if (candidato.PartidoId == partidoId)
            {
                if (todasAsignaciones.Any(a => a.CandidatoId == candidato.Id))
                    return (false, "Este candidato ya está asignado a un puesto dentro del partido.");
            }
            else
            {
                var alianzas = await _alianzaRepo.GetAllAsync();
                bool existeAlianza = alianzas.Any(al => (al.PartidoMayoristaId == partidoId && al.PartidoAliadoId == candidato.PartidoId) || 
                                                        (al.PartidoAliadoId == partidoId && al.PartidoMayoristaId == candidato.PartidoId));
                if (!existeAlianza)
                    return (false, "No existe una alianza vigente con el partido de este candidato.");

                var asignacionOrigen = todasAsignaciones.FirstOrDefault(a => a.CandidatoId == candidato.Id && a.Candidato.PartidoId == candidato.PartidoId);
                if (asignacionOrigen == null)
                    return (false, "Este candidato aliado no tiene un puesto asignado en su partido de origen.");

                if (asignacionOrigen.PuestoId != puesto.Id)
                    return (false, "Este candidato en su partido de origen aspira a un puesto diferente al seleccionado.");
            }

            var nuevaAsignacion = new AsignarCandidatoPuesto
            {
                CandidatoId = candidato.Id,
                PuestoId = puesto.Id,
                EleccionId = eleccionId
            };

            await _asignarRepo.AddAsync(nuevaAsignacion);
            return (true, string.Empty);
        }

        public async Task<(bool Success, string ErrorMessage)> EliminarAsignacionAsync(int userId, int asignacionId)
        {
            if (await _eleccionRepo.ExisteEleccionActivaAsync())
                return (false, "No se puede eliminar una asignación mientras exista una elección activa.");

            var asignacion = await _asignarRepo.GetByIdAsync(asignacionId);
            if (asignacion == null)
                return (false, "La asignación seleccionada no existe o ya fue eliminada.");

            var asignacionesDirigente = await _dirigenteRepo.GetAllAsync();
            var dirigente = asignacionesDirigente.FirstOrDefault(a => a.UsuarioId == userId);
            
            if (dirigente == null)
                return (false, "No tiene permisos para eliminar esta asignación.");

            await _asignarRepo.DeleteAsync(asignacion);
            return (true, string.Empty);
        }
    }
}