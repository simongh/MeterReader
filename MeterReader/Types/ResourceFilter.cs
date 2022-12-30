namespace MeterReader.Types
{
    public record ResourceFilter
    {
        public string Id { get; init; }

        public DateTimeOffset From { get; init; }

        public DateTimeOffset To { get; init; }

        public string Interval { get; init; }

        public override string ToString()
        {
            return $"from={From:yyyy-MM-ddTHH:mm:ss}&to={To:yyyy-MM-ddTHH:mm:ss}&function=sum&period={Interval}&offset=0&nulls=1";
        }
    }
}