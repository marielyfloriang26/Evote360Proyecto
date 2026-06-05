using Capa_Datos.Context;
using Capa_Datos.Entities;
using Capa_Datos.Repositories.Interfaces;

namespace Capa_Datos.Repositories.Implementations
{
    public class UsuarioRepository : RepositoryAsync<Usuario>, IUsuarioRepository
    {
        public UsuarioRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
