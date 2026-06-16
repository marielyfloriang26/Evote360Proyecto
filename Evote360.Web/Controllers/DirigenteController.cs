using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Evote360.Application.Interfaces;

namespace Evote360.Web.Controllers
{
    [Authorize(Roles = "DirigentePolitico")] // Restringe el acceso solo a Dirigentes Políticos
    public class DirigenteController : Controller
    {
        private readonly IDirigenteService _dirigenteService;

        public DirigenteController(IDirigenteService dirigenteService)
        {
            _dirigenteService = dirigenteService;
        }

        // GET: /Dirigente
        public async Task<IActionResult> Index()
        {
            // Extrae el nombre de usuario de la sesión actual
            string nombreUsuario = User.Identity?.Name ?? "";

            var model = await _dirigenteService.ObtenerDashboardDirigenteAsync(nombreUsuario);

            // Si saltó alguna regla de negocio (Inactivo, sin partido, etc.)
            if (!string.IsNullOrEmpty(model.ErrorAcceso))
            {
                // Guardamos el mensaje específico exigido por la rúbrica
                TempData["ErrorMessage"] = model.ErrorAcceso;
                
                // CORRECCIÓN: Redirigimos al Login de tu AuthController para que pinte el error allá
                return RedirectToAction("Login", "Auth"); 
            }

            return View(model);
        }
    }
}