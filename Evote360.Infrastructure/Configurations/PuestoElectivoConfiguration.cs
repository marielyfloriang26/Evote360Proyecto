using Evote360.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Evote360.Infrastructure.Configurations
{
    public class PuestoElectivoConfiguration : IEntityTypeConfiguration<PuestoElectivo>
    {
        public void Configure(EntityTypeBuilder<PuestoElectivo> builder)
        {
            builder.ToTable("PuestosElectivos");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Nombre)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(p => p.Descripcion)
                .HasMaxLength(500) 
                .IsRequired();

            builder.Property(p => p.Estado)
                .HasDefaultValue(true)
                .IsRequired();
        }
    }
}
