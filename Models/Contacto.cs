using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgenciaDeViajes.Models
{
    public class Contacto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty; // Inicializado

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty; // Inicializado

        [Required]
        [StringLength(1000)]
        public string Comentarios { get; set; } = string.Empty; // Inicializado

        public DateTime FechaContacto { get; set; } = DateTime.Now;
    }
}