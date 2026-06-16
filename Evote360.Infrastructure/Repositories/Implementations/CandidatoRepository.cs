using Evote360.Core.Entities;
using Evote360.Core.Interfaces;
using Evote360.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Evote360.Infrastructure.Repositories.Implementations
{
    public class CandidatoRepository : RepositoryAsync<Candidato>, ICandidatoRepository
    {
        public CandidatoRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public override async Task<IReadOnlyList<Candidato>> GetAllAsync()
        {
            return await _dbContext.Set<Candidato>()
                .Include(c => c.AsignacionesPuestos)
                .ThenInclude(a => a.Puesto)
                .ToListAsync();
        }

        public async Task<bool> HasParticipatedInElectionAsync(int candidatoId)
            {
                return await _dbContext.AsignacionesCandidatosPuestos
                    .AnyAsync(a => a.CandidatoId == candidatoId &&
                                    a.Eleccion != null &&
                                    (a.Eleccion.EstadoElectoral == "Activa" || a.Eleccion.EstadoElectoral == "Finalizada"));
            }

            public async Task<bool> HasAssignedPuestoVigenteAsync(int candidatoId)
            {
                return await _dbContext.AsignacionesCandidatosPuestos
                    .AnyAsync(a => a.CandidatoId == candidatoId &&
                                   (a.Eleccion == null ||
                                    a.Eleccion.EstadoElectoral == "Pendiente" ||
                                    a.Eleccion.EstadoElectoral == "Activa"));
            
            }
    }
}