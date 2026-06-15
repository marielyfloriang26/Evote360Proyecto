using System.Threading.Tasks;

namespace Evote360.Application.Services.Interfaces
{
    public interface IVotacionService
    {
        Task<(bool Success, string Message)> ValidarElectorAsync(string documentoIdentidad);
        Task<(bool Success, string Message)> ValidarIdentidadOcrAsync(byte[] imagenBytes, string nombreArchivo, string documentoEsperado);
        Task<(bool Success, string Message)> ValidarCodigoVerificacionAsync(string documentoIdentidad, string codigo);
        Task<System.Collections.Generic.IEnumerable<Evote360.Application.ViewModels.Votacion.PuestoDisponibleViewModel>> ObtenerPuestosDisponiblesAsync(string documentoIdentidad);
        Task<Evote360.Application.ViewModels.Votacion.BoletaPuestoViewModel?> ObtenerBoletaPuestoAsync(int puestoId);
        Task<(bool Success, string Message)> FinalizarVotacionAsync(string documentoIdentidad, System.Collections.Generic.Dictionary<int, int> selecciones);
    }
}
