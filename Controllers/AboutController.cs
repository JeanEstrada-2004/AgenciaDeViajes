using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // ¡Importante para Include()!
using AgenciaDeViajes.Data;
using AgenciaDeViajes.Models;

namespace AgenciaDeViajes.Controllers
{
    public class AboutController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AboutController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult about()
        {
            // Obtener todas las regiones con sus destinos relacionados
            var regionesConDestinos = _context.Regiones
                .Include(r => r.Destinos) // Carga los destinos relacionados
                .ToList();

            return View(regionesConDestinos);
        }
    }
}