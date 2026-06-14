using Evote360.Core.Entities;
using Evote360.Core.Interfaces;
using Evote360.Core.Enums;
using Evote360.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;


namespace Evote360.Infrastructure.Repositories.Implementations
{
    public class PuestoElectivoRepository : RepositoryAsync<PuestoElectivo>, IPuestoElectivoRepository
    {
        public PuestoElectivoRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
        public async Task<PuestoElectivo?> GetByNombreAsync(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre)) return null;
            
            string nombreLimpio = nombre.Trim().ToLower();
            
            return await _dbContext.Set<PuestoElectivo>()
                .FirstOrDefaultAsync(p => p.Nombre.Trim().ToLower() == nombreLimpio);
        }

        public async Task<bool> TieneCandidatosActivosAsignadosAsync(int puestoId)
        {
            // Usamos la propiedad de navegación que ya tenías creada
            return await _dbContext.Set<PuestoElectivo>()
                .Where(p => p.Id == puestoId)
                .SelectMany(p => p.AsignacionesCandidatos)
                .AnyAsync(a => a.Estado == EstadoEnum.Activo); 
                // Nota: Ajusta 'a.Estado' o el campo de activación según tu entidad AsignarCandidatoPuesto
        }

        public async Task<bool> FueUtilizadoEnEleccionAsync(int puestoId)
        {
            // Si el puesto ya acumuló votos en las mesas, significa que fue utilizado en una elección
            return await _dbContext.Set<PuestoElectivo>()
                .Where(p => p.Id == puestoId)
                .SelectMany(p => p.Votos)
                .AnyAsync();
        }
    }
}
