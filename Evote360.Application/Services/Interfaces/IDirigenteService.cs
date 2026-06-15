using System.Threading.Tasks;
using Evote360.Core.Entities;
using Evote360.Application.ViewModels.Dirigente; // Nota: Si Core no referencia a Web, puedes mover el ViewModel a Core.DTOs

namespace Evote360.Application.Interfaces
{
    public interface IDirigenteService
    {
        Task<Usuario?> ObtenerUsuarioConAsignacionAsync(int usuarioId);
        Task<HomeDirigenteViewModel?> ObtenerDashboardDirigenteAsync(int usuarioId);
    }
}