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
        [Column("IdReserva")]
        public int IdReserva { get; set; }

        [Required]
        [Column("IdUsuario")]
        public int IdUsuario { get; set; }

        [Required]
        [Column("IdDestino")]
        public int IdDestino { get; set; }

        [Required]
        [Column("FechaReserva", TypeName = "timestamp")]
        public DateTime FechaReserva { get; set; } = DateTime.Now;

        [Required]
        [Column("FechaTour", TypeName = "timestamp")]
        public DateTime FechaTour { get; set; }

        [Required]
        [Column("CantidadAdultos")]
        public int CantidadAdultos { get; set; }

        [Required]
        [Column("CantidadNinos")]
        public int CantidadNinos { get; set; }

        [Required]
        [Column("PrecioTotal", TypeName = "numeric(12,2)")]
        public decimal PrecioTotal { get; set; }

        [Required]
        [Column("Estado")]
        [MaxLength(30)]
        public string Estado { get; set; } = "Pendiente";

        [Column("FechaPago", TypeName = "timestamp")]
        public DateTime? FechaPago { get; set; }

        [Column("MetodoPago")]
        [MaxLength(50)]
        public string? MetodoPago { get; set; }

        [Column("ReferenciaPago")]
        [MaxLength(100)]
        public string? ReferenciaPago { get; set; }

        [ForeignKey("IdDestino")]
        public virtual Destino Destino { get; set; }

        [InverseProperty("Reserva")]
        public virtual ICollection<PasajeroReserva> PasajerosReserva { get; set; }

        [InverseProperty("Reserva")]
        public virtual ICollection<ReservaServicioAdicional> ReservaServiciosAdicionales { get; set; }
    }
}
