using Evote360.Infrastructure.Context;
using Evote360.Core.Entities;
using Evote360.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Evote360.Infrastructure.Repositories.Implementations
{
    public class EleccionRepository : RepositoryAsync<Eleccion>, IEleccionRepository
    {
        public EleccionRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
        public async Task<bool> ExisteEleccionActivaAsync()
        {
            // Regla: Comprueba si hay registros en la tabla Elecciones cuyo EstadoElectoral sea "Activa"
            return await _dbContext.Elecciones
                .AnyAsync(e => e.EstadoElectoral == "Activa");
        }

    }
}
