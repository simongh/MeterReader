namespace MeterReader.Types
{
    public class Options
    {
        public string Username { get; set; } = null!;

        public string Password { get; set; } = null!;

        public string Interval { get; set; } = "P1D";

        public string Url { get; set; } = "https://api.glowmarkt.com/api/v0-1/";

        public string ApplicationId { get; set; } = "b0f1b774-a586-4f72-9edd-27ead8aa7a8d";
    }
}