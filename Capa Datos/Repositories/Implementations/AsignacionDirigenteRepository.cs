using Capa_Datos.Context;
using Capa_Datos.Entities;
using Capa_Datos.Repositories.Interfaces;

namespace Capa_Datos.Repositories.Implementations
{
    public class AsignacionDirigenteRepository : RepositoryAsync<AsignacionDirigente>, IAsignacionDirigenteRepository
    {
        public AsignacionDirigenteRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
