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

        public IActionResult Destination(string? nombre, string? precio, string? duracion, string? region, int page = 1, int pageSize = 6)
        {
            var regiones = _context.Regiones
                .Include(r => r.Destinos)
                .ToList();

            // Filtro avanzado
            if (!string.IsNullOrEmpty(nombre))
            {
                regiones.ForEach(r => r.Destinos = r.Destinos
                    .Where(d => d.nom_destino.Contains(nombre, StringComparison.OrdinalIgnoreCase)).ToList());
            }

            if (!string.IsNullOrEmpty(precio) && decimal.TryParse(precio, out decimal pFiltrado))
            {
                regiones.ForEach(r => r.Destinos = r.Destinos
                    .Where(d => d.precio_tour == pFiltrado).ToList());
            }

            if (!string.IsNullOrEmpty(duracion))
            {
                regiones.ForEach(r => r.Destinos = r.Destinos
                    .Where(d => d.time_tour.Equals(duracion, StringComparison.OrdinalIgnoreCase)).ToList());
            }

            if (!string.IsNullOrEmpty(region))
            {
                regiones = regiones
                    .Where(r => r.desc_region.Split('-')[0].Trim()
                    .Contains(region, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            // Predicción IA
            var destinosPopulares = new List<Destino>();
            foreach (var regionItem in regiones)
            {
                foreach (var destino in regionItem.Destinos)
                {
                    var prediccion = _ia.PredecirPopularidad(destino.id_destino, 2, (float)destino.precio_tour);
                    if (prediccion.EsPopular)
                    {
                        destinosPopulares.Add(destino);
                    }
                }
            }

            // Paginación
            var destinosTodos = regiones.SelectMany(r => r.Destinos).ToList();
            int totalDestinos = destinosTodos.Count;
            var destinosPaginados = destinosTodos
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Distribuir los destinos paginados a sus regiones (por consistencia)
            foreach (var regionItem in regiones)
            {
                regionItem.Destinos = regionItem.Destinos
                    .Where(d => destinosPaginados.Any(dp => dp.id_destino == d.id_destino))
                    .ToList();
            }

            var viewModel = new RegionDestinoIAViewModel
            {
                Regiones = regiones,
                DestinosPopulares = destinosPopulares.Take(4).ToList(),
                PaginaActual = page,
                TotalPaginas = (int)Math.Ceiling((double)totalDestinos / pageSize)
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
