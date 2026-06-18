using Evote360.Application.DTOs.AsignacionDirigentePolitico;
using Evote360.Application.DTOs.PartidoPolitico;
using Evote360.Application.DTOs.Usuario;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Evote360.Application.Services.Interfaces
{
    public interface IAsignacionDirigentePoliticoService
    {
        Task<IEnumerable<AsignacionDirigentePoliticoDTO>> ObtenerAsignacionesDirigentesAsync();
        Task<AsignacionDirigentePoliticoDTO> ObtenerAsignacionDirigentePorIdAsync(int id);
        Task<AsignacionDirigentePoliticoDTO> CrearAsignacionAsync(CrearAsignacionDirPolDTO crearAsignacionDirPolDTO);
        Task EliminarAsignacionAsync(int id);
        Task<bool> HayEleccionActivaAsync();
        Task<IEnumerable<UsuarioDto>> ObtenerUsuariosDirigentesDisponiblesAsync();
        Task<IEnumerable<PartidoPoliticoDTO>> ObtenerPartidosDisponiblesAsync();
    }
}
