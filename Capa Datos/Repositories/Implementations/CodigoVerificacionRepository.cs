using Capa_Datos.Context;
using Capa_Datos.Entities;
using Capa_Datos.Repositories.Interfaces;

namespace Capa_Datos.Repositories.Implementations
{
    public class CodigoVerificacionRepository : RepositoryAsync<CodigoVerificacion>, ICodigoVerificacionRepository
    {
        public CodigoVerificacionRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
