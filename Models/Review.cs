using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgenciaDeViajes.Models
{
    [Table("Reviews")]
    public class Review
    {
        [Key]
        public int IdReview { get; set; }

        // Quitamos IdUsuario y la relación Usuario

        [Required]
        public int IdDestino { get; set; }  // FK a Destino

        [ForeignKey("IdDestino")]
        public virtual Destino Destino { get; set; }  // Relación Destino

        [Required]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        [Required]
        [Range(1, 5)]
        public int CalificacionAtencion { get; set; }

        [Required]
        [Range(1, 5)]
        public int CalificacionCalidad { get; set; }

        [Required]
        [Range(1, 5)]
        public int CalificacionPuntualidad { get; set; }

        [MaxLength(1000)]
        public string Comentario { get; set; } = "";

        // Resultado del análisis de sentimiento con IA (positivo, negativo, neutro)
        [MaxLength(20)]
        public string Sentimiento { get; set; } = "Neutro";

        // Para controlar si el review se muestra o no
        public bool Activo { get; set; } = true;
    }
}
