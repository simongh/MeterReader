using MeterReader.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeterReader.Configurations
{
    public class ReadingConfiguration : IEntityTypeConfiguration<Entities.Reading>
    {
        public void Configure(EntityTypeBuilder<Reading> builder)
        {
            builder.ToTable("Readings", "dbo");

            builder.HasId("ReadingId");
        }
    }
}