using System.Threading.Tasks;
using Evote360.Application.ViewModels.Dirigente;

namespace Evote360.Application.Interfaces
{
    public interface IDirigenteService
    {
        Task<HomeDirigenteViewModel> ObtenerDashboardDirigenteAsync(string nombreUsuario);
    }
}