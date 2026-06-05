using Capa_Datos.Context;
using Capa_Datos.Entities;
using Capa_Datos.Repositories.Interfaces;

namespace Capa_Datos.Repositories.Implementations
{
    public class PartidoPoliticoRepository : RepositoryAsync<PartidoPolitico>, IPartidoPoliticoRepository
    {
        public PartidoPoliticoRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
