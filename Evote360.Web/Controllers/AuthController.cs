using Evote360.Application.Interfaces;
using Evote360.Application.ViewModels.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Evote360.Web.Controllers;

public class AuthController : Controller
{
    private readonly IUsuarioService _usuarioService;

    public AuthController(IUsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
    }

    [HttpGet]
    public IActionResult Login()
    {
        if (User.Identity != null && User.Identity.IsAuthenticated)
        {
            if (User.IsInRole("Administrador"))
                return RedirectToAction("Index", "Administrador");

            if (User.IsInRole("DirigentePolitico"))
                return RedirectToAction("Index", "Dirigente");
            return RedirectToAction("Index", "Elecciones");
        }
        return View(new LoginViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        var usuario = await _usuarioService.LoginAsync(vm.NombreUsuario, vm.Contrasena);

        if (usuario == null)
        {
            ModelState.AddModelError(string.Empty, "Usuario o contraseña incorrectos.");
            return View(vm);
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new Claim(ClaimTypes.Name, usuario.NombreUsuario),
            new Claim(ClaimTypes.Role, usuario.Rol.ToString())
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        var authProperties = new AuthenticationProperties
        {
            IsPersistent = true
        };

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties);

        if (usuario.Rol == Evote360.Core.Enums.RolUsuarioEnum.Administrador) // Asegúrate de usar el nombre exacto de tu Enum (Admin o Administrador)
        {
            return RedirectToAction("Index", "Administrador");
        }

        // CAMBIO: Si es Dirigente Político, lo mandamos directo a su Panel Dirigente
        if (usuario.Rol == Evote360.Core.Enums.RolUsuarioEnum.DirigentePolitico)
        {
            return RedirectToAction("Index", "Dirigente");
        }

        // Por defecto (Elector)
        return RedirectToAction("Index", "Elecciones");

        /*if (usuario.Rol == Evote360.Core.Enums.RolUsuarioEnum.DirigentePolitico)
        {
            return RedirectToAction("Index", "Candidatos");
        }
        return RedirectToAction("Index", "Elecciones");*/
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }
    
    public IActionResult AccessDenied(string mensaje)
    {
        // Si viene un mensaje personalizado lo usamos, si no, uno por defecto
        ViewBag.ErrorMessage = mensaje;
        return View();
    }
}
