using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgenciaDeViajes.Models
{
    public class ReservaTour
    {
        [Key]
        public int Id { get; set; }

        // FK hacia Usuario
        [Required]
        public int UsuarioId { get; set; }

        // FK hacia Destino
        [Required]
        public int DestinoId { get; set; }

        [Required]
        public DateTime FechaReserva { get; set; }

        [Required]
        public int CantidadBoletos { get; set; }

        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal PrecioTotal { get; set; }

        // Navegación opcional
        public Usuario? Usuario { get; set; }
        public Destino? Destino { get; set; }
    }
}
