using Capa_Datos.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Capa_Datos.Configurations
{
    public class CodigoVerificacionConfiguration : IEntityTypeConfiguration<CodigoVerificacion>
    {
        public void Configure(EntityTypeBuilder<CodigoVerificacion> builder)
        {
            builder.ToTable("CodigosVerificacion");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Codigo)
                .HasMaxLength(6)
                .IsRequired();

            builder.Property(c => c.FechaGeneracion)
                .HasDefaultValueSql("GETDATE()")
                .IsRequired();

            builder.Property(c => c.FechaExpiracion)
                .IsRequired();

            builder.Property(c => c.Usado)
                .HasDefaultValue(false)
                .IsRequired();

            // N:1 with Ciudadano
            builder.HasOne(c => c.Ciudadano)
                .WithMany(ci => ci.CodigosVerificacion)
                .HasForeignKey(c => c.CiudadanoId)
                .OnDelete(DeleteBehavior.Cascade);

            // N:1 with Eleccion
            builder.HasOne(c => c.Eleccion)
                .WithMany(e => e.CodigosVerificacion)
                .HasForeignKey(c => c.EleccionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
