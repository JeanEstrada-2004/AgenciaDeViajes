using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgenciaDeViajes.Models
{
    public class Usuario
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdUsuario { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string NombreUsuario { get; set; }  // Correo

        [Required]
        [StringLength(150)]
        public string NombreCompleto { get; set; }

        [StringLength(20)]
        public string? Telefono { get; set; }

        [StringLength(20)]
        public string? DNI { get; set; }

        public DateTime? FechaNacimiento { get; set; }

        [Required]
        [StringLength(100)]
        public string Contrasena { get; set; }

        [Required]
        [StringLength(50)]
        public string Rol { get; set; } = "Cliente";

        [StringLength(20)]
        public string MetodoRegistro { get; set; } = "Manual";

        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

        [Required]
        public bool CorreoConfirmado { get; set; } = false;  // ✅ NUEVA COLUMNA
    }

}
