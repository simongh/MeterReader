namespace MeterReader.Entities
{
    public class Supplier
    {
        public int Id { get; init; }

        public string Name { get; set; } = null!;

        public virtual ICollection<Tariff> Tariffs { get; set; } = new HashSet<Tariff>();
    }
}