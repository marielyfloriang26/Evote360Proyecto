using Evote360.Infrastructure.Context;
using Evote360.Core.Entities;
using Evote360.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Evote360.Infrastructure.Repositories.Implementations
{
    public class AsignacionDirigenteRepository : RepositoryAsync<AsignacionDirigente>, IAsignacionDirigenteRepository
    {
        public AsignacionDirigenteRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public override async Task<IReadOnlyList<AsignacionDirigente>> GetAllAsync()
        {
            return await _dbContext.AsignacionDirigentes
                .Include(a => a.Usuario)
                .Include(a => a.Partido)
                .ToListAsync();
        }

        public override async Task<AsignacionDirigente?> GetByIdAsync(int id)
        {
            return await _dbContext.AsignacionDirigentes
                .Include(a => a.Usuario)
                .Include(a => a.Partido)
                .FirstOrDefaultAsync(a => a.Id == id);
        }
    }
}
