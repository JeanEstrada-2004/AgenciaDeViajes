using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgenciaDeViajes.Models
{
    [Table("Reservas")]
    public class Reserva
    {
        [Key]
        public int IdReserva { get; set; }

        [Required]
        public int IdUsuario { get; set; }

        [Required]
        public int IdDestino { get; set; }

        [Required]
        public DateTime FechaReserva { get; set; } = DateTime.Now;

        [Required]
        public DateTime FechaTour { get; set; }

        [Required]
        public int CantidadAdultos { get; set; }

        [Required]
        public int CantidadNinos { get; set; }

        [Required]
        [Column(TypeName = "numeric(12,2)")]
        public decimal PrecioTotal { get; set; }

        [Required]
        [MaxLength(30)]
        public string Estado { get; set; } = "Pendiente";

        public DateTime? FechaPago { get; set; }
        [MaxLength(50)]
        public string? MetodoPago { get; set; }
        [MaxLength(100)]
        public string? ReferenciaPago { get; set; }

        // Relaciones (EF)
        public List<PasajeroReserva>? Pasajeros { get; set; }
        public List<ReservaServicioAdicional>? ServiciosAdicionales { get; set; }
    }
}
