using Microsoft.AspNetCore.Mvc;
using AgenciaDeViajes.Models;
using AgenciaDeViajes.Data;
using System.Text.Json;
using System.Security.Claims;

namespace AgenciaDeViajes.Controllers
{
    public class CarritoCompraController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CarritoCompraController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Vista de datos de pasajeros
        public IActionResult ReservaDatos()
        {
            var json = HttpContext.Session.GetString("CarritoReserva");
            ReservaCarritoViewModel? model = null;
            if (!string.IsNullOrEmpty(json))
            {
                model = JsonSerializer.Deserialize<ReservaCarritoViewModel>(json);
            }
            if (model == null)
                return RedirectToAction("Index", "Home");

            return View(model);
        }

        // POST: Recibe los datos del formulario del destino y guarda en sesión
        [HttpPost]
        public IActionResult IniciarReserva(ReservaCarritoViewModel model)
        {
            HttpContext.Session.SetString("CarritoReserva", JsonSerializer.Serialize(model));
            return RedirectToAction("ReservaDatos");
        }

        // POST: Recibe datos de pasajeros y actualiza la sesión
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ReservaUsuarioPago(ReservaCarritoViewModel model)
        {
            // Actualiza la reserva en la sesión con los pasajeros agregados
            HttpContext.Session.SetString("CarritoReserva", JsonSerializer.Serialize(model));
            return RedirectToAction("ReservaUsuarioPago");
        }

        // GET: Vista de login/pago - SIEMPRE PASA EL MODELO
        public IActionResult ReservaUsuarioPago()
        {
            var json = HttpContext.Session.GetString("CarritoReserva");
            ReservaCarritoViewModel? model = null;
            if (!string.IsNullOrEmpty(json))
                model = JsonSerializer.Deserialize<ReservaCarritoViewModel>(json);
            if (model == null)
                return RedirectToAction("Index", "Home");

            return View(model);
        }

        // POST: Confirmar y guardar la reserva en la base de datos
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ConfirmarReserva()
        {
            // 1. Asegúrate que el usuario esté autenticado
            if (!User.Identity.IsAuthenticated)
            {
                TempData["Error"] = "Debe iniciar sesión para reservar.";
                return RedirectToAction("ReservaUsuarioPago");
            }

            // 2. Recuperar el modelo de la sesión
            var json = HttpContext.Session.GetString("CarritoReserva");
            ReservaCarritoViewModel? model = null;
            if (!string.IsNullOrEmpty(json))
                model = JsonSerializer.Deserialize<ReservaCarritoViewModel>(json);
            if (model == null)
                return RedirectToAction("Index", "Home");

            // 3. Obtener el usuario actual
            var nombreUsuario = User.Identity.Name;
            var usuario = _context.Usuarios.FirstOrDefault(u => u.NombreUsuario == nombreUsuario);
            if (usuario == null)
            {
                TempData["Error"] = "Debe iniciar sesión para reservar.";
                return RedirectToAction("ReservaUsuarioPago");
            }

            // 4. Guardar la reserva principal
            var reserva = new Reserva
            {
                IdUsuario = usuario.IdUsuario,
                IdDestino = model.DestinoId,
                FechaReserva = DateTime.Now,
                FechaTour = DateTime.Parse(model.FechaElegida),
                CantidadAdultos = model.CantidadAdultos,
                CantidadNinos = model.CantidadNinos,
                PrecioTotal = (decimal)(model.PrecioTour * (model.CantidadAdultos + model.CantidadNinos)),
                Estado = "Pendiente",
                MetodoPago = "MercadoPago", // ejemplo
                FechaPago = DateTime.Now
            };

            _context.Reservas.Add(reserva);
            _context.SaveChanges();

            // 5. Guardar pasajeros
            if (model.Pasajeros != null)
            {
                foreach (var pasajero in model.Pasajeros)
                {
                    var pasajeroDb = new PasajeroReserva
                    {
                        IdReserva = reserva.IdReserva,
                        Tipo = pasajero.Tipo,
                        NombreCompleto = pasajero.NombreCompleto,
                        Documento = pasajero.Documento,
                        Telefono = pasajero.Telefono,
                        Pais = pasajero.Pais
                    };
                    _context.PasajerosReserva.Add(pasajeroDb);
                }
            }

            // 6. Guardar servicios adicionales
            if (model.ServiciosAdicionales != null)
            {
                foreach (var serv in model.ServiciosAdicionales)
                {
                    var servDb = new ReservaServicioAdicional
                    {
                        IdReserva = reserva.IdReserva,
                        Servicio = serv
                    };
                    _context.ReservaServiciosAdicionales.Add(servDb);
                }
            }

            _context.SaveChanges();

            // Limpia la sesión del carrito
            HttpContext.Session.Remove("CarritoReserva");

            // Redirigir a la página de confirmación y pasar el ID de reserva
            return RedirectToAction("ReservaConfirmacion", new { id = reserva.IdReserva });
        }

        // GET: Confirmación de reserva
        public IActionResult ReservaConfirmacion(int id)
        {
            var reserva = _context.Reservas
                .Where(r => r.IdReserva == id)
                .FirstOrDefault();

            if (reserva == null)
                return NotFound();

            return View(reserva);
        }
    }
}
