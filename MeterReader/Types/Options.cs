namespace MeterReader.Types
{
    public class Options
    {
        public string Username { get; set; } = null!;

        public string Password { get; set; } = null!;

        public DateTimeOffset StartDate { get; set; } = DateTimeOffset.UtcNow.Date;
    }
}