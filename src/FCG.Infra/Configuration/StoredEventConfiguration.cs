using FCG.Core.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FCG.Infra.Configuration
{
    public class StoredEventConfiguration : IEntityTypeConfiguration<StoredEvent>
    {
        public void Configure(EntityTypeBuilder<StoredEvent> builder)
        {
            builder.ToTable("StoredEvents");
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Payload)
                .HasColumnType("nvarchar(max)")
                .IsRequired();
        }
    }
}
