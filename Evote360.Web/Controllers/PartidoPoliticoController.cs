using Evote360.Application.Interfaces;
using Evote360.Application.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Evote360.Web.Controllers;
    public class PartidoPoliticoController : Controller
    {
        private readonly IPartidoPoliticoService _partidoService;

        // Inyecta el servicio
        public PartidoPoliticoController(IPartidoPoliticoService partidoService)
        {
            _partidoService = partidoService;
        }

        // LISTADO PRINCIPAL (Index)
        public async Task<IActionResult> Index()
        {
            var listado = await _partidoService.GetAllViewModelAsync();
            return View(listado); // Pasa la lista de vm a la vista
        }

        // CREAR (GET)
        public IActionResult Create()
        {
            // Mandamos el ViewModel de guardado vacío para limpiar el formulario
            return View(new SavePartidoPoliticoViewModel());
        }

        // CREAR (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SavePartidoPoliticoViewModel vm)
        {
            // Valida los [Required] y [MaxLength] del ViewModel
            if (!ModelState.IsValid)
            {
                return View(vm); // Si hay errores, devuelve el formulario con los datos
            }

            await _partidoService.AddAsync(vm);
            return RedirectToAction(nameof(Index)); // Si todo sale bien, vuelve al listado
        }

        // EDITAR (GET)
        public async Task<IActionResult> Edit(int id)
        {
            var partido = await _partidoService.GetByIdSaveViewModelAsync(id);
            
            if (partido == null)
            {
                return NotFound();
            }

            return View(partido); // Envia los datos actuales del partido al formulario
        }

        // EDITAR (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SavePartidoPoliticoViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm); // Si falla alguna validaciOn, vuelve a mostrar el formulario con los errores
            }

            await _partidoService.UpdateAsync(vm);
            return RedirectToAction(nameof(Index));
        }

        // ELIMINAR (GET)
        public async Task<IActionResult> Delete(int id)
        {
            var partido = await _partidoService.GetByIdSaveViewModelAsync(id);
            
            if (partido == null)
            {
                return NotFound();
            }

            return View(partido);
        }

        // ELIMINAR (POST)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _partidoService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
