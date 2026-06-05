using Capa_Datos.Entities;
using Microsoft.EntityFrameworkCore;

namespace Capa_Datos.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<PuestoElectivo> PuestosElectivos { get; set; } = null!;
        public DbSet<Ciudadano> Ciudadanos { get; set; } = null!;
        public DbSet<PartidoPolitico> PartidosPoliticos { get; set; } = null!;
        public DbSet<Usuario> Usuarios { get; set; } = null!;
        public DbSet<AsignacionDirigente> AsignacionDirigentes { get; set; } = null!;
        public DbSet<Candidato> Candidatos { get; set; } = null!;
        public DbSet<Eleccion> Elecciones { get; set; } = null!;
        public DbSet<AlianzaPolitica> AlianzasPoliticas { get; set; } = null!;
        public DbSet<AsignarCandidatoPuesto> AsignacionesCandidatosPuestos { get; set; } = null!;
        public DbSet<Voto> Votos { get; set; } = null!;
        public DbSet<CodigoVerificacion> CodigosVerificacion { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Apply all entity configurations in this assembly
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }
    }
}
