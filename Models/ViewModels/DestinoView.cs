namespace AgenciaDeViajes.Models.ViewModels
{
    public class DestinoView
    {
        public int id_destino { get; set; }
        public int id_region { get; set; }
        public string nom_destino { get; set; } = string.Empty;
        public string desc_destino { get; set; } = string.Empty;
        public decimal precio_tour { get; set; }
        public int num_entradas { get; set; } = 0;
        public string time_tour { get; set; } = string.Empty;
        public string ImgDest_url { get; set; } = string.Empty;
    }
}
