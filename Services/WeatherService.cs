using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using AgenciaDeViajes.Models; // Asegúrate que este using está aquí

namespace AgenciaDeViajes.Services
{
    public class WeatherService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public WeatherService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["OpenWeatherMap:ApiKey"];
        }

        // Cambia WeatherInfo por WeatherResult
        public async Task<WeatherResult?> GetWeatherAsync(string city)
        {
            // Puedes agregar ",PE" para asegurar que busca en Perú, por ejemplo: "Cusco,PE"
            string url = $"https://api.openweathermap.org/data/2.5/weather?q={city},PE&appid={_apiKey}&units=metric&lang=es";

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return null;

            using var stream = await response.Content.ReadAsStreamAsync();
            using var doc = await JsonDocument.ParseAsync(stream);

            var weather = doc.RootElement.GetProperty("weather")[0];
            var main = doc.RootElement.GetProperty("main");

            return new WeatherResult
            {
                Description = weather.GetProperty("description").GetString() ?? "",
                Temperature = main.GetProperty("temp").GetDouble(),
                Icon = weather.GetProperty("icon").GetString() ?? "",
                City = doc.RootElement.GetProperty("name").GetString() ?? ""
            };
        }
    }
}
