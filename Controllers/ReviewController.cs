using Microsoft.AspNetCore.Mvc;
using AgenciaDeViajes.Data;
using AgenciaDeViajes.Models;
using AgenciaDeViajes.ML;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AgenciaDeViajes.Controllers
{
    public class ReviewController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly SentimientoPredictionService _sentimientoService;

        public ReviewController(ApplicationDbContext context, SentimientoPredictionService sentimientoService)
        {
            _context = context;
            _sentimientoService = sentimientoService;
        }

        [HttpPost]
        public async Task<IActionResult> CrearReview(int destinoId, int atencion, int calidad, int puntualidad, string comentario)
        {
            if (!User.Identity.IsAuthenticated)
                return Json(new { success = false, message = "Debes iniciar sesión para enviar un comentario." });

            if (string.IsNullOrWhiteSpace(comentario))
                return Json(new { success = false, message = "El comentario no puede estar vacío." });

            string sentimiento = "Neutro";

            try
            {
                bool esPositivo = _sentimientoService.PredecirSentimiento(comentario);
                sentimiento = esPositivo ? "Positivo" : "Negativo";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al predecir sentimiento: {ex.Message}");
            }

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
