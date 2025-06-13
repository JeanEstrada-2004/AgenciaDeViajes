using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgenciaDeViajes.Models
{
    [Table("Destinos")]
    public class Destino
    {
        [Key]
        [Column("id_destino")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_destino { get; set; }

        [Required]
        [Column("id_region")]
        public int id_region { get; set; }

        [ForeignKey("id_region")]
        public Region? Region { get; set; }

        [Required]
        [Column("nom_destino")]
        [StringLength(255)]
        public string nom_destino { get; set; }

        [Column("desc_destino")]
        public string desc_destino { get; set; }

        [Required]
        [Column("precio_tour", TypeName = "numeric(10,2)")]
        public decimal precio_tour { get; set; }

        [Column("num_entradas")]
        public int num_entradas { get; set; }

        [Column("time_tour")]
        [StringLength(50)]
        public string time_tour { get; set; }

        [Column("ImgDest_url")]
        [Display(Name = "Imagen")]
        public string ImgDest_url { get; set; }
    }
}
