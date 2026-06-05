using Capa_Datos.Context;
using Capa_Datos.Entities;
using Capa_Datos.Repositories.Interfaces;

namespace Capa_Datos.Repositories.Implementations
{
    public class CandidatoRepository : RepositoryAsync<Candidato>, ICandidatoRepository
    {
        public CandidatoRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
