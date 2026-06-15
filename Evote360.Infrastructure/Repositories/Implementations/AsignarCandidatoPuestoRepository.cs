using Evote360.Infrastructure.Context;
using Evote360.Core.Entities;
using Evote360.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Evote360.Infrastructure.Repositories.Implementations
{
    public class AsignarCandidatoPuestoRepository : RepositoryAsync<AsignarCandidatoPuesto>, IAsignarCandidatoPuestoRepository
    {
        public AsignarCandidatoPuestoRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
        public override async Task<IReadOnlyList<AsignarCandidatoPuesto>> GetAllAsync()
        {
            return await _dbContext.AsignacionesCandidatosPuestos
                .Include(a => a.Puesto)                       // Carga el puesto electivo
                .Include(a => a.Candidato)                    // Carga el candidato
                    .ThenInclude(c => c.Partido)              // Carga el partido de origen del candidato
                .Include(a => a.Eleccion)                     // Carga la elección asociada
                .ToListAsync();
        }
    }
}
