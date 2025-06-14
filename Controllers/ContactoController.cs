using Microsoft.AspNetCore.Mvc;
using AgenciaDeViajes.Models;
using System.Threading.Tasks;
using AgenciaDeViajes.Services;
using System;
using AgenciaDeViajes.Data;
using Microsoft.Extensions.Logging;

namespace AgenciaDeViajes.Controllers
{
    public class ContactoController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;
        private readonly ILogger<ContactoController> _logger;

        public ContactoController(
            ApplicationDbContext context,
            IEmailService emailService,
            ILogger<ContactoController> logger)
        {
            _context = context;
            _emailService = emailService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            // Limpia cualquier mensaje previo
            TempData.Remove("MensajeExito");
            TempData.Remove("MensajeError");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(ContactoViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                // Guardar en base de datos
                var contacto = new Contacto
                {
                    Nombre = model.Nombre,
                    Email = model.Email,
                    Comentarios = model.Comentarios,
                    FechaContacto = DateTime.UtcNow
                };

                _context.Contactos.Add(contacto);
                await _context.SaveChangesAsync();

                // Enviar correo de confirmación
                var subject = "Confirmación de contacto - Agencia de Viajes Perú";
                var body = $@"
                    <h2>Hola {model.Nombre},</h2>
                    <p>Hemos recibido tu mensaje correctamente:</p>
                    <blockquote>{model.Comentarios}</blockquote>
                    <p>Nos pondremos en contacto contigo a la brevedad.</p>
                    <p>Saludos cordiales,<br>El equipo de Agencia de Viajes Perú</p>";

                await _emailService.SendEmailAsync(model.Email, subject, body);

                // Mensaje de éxito y redirección para evitar reenvío
                TempData["MensajeExito"] = "¡Gracias por contactarnos! Tu mensaje ha sido enviado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al procesar formulario de contacto");
                TempData["MensajeError"] = "Ocurrió un error al enviar tu mensaje. Por favor, inténtalo nuevamente.";
                return View(model);
            }
        }
    }
}