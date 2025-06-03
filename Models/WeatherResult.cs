namespace AgenciaDeViajes.Models
{
    public class WeatherResult
    {
        public string City { get; set; } = null!;
        public string Description { get; set; } = null!;
        public double Temperature { get; set; }
        public string Icon { get; set; } = null!;
    }
}