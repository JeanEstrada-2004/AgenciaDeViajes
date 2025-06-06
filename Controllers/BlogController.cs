using Microsoft.AspNetCore.Mvc;
using AgenciaDeViajes.Data;
using AgenciaDeViajes.Models;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Linq;

namespace AgenciaDeViajes.Controllers
{
    public class BlogController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BlogController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Mostrar todas las entradas
        public IActionResult Index()
        {
            var publicaciones = _context.EntradasBlog
                .OrderByDescending(p => p.FechaPublicacion)
                .ToList();
            return View(publicaciones);
        }

        // Mostrar el detalle de una entrada
        public IActionResult Details(int? id)
        {
            if (id == null)
                return NotFound();

            var entrada = _context.EntradasBlog.FirstOrDefault(e => e.Id == id);

            if (entrada == null)
                return NotFound();

            return View(entrada);
        }

        // Mostrar formulario para crear o editar entrada
        [Authorize(Roles = "Admin")]
        public IActionResult Create(int? id)
        {
            if (id == null)
            {
                // Nueva entrada vacía
                return View(new EntradaBlog());
            }
            else
            {
                // Editar entrada existente
                var entrada = _context.EntradasBlog.FirstOrDefault(e => e.Id == id);
                if (entrada == null)
                    return NotFound();

                return View(entrada);
            }
        }

        // Guardar nueva entrada o actualizar existente
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public IActionResult Create(EntradaBlog entrada)
        {
            if (ModelState.IsValid)
            {
                if (entrada.Id == 0)
                {
                    // Nueva entrada: asignar fecha UTC actual
                    entrada.FechaPublicacion = DateTime.UtcNow;
                    _context.EntradasBlog.Add(entrada);
                }
                else
                {
                    // Editar entrada existente
                    var entradaExistente = _context.EntradasBlog.FirstOrDefault(e => e.Id == entrada.Id);
                    if (entradaExistente == null)
                        return NotFound();

                    entradaExistente.Ciudad = entrada.Ciudad;
                    entradaExistente.Titulo = entrada.Titulo;
                    entradaExistente.Descripcion = entrada.Descripcion;
                    entradaExistente.FotoUrl = entrada.FotoUrl;
                    entradaExistente.Contenido = entrada.Contenido;

                    // Asegurarse que la fecha tenga Kind UTC para evitar error
                    entradaExistente.FechaPublicacion = DateTime.SpecifyKind(entrada.FechaPublicacion, DateTimeKind.Utc);

                    _context.EntradasBlog.Update(entradaExistente);
                }

                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(entrada);
        }

        // Mostrar confirmación para eliminar (opcional)
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var entrada = _context.EntradasBlog.FirstOrDefault(e => e.Id == id);
            if (entrada == null)
                return NotFound();

            return View(entrada);
        }

        // POST: Eliminar la entrada
        [HttpPost, ActionName("DeleteConfirmed")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var entrada = _context.EntradasBlog.Find(id);
            if (entrada == null)
                return NotFound();

            _context.EntradasBlog.Remove(entrada);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }


        
    }
}
