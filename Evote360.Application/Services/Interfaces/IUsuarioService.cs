
using Evote360.Application.ViewModels.Usuario;

namespace Evote360.Application.Interfaces;

public interface IUsuarioService
{
    // Para listar todos los usuarios en la tabla principal 
    Task<List<UsuarioViewModel>> GetAllViewModelAsync();

    // busca un usuario especifico cuando se va a editar o reactivar
    Task<SaveUsuarioViewModel> GetByIdSaveViewModelAsync(int id);

    // registra un nuevo usuario
    Task AddAsync(SaveUsuarioViewModel vm);

    Task UpdateAsync(SaveUsuarioViewModel vm);


    Task<bool> ExisteNombreUsuarioAsync(string nombreUsuario, int idActual = 0);

    Task<bool> ExisteCorreoAsync(string correo, int idActual = 0);

    Task<bool> EsUnicoAdminActivoAsync(int id);

    // verifica si un dirigente politico ya tiene un partido asignado
    Task<bool> TienePartidoAsignadoAsync(int usuarioId);

    Task<bool> ExisteEleccionActivaAsync();
}