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
        public IActionResult Crear()
        {
            // Manda el ViewModel de guardado vacio para limpiar el formulario
            return View(new SavePartidoPoliticoViewModel());
        }

        // CREAR (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(SavePartidoPoliticoViewModel vm)
        {
            // validacion manual, el archivo es obligatorio
            if (vm.File == null || vm.File.Length == 0)
            {
                // Esto vincula el error directamente al campo File de la vista
                ModelState.AddModelError("File", "El logo del partido es requerido.");
            }

            // Las siglas no pueden repetirse
            if (!string.IsNullOrWhiteSpace(vm.Siglas))
            {
                bool yaExiste = await _partidoService.ExisteSiglasAsync(vm.Siglas);
                if (yaExiste)
                {
                    ModelState.AddModelError("Siglas", "Ya existe un partido político registrado con estas siglas.");
                }
            }

            // Valida los [Required] y [MaxLength] del ViewModel
            if (!ModelState.IsValid)
            {
                return View(vm); // Si hay errores, devuelve el formulario con los datos
            }

            // cargar y guardar el archivo en wwwroot
            if (vm.File != null && vm.File.Length > 0)
            {
                // ruta de la carpeta base 
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\partidos");
            
            // se asegura que la carp exista 
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            // genera un nombre unico para el archivo y no se sobreescriba
            string uniqueFileName = Guid.NewGuid().ToString() + "_" + vm.File.FileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            // guarda el archivo en el disco 
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await vm.File.CopyToAsync(fileStream);
            }

            // guarda la ruta web para usarla en las etiquetas img
            vm.LogoUrl = "/images/partidos/" + uniqueFileName;
            }

            await _partidoService.AddAsync(vm);
            TempData["SuccessMessage"] = "Partido político registrado exitosamente.";
            return RedirectToAction(nameof(Index)); // Si todo sale bien, vuelve al listado
        }

        // EDITAR (GET)
        public async Task<IActionResult> Editar(int id)
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
        public async Task<IActionResult> Editar(SavePartidoPoliticoViewModel vm)
        {
            if (!string.IsNullOrWhiteSpace(vm.Siglas))
        {
            // Le manda las siglas Y el id actual 
            bool yaExisteOtrasSiglas = await _partidoService.ExisteSiglasAsync(vm.Siglas, vm.Id);
            if (yaExisteOtrasSiglas)
            {
                ModelState.AddModelError("Siglas", "Ya existe un partido político registrado con estas siglas.");
            }
        }
            if (!ModelState.IsValid)
            {
                return View(vm); // Si falla alguna validaciOn, vuelve a mostrar el formulario con los errores
            }

            await _partidoService.UpdateAsync(vm);
            TempData["SuccessMessage"] = "Partido político actualizado exitosamente.";
            return RedirectToAction(nameof(Index));
        }

        // ELIMINAR (GET)
        /*public async Task<IActionResult> Delete(int id)
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
        } */
        // A. PANTALLA CONFIRMAR ACTIVACIÓN (GET)
public async Task<IActionResult> ConfirmarActivar(int id)
{
    var partido = await _partidoService.GetByIdSaveViewModelAsync(id);
    if (partido == null) return NotFound();
    
    ViewBag.NombrePartido = $"{partido.Nombre} ({partido.Siglas})";
    return View(partido.Id); // Pasa solo el ID como un int al modelo de la vista
}

// ACTIVAR (POST)
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Activar(int id)
{
    var partido = await _partidoService.GetByIdSaveViewModelAsync(id);
    if (partido != null)
    {
        partido.Estado = true;
        await _partidoService.UpdateAsync(partido);
        TempData["SuccessMessage"] = "El partido político ha sido activado con éxito.";
    }
    return RedirectToAction(nameof(Index));
}

//  CONFIRMAR DESACTIVACION (GET)
public async Task<IActionResult> ConfirmarDesactivar(int id)
{
    var partido = await _partidoService.GetByIdSaveViewModelAsync(id);
    if (partido == null) return NotFound();
    
    ViewBag.NombrePartido = $"{partido.Nombre} ({partido.Siglas})";
    return View(partido.Id); // Pasa solo el id como un int al modelo de la vista
}

// DESACTIVAR (POST)
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Desactivar(int id)
{
    var partido = await _partidoService.GetByIdSaveViewModelAsync(id);
    if (partido != null)
    {
        partido.Estado = false;
        await _partidoService.UpdateAsync(partido);
        TempData["SuccessMessage"] = "El partido político ha sido desactivado con éxito.";
    }
    return RedirectToAction(nameof(Index));
}
    }
