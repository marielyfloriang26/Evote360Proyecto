using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Evote360.Application.Services.Interfaces;
using Evote360.Application.ViewModels.Candidatos;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Evote360.Web.Controllers
{
    public class CandidatosController : Controller
    {
        private readonly ICandidatoService _candidatoService;

        public CandidatosController(ICandidatoService candidatoService)
        {
            _candidatoService = candidatoService;
        }

        private int GetUserId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(userIdClaim, out int userId))
            {
                return userId;
            }
            return 1; // para pruebas sin login
        }

        public async Task<IActionResult> Index()
        {
            var result = await _candidatoService.GetIndexDataAsync(GetUserId());
            if (!result.Success)
            {
                string mensaje = !string.IsNullOrEmpty(result.ErrorMessage) 
                    ? result.ErrorMessage 
                    : "Usted no tiene un partido político asignado. Por favor, comuníquese con el administrador.";
                    
                return RedirectToAction("AccessDenied", "Auth", new { mensaje = mensaje });
            }

            ViewBag.HasActiveElection = result.HasActiveElection;
            return View(result.Candidatos);
        }

        public async Task<IActionResult> Crear()
        {
            var result = await _candidatoService.ValidateCreateAccessAsync(GetUserId());
            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.ErrorMessage;
                if (result.ErrorMessage.Contains("elección activa"))
                    return RedirectToAction(nameof(Index));
                return RedirectToAction("Index", "Home");
            }

            return View(new CrearCandidatoViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(CrearCandidatoViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _candidatoService.CreateCandidatoAsync(GetUserId(), model);
            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.ErrorMessage;
                if (result.ErrorMessage.Contains("elección activa"))
                    return RedirectToAction(nameof(Index));
                
                if (result.ErrorMessage.Contains("partido político"))
                    return RedirectToAction("Index", "Home");

                return View(model);
            }

            TempData["SuccessMessage"] = "Candidato creado exitosamente.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Editar(int id)
        {
            var result = await _candidatoService.GetEditarDataAsync(GetUserId(), id);
            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.ErrorMessage;
                if (result.ErrorMessage.Contains("partido político asignado"))
                    return RedirectToAction("Index", "Home");
                return RedirectToAction(nameof(Index));
            }

            return View(result.Model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, EditarCandidatoViewModel model)
        {
            if (id != model.Id) return BadRequest();

            if (model.HaParticipado)
            {
                ModelState.Remove("Nombre");
                ModelState.Remove("Apellido");
                ModelState.Remove("Foto");
            }

            if (!ModelState.IsValid) return View(model);

            var result = await _candidatoService.UpdateCandidatoAsync(GetUserId(), model);
            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.ErrorMessage;
                if (result.ErrorMessage.Contains("partido político asignado"))
                    return RedirectToAction("Index", "Home");
                return RedirectToAction(nameof(Index));
            }

            TempData["SuccessMessage"] = "Candidato actualizado exitosamente.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ConfirmarActivar(int id)
        {
            var result = await _candidatoService.GetConfirmacionDataAsync(GetUserId(), id, true);
            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.ErrorMessage;
                if (result.ErrorMessage.Contains("partido político asignado"))
                    return RedirectToAction("Index", "Home");
                return RedirectToAction(nameof(Index));
            }

            ViewBag.NombreCandidato = result.NombreCandidato;
            return View(id);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Activar(int id)
        {
            var result = await _candidatoService.ActivarCandidatoAsync(GetUserId(), id);
            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.ErrorMessage;
                if (result.ErrorMessage.Contains("partido político asignado"))
                    return RedirectToAction("Index", "Home");
                return RedirectToAction(nameof(Index));
            }

            TempData["SuccessMessage"] = "Candidato activado exitosamente.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ConfirmarDesactivar(int id)
        {
            var result = await _candidatoService.GetConfirmacionDataAsync(GetUserId(), id, false);
            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.ErrorMessage;
                if (result.ErrorMessage.Contains("partido político asignado"))
                    return RedirectToAction("Index", "Home");
                return RedirectToAction(nameof(Index));
            }

            ViewBag.NombreCandidato = result.NombreCandidato;
            return View(id);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Desactivar(int id)
        {
            var result = await _candidatoService.DesactivarCandidatoAsync(GetUserId(), id);
            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.ErrorMessage;
                if (result.ErrorMessage.Contains("partido político asignado"))
                    return RedirectToAction("Index", "Home");
                return RedirectToAction(nameof(Index));
            }

            TempData["SuccessMessage"] = "Candidato desactivado exitosamente.";
            return RedirectToAction(nameof(Index));
        }
    }
}