using Capa_Datos.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Capa_Datos.Configurations
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

            builder.Property(c => c.NombreCompleto)
                .HasMaxLength(150)
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
