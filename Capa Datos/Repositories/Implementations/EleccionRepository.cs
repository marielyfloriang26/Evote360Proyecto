using Capa_Datos.Context;
using Capa_Datos.Entities;
using Capa_Datos.Repositories.Interfaces;

namespace Capa_Datos.Repositories.Implementations
{
    public class EleccionRepository : RepositoryAsync<Eleccion>, IEleccionRepository
    {
        public EleccionRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
