using System.Text;
using System.Security.Cryptography;
using AgenciaDeViajes.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using AgenciaDeViajes.Data;
using AgenciaDeViajes.Models;

namespace AgenciaDeViajes.Controllers
{
    [Route("Login")]
    public class LoginController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailSender;

        public LoginController(ApplicationDbContext context, IEmailSender emailSender)
        {
            _context = context;
            _emailSender = emailSender;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("Iniciar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Iniciar(string nombreUsuario, string contrasena)
        {
            var usuario = _context.Usuarios
                .FirstOrDefault(u => u.NombreUsuario == nombreUsuario && u.Contrasena == contrasena);

            if (usuario != null)
            {
                if (!usuario.CorreoConfirmado)
                {
                    ViewBag.Error = "Debes confirmar tu correo antes de iniciar sesión.";
                    return View("Index");
                }

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, usuario.NombreUsuario),
                    new Claim(ClaimTypes.Role, usuario.Rol ?? "Usuario")
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Usuario o contraseña incorrecta";
            return View("Index");
        }

        [HttpGet("GoogleLogin")]
        public IActionResult GoogleLogin()
        {
            var redirectUrl = Url.Action("GoogleResponse");
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, "Google");
        }

        [HttpGet("GoogleResponse")]
        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync("Google");
            if (!result.Succeeded)
                return RedirectToAction("Index");

            var claims = result.Principal.Identities.FirstOrDefault()?.Claims;
            var email = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var name = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

            if (email == null) return RedirectToAction("Index");

            var usuario = _context.Usuarios.FirstOrDefault(u => u.NombreUsuario == email);
            
            // Usuario nuevo con Google
            if (usuario == null)
            {
                usuario = new Usuario
                {
                    NombreUsuario = email,
                    NombreCompleto = name ?? "Usuario Google",
                    Contrasena = "google_password", // Contraseña simple para usuarios de Google
                    Rol = "Cliente",
                    MetodoRegistro = "Google",
                    FechaRegistro = DateTime.UtcNow,
                    CorreoConfirmado = false // Requerirá confirmación como los manuales
                };

                _context.Usuarios.Add(usuario);
                _context.SaveChanges();

                // Generar token de confirmación (igual que en registro normal)
                string token = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(usuario.NombreUsuario + usuario.IdUsuario)));

                var callbackUrl = Url.Action("ConfirmarCorreo", "Registro", new
                {
                    userId = usuario.IdUsuario,
                    token = token
                }, protocol: HttpContext.Request.Scheme);

                string mensaje = $@"
                <h3>Hola {usuario.NombreCompleto}!</h3>
                <p>Gracias por iniciar sesión con Google. Por favor confirma tu correo haciendo clic en el siguiente enlace:</p>
                <p><a href='{callbackUrl}'>Confirmar mi cuenta</a></p>";

                await _emailSender.SendEmailAsync(usuario.NombreUsuario, "Confirma tu cuenta", mensaje);

                // Mensaje amigable indicando que debe confirmar
                ViewBag.Error = "Se ha enviado un correo de confirmación. Por favor revisa tu bandeja de entrada.";
                return View("Index");
            }

            // Usuario existente pero no confirmado
            if (!usuario.CorreoConfirmado)
            {
                ViewBag.Error = "Debes confirmar tu correo antes de iniciar sesión.";
                return View("Index");
            }

            // Usuario confirmado - Iniciar sesión
            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, usuario.NombreUsuario),
                new Claim(ClaimTypes.Role, usuario.Rol ?? "Cliente")
            }, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
            return RedirectToAction("Index", "Home");
        }

        [HttpPost("Logout")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}