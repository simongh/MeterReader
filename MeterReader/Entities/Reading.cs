namespace MeterReader.Entities
{
    public class Reading
    {
        public int Id { get; init; }

        public DateTimeOffset Created { get; init; }

        public decimal Value { get; init; }

        public TariffType Type { get; init; }
    }
}