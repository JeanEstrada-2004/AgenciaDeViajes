using Microsoft.AspNetCore.Mvc;
using AgenciaDeViajes.Data;
using AgenciaDeViajes.Models;
using Microsoft.EntityFrameworkCore;

namespace AgenciaDeViajes.Controllers
{
    public class PanelUsuarioController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PanelUsuarioController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Helper para obtener el usuario actual y setear ViewBag.NombreCompleto
        private Usuario? GetUsuarioActual()
        {
            var nombreUsuario = User.Identity?.Name;
            var usuario = _context.Usuarios.FirstOrDefault(u => u.NombreUsuario == nombreUsuario);
            // Mostramos el nombre completo si existe, si no, el nombre de usuario
            if (usuario != null && !string.IsNullOrEmpty(usuario.NombreCompleto))
                ViewBag.NombreCompleto = usuario.NombreCompleto;
            else if (usuario != null)
                ViewBag.NombreCompleto = usuario.NombreUsuario;
            else
                ViewBag.NombreCompleto = "Usuario";
            return usuario;
        }

        // Dashboard principal
        public IActionResult Dashboard()
        {
            ViewBag.Active = "Dashboard";
            var usuario = GetUsuarioActual();
            if (usuario == null)
                return RedirectToAction("Index", "Home");

            // Obtener la próxima reserva (la más próxima en fecha, pendiente o confirmada)
            var proximaReserva = _context.Reservas
                .Include(r => r.Destino)
                .Where(r => r.IdUsuario == usuario.IdUsuario && r.FechaTour >= DateTime.Now)
                .OrderBy(r => r.FechaTour)
                .FirstOrDefault();

            // Contar pagos pendientes (si tienes esa lógica)
            int pagosPendientes = _context.Reservas.Count(r => r.IdUsuario == usuario.IdUsuario && r.Estado == "Pendiente");

            // Notificaciones simuladas: ejemplo
            int notificaciones = 0;
            string ultimaNotificacion = "";

            if (proximaReserva != null)
            {
                notificaciones = 1;
                ultimaNotificacion = "¡Tu reserva fue confirmada!";
            }

            ViewBag.ProximaReserva = proximaReserva;
            ViewBag.PagosPendientes = pagosPendientes;
            ViewBag.Notificaciones = notificaciones;
            ViewBag.UltimaNotificacion = ultimaNotificacion;

            return View();
        }

        // Historial y próximas reservas
        public IActionResult MisReservas()
        {
            ViewBag.Active = "MisReservas";
            var usuario = GetUsuarioActual();
            if (usuario == null)
                return RedirectToAction("Index", "Home");

            var reservas = _context.Reservas
                .Include(r => r.Destino)
                .Where(r => r.IdUsuario == usuario.IdUsuario)
                .OrderByDescending(r => r.FechaTour)
                .ToList();

            return View(reservas);
        }

        // Datos personales
        public IActionResult Perfil()
        {
            ViewBag.Active = "Perfil";
            GetUsuarioActual();
            return View();
        }

        // Soporte o contacto
        public IActionResult Soporte()
        {
            ViewBag.Active = "Soporte";
            GetUsuarioActual();
            return View();
        }

        // Notificaciones
        public IActionResult Notificaciones()
        {
            ViewBag.Active = "Notificaciones";
            GetUsuarioActual();
            return View();
        }
    }
}
