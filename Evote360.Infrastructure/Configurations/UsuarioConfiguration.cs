using Evote360.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Evote360.Infrastructure.Configurations
{
    public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> builder)
        {
            builder.ToTable("Usuarios");

            builder.HasKey(u => u.Id);

            builder.Property(u => u.NombreUsuario)
                .HasMaxLength(50)
                .IsRequired();

            builder.HasIndex(u => u.NombreUsuario)
                .IsUnique();

            builder.Property(u => u.ClaveHash)
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(u => u.Rol)
                .HasMaxLength(30)
                .IsRequired();

            builder.Property(u => u.Estado)
                .HasDefaultValue(true)
                .IsRequired();
        }
    }
}
