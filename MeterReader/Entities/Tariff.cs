namespace MeterReader.Entities
{
    public class Tariff
    {
        public int Id { get; init; }

        public int SupplierId { get; init; }

        public DateOnly StartDate { get; set; }

        public TariffType Type { get; set; }

        public decimal UnitCost { get; set; }

        public virtual Supplier Supplier { get; init; }
    }
}