using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgenciaDeViajes.Models
{
    [Table("PasajerosReserva")]
    public class PasajeroReserva
    {
        [Key]
        [Column("IdPasajero")]
        public int IdPasajero { get; set; }

        [Required]
        [Column("IdReserva")]
        public int IdReserva { get; set; }

        [Column("Tipo")]
        public string Tipo { get; set; }

        [Column("NombreCompleto")]
        public string NombreCompleto { get; set; }

        [Column("Documento")]
        public string Documento { get; set; }

        [Column("Telefono")]
        public string Telefono { get; set; }

        [Column("Pais")]
        public string Pais { get; set; }

        [ForeignKey("IdReserva")]
        public virtual Reserva Reserva { get; set; }
    }
}
