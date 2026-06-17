using Evote360.Infrastructure.Context;
using Evote360.Core.Entities;
using Evote360.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Evote360.Infrastructure.Repositories.Implementations
{
    public class PartidoPoliticoRepository : RepositoryAsync<PartidoPolitico>, IPartidoPoliticoRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public PartidoPoliticoRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
           _dbContext = dbContext; 
        }
        public async Task<PartidoPolitico> GetPartidoConRelacionesOptimizadoAsync(int id)
        {
            return await _dbContext.Set<PartidoPolitico>()
                .Include(p => p.Candidatos)
                .Include(p => p.AsignacionesDirigentes)       
                    .ThenInclude(a => a.Usuario)           
                .AsSplitQuery() 
                .FirstOrDefaultAsync(p => p.Id == id);        
        }
    }
}
