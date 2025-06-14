using Microsoft.AspNetCore.Mvc;
using AgenciaDeViajes.Data;
using AgenciaDeViajes.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace AgenciaDeViajes.ViewComponents
{
    public class ReviewListViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public ReviewListViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(int destinoId)
        {
            var reviews = await _context.Reviews
                // Ya no incluye la relación a Usuario, porque el modelo Review no tiene esa propiedad
                .Where(r => r.IdDestino == destinoId && r.Activo)
                .OrderByDescending(r => r.FechaCreacion)
                .ToListAsync();

            return View(reviews);
        }
    }
}
