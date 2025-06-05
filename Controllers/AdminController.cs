using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using AgenciaDeViajes.Data;
using AgenciaDeViajes.Models;
using AgenciaDeViajes.ViewModels;


namespace AgenciaDeViajes.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ============================
        // Página principal de administración
        // ============================
        public IActionResult Index()
        {
            return View();
        }

        // ============================
        // Panel de administración de Regiones y Destinos
        // ============================
        public IActionResult AdminDestinos()
        {
            var regiones = _context.Regiones
                .Include(r => r.Destinos)
                .ToList();

            return View(regiones);
        }

        // ========================================================
        // ================== CRUD REGIONES ========================
        // ========================================================

        // GET: Crear Región
        [HttpGet]
        public IActionResult CreateRegion()
        {
            return View();
        }

        // POST: Crear Región
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateRegion(Region region)
        {
            if (ModelState.IsValid)
            {
                _context.Regiones.Add(region);
                _context.SaveChanges();
                return RedirectToAction(nameof(AdminDestinos));
            }
            return View(region);
        }

        // GET: Editar Región
        [HttpGet]
        public IActionResult EditRegion(int id)
        {
            var region = _context.Regiones.Find(id);
            if (region == null) return NotFound();
            return View(region);
        }

        // POST: Editar Región
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditRegion(Region region)
        {
            if (ModelState.IsValid)
            {
                _context.Update(region);
                _context.SaveChanges();
                return RedirectToAction(nameof(AdminDestinos));
            }
            return View(region);
        }

        // POST: Eliminar Región
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteRegion(int id)
        {
            var region = _context.Regiones.Find(id);
            if (region != null)
            {
                _context.Regiones.Remove(region);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(AdminDestinos));
        }

        // ========================================================
        // ================== CRUD DESTINOS ========================
        // ========================================================

        // GET: Crear Destino
        [HttpGet]
        public IActionResult CreateDestino()
        {
            ViewBag.Regiones = _context.Regiones.ToList(); // para el dropdown
            return View();
        }

        // POST: Crear Destino
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateDestino(Destino destino)
        {
            if (ModelState.IsValid)
            {

                _context.Destinos.Add(destino);
                _context.SaveChanges();
                return RedirectToAction(nameof(AdminDestinos));
            }

            // 🚨 Diagnóstico: imprimir errores en consola
            foreach (var key in ModelState.Keys)
            {
                var errors = ModelState[key].Errors;
                foreach (var error in errors)
                {
                    Console.WriteLine($"Error en {key}: {error.ErrorMessage}");
                }
            }

            ViewBag.Regiones = _context.Regiones.ToList();
            return View(destino);
        }


        // GET: Editar Destino
        [HttpGet]
        public IActionResult EditDestino(int id)
        {
            var destino = _context.Destinos.Find(id);
            if (destino == null) return NotFound();

            ViewBag.Regiones = _context.Regiones.ToList();
            return View(destino);
        }

        // POST: Editar Destino
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditDestino(Destino destino)
        {
            if (ModelState.IsValid)
            {
                _context.Update(destino);
                _context.SaveChanges();
                return RedirectToAction(nameof(AdminDestinos));
            }

            ViewBag.Regiones = _context.Regiones.ToList();
            return View(destino);
        }

        // POST: Eliminar Destino
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteDestino(int id)
        {
            var destino = _context.Destinos.Find(id);
            if (destino != null)
            {
                _context.Destinos.Remove(destino);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(AdminDestinos));
        }

        // ============================
        // Visualizar Panel del Admin
        // ============================
        public IActionResult Panel()
        {
            ViewBag.TotalUsuarios = _context.Usuarios.Count();
            ViewBag.Administradores = _context.Usuarios.Count(u => u.Rol == "Admin");
            ViewBag.Clientes = _context.Usuarios.Count(u => u.Rol == "Cliente");
            ViewBag.TotalRegiones = _context.Regiones.Count();
            ViewBag.TotalDestinos = _context.Destinos.Count();
            return View();
        }

        // ============================
        // Ver usuarios registrados
        // ============================
        public IActionResult VerUsuarios()
        {
            var usuarios = _context.Usuarios.ToList();
            return View(usuarios);
        }

        // ============================
        // GET: Editar Rol de Usuario
        // ============================
        [HttpGet]
        public IActionResult EditarRol(int id)
        {
            var usuario = _context.Usuarios.Find(id);
            if (usuario == null)
                return NotFound();

            return View(usuario);
        }

        // ============================
        // POST: Editar Rol de Usuario
        // ============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditarRol(int id, string nuevoRol)
        {
            var usuario = _context.Usuarios.Find(id);
            if (usuario == null)
                return NotFound();

            usuario.Rol = nuevoRol;
            _context.SaveChanges();

            return RedirectToAction(nameof(VerUsuarios));
        }


        // ============================
        // Estadísticas del Admin
        // ============================
        public IActionResult Estadisticas()
        {
            var usuarios = _context.Usuarios.ToList();
            var agrupadoPorMes = usuarios
                .GroupBy(u => new { u.FechaRegistro.Year, u.FechaRegistro.Month })
                .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
                .Select(g => new {
                    Mes = $"{g.Key.Month:D2}/{g.Key.Year}",
                    Total = g.Count()
                }).ToList();

            var viewModel = new UsuariosPorMesViewModel
            {
                Meses = agrupadoPorMes.Select(x => x.Mes).ToList(),
                Totales = agrupadoPorMes.Select(x => x.Total).ToList()
            };

            return View(viewModel);
        }


    }

}