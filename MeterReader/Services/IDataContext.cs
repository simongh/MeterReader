using Microsoft.EntityFrameworkCore;

namespace MeterReader.Services
{
    internal interface IDataContext : IDisposable
    {
        DbSet<Entities.Reading> Readings { get; set; }
        DbSet<Entities.Setting> Settings { get; set; }
        DbSet<Entities.Supplier> Suppliers { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}