using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evote360.Application.DTOs.Candidato
{
    public class CandidatoDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public IFormFile? Foto { get; set; }
        public string? FotoUrl { get; set; }
        public string PuestoAsociado { get; set; } = string.Empty;
        public bool Estado { get; set; }

    }
}
