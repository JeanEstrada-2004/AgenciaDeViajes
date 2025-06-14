using System.ComponentModel.DataAnnotations;

namespace AgenciaDeViajes.Models
{

    public class Tour
    {

        public int Id { get; set; }
        [Required]
        public string Nombre { get; set; } = string.Empty;
        [Required]
        public string Descripcion { get; set; } = string.Empty;     
        public decimal Precio { get; set; }
        public bool Destacado { get; set; }
    }
}