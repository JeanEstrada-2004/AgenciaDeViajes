using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AgenciaDeViajes.Data;
using AgenciaDeViajes.Models;
using AgenciaDeViajes.Models.ViewModels;
using AgenciaDeViajes.Services;

namespace AgenciaDeViajes.Controllers
{
    public class ListaToursController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly WeatherService _weatherService;
        private readonly TourPopularityService _ia;

        public ListaToursController(ApplicationDbContext context, WeatherService weatherService, TourPopularityService ia)
        {
            _context = context;
            _weatherService = weatherService;
            _ia = ia;
        }

        public IActionResult Destination()
        {
            // 1. Cargar regiones con destinos
            var regiones = _context.Regiones
                .Include(r => r.Destinos)
                .ToList();

            // 2. Obtener destinos populares usando IA
            var destinosPopulares = new List<Destino>();
            foreach (var region in regiones)
            {
                foreach (var destino in region.Destinos)
                {
                    var prediccion = _ia.PredecirPopularidad(
                        destino.id_destino,
                        2,
                        (float)destino.precio_tour
                    );

                    if (prediccion.EsPopular)// ¡Así forzamos a que sí muestre!
                    {
                        destinosPopulares.Add(destino);
                    }

                    /* Console.WriteLine($"▶️ {destino.nom_destino} → " +
                    $"Popular: {prediccion.EsPopular} | " +
                    $"Probabilidad: {prediccion.Probability} | " +
                    $"Score: {prediccion.Score}"); */
                }
            }

            // 3. Crear ViewModel combinado
            var viewModel = new RegionDestinoIAViewModel
            {
                Regiones = regiones,
                DestinosPopulares = destinosPopulares.Take(4).ToList()
            };
            
            return View(viewModel);
        }

        // Mostrar detalles de un destino
        [HttpGet]
        public async Task<IActionResult> Details(string nombreSeo)
        {
            if (string.IsNullOrEmpty(nombreSeo))
                return NotFound();

            var destino = await _context.Destinos
                .Include(d => d.Region)
                .FirstOrDefaultAsync(d =>
                    d.nom_destino.ToLower().Replace(" ", "-") == nombreSeo.ToLower()
                );

            if (destino == null)
                return NotFound();

            string? regionName = destino.Region?.desc_region?.Split('-')[0].Trim();

            WeatherResult? clima = null;
            if (!string.IsNullOrEmpty(regionName))
            {
                clima = await _weatherService.GetWeatherAsync(regionName);
            }

            var viewModel = new TourDetailsViewModel
            {
                Destino = destino,
                Clima = clima
            };

            return View(viewModel);
        }
    }
}
