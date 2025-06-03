using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AgenciaDeViajes.Data;
using AgenciaDeViajes.Models;
using AgenciaDeViajes.Services; // <--- AGREGA ESTO

namespace AgenciaDeViajes.Controllers
{
    public class ListaToursController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly WeatherService _weatherService;

        public ListaToursController(ApplicationDbContext context, WeatherService weatherService)
        {
            _context = context;
            _weatherService = weatherService;
        }

        // Método para listar todas las regiones con sus destinos
        public IActionResult Destination()
        {
            var regionesConDestinos = _context.Regiones
                .Include(r => r.Destinos)
                .ToList();

            return View(regionesConDestinos);
        }

        // Mostrar detalles de un destino con URL SEO + clima
        [HttpGet]
        public async Task<IActionResult> Details(string nombreSeo)
        {
            if (string.IsNullOrEmpty(nombreSeo))
                return NotFound();

            // Busca el destino por el "slug" (nombreSeo)
            var destino = await _context.Destinos
                .Include(d => d.Region) // Para tener acceso al nombre de la región
                .FirstOrDefaultAsync(d =>
                    d.nom_destino.ToLower().Replace(" ", "-") == nombreSeo.ToLower()
                );

            if (destino == null)
                return NotFound();

            // Busca el nombre de la región asociada (puede ser null si no se incluye arriba)
            string? regionName = destino.Region?.desc_region?.Split('-')[0].Trim();

            WeatherResult? clima = null;
            if (!string.IsNullOrEmpty(regionName))
            {
                // Consulta clima usando la región como ciudad
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
