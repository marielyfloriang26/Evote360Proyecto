using Evote360.Application.DTOs;
using Evote360.Application.ViewModels.Usuario;

namespace Evote360.Application.Interfaces;

public interface IUsuarioService
{
    // Para listar todos los usuarios en la tabla principal 
    Task<List<UsuarioDto>> GetAllDtoAsync();

    // busca un usuario especifico cuando se va a editar o reactivar
    Task<SaveUsuarioDto> GetByIdSaveDtoAsync(int id);

    // registra un nuevo usuario
    Task AddAsync(SaveUsuarioDto vm);

    Task UpdateAsync(SaveUsuarioDto vm);


    Task<bool> ExisteNombreUsuarioAsync(string nombreUsuario, int idActual = 0);

    Task<bool> ExisteCorreoAsync(string correo, int idActual = 0);

    Task<bool> EsUnicoAdminActivoAsync(int id);

    // verifica si un dirigente politico ya tiene un partido asignado
    Task<bool> TienePartidoAsignadoAsync(int usuarioId);

    Task<bool> ExisteEleccionActivaAsync();
}

