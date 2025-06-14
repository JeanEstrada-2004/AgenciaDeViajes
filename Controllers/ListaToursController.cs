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

            // Convertimos cada Region en RegionView
            var regionesView = regiones.Select(r => new RegionView
            {
                id_region = r.id_region,
                num_tours = r.num_tours,
                desc_region = r.desc_region,
                ImgReg_url = r.ImgReg_url,
                Destinos = r.Destinos.Select(d => new DestinoView
                {
                    id_destino = d.id_destino,
                    id_region = d.id_region,
                    nom_destino = d.nom_destino,
                    desc_destino = d.desc_destino,
                    precio_tour = d.precio_tour,
                    num_entradas = d.num_entradas,
                    time_tour = d.time_tour,
                    ImgDest_url = d.ImgDest_url
                }).ToList()
            }).ToList();

            return View(regionesView);
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
