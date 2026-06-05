using Capa_Datos.Context;
using Capa_Datos.Entities;
using Capa_Datos.Repositories.Interfaces;

namespace Capa_Datos.Repositories.Implementations
{
    public class AsignarCandidatoPuestoRepository : RepositoryAsync<AsignarCandidatoPuesto>, IAsignarCandidatoPuestoRepository
    {
        public AsignarCandidatoPuestoRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
