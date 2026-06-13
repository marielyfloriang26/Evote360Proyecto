using Evote360.Application.DTOs;
using Evote360.Application.Interfaces;
using Evote360.Application.ViewModels.Usuario;
using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Authorization; // TEMPORAL AUTORIZACION

namespace Evote360.Web.Controllers;

// [Authorize(Roles = "Administrador")] // AUTORIZACION TEMPORAL
public class UsuarioController : Controller
{
    private readonly IUsuarioService _usuarioService;

    public UsuarioController(IUsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
    }

    // LISTADO PRINCIPAL (INDEX)
    
    public async Task<IActionResult> Index()
    {
        // El servicio retorna una lista de dto
        var usuariosDto = await _usuarioService.GetAllDtoAsync();

        // Mapeo manual de dto a vm
        var usuariosVm = usuariosDto.Select(u => new UsuarioViewModel
        {
            Id = u.Id,
            Nombre = u.Nombre,
            Apellido = u.Apellido,
            Correo = u.Correo,
            NombreUsuario = u.NombreUsuario,
            Rol = u.Rol,
            Estado = u.Estado
        }).ToList();

        return View(usuariosVm);
    }

    // CREAR USUARIO 
       public IActionResult Crear()
    {
        return View(new SaveUsuarioViewModel());
    }

    // CREAR USUARIO
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(SaveUsuarioViewModel vm)
    {
        // validacion 
        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        // Valida que sea unico el nombre de usuario mediante el servicio
        if (await _usuarioService.ExisteNombreUsuarioAsync(vm.NombreUsuario))
        {
            ModelState.AddModelError("NombreUsuario", "El nombre de usuario ya se encuentra registrado.");
            return View(vm);
        }

        // valida que sea unico el correo mediante el servicio
        if (await _usuarioService.ExisteCorreoAsync(vm.Correo))
        {
            ModelState.AddModelError("Correo", "Este correo electrónico ya está siendo utilizado por otro usuario.");
            return View(vm);
        }

        // mapeo del ViewModel al dto
        var dto = new SaveUsuarioDto
        {
            Nombre = vm.Nombre,
            Apellido = vm.Apellido,
            Correo = vm.Correo,
            NombreUsuario = vm.NombreUsuario,
            Contrasena = vm.Contrasena!,
            Rol = vm.Rol,
            Estado = true
        };

        await _usuarioService.AddAsync(dto);
        return RedirectToAction(nameof(Index));
    }

    // EDITAR USUARIO 
    public async Task<IActionResult> Editar(int id)
    {
        // solicita el dto a service
        var dto = await _usuarioService.GetByIdSaveDtoAsync(id);
        
        if (dto == null)
        {
            return NotFound();
        }

        // mapea el dto de retorno hacia el vm que procesara la vista
        var vm = new SaveUsuarioViewModel
        {
            Id = dto.Id,
            Nombre = dto.Nombre,
            Apellido = dto.Apellido,
            Correo = dto.Correo,
            NombreUsuario = dto.NombreUsuario,
            Rol = dto.Rol,
            Estado = dto.Estado
            // Contrasena y ConfirmarContrasena se omiten por seguridad en la carga
        };

        return View(vm);
    }

    // EDITAR USUARIO 
   
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(SaveUsuarioViewModel vm)
    {
        if (string.IsNullOrEmpty(vm.Contrasena))
    {
        ModelState.Remove("Contrasena");
        ModelState.Remove("ConfirmarContrasena");
    }

        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        // valida que si escribio una contraseña coincida con su confirmacion
        if (!string.IsNullOrEmpty(vm.Contrasena) && vm.Contrasena != vm.ConfirmarContrasena)
        {
            ModelState.AddModelError("ConfirmarContrasena", "Las contraseñas ingresadas no coinciden.");
            return View(vm);
        }

        // Valida duplicidad de username excluyendo al registro actual
        if (await _usuarioService.ExisteNombreUsuarioAsync(vm.NombreUsuario, vm.Id))
        {
            ModelState.AddModelError("NombreUsuario", "El nombre de usuario ya está asignado a otra persona.");
            return View(vm);
        }

        // valida duplicidad de correo excluyendo al registro actual
        if (await _usuarioService.ExisteCorreoAsync(vm.Correo, vm.Id))
        {
            ModelState.AddModelError("Correo", "Este correo electrónico ya pertenece a otro usuario.");
            return View(vm);
        }

        // impide que se inactive al unico admn activo del sistema
        if (vm.Rol != "Administrador" || !vm.Estado)
        {
            if (await _usuarioService.EsUnicoAdminActivoAsync(vm.Id))
            {
                ModelState.AddModelError(string.Empty, "No es posible desactivar ni cambiar el rol de este usuario porque es el único Administrador activo del sistema.");
                return View(vm);
            }
        }

       
        var dto = new SaveUsuarioDto
        {
            Id = vm.Id,
            Nombre = vm.Nombre,
            Apellido = vm.Apellido,
            Correo = vm.Correo,
            NombreUsuario = vm.NombreUsuario,
            Contrasena = vm.Contrasena!, // Puede ir vacia o con texto, el servicio lo manejara
            Rol = vm.Rol,
            Estado = vm.Estado
        };

        await _usuarioService.UpdateAsync(dto);
        return RedirectToAction(nameof(Index));
    }

    
    // ACTIVAR / DESACTIVAR
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CambiarEstado(int id)
    {
        // impide cualquier alteracion transaccional si hay procesos electorales activos
        if (await _usuarioService.ExisteEleccionActivaAsync())
        {
            TempData["ErrorMessage"] = "No se pueden modificar usuarios mientras exista un proceso electoral activo.";
            return RedirectToAction(nameof(Index));
        }

        // Recupera los datos de persistencia
        var dto = await _usuarioService.GetByIdSaveDtoAsync(id);
        if (dto == null)
        {
            return NotFound();
        }

        if (dto.Estado) 
        {
            // Evita desactivacion si es el unico admn operativo
            if (await _usuarioService.EsUnicoAdminActivoAsync(id))
            {
                TempData["ErrorMessage"] = "Acción denegada: Este usuario es el único Administrador activo en el sistema.";
                return RedirectToAction(nameof(Index));
            }

            // Evita desactivacion si es un dirigente politico con asignacion de partido vigente
            if (dto.Rol == "Dirigente Politico" && await _usuarioService.TienePartidoAsignadoAsync(id))
            {
                TempData["ErrorMessage"] = "No se puede desactivar este usuario porque es un Dirigente Político con un partido asignado.";
                return RedirectToAction(nameof(Index));
            }

            dto.Estado = false;
        }
        else
        {
            dto.Estado = true;
        }

        // manda el DTO actualizado de vuelta al servicio para efectuar el cambio en cascada
        await _usuarioService.UpdateAsync(dto);
        return RedirectToAction(nameof(Index));
    }
    // VISTA CONFIRMAR DESACTIVAR (GET)
    [HttpGet]
    public async Task<IActionResult> ConfirmarDesactivar(int id)
    {
        var usuario = await _usuarioService.GetByIdSaveDtoAsync(id); 
        if (usuario == null) return NotFound();

        ViewBag.NombreUsuario = $"{usuario.Nombre} {usuario.Apellido}";
        return View(id);
    }

    //  (POST)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Desactivar(int id)
    {
        if (await _usuarioService.ExisteEleccionActivaAsync())
        {
            TempData["ErrorMessage"] = "No se pueden modificar usuarios mientras exista un proceso electoral activo.";
            return RedirectToAction(nameof(Index));
        }

        var dto = await _usuarioService.GetByIdSaveDtoAsync(id);
        if (dto == null) return NotFound();

        if (await _usuarioService.EsUnicoAdminActivoAsync(id))
        {
            TempData["ErrorMessage"] = "Acción denegada: Este usuario es el único Administrador activo en el sistema.";
            return RedirectToAction(nameof(Index));
        }

        if (dto.Rol == "Dirigente Politico" && await _usuarioService.TienePartidoAsignadoAsync(id))
        {
            TempData["ErrorMessage"] = "No se puede desactivar este usuario porque es un Dirigente Político con un partido asignado.";
            return RedirectToAction(nameof(Index));
        }

        dto.Estado = false;
        await _usuarioService.UpdateAsync(dto);

        TempData["SuccessMessage"] = "Usuario desactivado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    // (GET)
    [HttpGet]
    public async Task<IActionResult> ConfirmarActivar(int id)
    {
        var usuario = await _usuarioService.GetByIdSaveDtoAsync(id);
        if (usuario == null) return NotFound();

        ViewBag.NombreUsuario = $"{usuario.Nombre} {usuario.Apellido}";
        return View(id);
    }

    // (POST)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Activar(int id)
    {
        if (await _usuarioService.ExisteEleccionActivaAsync())
        {
            TempData["ErrorMessage"] = "No se pueden modificar usuarios mientras exista un proceso electoral activo.";
            return RedirectToAction(nameof(Index));
        }

        var dto = await _usuarioService.GetByIdSaveDtoAsync(id);
        if (dto == null) return NotFound();

        dto.Estado = true;
        await _usuarioService.UpdateAsync(dto);

        TempData["SuccessMessage"] = "Usuario activado correctamente.";
        return RedirectToAction(nameof(Index));
    }
}