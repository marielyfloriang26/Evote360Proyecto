using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evote360.Application.DTOs
{
    public class CandidatoDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public IFormFile Foto { get; set; }
        public string PuestoAsociado { get; set; }
        public bool Estado { get; set; }

    }
}
