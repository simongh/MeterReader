namespace MeterReader.Types
{
    public record ResourceFilter
    {
        public string Id { get; set; }

        public DateTimeOffset From { get; set; }

        public DateTimeOffset To { get; set; }

        public override string ToString()
        {
            return $"from={From:yyyy-MM-ddTHH:mm:ss}&to={To:yyyy-MM-ddTHH:mm:ss}&function=sum&period=PT1H&offset=0&nulls=1";
        }
    }
}