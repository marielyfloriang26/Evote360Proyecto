using System.Collections.Generic;
using System.Threading.Tasks;
using Evote360.Application.ViewModels.Administrador;

namespace Evote360.Application.Interfaces
{
    public interface IAdministradorService
    {
        Task<HomeAdministradorViewModel> ObtenerDashboardAdminAsync();
        Task<List<ResumenEleccionViewModel>> ObtenerResumenPorAnioAsync(int anio);
    }
}