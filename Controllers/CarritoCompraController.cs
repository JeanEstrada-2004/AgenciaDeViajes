using Microsoft.AspNetCore.Mvc;
using AgenciaDeViajes.Models;
using AgenciaDeViajes.Data;
using System.Text.Json;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.EntityFrameworkCore;

namespace AgenciaDeViajes.Controllers
{
    public class CarritoCompraController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CarritoCompraController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Helper para usuario actual
        private Usuario? GetUsuarioActual()
        {
            var nombreUsuario = User.Identity?.Name;
            return _context.Usuarios.FirstOrDefault(u => u.NombreUsuario == nombreUsuario);
        }

        // GET: Vista de datos de pasajeros
        public IActionResult ReservaDatos()
        {
            var json = HttpContext.Session.GetString("CarritoReserva");
            ReservaCarritoViewModel? model = null;
            if (!string.IsNullOrEmpty(json))
                model = JsonSerializer.Deserialize<ReservaCarritoViewModel>(json);

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

        // POST: Recibe datos de pasajeros y actualiza la sesión y CREA la reserva pendiente
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ReservaUsuarioPago(ReservaCarritoViewModel model)
        {
            HttpContext.Session.SetString("CarritoReserva", JsonSerializer.Serialize(model));
            var usuario = GetUsuarioActual();
            if (usuario == null) return RedirectToAction("Index", "Home");

            // Evitar duplicidad: solo crea si no existe pendiente para este usuario/destino/fecha
            var existeReserva = _context.Reservas.Any(r =>
                r.IdUsuario == usuario.IdUsuario &&
                r.IdDestino == model.DestinoId &&
                r.FechaTour == DateTime.Parse(model.FechaElegida) &&
                r.Estado == "Pendiente"
            );

            if (!existeReserva)
            {
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
                    MetodoPago = "MercadoPago",
                    FechaPago = null
                };
                _context.Reservas.Add(reserva);
                _context.SaveChanges();

                // Guardar pasajeros
                if (model.Pasajeros != null && model.Pasajeros.Any())
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

                // Guardar servicios adicionales
                if (model.ServiciosAdicionales != null && model.ServiciosAdicionales.Any())
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
            }

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

        // INTEGRACIÓN MERCADOPAGO CHECKOUT PRO
        [HttpPost]
        public async Task<IActionResult> CrearPreferenciaPRO()
        {
            var json = HttpContext.Session.GetString("CarritoReserva");
            ReservaCarritoViewModel? model = null;
            if (!string.IsNullOrEmpty(json))
                model = JsonSerializer.Deserialize<ReservaCarritoViewModel>(json);

            if (model == null)
                return Json(new { message = "No hay datos de reserva" });

            // CAMBIA ESTA URL cada vez que ngrok te dé una nueva, SOLO httpS.
            string publicBaseUrl = "https://3db0-190-239-89-127.ngrok-free.app"; // <-- ACTUALIZA aquí tu URL pública de ngrok

            string successUrl = $"{publicBaseUrl}/CarritoCompra/ReservaConfirmacion";
            string failureUrl = $"{publicBaseUrl}/CarritoCompra/ReservaUsuarioPago";
            string pendingUrl = $"{publicBaseUrl}/CarritoCompra/ReservaUsuarioPago";

            var preferencia = new
            {
                items = new[] {
                    new {
                        title = model.DestinoNombre,
                        quantity = model.CantidadAdultos + model.CantidadNinos,
                        currency_id = "PEN",
                        unit_price = (decimal)model.PrecioTour
                    }
                },
                back_urls = new {
                    success = successUrl,
                    failure = failureUrl,
                    pending = pendingUrl
                },
                auto_return = "approved"
            };

            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "TEST-5586223908775088-060620-aeed839ab88bce6ce58189a811185b43-2479968511");
            var content = new StringContent(JsonSerializer.Serialize(preferencia), Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync("https://api.mercadopago.com/checkout/preferences", content);

            var respuesta = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                string msg = respuesta;
                try
                {
                    var doc = JsonDocument.Parse(respuesta);
                    if (doc.RootElement.TryGetProperty("message", out var m))
                        msg = m.GetString() ?? msg;
                }
                catch { }
                return Json(new { message = "Error MercadoPago: " + msg });
            }

            using var docOk = JsonDocument.Parse(respuesta);
            var preferenceId = docOk.RootElement.GetProperty("id").GetString();

            return Json(new { preferenceId = preferenceId });
        }

        // Ya NO necesitas ConfirmarReserva POST si usas el flujo anterior

        // GET: Confirmación de reserva (desde MercadoPago o flujo interno)
        [HttpGet]
        public IActionResult ReservaConfirmacion(
            int? id = null,
            string collection_id = null,
            string collection_status = null,
            string payment_id = null,
            string status = null,
            string preference_id = null)
        {
            var usuario = GetUsuarioActual();
            Reserva? reserva = null;

            if (usuario != null)
            {
                // Busca la reserva pendiente más reciente de ese usuario para ese destino y fecha
                reserva = _context.Reservas
                    .Include(r => r.Destino)
                    .Where(r => r.IdUsuario == usuario.IdUsuario && r.Estado == "Pendiente")
                    .OrderByDescending(r => r.FechaReserva)
                    .FirstOrDefault();

                // Si el pago fue exitoso (status == "approved"), confirma la reserva
                if (reserva != null && status == "approved")
                {
                    reserva.Estado = "Confirmada";
                    reserva.FechaPago = DateTime.Now;
                    reserva.ReferenciaPago = payment_id;
                    _context.SaveChanges();
                }
            }

            ViewBag.CollectionId = collection_id;
            ViewBag.Status = status;
            ViewBag.PaymentId = payment_id;
            ViewBag.PreferenceId = preference_id;

            return View(reserva);
        }
    }
}
