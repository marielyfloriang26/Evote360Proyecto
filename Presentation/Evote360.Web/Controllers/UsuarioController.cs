using Evote360.Application.DTOs;
using Evote360.Application.DTOs.Usuario;
using Evote360.Application.Interfaces;
using Evote360.Application.ViewModels.Usuario;
using Evote360.Core.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Evote360.Web.Controllers;

[Authorize(Roles = nameof(RolUsuarioEnum.Administrador))] // AUTORIZACION 
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
        bool eleccionActiva = await _usuarioService.ExisteEleccionActivaAsync();
        ViewBag.ExisteEleccionActiva = eleccionActiva;
        
        if (eleccionActiva)
        {
            ViewBag.EleccionMessage = "No se pueden modificar usuarios mientras exista una elección activa.";
        }
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
       public async Task<IActionResult> Crear()
    {
        if (await _usuarioService.ExisteEleccionActivaAsync())
        {
            TempData["ErrorMessage"] = "No se puede crear un usuario mientras exista una elección activa.";
            return RedirectToAction(nameof(Index));
        }
        return View(new SaveUsuarioViewModel());
    }

    // CREAR USUARIO
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(SaveUsuarioViewModel vm)
    {
        if (await _usuarioService.ExisteEleccionActivaAsync())
        {
            ModelState.AddModelError(string.Empty, "No se puede crear un usuario mientras exista una elección activa.");
            return View(vm);
        }
        ModelState.Remove("Estado");

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

        var estadoFormulario = Request.Form["Estado"].ToString().Contains("true");

        
        var dto = new SaveUsuarioDto
        {
            Nombre = vm.Nombre,
            Apellido = vm.Apellido,
            Correo = vm.Correo,
            NombreUsuario = vm.NombreUsuario,
            Contrasena = vm.Contrasena!,
            Rol = vm.Rol,
            Estado = estadoFormulario
        };

        try
        {
            await _usuarioService.AddAsync(dto);
            return RedirectToAction(nameof(Index));
        }
        catch (System.Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(vm);
        }
    }

    // EDITAR USUARIO 
    public async Task<IActionResult> Editar(int id)
    {
        if (await _usuarioService.ExisteEleccionActivaAsync())
        {
            TempData["ErrorMessage"] = "No se puede editar un usuario mientras exista una elección activa.";
            return RedirectToAction(nameof(Index));
        }
        // solicita el dto a service
        var dto = await _usuarioService.GetByIdSaveDtoAsync(id);
        
        if (dto == null)
        {
            return NotFound();
        }

        
        var vm = new EditarUsuarioViewModel
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
    public async Task<IActionResult> Editar(EditarUsuarioViewModel vm)
    {
        if (await _usuarioService.ExisteEleccionActivaAsync())
        {
            ModelState.AddModelError(string.Empty, "No se puede editar un usuario mientras exista una elección activa.");
            return View(vm);
        }
        ModelState.Remove("Estado");

        if (string.IsNullOrEmpty(vm.Contrasena))
    {
        ModelState.Remove("Contrasena");
        ModelState.Remove("ConfirmarContrasena");
    }

        
        // valida que si escribio una contraseña coincida con su confirmacion
        if (!string.IsNullOrEmpty(vm.Contrasena) && vm.Contrasena != vm.ConfirmarContrasena)
        {
            ModelState.AddModelError("ConfirmarContrasena", "Las contraseñas ingresadas no coinciden.");
            return View(vm);
        }

        if (!ModelState.IsValid)
        {
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

        var estadoFormulario = Request.Form["Estado"].ToString().Contains("true");

        // Valida cambio de rol de dirigente politico con partido asignado
        var usuarioActual = await _usuarioService.GetByIdSaveDtoAsync(vm.Id);

        if (usuarioActual != null && usuarioActual.Rol == RolUsuarioEnum.DirigentePolitico && vm.Rol != RolUsuarioEnum.DirigentePolitico)
        {
            if (await _usuarioService.TienePartidoAsignadoAsync(vm.Id))
            {
                ModelState.AddModelError(string.Empty, "No se puede cambiar el rol de este usuario porque tiene un partido político asignado como dirigente.");
                return View(vm);
            }
        }

        // impide que se inactive al unico admn activo del sistema
        if (vm.Rol != RolUsuarioEnum.Administrador || !estadoFormulario)
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
            Estado = estadoFormulario
        };

        try
        {
            await _usuarioService.UpdateAsync(dto);
            TempData["SuccessMessage"] = "Usuario actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }
        catch (System.Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(vm);
        }
    }

    
   /* // ACTIVAR / DESACTIVAR
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
            if (dto.Rol == RolUsuarioEnum.DirigentePolitico && await _usuarioService.TienePartidoAsignadoAsync(id))
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
    } */
    // VISTA CONFIRMAR DESACTIVAR (GET)
    
    [HttpGet]
    public async Task<IActionResult> ConfirmarDesactivar(int id)
    {
        if (await _usuarioService.ExisteEleccionActivaAsync())
        {
            TempData["ErrorMessage"] = "No se puede desactivar un usuario mientras exista una elección activa.";
            return RedirectToAction(nameof(Index));
        }

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
            TempData["ErrorMessage"] = "No se puede desactivar un usuario mientras exista una elección activa.";
            return RedirectToAction(nameof(Index));
        }

        var dto = await _usuarioService.GetByIdSaveDtoAsync(id);
        if (dto == null) return NotFound();

        if (await _usuarioService.EsUnicoAdminActivoAsync(id))
        {
            TempData["ErrorMessage"] = "Acción denegada: Este usuario es el único Administrador activo en el sistema.";
            return RedirectToAction(nameof(Index));
        }

        if (dto.Rol == RolUsuarioEnum.DirigentePolitico && await _usuarioService.TienePartidoAsignadoAsync(id))
        {
            TempData["ErrorMessage"] = "No se puede desactivar este usuario porque es un Dirigente Político con un partido asignado.";
            return RedirectToAction(nameof(Index));
        }

        dto.Estado = false;
        try
        {
            await _usuarioService.UpdateAsync(dto);
            TempData["SuccessMessage"] = "Usuario desactivado correctamente.";
        }
        catch (System.Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }
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
            TempData["ErrorMessage"] = "No se puede activar un usuario mientras exista una elección activo.";
            return RedirectToAction(nameof(Index));
        }

        var dto = await _usuarioService.GetByIdSaveDtoAsync(id);
        if (dto == null) return NotFound();

        dto.Estado = true;
        try
        {
            await _usuarioService.UpdateAsync(dto);
            TempData["SuccessMessage"] = "Usuario activado correctamente.";
        }
        catch (System.Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }
        return RedirectToAction(nameof(Index));
    }
} 