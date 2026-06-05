using Capa_Datos.Context;
using Capa_Datos.Entities;
using Capa_Datos.Repositories.Interfaces;

namespace Capa_Datos.Repositories.Implementations
{
    public class CiudadanoRepository : RepositoryAsync<Ciudadano>, ICiudadanoRepository
    {
        public CiudadanoRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
