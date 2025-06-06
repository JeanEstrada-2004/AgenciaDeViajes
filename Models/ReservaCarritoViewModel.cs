using System;
using System.Collections.Generic;

namespace AgenciaDeViajes.Models
{
    public class ReservaCarritoViewModel
    {
        // Datos principales
        public int DestinoId { get; set; }
        public string DestinoNombre { get; set; } = "";
        public string DestinoImagenUrl { get; set; } = "";
        public string DestinoDuracion { get; set; } = "";
        public double PrecioTour { get; set; }
        public string FechaElegida { get; set; } = ""; // Si lo prefieres, cámbiala a DateTime

        // Selección del usuario
        public int CantidadAdultos { get; set; }
        public int CantidadNinos { get; set; }
        public List<string> ServiciosAdicionales { get; set; } = new List<string>();

        // -- Pasajeros se llenará en la siguiente vista --
        public List<PasajeroViewModel> Pasajeros { get; set; } = new List<PasajeroViewModel>();
    }

    public class PasajeroViewModel
    {
        public string Tipo { get; set; } = ""; // "Adulto" o "Niño"
        public string NombreCompleto { get; set; } = "";
        public string Documento { get; set; } = "";
        public string Telefono { get; set; } = "";
        public string Pais { get; set; } = "";
    }
}
