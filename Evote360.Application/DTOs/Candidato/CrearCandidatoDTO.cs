using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evote360.Application.DTOs.Candidato
{
    public class CrearCandidatoDTO
    {
        public string Nombre { get; set; } = null!;
        public string Apellido { get; set; } = null!;
        public IFormFile Foto { get; set; } = null!;
        public int PartidoId { get; set; }
        public bool Estado { get; set; } = true;
    }
}
