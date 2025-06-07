using Microsoft.AspNetCore.Mvc;
using AgenciaDeViajes.Models;
using AgenciaDeViajes.Data;
using System.Text.Json;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
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

        // === INTEGRACIÓN MERCADOPAGO REST API ===
        [HttpPost]
        public async Task<IActionResult> CrearPreferencia()
        {
            var json = HttpContext.Session.GetString("CarritoReserva");
            ReservaCarritoViewModel? model = null;
            if (!string.IsNullOrEmpty(json))
                model = JsonSerializer.Deserialize<ReservaCarritoViewModel>(json);

            if (model == null)
                return BadRequest("No hay datos de reserva.");

            // Construir el objeto preferencia
            var preference = new MercadoPagoPreferenceRequest
            {
                items = new List<MercadoPagoPreferenceRequest.Item>
                {
                    new MercadoPagoPreferenceRequest.Item
                    {
                        title = model.DestinoNombre,
                        quantity = model.CantidadAdultos + model.CantidadNinos,
                        currency_id = "PEN",
                        unit_price = (decimal)model.PrecioTour
                    }
                },
                back_urls = new MercadoPagoPreferenceRequest.BackUrls
                {
                    success = Url.Action("ReservaConfirmacion", "CarritoCompra", null, Request.Scheme),
                    failure = Url.Action("ReservaUsuarioPago", "CarritoCompra", null, Request.Scheme),
                    pending = Url.Action("ReservaUsuarioPago", "CarritoCompra", null, Request.Scheme)
                },
                auto_return = "approved"
            };

            // Llama a la API REST de MercadoPago
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "TEST-5586223908775088-060620-aeed839ab88bce6ce58189a811185b43-2479968511");
            var content = new StringContent(JsonSerializer.Serialize(preference), Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync("https://api.mercadopago.com/checkout/preferences", content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                return BadRequest("Error al crear preferencia MercadoPago: " + error);
            }

            var respuesta = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(respuesta);
            var initPoint = doc.RootElement.GetProperty("init_point").GetString();

            // Devuelve el URL de pago (init_point) al frontend
            return Json(new { init_point = initPoint });
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

            return RedirectToAction("ReservaConfirmacion", new { id = reserva.IdReserva });
        }

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
