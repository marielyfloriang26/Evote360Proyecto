using Evote360.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Evote360.Infrastructure.Configurations
{
    public class PartidoPoliticoConfiguration : IEntityTypeConfiguration<PartidoPolitico>
    {
        public void Configure(EntityTypeBuilder<PartidoPolitico> builder)
        {
            builder.ToTable("PartidosPoliticos");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Nombre)
                .HasMaxLength(150)
                .IsRequired();

            builder.Property(p => p.Siglas)
                .HasMaxLength(10)
                .IsRequired();

            builder.HasIndex(p => p.Siglas)
                .IsUnique();

            builder.Property(p => p.LogoUrl)
                .HasMaxLength(250)
                .IsRequired();

            builder.Property(p => p.Estado)
                .HasDefaultValue(true)
                .IsRequired();

            builder.Property(p => p.Descripcion)
                .HasMaxLength(500) 
                .IsRequired(false);
        }
    }
}
