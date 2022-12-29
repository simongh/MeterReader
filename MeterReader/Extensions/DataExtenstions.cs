using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeterReader
{
    internal static class DataExtenstions
    {
        public static void HasId<T>(this EntityTypeBuilder<T> builder, string name) where T : class
        {
            builder.Property("Id").HasColumnName(name).ValueGeneratedOnAdd();
        }
    }
}