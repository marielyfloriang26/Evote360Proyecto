using Evote360.Infrastructure.Context;
using Evote360.Core.Entities;
using Evote360.Core.Interfaces;

namespace Evote360.Infrastructure.Repositories.Implementations
{
    public class AsignarCandidatoPuestoRepository : RepositoryAsync<AsignarCandidatoPuesto>, IAsignarCandidatoPuestoRepository
    {
        public AsignarCandidatoPuestoRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
