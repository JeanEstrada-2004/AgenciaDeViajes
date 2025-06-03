namespace AgenciaDeViajes.Models
{
    public class TourDetailsViewModel
    {
        public Destino Destino { get; set; } = null!;
        public WeatherResult? Clima { get; set; }
    }
}
