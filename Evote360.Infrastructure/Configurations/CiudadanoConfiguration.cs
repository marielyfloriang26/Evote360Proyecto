using Evote360.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Evote360.Infrastructure.Configurations
{
    public class CiudadanoConfiguration : IEntityTypeConfiguration<Ciudadano>
    {
        public void Configure(EntityTypeBuilder<Ciudadano> builder)
        {
            builder.ToTable("Ciudadanos");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Cedula)
                .HasMaxLength(11)
                .IsRequired();

            builder.HasIndex(c => c.Cedula)
                .IsUnique();

            builder.Property(c => c.Nombre)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(c => c.Apellido)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(c => c.Correo)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(c => c.HaVotado)
                .HasDefaultValue(false)
                .IsRequired();

            builder.Property(c => c.Estado)
                .HasDefaultValue(true)
                .IsRequired();
        }
    }
}
