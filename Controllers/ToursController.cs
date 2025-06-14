using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DLL.Csharp.Swagger.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TravelAgencyController : ControllerBase
    {
        private readonly ILogger<TravelAgencyController> _logger;
        private static List<TourPackage> packages = new List<TourPackage>();

        public TravelAgencyController(ILogger<TravelAgencyController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<TourPackage> Get()
        {
            return packages.ToArray();
        }

        [HttpPost("{quantity}")]
        public ActionResult<List<TourPackage>> PostMany(int quantity)
        {
            string[] names = { "Juan", "María", "Carlos", "Lucía", "Pedro", "Ana", "Luis", "Rosa", "Jorge", "Sofía" };
            string[] lastNames = { "García", "Rodríguez", "López", "Martínez", "Pérez", "González", "Hernández", "Díaz", "Moreno", "Torres" };
            string[] destinations = { "Machu Picchu", "Cusco", "Arequipa", "Lima", "Iquitos", "Trujillo", "Puno", "Nazca", "Paracas", "Huaraz" };
            string[] activities = { "Tour Arqueológico", "Excursión en Kayak", "Senderismo", "Avistamiento de Aves", "Tour Gastronómico", "Paseo en Bote", "Visita a Museos", "Tour Fotográfico" };

            var random = new Random();
            var newPackages = new List<TourPackage>();

            for (int i = 0; i < quantity; i++)
            {
                var package = new TourPackage
                {
                    Id = Guid.NewGuid(),
                    ClientName = $"{names[random.Next(names.Length)]} {lastNames[random.Next(lastNames.Length)]}",
                    Destination = destinations[random.Next(destinations.Length)],
                    Activity = activities[random.Next(activities.Length)],
                    DurationDays = random.Next(1, 15),
                    Price = Math.Round(random.NextDouble() * 2000 + 500, 2),
                    StartDate = DateTime.Now.AddDays(random.Next(1, 90)),
                    IsPremium = random.Next(2) == 1
                };
                newPackages.Add(package);
            }

            packages.AddRange(newPackages);
            return CreatedAtAction(nameof(Get), newPackages);
        }

        [HttpPost]
        public ActionResult<TourPackage> Post(TourPackage package)
        {
            package.Id = Guid.NewGuid();
            packages.Add(package);
            return CreatedAtAction(nameof(Get), new { id = package.Id }, package);
        }

        [HttpPut("{id}")]
        public ActionResult<TourPackage> Put(Guid id, TourPackage package)
        {
            var existingPackage = packages.FirstOrDefault(p => p.Id == id);
            if (existingPackage == null)
            {
                return NotFound();
            }

            existingPackage.ClientName = package.ClientName;
            existingPackage.Destination = package.Destination;
            existingPackage.Activity = package.Activity;
            existingPackage.DurationDays = package.DurationDays;
            existingPackage.Price = package.Price;
            existingPackage.StartDate = package.StartDate;
            existingPackage.IsPremium = package.IsPremium;

            return Ok(existingPackage);
        }

        [HttpDelete("{id}")]
        public ActionResult<string> Delete(Guid id)
        {
            var existingPackage = packages.FirstOrDefault(p => p.Id == id);
            if (existingPackage == null)
                return NotFound();
            
            packages.Remove(existingPackage); 
            return Ok("Paquete turístico eliminado exitosamente");
        }
    }

    public class TourPackage
    {
        public Guid Id { get; set; }
        public string ClientName { get; set; }
        public string Destination { get; set; }
        public string Activity { get; set; }
        public int DurationDays { get; set; }
        public double Price { get; set; }
        public DateTime StartDate { get; set; }
        public bool IsPremium { get; set; }
    }
}