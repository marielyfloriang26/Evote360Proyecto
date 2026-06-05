using Capa_Datos.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Capa_Datos.Configurations
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

            builder.Property(p => p.Estado)
                .HasDefaultValue(true)
                .IsRequired();
        }
    }
}
