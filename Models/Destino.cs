using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgenciaDeViajes.Models
{
    public class Destino
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_destino { get; set; }

        [Required]
        public int id_region { get; set; }

        [ForeignKey("id_region")]
        public Region? Region { get; set; }


        [Required]
        [StringLength(255)]
        public string nom_destino { get; set; }

        public string desc_destino { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal precio_tour { get; set; }

        public int num_entradas { get; set; }

        [StringLength(50)]
        public string time_tour { get; set; }

        [Display(Name = "Imagen")]
        public string ImgDest_url { get; set; }
    }
}
