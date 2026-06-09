using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evote360.Application.DTOs
{
    public class CrearCandidatoDTO
    {
        string NombreCandidato { get; set; }
        string ApellidoCandidato { get; set; }
        IFormFile FotoCandidato { get; set; }
    }
}
