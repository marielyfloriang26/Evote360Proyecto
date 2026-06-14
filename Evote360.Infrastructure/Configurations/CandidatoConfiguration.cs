using Evote360.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Evote360.Infrastructure.Configurations
{
    public class CandidatoConfiguration : IEntityTypeConfiguration<Candidato>
    {
        public void Configure(EntityTypeBuilder<Candidato> builder)
        {
            builder.ToTable("Candidatos");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Nombre)
                .HasMaxLength(150)
                .IsRequired();

            builder.Property(c => c.FotoUrl)
                .HasMaxLength(250)
                .IsRequired(false);

            builder.Property(c => c.Estado)
                .HasDefaultValue(true)
                .IsRequired();

            // N:1 with PartidoPolitico
            builder.HasOne(c => c.Partido)
                .WithMany(p => p.Candidatos)
                .HasForeignKey(c => c.PartidoId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
