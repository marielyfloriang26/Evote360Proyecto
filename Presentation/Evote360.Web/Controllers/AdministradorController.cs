using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Evote360.Application.Interfaces;
using Evote360.Application.ViewModels.Administrador;

namespace Evote360.Web.Controllers
{
    [Authorize(Roles = "Administrador")] // requerimiento para el módulo administrativo
    public class AdministradorController : Controller
    {
        private readonly IAdministradorService _adminService;

        public AdministradorController(IAdministradorService adminService)
        {
            _adminService = adminService;
        }

        // GET: /Administrador
        public async Task<IActionResult> Index(int? anioElectoral)
        {
            var model = await _adminService.ObtenerDashboardAdminAsync();

            if (model.AniosDisponibles.Count == 0)
            {
                TempData["InfoMessage"] = "No existen elecciones registradas en el sistema.";
                return View(model);
            }

            // Por defecto se selecciona el año más reciente disponible
            if (!anioElectoral.HasValue)
            {
                model.AnioElectoral = model.AniosDisponibles[0];
            }
            else
            {
                model.AnioElectoral = anioElectoral;
            }

            // Cargar el resumen cruzado filtrado por el año correspondiente
            model.EleccionesResumen = await _adminService.ObtenerResumenPorAnioAsync(model.AnioElectoral.Value);

            if (model.EleccionesResumen.Count == 0)
            {
                TempData["InfoMessage"] = "No existen elecciones registradas para el año seleccionado.";
            }

            return View(model);
        }

        // POST: /Administrador/ObtenerResumen
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ObtenerResumen(HomeAdministradorViewModel modelFromForm)
        {
            if (!modelFromForm.AnioElectoral.HasValue)
            {
                TempData["ErrorMessage"] = "Debe seleccionar un año para consultar el resumen electoral.";
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Index), new { anioElectoral = modelFromForm.AnioElectoral });
        }
    }
}