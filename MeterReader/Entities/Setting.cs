namespace MeterReader.Entities
{
    public class Setting
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string Value { get; set; } = null!;
    }
}