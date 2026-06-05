using Capa_Datos.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Capa_Datos.Configurations
{
    public class VotoConfiguration : IEntityTypeConfiguration<Voto>
    {
        public void Configure(EntityTypeBuilder<Voto> builder)
        {
            builder.ToTable("Votos");

            builder.HasKey(v => v.Id);

            // N:1 with Eleccion
            builder.HasOne(v => v.Eleccion)
                .WithMany(e => e.Votos)
                .HasForeignKey(v => v.EleccionId)
                .OnDelete(DeleteBehavior.Cascade);

            // N:1 with PuestoElectivo
            builder.HasOne(v => v.Puesto)
                .WithMany(p => p.Votos)
                .HasForeignKey(v => v.PuestoId)
                .OnDelete(DeleteBehavior.Restrict);

            // N:1 with Candidato (nullable)
            builder.HasOne(v => v.Candidato)
                .WithMany(c => c.Votos)
                .HasForeignKey(v => v.CandidatoId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
