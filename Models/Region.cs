using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgenciaDeViajes.Models
{
    public class Region
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_region { get; set; }

        public int num_tours { get; set; }

        [Required]
        public string desc_region { get; set; }

        public string ImgReg_url { get; set; }

        // Relación: Una región tiene muchos destinos
        public List<Destino> Destinos { get; set; } = new List<Destino>();
    }
}
