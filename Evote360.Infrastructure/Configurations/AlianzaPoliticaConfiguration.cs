using Evote360.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Evote360.Infrastructure.Configurations
{
    public class AlianzaPoliticaConfiguration : IEntityTypeConfiguration<AlianzaPolitica>
    {
        public void Configure(EntityTypeBuilder<AlianzaPolitica> builder)
        {
            builder.ToTable("AlianzasPoliticas");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Estado)
                .HasConversion<string>()
                .IsRequired();

            // N:1 with Eleccion
            builder.HasOne(a => a.Eleccion)
                .WithMany(e => e.Alianzas)
                .HasForeignKey(a => a.EleccionId)
                .OnDelete(DeleteBehavior.Cascade);

            // N:1 with PartidoMayorista
            builder.HasOne(a => a.PartidoMayorista)
                .WithMany(p => p.AlianzasComoMayorista)
                .HasForeignKey(a => a.PartidoMayoristaId)
                .OnDelete(DeleteBehavior.Restrict);

            // N:1 with PartidoAliado
            builder.HasOne(a => a.PartidoAliado)
                .WithMany(p => p.AlianzasComoAliado)
                .HasForeignKey(a => a.PartidoAliadoId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
