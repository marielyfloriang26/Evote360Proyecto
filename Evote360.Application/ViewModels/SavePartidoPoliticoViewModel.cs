using System.ComponentModel.DataAnnotations;

namespace Evote360.Application.ViewModels;

    public class SavePartidoPoliticoViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del partido es obligatorio.")]
        [MaxLength(150, ErrorMessage = "El nombre no puede exceder los 150 caracteres.")]
        public string Nombre { get; set; } = null!;

        [Required(ErrorMessage = "Las siglas son obligatorias.")]
        [MaxLength(10, ErrorMessage = "Las siglas no pueden exceder los 10 caracteres.")]
        public string Siglas { get; set; } = null!;

        [Required(ErrorMessage = "Debe proporcionar una URL para el logo.")]
        public string LogoUrl { get; set; } = null!;

        [MaxLength(500, ErrorMessage = "La descripción no puede exceder los 500 caracteres.")]
        public string? Descripcion { get; set; }

        public bool Estado { get; set; } = true;
    }
