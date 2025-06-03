using System.Collections.Generic;

namespace AgenciaDeViajes.Models.ViewModels
{
    public class RegionView
    {
        public int id_region { get; set; }
        public int num_tours { get; set; }
        public string desc_region { get; set; } = string.Empty;
        public string ImgReg_url { get; set; } = string.Empty;

        public ICollection<DestinoView> Destinos { get; set; } = new List<DestinoView>();
    }
}
