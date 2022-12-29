using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeterReader.Configurations
{
    public class SupplierConfiguration : IEntityTypeConfiguration<Entities.Supplier>
    {
        public void Configure(EntityTypeBuilder<Entities.Supplier> builder)
        {
            builder.ToTable("Suppliers", "dbo");

            builder.HasId("SupplierId");

            builder
                .Property(p => p.Name)
                .HasMaxLength(200)
                .IsRequired();
        }
    }
}