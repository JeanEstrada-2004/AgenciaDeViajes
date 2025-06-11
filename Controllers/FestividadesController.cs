using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AgenciaDeViajes.Data;

namespace AgenciaDeViajes.Controllers
{
    public class FestividadesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FestividadesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var festividades = await _context.Festividades.ToListAsync();
            return View(festividades);
        }
    }
}
