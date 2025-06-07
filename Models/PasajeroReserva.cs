using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgenciaDeViajes.Models
{
    [Table("PasajerosReserva")]
    public class PasajeroReserva
    {
        [Key]
        public int IdPasajero { get; set; }

        [Required]
        public int IdReserva { get; set; }

        [Required]
        [MaxLength(10)]
        public string Tipo { get; set; } = ""; // "Adulto" o "Niño"

        [Required]
        [MaxLength(150)]
        public string NombreCompleto { get; set; } = "";

        [MaxLength(50)]
        public string? Documento { get; set; }

        [MaxLength(20)]
        public string? Telefono { get; set; }

        [MaxLength(50)]
        public string? Pais { get; set; }
    }
}
