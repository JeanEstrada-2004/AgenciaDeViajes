using Microsoft.AspNetCore.Mvc;
using AgenciaDeViajes.Data;
using AgenciaDeViajes.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace AgenciaDeViajes.Controllers
{
    public class ReviewController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReviewController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST: Review/Crear
        [HttpPost]
        public async Task<IActionResult> CrearReview(int destinoId, int atencion, int calidad, int puntualidad, string comentario)
        {
            if (!User.Identity.IsAuthenticated)
                return Json(new { success = false, message = "Debes iniciar sesión para enviar un comentario." });

            // Ya no buscamos usuario, no usamos IdUsuario

            // Por ahora dejamos "Neutro" para que puedas integrar IA después.
            string sentimiento = "Neutro";

            var review = new Review
            {
                IdDestino = destinoId,
                CalificacionAtencion = atencion,
                CalificacionCalidad = calidad,
                CalificacionPuntualidad = puntualidad,
                Comentario = comentario,
                Sentimiento = sentimiento,
                Activo = true,
                FechaCreacion = DateTime.Now
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Comentario guardado correctamente." });
        }

        // GET: Review/ListarPorDestino/5
        public async Task<IActionResult> ListarPorDestino(int destinoId)
        {
            var reviews = await _context.Reviews
                .Where(r => r.IdDestino == destinoId && r.Activo)
                .OrderByDescending(r => r.FechaCreacion)
                .ToListAsync();

            return PartialView("_ReviewsListPartial", reviews);
        }
    }
}
