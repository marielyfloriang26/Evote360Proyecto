using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using Evote360.Application.Interfaces;

namespace Evote360.Web.Controllers
{
    // Asegúrate de que el string "DirigentePolitico" coincida exactamente con el que guardas al loguearte
    [Authorize(Roles = "DirigentePolitico")] 
    public class DirigenteController : Controller
    {
        private readonly IDirigenteService _dirigenteService;

        public DirigenteController(IDirigenteService dirigenteService)
        {
            _dirigenteService = dirigenteService;
        }

        public async Task<IActionResult> Index()
        {
            // Extraemos el ID del usuario de forma flexible
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                              ?? User.FindFirst(ClaimTypes.Sid)?.Value 
                              ?? User.FindFirst("Id")?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int usuarioId))
            {
                return RedirectToAction("Login", "Auth");
            }

            // Llamamos al servicio para compilar el modelo del Home del Dirigente
            var model = await _dirigenteService.ObtenerDashboardDirigenteAsync(usuarioId);
            
            if (model == null)
            {
                // Si el servicio retorna null, aplicamos la regla del PDF inyectando el mensaje sugerido
                TempData["ErrorMessage"] = "No tiene un partido político asignado o se encuentra inactivo. Por favor, póngase en contacto con un administrador.";
                return RedirectToAction("Index", "Home"); 
            }

            return View(model);
        }
    }
}