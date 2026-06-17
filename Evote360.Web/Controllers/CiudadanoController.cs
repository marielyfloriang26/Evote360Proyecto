using Evote360.Application.DTOs;
using Evote360.Application.Services.Interfaces;
using Evote360.Application.ViewModels.Ciudadano;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Evote360.Web.Controllers
{
    [Authorize(Roles = "Administrador")] // requerimiento para el módulo administrativo
    public class CiudadanoController : Controller
    {
        private readonly ICiudadanoService _ciudadanoService;

        public CiudadanoController(ICiudadanoService ciudadanoService)
        {
            _ciudadanoService = ciudadanoService;
        }

        public async Task<IActionResult> Index()
        {
            bool eleccionActiva = await _ciudadanoService.ExisteEleccionActivaAsync();
            if (eleccionActiva)
            {
                ViewBag.AlertaEleccion = "No se pueden modificar ciudadanos mientras exista una elección activa.";
            }
            ViewBag.EleccionActiva = eleccionActiva;
            
            var ciudadanos = await _ciudadanoService.ObtenerTodosAsync();
            return View(ciudadanos);
        }

        public async Task<IActionResult> Crear()
        {
            if (await _ciudadanoService.ExisteEleccionActivaAsync())
            {
                TempData["ErrorMessage"] = "No se puede crear un ciudadano mientras exista una elección activa.";
                return RedirectToAction(nameof(Index));
            }

            var vm = new CiudadanoSaveViewModel { Estado = true };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(CiudadanoSaveViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var dto = new CiudadanoSaveDto
            {
                Cedula = vm.Cedula,
                Nombre = vm.Nombre,
                Apellido = vm.Apellido,
                Correo = vm.Correo,
                Estado = vm.Estado
            };

            var (success, message) = await _ciudadanoService.CrearAsync(dto);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, message);
                return View(vm);
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Editar(int id)
        {
            if (await _ciudadanoService.ExisteEleccionActivaAsync())
            {
                TempData["ErrorMessage"] = "No se puede editar un ciudadano mientras exista una elección activa.";
                return RedirectToAction(nameof(Index));
            }

            var dto = await _ciudadanoService.ObtenerPorIdAsync(id);
            if (dto == null) return NotFound();

            var vm = new CiudadanoSaveViewModel
            {
                Id = dto.Id,
                Cedula = dto.Cedula,
                Nombre = dto.Nombre,
                Apellido = dto.Apellido,
                Correo = dto.Correo,
                Estado = dto.Estado,
                BloquearCedula = await _ciudadanoService.HaParticipadoEnEleccionesAsync(id)
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(CiudadanoSaveViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var dto = new CiudadanoSaveDto
            {
                Id = vm.Id,
                Cedula = vm.Cedula,
                Nombre = vm.Nombre,
                Apellido = vm.Apellido,
                Correo = vm.Correo,
                Estado = vm.Estado
            };

            var (success, message) = await _ciudadanoService.EditarAsync(dto);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, message);
                return View(vm);
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ConfirmarActivar(int id)
        {
            if (await _ciudadanoService.ExisteEleccionActivaAsync()) return RedirectToAction(nameof(Index));
            var dto = await _ciudadanoService.ObtenerPorIdAsync(id);
            if (dto == null) return NotFound();
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Activar(int id)
        {
            var (success, message) = await _ciudadanoService.ActivarAsync(id);
            if (!success) TempData["ErrorMessage"] = message;
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ConfirmarDesactivar(int id)
        {
            if (await _ciudadanoService.ExisteEleccionActivaAsync()) return RedirectToAction(nameof(Index));
            var dto = await _ciudadanoService.ObtenerPorIdAsync(id);
            if (dto == null) return NotFound();
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Desactivar(int id)
        {
            var (success, message) = await _ciudadanoService.DesactivarAsync(id);
            if (!success) TempData["ErrorMessage"] = message;
            return RedirectToAction(nameof(Index));
        }
    }
}