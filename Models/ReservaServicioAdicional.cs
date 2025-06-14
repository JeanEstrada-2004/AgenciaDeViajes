using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgenciaDeViajes.Models
{
    [Table("ReservaServiciosAdicionales")]
    public class ReservaServicioAdicional
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Required]
        [Column("IdReserva")]
        public int IdReserva { get; set; }

        [Column("Servicio")]
        public string Servicio { get; set; }

        [ForeignKey("IdReserva")]
        public virtual Reserva Reserva { get; set; }
    }
}
