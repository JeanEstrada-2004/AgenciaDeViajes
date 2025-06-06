using Microsoft.AspNetCore.Mvc;
using AgenciaDeViajes.Models;
using System.Text.Json; // <-- Para serializar el modelo y usar Session

namespace AgenciaDeViajes.Controllers
{
    public class CarritoCompraController : Controller
    {
        // GET: Vista de datos de pasajeros
        public IActionResult ReservaDatos()
        {
            // Recuperar la reserva desde la sesión
            var json = HttpContext.Session.GetString("CarritoReserva");
            ReservaCarritoViewModel? model = null;
            if (!string.IsNullOrEmpty(json))
            {
                model = JsonSerializer.Deserialize<ReservaCarritoViewModel>(json);
            }
            if (model == null)
                return RedirectToAction("Index", "Home"); // O página que prefieras

            return View(model);
        }

        // POST: Recibe los datos del formulario del destino y guarda en sesión
        [HttpPost]
        public IActionResult IniciarReserva(ReservaCarritoViewModel model)
        {
            // Guardar en sesión la reserva
            HttpContext.Session.SetString("CarritoReserva", JsonSerializer.Serialize(model));
            // Redirigir a la siguiente vista
            return RedirectToAction("ReservaDatos");
        }

        public IActionResult ReservaUsuarioPago()
        {
            return View();
        }

        public IActionResult ReservaConfirmacion()
        {
            return View();
        }
    }
}
