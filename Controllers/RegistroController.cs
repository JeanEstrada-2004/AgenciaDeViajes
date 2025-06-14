using Microsoft.AspNetCore.Mvc;
using AgenciaDeViajes.Data;
using AgenciaDeViajes.Models;
using AgenciaDeViajes.Services;
using System.Security.Cryptography;
using System.Text;

namespace AgenciaDeViajes.Controllers
{
    public class RegistroController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailSender;

        public RegistroController(ApplicationDbContext context, IEmailSender emailSender)
        {
            _context = context;
            _emailSender = emailSender;
        }

        [HttpGet]
        public IActionResult Registrar()
        {
            return View(new Usuario());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registrar(Usuario usuario, string confirmarContrasena)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Error = "Por favor, completa todos los campos correctamente.";
                return View(usuario);
            }

            if (usuario.Contrasena != confirmarContrasena)
            {
                ViewBag.Error = "Las contraseñas no coinciden.";
                return View(usuario);
            }

            if (_context.Usuarios.Any(u => u.NombreUsuario == usuario.NombreUsuario))
            {
                ViewBag.Error = "Este correo ya está registrado.";
                return View(usuario);
            }

            usuario.Rol = "Cliente";
            usuario.MetodoRegistro = "Manual";
            // Asegurando que todas las fechas sean UTC
            usuario.FechaRegistro = DateTime.UtcNow;
            if (usuario.FechaNacimiento.HasValue)
            {
                usuario.FechaNacimiento = DateTime.SpecifyKind(usuario.FechaNacimiento.Value, DateTimeKind.Utc);
            }
            usuario.CorreoConfirmado = false;

            _context.Usuarios.Add(usuario);
            _context.SaveChanges();

            // Generar token simple con hash
            string token = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(usuario.NombreUsuario + usuario.IdUsuario)));

            var callbackUrl = Url.Action("ConfirmarCorreo", "Registro", new
            {
                userId = usuario.IdUsuario,
                token = token
            }, protocol: HttpContext.Request.Scheme);

            string mensaje = $"<h3>Hola {usuario.NombreCompleto}!</h3>" +
                "<p>Gracias por registrarte en nuestra agencia. Por favor confirma tu correo haciendo clic en el siguiente enlace:</p>" +
                $"<p><a href='{callbackUrl}'>Confirmar mi cuenta</a></p>";

            await _emailSender.SendEmailAsync(usuario.NombreUsuario, "Confirma tu cuenta", mensaje);

            TempData["MensajeExito"] = "Cuenta creada. Revisa tu correo para confirmar tu cuenta antes de iniciar sesión.";
            return RedirectToAction("Index", "Login");
        }

        [HttpGet]
        public IActionResult ConfirmarCorreo(int userId, string token)
        {
            var usuario = _context.Usuarios.FirstOrDefault(u => u.IdUsuario == userId);
            if (usuario == null) return NotFound();

            string expectedToken = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(usuario.NombreUsuario + usuario.IdUsuario)));

            if (token == expectedToken)
            {
                usuario.CorreoConfirmado = true; // Nueva fecha de confirmación en UTC
                _context.SaveChanges();
                ViewBag.Mensaje = "✅ Tu cuenta ha sido confirmada. Ya puedes iniciar sesión.";
            }
            else
            {
                ViewBag.Mensaje = "❌ Enlace de confirmación inválido.";
            }

            return View();
        }
    }
}