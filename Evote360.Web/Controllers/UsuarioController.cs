using Evote360.Application.Interfaces;
using Evote360.Application.ViewModels.Usuario;
using Microsoft.AspNetCore.Mvc;

namespace Evote360.Web.Controllers;

public class UsuarioController : Controller
{
    private readonly IUsuarioService _usuarioService;

    public UsuarioController(IUsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
    }


    // LISTADO PRINCIPAL 
    public async Task<IActionResult> Index()
    {
        var usuarios = await _usuarioService.GetAllViewModelAsync();
        return View(usuarios);
    }

   
    //CREAR USUARIO (PANTALLA)
    
    public IActionResult Crear()
    {
        // Retorna la vista con un modelo limpio
        return View(new SaveUsuarioViewModel());
    }

    
    // CREAR USUARIO 
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(SaveUsuarioViewModel vm)
    {
        // 1. Validar las anotaciones básicas de datos del ViewModel ([Required], [EmailAddress], etc.)
        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        // 2. REGLA DEL PDF: Validar si el Nombre de Usuario ya existe
        if (await _usuarioService.ExisteNombreUsuarioAsync(vm.NombreUsuario))
        {
            ModelState.AddModelError("NombreUsuario", "El nombre de usuario ya se encuentra registrado.");
            return View(vm);
        }

        // 3. REGLA DEL PDF: Validar si el Correo ya existe
        if (await _usuarioService.ExisteCorreoAsync(vm.Correo))
        {
            ModelState.AddModelError("Correo", "Este correo electrónico ya está siendo utilizado por otro usuario.");
            return View(vm);
        }

        // Si pasa todas las validaciones, se registra
        await _usuarioService.AddAsync(vm);
        return RedirectToAction(nameof(Index));
    }

   
    public async Task<IActionResult> Editar(int id)
    {
        var vm = await _usuarioService.GetByIdSaveViewModelAsync(id);
        
        if (vm == null)
        {
            return NotFound();
        }

        return View(vm);
    }

    
    //  EDITAR USUARIO 
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(SaveUsuarioViewModel vm)
    {
        // Para editar, removemos temporalmente la validación requerida de contraseñas de las anotaciones,
        // ya que el PDF dice que en edición el campo contraseña puede quedarse vacío si no se desea cambiar.
        ModelState.Remove("Contrasena");
        ModelState.Remove("ConfirmarContrasena");

        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        // REGLA DEL PDF: Si el usuario escribió algo en el campo contraseña, hay que validar que coincida con la confirmación
        if (!string.IsNullOrEmpty(vm.Contrasena) && vm.Contrasena != vm.ConfirmarContrasena)
        {
            ModelState.AddModelError("ConfirmarContrasena", "Las contraseñas ingresadas no coinciden.");
            return View(vm);
        }

        // REGLA DEL PDF: Validar Nombre de Usuario único (excluyendo al usuario actual)
        if (await _usuarioService.ExisteNombreUsuarioAsync(vm.NombreUsuario, vm.Id))
        {
            ModelState.AddModelError("NombreUsuario", "El nombre de usuario ya está asignado a otra persona.");
            return View(vm);
        }

        // REGLA DEL PDF: Validar Correo único (excluyendo al usuario actual)
        if (await _usuarioService.ExisteCorreoAsync(vm.Correo, vm.Id))
        {
            ModelState.AddModelError("Correo", "Este correo electrónico ya pertenece a otro usuario.");
            return View(vm);
        }

        // REGLA DEL PDF: Validar que no se desactive o se le cambie el rol al ÚNICO Administrador Activo del sistema
        if (vm.Rol != "Administrador" || !vm.Estado)
        {
            if (await _usuarioService.EsUnicoAdminActivoAsync(vm.Id))
            {
                ModelState.AddModelError(string.Empty, "No es posible desactivar ni cambiar el rol de este usuario porque es el único Administrador activo del sistema.");
                return View(vm);
            }
        }

        await _usuarioService.UpdateAsync(vm);
        return RedirectToAction(nameof(Index));
    }

    // ==========================================
    // 6. ACCIÓN PARA ACTIVAR / DESACTIVAR (POST directo desde la tabla)
    // ==========================================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CambiarEstado(int id)
    {
        // REGLA DEL PDF: El sistema no debe permitir que se inicien procesos de votación si hay elecciones activas.
        // Aunque esto aplica más a eliminar/desactivar entidades, protegemos el estado de los usuarios.
        if (await _usuarioService.ExisteEleccionActivaAsync())
        {
            TempData["ErrorMessage"] = "No se pueden modificar usuarios mientras exista un proceso electoral activo.";
            return RedirectToAction(nameof(Index));
        }

        var usuarioVm = await _usuarioService.GetByIdSaveViewModelAsync(id);
        if (usuarioVm == null)
        {
            return NotFound();
        }

        // REGLA DEL PDF: Si está activo y se va a desactivar, validar que no sea el único Admin del sistema
        if (usuarioVm.Estado) 
        {
            if (await _usuarioService.EsUnicoAdminActivoAsync(id))
            {
                TempData["ErrorMessage"] = "Acción denegada: Este usuario es el único Administrador activo en el sistema.";
                return RedirectToAction(nameof(Index));
            }

            // REGLA DEL PDF: Si es un Dirigente Político y tiene un partido asignado, no puede ser desactivado/eliminado
            if (usuarioVm.Rol == "Dirigente Politico" && await _usuarioService.TienePartidoAsignadoAsync(id))
            {
                TempData["ErrorMessage"] = "No se puede desactivar este usuario porque es un Dirigente Político con un partido asignado.";
                return RedirectToAction(nameof(Index));
            }

            // Invertimos el estado de activo a inactivo
            usuarioVm.Estado = false;
        }
        else
        {
            // Si estaba inactivo, pasa a activo directamente
            usuarioVm.Estado = true;
        }

        await _usuarioService.UpdateAsync(usuarioVm);
        return RedirectToAction(nameof(Index));
    }
}