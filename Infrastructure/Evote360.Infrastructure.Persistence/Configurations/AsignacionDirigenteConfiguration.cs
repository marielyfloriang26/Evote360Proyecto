using Evote360.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Evote360.Infrastructure.Configurations
{
    public class AsignacionDirigenteConfiguration : IEntityTypeConfiguration<AsignacionDirigente>
    {
        public void Configure(EntityTypeBuilder<AsignacionDirigente> builder)
        {
            builder.ToTable("AsignacionDirigentes");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.FechaAsignacion)
                .HasDefaultValueSql("GETDATE()")
                .IsRequired();

            // 1:1 with Usuario
            builder.HasOne(a => a.Usuario)
                .WithOne(u => u.AsignacionDirigente)
                .HasForeignKey<AsignacionDirigente>(a => a.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            // N:1 with PartidoPolitico
            builder.HasOne(a => a.Partido)
                .WithMany(p => p.AsignacionesDirigentes)
                .HasForeignKey(a => a.PartidoId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
