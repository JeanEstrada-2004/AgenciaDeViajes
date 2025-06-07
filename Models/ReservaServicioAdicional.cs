using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgenciaDeViajes.Models
{
    [Table("ReservaServiciosAdicionales")]
    public class ReservaServicioAdicional
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int IdReserva { get; set; }

        [Required]
        [MaxLength(100)]
        public string Servicio { get; set; } = "";
    }
}
