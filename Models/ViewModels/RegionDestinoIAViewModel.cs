using AgenciaDeViajes.Models;

namespace AgenciaDeViajes.Models.ViewModels
{
    public class RegionDestinoIAViewModel
    {
        public List<Region> Regiones { get; set; }
        public List<Destino> DestinosPopulares { get; set; }
        public int PaginaActual { get; set; }
        public int TotalPaginas { get; set; }

    }
}
