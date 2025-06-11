using Microsoft.AspNetCore.Mvc;
using AgenciaDeViajes.Models;

namespace AgenciaDeViajes.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FestividadesApiController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetFestividades()
        {
            var festividades = new List<Festividad>
            {
                new Festividad
                {
                    Id = 1,
                    Nombre = "Fiesta de la Candelaria",
                    Fecha = new DateTime(2024, 2, 2),
                    Lugar = "Puno",
                    Descripcion = "Celebración religiosa y cultural con danzas y música.",
                    ImagenUrl = "https://medialab.news/wp-content/uploads/2023/02/Candelaria.jpg",
                    Tipo = "Religiosa y Cultural"
                },
                new Festividad
                {
                    Id = 2,
                    Nombre = "Inti Raymi",
                    Fecha = new DateTime(2024, 6, 24),
                    Lugar = "Cusco",
                    Descripcion = "Fiesta del sol incaico con representación teatral.",
                    ImagenUrl = "https://th.bing.com/th/id/OIP.2Kd-Tace1SknBZpS28qagwHaE4?w=638&h=420&rs=1&pid=ImgDetMain",
                    Tipo = "Cultural"
                }
            };

            return Ok(festividades);
        }
    }
}
