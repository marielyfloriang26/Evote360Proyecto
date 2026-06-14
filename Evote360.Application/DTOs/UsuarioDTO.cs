using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Evote360.Core.Enums;

namespace Evote360.Application.DTOs
{
    public class UsuarioDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string NombreUsuario { get; set; } = string.Empty;
        public RolUsuarioEnum Rol { get; set; }
        public bool Estado { get; set; }
    }
}
