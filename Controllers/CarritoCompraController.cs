using Microsoft.AspNetCore.Mvc;
using AgenciaDeViajes.Models;
using AgenciaDeViajes.Data;
using System.Text.Json;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;

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

        // POST: Recibe datos de pasajeros y actualiza la sesión
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ReservaUsuarioPago(ReservaCarritoViewModel model)
        {
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

        // === INTEGRACIÓN MERCADOPAGO CHECKOUT PRO ===
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
            string publicBaseUrl = "https://3db0-190-239-89-127.ngrok-free.app"; // <-- ACTUALIZA aquí tu URL pública de ngrok (sin espacios al final)

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
        // === FINAL INTEGRACIÓN MERCADO PAGO ===

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ConfirmarReserva()
        {
            if (!User.Identity.IsAuthenticated)
            {
                TempData["Error"] = "Debe iniciar sesión para reservar.";
                return RedirectToAction("ReservaUsuarioPago");
            }

            var json = HttpContext.Session.GetString("CarritoReserva");
            ReservaCarritoViewModel? model = null;
            if (!string.IsNullOrEmpty(json))
                model = JsonSerializer.Deserialize<ReservaCarritoViewModel>(json);
            if (model == null)
                return RedirectToAction("Index", "Home");

            var nombreUsuario = User.Identity.Name;
            var usuario = _context.Usuarios.FirstOrDefault(u => u.NombreUsuario == nombreUsuario);
            if (usuario == null)
            {
                TempData["Error"] = "Debe iniciar sesión para reservar.";
                return RedirectToAction("ReservaUsuarioPago");
            }

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
                FechaPago = DateTime.Now
            };

            _context.Reservas.Add(reserva);
            _context.SaveChanges();

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

            HttpContext.Session.Remove("CarritoReserva");

            // Aquí pasamos el id para cuando el usuario viene del flujo interno (no MercadoPago)
            return RedirectToAction("ReservaConfirmacion", new { id = reserva.IdReserva });
        }

        // Ahora soporta ambos flujos: con parámetros de MercadoPago o con id local
        [HttpGet]
        public IActionResult ReservaConfirmacion(
            int? id = null,
            string collection_id = null,
            string collection_status = null,
            string payment_id = null,
            string status = null,
            string preference_id = null)
        {
            Reserva? reserva = null;

            // Si venimos del flujo local, usamos el id
            if (id.HasValue)
                reserva = _context.Reservas.FirstOrDefault(r => r.IdReserva == id.Value);

            // Si venimos desde MercadoPago y no hay id, puedes mostrar una confirmación genérica
            ViewBag.CollectionId = collection_id;
            ViewBag.Status = status;
            ViewBag.PaymentId = payment_id;
            ViewBag.PreferenceId = preference_id;

            return View(reserva); // La view puede trabajar con reserva nula o no
        }
    }
}
