using Evote360.Infrastructure.Context;
using Evote360.Core.Entities;
using Evote360.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Evote360.Infrastructure.Repositories.Implementations
{
    public class CiudadanoRepository : RepositoryAsync<Ciudadano>, ICiudadanoRepository
    {
        public CiudadanoRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<Ciudadano?> ObtenerPorCedulaAsync(string cedula)
        {
            return await _dbContext.Set<Ciudadano>()
                .FirstOrDefaultAsync(c => c.Cedula == cedula);
        }

        public async Task<Ciudadano?> ObtenerPorCorreoAsync(string correo)
        {
            return await _dbContext.Set<Ciudadano>()
                .FirstOrDefaultAsync(c => c.Correo.ToLower() == correo.ToLower());
        }
        public async Task<bool> HaParticipadoEnEleccionesAsync(int id)
        {
            // Valida contra la propiedad flag de votación o si existe registro histórico 
            // en tablas transaccionales (Adecuar según tu entidad de Votos/Participantes)
            var ciudadano = await _dbContext.Set<Ciudadano>().FindAsync(id);
            if (ciudadano == null) return false;
            
            return ciudadano.HaVotado;
        }
        public async Task ResetearEstadoVotacionAsync()
        {
            await _dbContext.Set<Ciudadano>()
                .ExecuteUpdateAsync(s => s.SetProperty(c => c.HaVotado, false));
        }
    }
}
