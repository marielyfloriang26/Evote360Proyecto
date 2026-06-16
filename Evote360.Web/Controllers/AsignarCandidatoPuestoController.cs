using Evote360.Application.Services.Interfaces;
using Evote360.Application.ViewModels.Asignaciones;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Evote360.Web.Controllers
{
    [Authorize(Roles = "DirigentePolitico")]
    public class AsignarCandidatoPuestoController : Controller
    {
        private readonly IAsignarCandidatoPuestoService _asignacionService;

        public AsignarCandidatoPuestoController(IAsignarCandidatoPuestoService asignacionService)
        {
            _asignacionService = asignacionService;
        }

        private int ObtenerUserIdLogueado()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int.TryParse(userIdClaim, out int userId);
            return userId;
        }

        public async Task<IActionResult> Index()
        {
            int userId = ObtenerUserIdLogueado();
            var result = await _asignacionService.GetIndexDataAsync(userId);
            
            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.ErrorMessage;
            }

            return View(result.Data);
        }

        public async Task<IActionResult> Agregar()
        {
            int userId = ObtenerUserIdLogueado();
            var result = await _asignacionService.GetFormFieldsAsync(userId);

            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.ErrorMessage;
                return RedirectToAction(nameof(Index));
            }

            return View(result.Form);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Agregar(CrearAsignacionViewModel model)
        {
            int userId = ObtenerUserIdLogueado();

            if (!ModelState.IsValid)
            {
                var fields = await _asignacionService.GetFormFieldsAsync(userId);
                model.CandidatosDisponibles = fields.Form.CandidatosDisponibles;
                model.PuestosDisponibles = fields.Form.PuestosDisponibles;
                return View(model);
            }

            var result = await _asignacionService.RegistrarAsignacionAsync(userId, model);
            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage);
                var fields = await _asignacionService.GetFormFieldsAsync(userId);
                model.CandidatosDisponibles = fields.Form.CandidatosDisponibles;
                model.PuestosDisponibles = fields.Form.PuestosDisponibles;
                return View(model);
            }

            TempData["SuccessMessage"] = "Asignación registrada exitosamente.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ConfirmarEliminar(int id)
        {
            // Retorna una pequeña vista de confirmación como lo pide el flujo
            ViewBag.AsignacionId = id;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int id)
        {
            int userId = ObtenerUserIdLogueado();
            var result = await _asignacionService.EliminarAsignacionAsync(userId, id);

            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.ErrorMessage;
            }
            else
            {
                TempData["SuccessMessage"] = "Relación desvinculada correctamente.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}