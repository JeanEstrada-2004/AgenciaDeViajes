using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgenciaDeViajes.Models
{
    [Table("Usuarios")]
    public class Usuario
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdUsuario { get; set; }

        [Required]
        [StringLength(100)]
        public string NombreUsuario { get; set; }

        [Required]
        [StringLength(100)]
        public string Contrasena { get; set; }

        [Required]
        [StringLength(50)]
        public string Rol { get; set; } = "Admin";

        // NUEVO: para mostrar el nombre real del usuario
        [StringLength(150)]
        public string? NombreCompleto { get; set; }
    }
}
