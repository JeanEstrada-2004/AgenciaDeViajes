using Microsoft.AspNetCore.Mvc;

namespace AgenciaDeViajes.Controllers
{
    public class PanelUsuarioController : Controller
    {
        // Dashboard principal
        public IActionResult Dashboard()
        {
            ViewBag.Active = "Dashboard";
            return View();
        }

        // Historial y próximas reservas
        public IActionResult MisReservas()
        {
            ViewBag.Active = "MisReservas";
            return View();
        }

        // Datos personales
        public IActionResult Perfil()
        {
            ViewBag.Active = "Perfil";
            return View();
        }

        // Soporte o contacto
        public IActionResult Soporte()
        {
            ViewBag.Active = "Soporte";
            return View();
        }

        // Notificaciones
        public IActionResult Notificaciones()
        {
            ViewBag.Active = "Notificaciones";
            return View();
        }
    }
}
