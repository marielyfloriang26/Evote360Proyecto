using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Evote360.Application.Interfaces;

namespace Evote360.Web.Controllers
{
    [Authorize(Roles = "DirigentePolitico")] // requerimiento para el módulo DirigentePolitico
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

            if (!string.IsNullOrEmpty(model.ErrorAcceso))
            {
                // le muestras una vista de acceso denegado con el error
                return RedirectToAction("AccessDenied", "Auth", new { mensaje = model.ErrorAcceso }); 
            }

            return View(model);
        }
    }
}