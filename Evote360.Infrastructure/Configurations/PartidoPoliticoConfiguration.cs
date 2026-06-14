using Evote360.Core.Entities;
using Evote360.Core.Enums;
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
            .HasConversion<int>() 
            .HasColumnType("int")
                .HasDefaultValue(EstadoEnum.Activo)
                .IsRequired();


            builder.Property(p => p.Descripcion)
                .HasMaxLength(500) 
                .IsRequired(false); 

        }
    }
}
