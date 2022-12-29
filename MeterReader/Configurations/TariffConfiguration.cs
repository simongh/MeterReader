using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeterReader.Configurations
{
    public class TariffConfiguration : IEntityTypeConfiguration<Entities.Tariff>
    {
        public void Configure(EntityTypeBuilder<Entities.Tariff> builder)
        {
            builder.ToTable("Tariffs", "dbo");

            builder.HasId("TariffId");
        }
    }
}