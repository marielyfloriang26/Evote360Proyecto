using Capa_Datos.Context;
using Capa_Datos.Entities;
using Capa_Datos.Repositories.Interfaces;

namespace Capa_Datos.Repositories.Implementations
{
    public class VotoRepository : RepositoryAsync<Voto>, IVotoRepository
    {
        public VotoRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
