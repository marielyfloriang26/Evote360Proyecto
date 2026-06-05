using Capa_Datos.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Capa_Datos.Configurations
{
    public class AsignarCandidatoPuestoConfiguration : IEntityTypeConfiguration<AsignarCandidatoPuesto>
    {
        public void Configure(EntityTypeBuilder<AsignarCandidatoPuesto> builder)
        {
            builder.ToTable("AsignarCandidatoPuesto");

            builder.HasKey(a => a.Id);

            // N:1 with Candidato
            builder.HasOne(a => a.Candidato)
                .WithMany(c => c.AsignacionesPuestos)
                .HasForeignKey(a => a.CandidatoId)
                .OnDelete(DeleteBehavior.Restrict);

            // N:1 with PuestoElectivo
            builder.HasOne(a => a.Puesto)
                .WithMany(p => p.AsignacionesCandidatos)
                .HasForeignKey(a => a.PuestoId)
                .OnDelete(DeleteBehavior.Restrict);

            // N:1 with Eleccion
            builder.HasOne(a => a.Eleccion)
                .WithMany(e => e.AsignacionesCandidatos)
                .HasForeignKey(a => a.EleccionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
