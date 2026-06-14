using Evote360.Application.DTOs;
using Evote360.Application.Interfaces;
using Evote360.Application.ViewModels;
using Evote360.Core.Enums;
using Evote360.Web.Helpers;
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
            ModelState.Remove("Estado");

            // validacion manual, el archivo es obligatorio
            if (vm.File == null || vm.File.Length == 0)
            {
                // Esto vincula el error directamente al campo File de la vista
                ModelState.AddModelError("File", "El logo del partido es requerido.");
            }
            else
            { // verifica si el arch es falso
            var extension = Path.GetExtension(vm.File.FileName).ToLower();
            var extensionesPermitidas = new[] { ".jpg", ".jpeg", ".png" };

            if (!extensionesPermitidas.Contains(extension) || vm.File.Length < 100)
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
            var estadoFormulario = Request.Form["Estado"].ToString().Contains("true") ? EstadoEnum.Activo : EstadoEnum.Inactivo;
            var dto = new PartidoPoliticoSaveDto
            {
                Id = 0,
                Nombre = vm.Nombre,
                Siglas = vm.Siglas,
                Descripcion = vm.Descripcion,
                Estado = estadoFormulario, //EstadoEnum.Activo,
                //(vm.Estado == EstadoEnum.Activo || Request.Form["Estado"] == "true") ? EstadoEnum.Activo : EstadoEnum.Inactivo,
                LogoUrl = "" // Inicia vacio temporalmente
            };

            var returnPartido = await _partidoService.AddAsync(dto);

            if (returnPartido != null && returnPartido.Id != 0)
            {
            dto.Id = returnPartido.Id;

            // Llama al helper usando el id real obtenido
            dto.LogoUrl = await UploadFile.Upload(vm.File!, dto.Id, "Partidos");

            //Actualiza con la ruta definitiva
            await _partidoService.UpdateAsync(dto);
            }

                TempData["SuccessMessage"] = "Partido político registrado exitosamente.";
                return RedirectToAction(nameof(Index)); // Si todo sale bien, vuelve al listado
            }

        // EDITAR (GET)
        public async Task<IActionResult> Editar(int id)
        {
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

            return View(partidoVm); // Envia los datos actuales del partido al formulario
        }

        // EDITAR (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(SavePartidoPoliticoViewModel vm)
        {
            ModelState.Remove("Estado");

            if (!string.IsNullOrWhiteSpace(vm.Siglas) && await _partidoService.ExisteSiglasAsync(vm.Siglas, vm.Id))
        {
            ModelState.AddModelError("Siglas", "Ya existe un partido político registrado con estas siglas.");
        }

        if (vm.File != null && vm.File.Length > 0)
        {
            var extension = Path.GetExtension(vm.File.FileName).ToLower();
            var extensionesPermitidas = new[] { ".jpg", ".jpeg", ".png" };

            if (!extensionesPermitidas.Contains(extension) || vm.File.Length < 100)
            {
                ModelState.AddModelError("File", "El logo del partido debe ser una imagen válida.");
            }
        }
      
            if (!ModelState.IsValid)
            {
                return View(vm); 
            }

            var estadoFormulario = Request.Form["Estado"].ToString().Contains("true") ? EstadoEnum.Activo : EstadoEnum.Inactivo;

            // instancia el dto de guardado con la info de la pantalla
            var dto = new PartidoPoliticoSaveDto
            {
                Id = vm.Id,
                Nombre = vm.Nombre,
                Siglas = vm.Siglas,
                Descripcion = vm.Descripcion,
                Estado = estadoFormulario, //Request.Form["Estado"].ToString().Contains("true") ? EstadoEnum.Activo : EstadoEnum.Inactivo
                LogoUrl = vm.LogoUrl
            };

            if (vm.File != null && vm.File.Length > 0)
            {
            var currentDto = await _partidoService.GetByIdSaveDtoAsync(vm.Id);
            string currentImagePath = currentDto?.LogoUrl ?? "";
            

           /* if (currentDto != null)
            {
                currentImagePath = currentDto.LogoUrl ?? "";
            } */

            // Ejecuta el helper pasando los datos de edición
            dto.LogoUrl = await UploadFile.Upload(vm.File, dto.Id, "Partidos", true, currentImagePath);
            }

            // Actualiza el registro completo
            await _partidoService.UpdateAsync(dto);
            
        /*    // procesa la nueva imagen solo si el usuario subio una
        if (vm.File != null && vm.File.Length > 0)
        {
            string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\partidos");
            
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            string uniqueFileName = Guid.NewGuid().ToString() + "_" + vm.File.FileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await vm.File.CopyToAsync(fileStream);
            }

            // Si sube un archivo nuevo, sobreescribe la propiedad logo con la nueva ruta
            vm.LogoUrl = "/images/partidos/" + uniqueFileName;
        }
            var updateDto = new PartidoPoliticoSaveDto
        {
            Id = vm.Id,
            Nombre = vm.Nombre,
            Siglas = vm.Siglas,
            LogoUrl = vm.LogoUrl,
            Descripcion = vm.Descripcion,
            Estado = vm.Estado
        }; */

           TempData["SuccessMessage"] = "Partido político actualizado exitosamente.";
           return RedirectToAction(nameof(Index));
        }

        
        // (GET)
        public async Task<IActionResult> ConfirmarActivar(int id)
        {
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
            var dto = await _partidoService.GetByIdSaveDtoAsync(id);
            if (dto != null)
        {
            dto.Estado = EstadoEnum.Activo; 
            await _partidoService.UpdateAsync(dto);
            TempData["SuccessMessage"] = "El partido político ha sido activado con éxito.";
        }
            return RedirectToAction(nameof(Index));
        }

        //  CONFIRMAR DESACTIVACION (GET)
        public async Task<IActionResult> ConfirmarDesactivar(int id)
        {
            var dto = await _partidoService.GetByIdSaveDtoAsync(id);
            if (dto == null) return NotFound();
        
            ViewBag.NombrePartido = $"{dto.Nombre} ({dto.Siglas})";
            return View(dto.Id);  
        }

        // DESACTIVAR (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Desactivar(int id)
        {
            var dto = await _partidoService.GetByIdSaveDtoAsync(id);
            if (dto != null)
        {
            dto.Estado = EstadoEnum.Inactivo; 
            await _partidoService.UpdateAsync(dto);
            TempData["SuccessMessage"] = "El partido político ha sido desactivado con éxito.";
        }
            return RedirectToAction(nameof(Index));
        }
            }