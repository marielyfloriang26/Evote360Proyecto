using Evote360.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Evote360.Infrastructure.Configurations
{
    public class EleccionConfiguration : IEntityTypeConfiguration<Eleccion>
    {
        public void Configure(EntityTypeBuilder<Eleccion> builder)
        {
            builder.ToTable("Elecciones");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Nombre)
                .HasMaxLength(150)
                .IsRequired();

            builder.Property(e => e.FechaInicio)
                .IsRequired();

            builder.Property(e => e.FechaFin)
                .IsRequired();

            builder.Property(e => e.EstadoElectoral)
                .HasMaxLength(20)
                .IsRequired();
        }
    }
}
