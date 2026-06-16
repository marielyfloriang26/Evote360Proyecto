using Evote360.Infrastructure.Context;
using Evote360.Core.Entities;
using Evote360.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Evote360.Core.Common;

namespace Evote360.Infrastructure.Repositories.Implementations
{
    public class EleccionRepository : RepositoryAsync<Eleccion>, IEleccionRepository
    {
        public EleccionRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
        public async Task<bool> ExisteEleccionActivaAsync()
        {
            // Regla: Comprueba si hay registros en la tabla Elecciones cuyo EstadoElectoral sea "Activa"
            return await _dbContext.Elecciones
                .AnyAsync(e => e.EstadoElectoral == "Activa");
        }
        public async Task<IReadOnlyList<Eleccion>> ObtenerTodasOrdenadasAsync()
        {
            return await _dbContext.Elecciones
                .OrderByDescending(e => e.EstadoElectoral == EstadosEleccion.Activa)
                .ThenByDescending(e => e.FechaInicio)
                .ToListAsync();
        }

        public async Task<int> ObtenerCantidadCiudadanosQueVotaronAsync(int eleccionId)
        {
            return await _dbContext.Votos
                .Where(v => v.EleccionId == eleccionId)
                .Select(v => v.Id)
                .CountAsync();
        }

        public async Task<int> ObtenerCantidadVotosPorOpcionAsync(int eleccionId, int puestoId, int? candidatoId)
        {
            return await _dbContext.Votos
                .CountAsync(v => v.EleccionId == eleccionId 
                              && v.PuestoId == puestoId 
                              && v.CandidatoId == candidatoId);
        }

        public async Task<int> ObtenerTotalVotosPorPuestoAsync(int eleccionId, int puestoId)
        {
            return await _dbContext.Votos
                .CountAsync(v => v.EleccionId == eleccionId 
                              && v.PuestoId == puestoId);
        }

        public async Task<List<PuestoElectivo>> ObtenerPuestosActivosAsync()
        {
            return await _dbContext.PuestosElectivos
                .Where(p => p.Estado == true)
                .ToListAsync();
        }

        public async Task<List<PartidoPolitico>> ObtenerPartidosActivosAsync()
        {
            return await _dbContext.PartidosPoliticos
                .Where(p => p.Estado == true)
                .ToListAsync();
        }

        public async Task<List<AsignarCandidatoPuesto>> ObtenerAsignacionesPorPuestoAsync(int puestoId)
        {
            return await _dbContext.AsignacionesCandidatosPuestos
                .Include(a => a.Candidato)
                .ThenInclude(c => c.Partido)
                .Where(a => a.PuestoId == puestoId && a.Candidato.Estado == true)
                .ToListAsync();
        }

        public async Task<bool> ExisteAsignacionCandidatoAsync(int puestoId, int partidoId)
        {
            return await _dbContext.AsignacionesCandidatosPuestos
                .Include(a => a.Candidato)
                .AnyAsync(a => a.PuestoId == puestoId 
                               && a.Candidato.PartidoId == partidoId 
                               && a.Candidato.Estado == true 
                               && a.Estado == true);
        }

        public async Task<List<int>> ObtenerAniosConEleccionesAsync()
        {
            return await _dbContext.Elecciones
                .Select(e => e.FechaInicio.Year)
                .Distinct()
                .OrderByDescending(y => y)
                .ToListAsync();
        }

        public async Task<int> ObtenerCantidadCandidatosRealesPorEleccionAsync(int eleccionId)
        {
            // Cuenta candidatos únicos reales (sin duplicar por alianzas) en una elección
            return await _dbContext.AsignacionesCandidatosPuestos
                .Where(a => a.EleccionId == eleccionId)
                .Select(a => a.CandidatoId)
                .Distinct()
                .CountAsync();
        }

        public async Task<int> ObtenerCantidadPartidosPorEleccionAsync(int eleccionId)
        {
            // Cuenta los partidos que tienen asignaciones en esta elección específica
            return await _dbContext.AsignacionesCandidatosPuestos
                .Where(a => a.EleccionId == eleccionId)
                .Include(a => a.Candidato)
                .Select(a => a.Candidato.PartidoId)
                .Distinct()
                .CountAsync();
        }

    }
}
