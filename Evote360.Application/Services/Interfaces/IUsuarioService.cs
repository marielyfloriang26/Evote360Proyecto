using Evote360.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evote360.Application.Services.Interfaces
{
    public interface IUsuarioService
    {
        public Task<IEnumerable<UsuarioDTO>> GetAllUsuarios();
    }
}
