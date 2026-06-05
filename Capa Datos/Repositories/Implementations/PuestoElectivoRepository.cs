using Capa_Datos.Context;
using Capa_Datos.Entities;
using Capa_Datos.Repositories.Interfaces;

namespace Capa_Datos.Repositories.Implementations
{
    public class PuestoElectivoRepository : RepositoryAsync<PuestoElectivo>, IPuestoElectivoRepository
    {
        public PuestoElectivoRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
