using Capa_Datos.Context;
using Capa_Datos.Entities;
using Capa_Datos.Repositories.Interfaces;

namespace Capa_Datos.Repositories.Implementations
{
    public class AlianzaPoliticaRepository : RepositoryAsync<AlianzaPolitica>, IAlianzaPoliticaRepository
    {
        public AlianzaPoliticaRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
