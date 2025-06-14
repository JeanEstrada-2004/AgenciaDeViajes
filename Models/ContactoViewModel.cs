using System.ComponentModel.DataAnnotations;
using System;

namespace AgenciaDeViajes.Models
{
    public class ContactoViewModel
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        public string Nombre { get; set; } = string.Empty; // Inicializado

        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress(ErrorMessage = "Correo no válido")]
        public string Email { get; set; } = string.Empty; // Inicializado

        [Required(ErrorMessage = "Debe ingresar un mensaje")]
        [StringLength(1000, ErrorMessage = "El mensaje no puede exceder 1000 caracteres")]
        public string Comentarios { get; set; } = string.Empty; // Inicializado

        public DateTime FechaContacto { get; set; } = DateTime.Now;
    }
}