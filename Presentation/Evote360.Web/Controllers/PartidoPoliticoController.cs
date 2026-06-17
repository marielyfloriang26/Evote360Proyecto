using Evote360.Application.DTOs;
using Evote360.Application.DTOs.PartidoPolitico;
using Evote360.Application.Interfaces;
using Evote360.Application.Services.Interfaces;
using Evote360.Application.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Evote360.Web.Controllers;
    public class PartidoPoliticoController : Controller
    {
        private readonly IPartidoPoliticoService _partidoService;
        private readonly IFileStorageService _fileStorageService;
        private readonly IEleccionService _eleccionService;

        
        public PartidoPoliticoController(IPartidoPoliticoService partidoService, IFileStorageService fileStorageService, IEleccionService eleccionService)
        {
            _partidoService = partidoService;
            _fileStorageService = fileStorageService;
            _eleccionService = eleccionService;
        }

        // LISTADO PRINCIPAL 
        public async Task<IActionResult> Index()
        {
            bool hayEleccionActiva = await _eleccionService.ExisteEleccionActivaAsync(); 
        ViewBag.EleccionActiva = hayEleccionActiva;

        if (hayEleccionActiva)
        {
            TempData["ErrorMessage"] = "No se pueden modificar partidos políticos mientras exista una elección activa.";
        }
            var dtos = await _partidoService.GetAllDtoAsync();
            
            var listado = dtos.Select(p => new PartidoPoliticoViewModel
        {
            Id = p.Id,
            Nombre = p.Nombre,
            Siglas = p.Siglas,
            LogoUrl = p.LogoUrl!,
            Descripcion = p.Descripcion,
            Estado = p.Estado 
        }).ToList();

            return View(listado); 
        }

        // CREAR (GET)
        public async Task<IActionResult> Crear()
        {
            if (await _eleccionService.ExisteEleccionActivaAsync()) 
            {
                TempData["ErrorMessage"] = "No se puede crear un partido político mientras exista una elección activa.";
                return RedirectToAction(nameof(Index));
            }

            return View(new SavePartidoPoliticoViewModel());
        }

        // CREAR (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(SavePartidoPoliticoViewModel vm)
        {
            if (await _eleccionService.ExisteEleccionActivaAsync()) 
            {
                TempData["ErrorMessage"] = "No se puede crear un partido político mientras exista una elección activa.";
                return RedirectToAction(nameof(Index));
            }
            ModelState.Remove("Estado");

            
            if (vm.File == null || vm.File.Length == 0)
            {
                // Esto vincula el error directamente al campo File de la vista
                ModelState.AddModelError("File", "El logo del partido es requerido.");
            }
            else
            { // verifica si el arch es falso
            var extension = Path.GetExtension(vm.File.FileName).ToLower();
            var extensionesPermitidas = new[] { ".jpg", ".jpeg", ".png" };

            if (!extensionesPermitidas.Contains(extension) || vm.File.Length < 100 || !ValidarImagenReal(vm.File))
            {
                ModelState.AddModelError("File", "El logo del partido debe ser una imagen válida.");
            }
            
            }

            // Las siglas no pueden repetirse
            if (!string.IsNullOrWhiteSpace(vm.Siglas) && await _partidoService.ExisteSiglasAsync(vm.Siglas))
            {
                ModelState.AddModelError("Siglas", "Ya existe un partido político registrado con estas siglas.");
                
            }

            // ojo
          /*  if (ModelState.ContainsKey("Estado"))
            {
                ModelState["Estado"].Errors.Clear();
            } */

            if (!ModelState.IsValid)
            {
                return View(vm); // Si hay errores, devuelve el formulario con los datos
            }

            // OJO Captura el valor real del interruptor de la vista de forma segura
            var estadoFormulario = Request.Form["Estado"].ToString().Contains("true");

            var dto = new PartidoPoliticoSaveDto
            {
                Id = 0,
                Nombre = vm.Nombre,
                Siglas = vm.Siglas,
                Descripcion = vm.Descripcion,
                Estado = estadoFormulario, 
                LogoUrl = "" // Inicia vacio temporalmente
            };

            var returnPartido = await _partidoService.AddAsync(dto);

            if (returnPartido != null && returnPartido.Id != 0)
            {
            dto.Id = returnPartido.Id;

           
            dto.LogoUrl = await _fileStorageService.SaveFileAsync(vm.File!, "Partidos");

            //Actualiza con la ruta definitiva
            await _partidoService.UpdateAsync(dto);
            }

                TempData["SuccessMessage"] = "Partido político registrado exitosamente.";
                return RedirectToAction(nameof(Index)); 
            }

        // EDITAR (GET)
        public async Task<IActionResult> Editar(int id)
        {
            if (await _eleccionService.ExisteEleccionActivaAsync()) 
            {
                TempData["ErrorMessage"] = "No se puede editar un partido político mientras exista una elección activa.";
                return RedirectToAction(nameof(Index));
            }

            var dto = await _partidoService.GetByIdSaveDtoAsync(id);
        
        if (dto == null)
        {
            return NotFound();
        }

        var partidoVm = new SavePartidoPoliticoViewModel
        {
            Id = dto.Id,
            Nombre = dto.Nombre,
            Siglas = dto.Siglas,
            LogoUrl = dto.LogoUrl!,
            Descripcion = dto.Descripcion,
            Estado = dto.Estado
        };

            return View(partidoVm); 
        }

        // EDITAR (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(SavePartidoPoliticoViewModel vm)
        {
            if (await _eleccionService.ExisteEleccionActivaAsync()) 
            {
                TempData["ErrorMessage"] = "No se puede editar un partido político mientras exista una elección activa.";
                return RedirectToAction(nameof(Index));
            }

            ModelState.Remove("Estado");

            if (!string.IsNullOrWhiteSpace(vm.Siglas) && await _partidoService.ExisteSiglasAsync(vm.Siglas, vm.Id))
        {
            ModelState.AddModelError("Siglas", "Ya existe un partido político registrado con estas siglas.");
        }

        if (vm.File != null && vm.File.Length > 0)
        {
            var extension = Path.GetExtension(vm.File.FileName).ToLower();
            var extensionesPermitidas = new[] { ".jpg", ".jpeg", ".png" };

            if (!extensionesPermitidas.Contains(extension) || vm.File.Length < 100 || !ValidarImagenReal(vm.File))
            {
                ModelState.AddModelError("File", "El logo del partido debe ser una imagen válida.");
            }
        }
      
            if (!ModelState.IsValid)
            {
                return View(vm); 
            }


            // instancia el dto de guardado con la info de la pantalla
            var dto = new PartidoPoliticoSaveDto
            {
                Id = vm.Id,
                Nombre = vm.Nombre,
                Siglas = vm.Siglas,
                Descripcion = vm.Descripcion,
                LogoUrl = vm.LogoUrl,

                Estado = Request.Form["Estado"].ToString().Contains("true") 

            };

            if (vm.File != null && vm.File.Length > 0)
            {
            var currentDto = await _partidoService.GetByIdSaveDtoAsync(vm.Id);
            string currentImagePath = currentDto?.LogoUrl ?? "";
    

            if (!string.IsNullOrEmpty(currentImagePath))
            {
                _fileStorageService.DeleteFile(currentImagePath);
            }

            // Ejecuta  pasando los datos de edicion
            dto.LogoUrl = await _fileStorageService.SaveFileAsync(vm.File, "Partidos");
            }

            try 
            {
                await _partidoService.UpdateAsync(dto);
                
                TempData["SuccessMessage"] = "Partido político actualizado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(vm);
            }
        }

        
        // (GET)
        public async Task<IActionResult> ConfirmarActivar(int id)
        {
            if (await _eleccionService.ExisteEleccionActivaAsync()) 
            {
                TempData["ErrorMessage"] = "No se puede activar un partido político mientras exista una elección activa.";
                return RedirectToAction(nameof(Index));
            }
            var dto = await _partidoService.GetByIdSaveDtoAsync(id);
            if (dto == null) return NotFound();
            
            ViewBag.NombrePartido = $"{dto.Nombre} ({dto.Siglas})";
            return View(dto.Id); // Pasa solo el ID como un int al modelo de la vista
        }

        // ACTIVAR (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Activar(int id)
        {
            if (await _eleccionService.ExisteEleccionActivaAsync()) 
            {
                TempData["ErrorMessage"] = "No se puede activar un partido político mientras exista una elección activa.";
                return RedirectToAction(nameof(Index));
            }
            try
            {
                var dto = await _partidoService.GetByIdSaveDtoAsync(id);
                if (dto != null)
                {
                    dto.Estado = true; 
                    await _partidoService.UpdateAsync(dto);
                    TempData["SuccessMessage"] = "El partido político ha sido activado con éxito.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }

        //  CONFIRMAR DESACTIVACION (GET)
        public async Task<IActionResult> ConfirmarDesactivar(int id)
        {
            if (await _eleccionService.ExisteEleccionActivaAsync()) 
            {
                TempData["ErrorMessage"] = "No se puede desactivar un partido político mientras exista una elección activa.";
                return RedirectToAction(nameof(Index));
            }

            var dto = await _partidoService.GetByIdSaveDtoAsync(id);
            if (dto == null) return NotFound();

            if (await _partidoService.TieneDirigenteActivoAsync(id))
        {
        TempData["ErrorMessage"] = "No se puede desactivar este partido político porque tiene un dirigente político asignado.";
        return RedirectToAction(nameof(Index));
        }
        
        if (await _partidoService.TieneCandidatosActivosAsync(id))
        {
            TempData["ErrorMessage"] = "No se puede desactivar este partido político porque tiene candidatos activos registrados.";
            return RedirectToAction(nameof(Index));
        }
        
            ViewBag.NombrePartido = $"{dto.Nombre} ({dto.Siglas})";
            return View(dto.Id);  
        }

        // DESACTIVAR (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Desactivar(int id)
        {
            if (await _eleccionService.ExisteEleccionActivaAsync()) 
            {
                TempData["ErrorMessage"] = "No se puede desactivar un partido político mientras exista una elección activa.";
                return RedirectToAction(nameof(Index));
            }
            
            try
            {
                var dto = await _partidoService.GetByIdSaveDtoAsync(id);
                if (dto != null)
                {
                    if (await _partidoService.TieneDirigenteActivoAsync(id))
                    {
                        TempData["ErrorMessage"] = "No se puede desactivar este partido político porque tiene un dirigente político asignado.";
                        return RedirectToAction(nameof(Index));
                    }
                    if (await _partidoService.TieneCandidatosActivosAsync(id))
                    {
                        TempData["ErrorMessage"] = "No se puede desactivar este partido político porque tiene candidatos activos registrados.";
                        return RedirectToAction(nameof(Index));
                    }
                    dto.Estado = false; 
                    await _partidoService.UpdateAsync(dto);
                    TempData["SuccessMessage"] = "El partido político ha sido desactivado con éxito.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }

        private bool ValidarImagenReal(IFormFile file)
        {
            try
            {
                using (var stream = file.OpenReadStream())
                {
                    if (stream.Length < 4) return false;
                    byte[] buffer = new byte[4];
                    int bytesRead = stream.Read(buffer, 0, 4);
                    if (bytesRead < 4) return false;

                    // JPEG magic bytes: FF D8 FF
                    if (buffer[0] == 0xFF && buffer[1] == 0xD8 && buffer[2] == 0xFF) return true;
                    // PNG magic bytes: 89 50 4E 47
                    if (buffer[0] == 0x89 && buffer[1] == 0x50 && buffer[2] == 0x4E && buffer[3] == 0x47) return true;
                }
            }
            catch
            {
                return false;
            }
            return false;
        }
    }