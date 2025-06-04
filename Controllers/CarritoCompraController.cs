using Microsoft.AspNetCore.Mvc;

namespace AgenciaDeViajes.Controllers
{
    public class CarritoCompraController : Controller
    {
        public IActionResult ReservaDatos()
        {
            return View();
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
